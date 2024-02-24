open Feliz
open Feliz.UseElmish
open Feliz.Router
open Elmish
open Browser
open Fable.Core
open Fable.Core.JsInterop
open Thoth.Json
open Fable.WebWorker
open Fable.Standalone
open Fable.ReactToastify
open Feliz.Markdown
open Documentation
open Navigation
open MonacoEditor

importSideEffects "react-toastify/dist/ReactToastify.css"
importSideEffects "./monaco-vite.js"

[<RequireQualifiedAccess>]
type LogLevel =
    | Log
    | Warn
    | Error

type Model = {
    Logs: (string * LogLevel) list
    FSharpCode: string
    CompiledJavaScript: string
    Markdown: string
    IFrameUrl: string
    Worker: ObservableWorker<WorkerAnswer>
    TableOfContents: TableOfContents
    CurrentPage: Navigation.Page
    DocEntryNavigation: DocEntryNavigation
    Editor: Monaco.Editor.IStandaloneCodeEditor
    Markers: Monaco.Editor.IMarkerData array
}

type Msg =
    | Compile
    | SetIFrameUrl of string
    | SetFSharpCode of string
    | SetMarkdown of string
    | AddConsoleLog of LogLevel * string
    | Compiled of code: string * language: string * errors: Error array * stats: CompileStats
    | FetchedTableOfContents of TableOfContents
    | FetchTableOfContentsExn of exn
    | SetUrl of string list
    | CalculateMarkdownAndCodeValues
    | CalculateDocEntryNavigation
    | SetEditor of Monaco.Editor.IStandaloneCodeEditor

module Iframe =
    type MessageArgs<'msg> = {
        ConsoleLog: string -> 'msg
        ConsoleWarn: string -> 'msg
        ConsoleError: string -> 'msg
    }

    let command (args: MessageArgs<'Msg>) =
        let handler dispatch =
            window.addEventListener (
                "message",
                fun ev ->
                    let iframeMessageDecoder =
                        Decode.field "type" Decode.string
                        |> Decode.option
                        |> Decode.andThen (function
                            | Some "console_log" -> Decode.field "content" Decode.string |> Decode.map args.ConsoleLog
                            | Some "console_warn" -> Decode.field "content" Decode.string |> Decode.map args.ConsoleWarn
                            | Some "console_error" ->
                                Decode.field "content" Decode.string |> Decode.map args.ConsoleError
                            | _ -> Decode.fail "Invalid message")

                    Decode.fromValue "$" iframeMessageDecoder ev?data
                    |> function
                        | Error _ -> ()
                        | Ok msg -> dispatch msg
            )

        [ handler ]

module WebWorker =
    let create () = Worker.Create(Constants.worker)

    let command (worker: ObservableWorker<_>) =
        let handler dispatch =
            worker
            |> Observable.add (function
                | Loaded version -> ()
                | LoadFailed -> ()
                | ParsedCode errors -> ()
                | CompilationFinished(code, lang, errors, stats) -> dispatch (Compiled(code, lang, errors, stats))
                | CompilationsFinished(code, lang, errors, stats) -> ()
                | CompilerCrashed msg -> ()
                | FoundTooltip _ -> ()
                | FoundCompletions _ -> ()
                | FoundDeclarationLocation _ -> ())

        [ handler ]

module MonacoEditor =
    // Source: https://github.com/fable-compiler/repl/blob/main/src/App/Editor.fs#L131C1-L142C58
    let mapErrorToMarker (errors: Error[]) =
        errors
        |> Array.map (fun err ->
            jsOptions<Monaco.Editor.IMarkerData> (fun m ->
                m.startLineNumber <- err.StartLine
                m.endLineNumber <- err.EndLine
                m.startColumn <- float err.StartColumn + 1.
                m.endColumn <- float err.EndColumn + 1.
                m.message <- err.Message

                m.severity <-
                    match err.IsWarning with
                    | false -> Monaco.MarkerSeverity.Error
                    | true -> Monaco.MarkerSeverity.Warning))

    [<Erase>]
    type props =
        static member inline onChange(f: string -> unit) = Interop.mkAttr "onChange" f
        static member inline theme(value: string) = Interop.mkAttr "theme" value
        static member inline defaultLanguage(value: string) = Interop.mkAttr "defaultLanguage" value
        static member inline value(value: string) = Interop.mkAttr "value" value
        static member inline width(value: string) = Interop.mkAttr "width" value
        static member inline height(value: string) = Interop.mkAttr "height" value

        static member inline onMount(f: System.Func<Monaco.Editor.IStandaloneCodeEditor, Monaco.IExports, unit>) =
            Interop.mkAttr "onMount" f

    [<Erase>]
    type editor =
        static member inline editor(properties: IReactProperty list) =
            Interop.reactApi.createElement (import "Editor" "@monaco-editor/react", createObj !!properties)

let getCurrentPage tableOfContents url =
    let flattenCategories = List.collect _.Pages
    let pages = flattenCategories tableOfContents.Categories
    Page.fromUrl pages url

let init () =
    let fsharpOptions = [| "--define:FABLE_COMPILER"; "--langversion:preview" |]
    let worker = ObservableWorker(WebWorker.create (), WorkerAnswer.Decoder, "MAIN APP")

    CreateChecker(Constants.metadata, [||], Some ".txt", fsharpOptions)
    |> worker.Post

    let cmd =
        Cmd.batch [
            WebWorker.command worker
            Iframe.command {
                ConsoleLog = fun out -> AddConsoleLog(LogLevel.Log, out)
                ConsoleWarn = fun out -> AddConsoleLog(LogLevel.Warn, out)
                ConsoleError = fun out -> AddConsoleLog(LogLevel.Error, out)
            }
            Cmd.OfPromise.either loadTableOfContents () FetchedTableOfContents FetchTableOfContentsExn
        ]

    let currentUrl = Router.currentUrl ()
    let currentPage = getCurrentPage emptyTableOfContents currentUrl

    {
        Logs = []
        FSharpCode = ""
        CompiledJavaScript = ""
        Markdown = ""
        IFrameUrl = ""
        Worker = worker
        TableOfContents = emptyTableOfContents
        CurrentPage = currentPage
        DocEntryNavigation = {
            PreviousEntry = None
            NextEntry = None
        }
        Editor = Unchecked.defaultof<Monaco.Editor.IStandaloneCodeEditor>
        Markers = [||]
    },
    cmd

let compile model =
    let language = "javascript"
    let fsharpOptions = [||]
    CompileCode(model.FSharpCode, language, fsharpOptions) |> model.Worker.Post

let helloWorldCode = "printfn \"Hello, World!\""

let calculateMarkdownValue tableOfContents currentPage =
    match currentPage with
    | Page.DocEntry docPage -> docPage.MarkdownDocumentation
    | Page.NotFound
    | Page.Homepage -> tableOfContents.RootMarkdown
    | Page.TableOfContents -> TableOfContents.toMarkdownString tableOfContents

let calculateFSharpCodeValue currentPage =
    match currentPage with
    | Page.DocEntry docPage -> docPage.FSharpCode
    | Page.NotFound
    | Page.Homepage
    | Page.TableOfContents -> helloWorldCode

let setErrors (model: Model) =
    match model.Editor.getModel () with
    | None -> ()
    | Some textModel -> Monaco.editor.setModelMarkers (textModel, "FSharpErrors", ResizeArray model.Markers)

let update msg model =
    match msg with
    | Compile -> { model with Logs = [] }, Cmd.ofEffect (fun _ -> compile model)
    | SetIFrameUrl url -> { model with IFrameUrl = url }, Cmd.none
    | SetFSharpCode code -> { model with FSharpCode = code }, Cmd.none
    | SetMarkdown doc -> { model with Markdown = doc }, Cmd.none
    | SetEditor editor -> { model with Editor = editor }, Cmd.none
    | AddConsoleLog(level, output) ->
        let logs = (output, level) :: model.Logs
        { model with Logs = logs }, Cmd.none
    | Compiled(code, lang, errors, stats) ->
        let isSuccess = errors.Length = 0

        let toastCmd =
            Cmd.ofEffect (fun _ ->
                if isSuccess then
                    Toastify.success "Compiled Successfully."
                else
                    Toastify.error "There were errors :("
                |> ignore)

        let markers = MonacoEditor.mapErrorToMarker errors

        let logs =
            if isSuccess then
                model.Logs
            else
                model.Logs
                @ (errors
                   |> Array.map (fun error ->
                       let logLevel = if error.IsWarning then LogLevel.Warn else LogLevel.Error
                       error.Message, logLevel)
                   |> Array.toList)

        let model = {
            model with
                CompiledJavaScript = code
                Logs = logs
                Markers = markers
        }

        model,
        Cmd.batch [
            toastCmd
            Cmd.ofEffect (fun _ -> setErrors model)
            Cmd.OfFunc.perform Generator.generateHtmlBlobUrl model.CompiledJavaScript SetIFrameUrl
        ]
    | FetchedTableOfContents tableOfContents ->
        let url = Router.currentUrl ()
        let currentPage = getCurrentPage tableOfContents url

        {
            model with
                CurrentPage = currentPage
                TableOfContents = tableOfContents
        },
        Cmd.batch [
            Cmd.ofMsg CalculateMarkdownAndCodeValues
            Cmd.ofMsg CalculateDocEntryNavigation
        ]
    | FetchTableOfContentsExn exn -> model, Cmd.none // TODO: this.
    | SetUrl url ->
        let currentPage = getCurrentPage model.TableOfContents url

        { model with CurrentPage = currentPage },
        Cmd.batch [
            Cmd.ofMsg CalculateMarkdownAndCodeValues
            Cmd.ofMsg CalculateDocEntryNavigation
        ]
    | CalculateMarkdownAndCodeValues ->
        {
            model with
                FSharpCode = calculateFSharpCodeValue model.CurrentPage
                Markdown = calculateMarkdownValue model.TableOfContents model.CurrentPage
                Logs = []
        },
        Cmd.none
    | CalculateDocEntryNavigation ->
        let allEntries = TableOfContents.allEntries model.TableOfContents

        let currentEntry =
            match model.CurrentPage with
            | Page.DocEntry entry -> Entry entry
            | _ -> NotViewingEntry

        {
            model with
                DocEntryNavigation = getDocEntryNavigation currentEntry allEntries
        },
        Cmd.none

module View =
    [<ReactComponent>]
    let AppView () =
        let model, dispatch = React.useElmish (init, update)

        React.router [
            router.onUrlChanged (SetUrl >> dispatch)
            router.children [
                Html.header [
                    prop.style [ style.height (length.percent 10) ]
                    prop.children [
                        Html.nav [
                            Html.ul [
                                Html.li [
                                    Html.a [
                                        prop.href (Router.format [])
                                        prop.children [
                                            Html.img [ prop.src "img/fsharp.png"; prop.width 40; prop.height 40 ]
                                            Html.strong " F# For You!"
                                        ]
                                    ]
                                ]
                            ]
                            Html.ul [ Html.button [ prop.text "Run"; prop.onClick (fun _ -> dispatch Compile) ] ]
                        ]
                    ]
                ]
                Html.main [
                    prop.role "group"
                    prop.style [ style.height (length.percent 90); style.width (length.percent 100) ]
                    prop.children [
                        Html.section [
                            prop.style [ style.overflow.scroll; style.width (length.percent 50) ]
                            prop.children [
                                Markdown.markdown (model.Markdown)
                                Html.nav [
                                    Html.ul [
                                        Html.li [
                                            match model.DocEntryNavigation.PreviousEntry with
                                            | None -> Html.none
                                            | Some entry ->
                                                Html.a [
                                                    prop.href (Router.format entry.Route)
                                                    prop.text "Previous Page"
                                                ]
                                        ]
                                        Html.li [
                                            Html.a [
                                                prop.href (Router.format [ "table-of-contents" ])
                                                prop.text "Table of Contents"
                                            ]
                                        ]
                                        Html.li [
                                            match model.DocEntryNavigation.NextEntry with
                                            | None -> Html.none
                                            | Some entry ->
                                                Html.a [ prop.href (Router.format entry.Route); prop.text "Next Page" ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                        Html.section [
                            prop.style [ style.width (length.percent 50) ]
                            prop.children [
                                Html.section [
                                    prop.style [ style.height (length.percent 60) ]
                                    prop.children [
                                        MonacoEditor.editor.editor [
                                            MonacoEditor.props.defaultLanguage "fsharp"
                                            MonacoEditor.props.value model.FSharpCode
                                            MonacoEditor.props.theme "vs"
                                            MonacoEditor.props.onChange (SetFSharpCode >> dispatch)
                                            MonacoEditor.props.onMount (fun editor _ -> dispatch (SetEditor editor))
                                        ]
                                    ]
                                ]
                                Html.article [
                                    prop.style [ style.height (length.percent 40); style.overflow.scroll ]
                                    prop.children [
                                        Html.h4 "Output"
                                        for (log, level) in model.Logs do
                                            Html.p [
                                                prop.style [
                                                    style.color (
                                                        match level with
                                                        | LogLevel.Log -> "inherit"
                                                        | LogLevel.Warn -> "darkorange"
                                                        | LogLevel.Error -> "red"
                                                    )
                                                ]
                                                prop.text log
                                            ]

                                            Html.hr []
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
                Html.iframe [
                    prop.src model.IFrameUrl
                    prop.style [
                        style.position.absolute
                        style.width 0
                        style.height 0
                        style.border (0, borderStyle.hidden, "")
                    ]
                ]
                Toastify.container [
                    ContainerOption.autoClose 2000
                    ContainerOption.position Position.BottomRight
                    ContainerOption.theme Theme.Light
                ]
            ]
        ]

ReactDOM.createRoot(document.getElementById "app").render (View.AppView())
