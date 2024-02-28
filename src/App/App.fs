open Feliz
open Feliz.UseElmish
open Feliz.Router
open Thoth.Elmish
open Elmish
open Browser
open Fable.Core
open Fable.Core.JsInterop
open Fable.WebWorker
open Fable.Standalone
open Fable.ReactToastify
open Feliz.Markdown
open Navigation
open MonacoEditor
open System
open Feliz.UseMediaQuery

importSideEffects "react-toastify/dist/ReactToastify.css"
importSideEffects "./monaco-vite.js"

[<RequireQualifiedAccess>]
type LogLevel =
    | Log
    | Warn
    | Error

[<RequireQualifiedAccess>]
module LogLevel =
    let toCssColor logLevel =
        match logLevel with
        | LogLevel.Log -> "inherit"
        | LogLevel.Warn -> "darkorange"
        | LogLevel.Error -> "red"

type Model = {
    Logs: (string * LogLevel) list
    FSharpCode: string
    CompiledJavaScript: string
    Markdown: string
    IFrameUrl: string
    Worker: ObservableWorker<WorkerAnswer>
    TableOfContents: Documentation.TableOfContents
    CurrentPage: Navigation.Page
    DocEntryNavigation: DocEntryNavigation
    Editor: Monaco.Editor.IStandaloneCodeEditor
    Markers: Monaco.Editor.IMarkerData array
    Debouncer: Debouncer.State
}

type Msg =
    | Compile
    | SetIFrameUrl of string
    | SetFSharpCode of string
    | SetMarkdown of string
    | AddConsoleLog of LogLevel * string
    | Compiled of code: string * language: string * errors: Error array * stats: CompileStats
    | FetchedTableOfContents of Documentation.TableOfContents
    | FetchTableOfContentsExn of exn
    | SetUrl of string list
    | CalculateMarkdownAndCodeValues
    | CalculateDocEntryNavigation
    | SetMarkers of Monaco.Editor.IMarkerData array
    | SetEditor of Monaco.Editor.IStandaloneCodeEditor
    | DebouncerSelfMsg of Debouncer.SelfMessage<Msg>
    | ParseCode

[<Erase>]
type SyntaxHighlighter =
    static member inline language(value: string) = Interop.mkAttr "language" value
    static member inline style(value: string) = Interop.mkAttr "style" value
    static member inline children(value: ReactElement seq) = Interop.mkAttr "children" value

    static member inline highlighter(properties: IReactProperty list) =
        Interop.reactApi.createElement (
            import "Prism as ReactSyntaxHighlighter" "react-syntax-highlighter",
            createObj !!properties
        )

[<Erase>]
type MonacoEditor =
    static member inline onChange(f: string -> unit) = Interop.mkAttr "onChange" f
    static member inline theme(value: string) = Interop.mkAttr "theme" value
    static member inline defaultLanguage(value: string) = Interop.mkAttr "defaultLanguage" value
    static member inline value(value: string) = Interop.mkAttr "value" value
    static member inline width(value: string) = Interop.mkAttr "width" value
    static member inline height(value: string) = Interop.mkAttr "height" value

    static member inline onMount(f: System.Func<Monaco.Editor.IStandaloneCodeEditor, Monaco.IExports, unit>) =
        Interop.mkAttr "onMount" f

    static member inline editor(properties: IReactProperty list) =
        Interop.reactApi.createElement (import "Editor" "@monaco-editor/react", createObj !!properties)

module WebWorker =
    let create () = Worker.Create(Constants.worker)

    let command (worker: ObservableWorker<_>) =
        let handler dispatch =
            worker
            |> Observable.add (function
                | Loaded version -> ()
                | LoadFailed -> ()
                | ParsedCode errors -> errors |> Editor.mapErrorToMarker |> SetMarkers |> dispatch
                | CompilationFinished(code, lang, errors, stats) -> dispatch (Compiled(code, lang, errors, stats))
                | CompilationsFinished(code, lang, errors, stats) -> ()
                | CompilerCrashed msg -> ()
                | FoundTooltip _ -> ()
                | FoundCompletions _ -> ()
                | FoundDeclarationLocation _ -> ())

        [ handler ]

let getCurrentPage tableOfContents url =
    let pages = Documentation.TableOfContents.allPages tableOfContents
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
            Cmd.OfPromise.either Documentation.loadTableOfContents () FetchedTableOfContents FetchTableOfContentsExn
        ]

    let currentUrl = Router.currentUrl ()
    let tableOfContents = Documentation.emptyTableOfContents
    let currentPage = getCurrentPage tableOfContents currentUrl

    {
        Logs = []
        FSharpCode = ""
        CompiledJavaScript = ""
        Markdown = ""
        IFrameUrl = ""
        Worker = worker
        TableOfContents = tableOfContents
        CurrentPage = currentPage
        DocEntryNavigation = {
            PreviousEntry = None
            NextEntry = None
        }
        Editor = Unchecked.defaultof<Monaco.Editor.IStandaloneCodeEditor>
        Markers = [||]
        Debouncer = Debouncer.create ()
    },
    cmd

let compile model =
    let language = "javascript"
    let fsharpOptions = [||]
    CompileCode(model.FSharpCode, language, fsharpOptions) |> model.Worker.Post

let helloWorldCode = "printfn \"Hello, World!\""

let calculateMarkdownValue (tableOfContents: Documentation.TableOfContents) currentPage =
    match currentPage with
    | Page.DocPage docPage -> docPage.MarkdownDocumentation
    | Page.NotFound
    | Page.Homepage -> tableOfContents.RootMarkdown
    | Page.TableOfContents -> Documentation.TableOfContents.toMarkdownString tableOfContents

let calculateFSharpCodeValue currentPage =
    match currentPage with
    | Page.DocPage docPage -> docPage.FSharpCode
    | Page.NotFound
    | Page.Homepage
    | Page.TableOfContents -> helloWorldCode

let setModelMarkers (editor: Monaco.Editor.IStandaloneCodeEditor) (markers: Monaco.Editor.IMarkerData array) =
    match editor.getModel () with
    | None -> ()
    | Some textModel -> Monaco.editor.setModelMarkers (textModel, "FSharpErrors", ResizeArray markers)

let errorToLogLevel (error: Error) =
    if error.IsWarning then LogLevel.Warn else LogLevel.Error

let toastNotificationFromErrors (errors: Error array) =
    match errors with
    | [||] -> Toastify.success "Compiled Successfully."
    | _ -> Toastify.error "Failed to Compile."

let update msg model =
    match msg with
    | Compile -> { model with Logs = [] }, Cmd.ofEffect (fun _ -> compile model)
    | SetIFrameUrl url -> { model with IFrameUrl = url }, Cmd.none
    | SetMarkdown doc -> { model with Markdown = doc }, Cmd.none
    | SetEditor editor -> { model with Editor = editor }, Cmd.none
    | ParseCode -> model, Cmd.ofEffect (fun _ -> WorkerRequest.ParseCode(model.FSharpCode, [||]) |> model.Worker.Post)
    | SetFSharpCode code ->
        let (debouncerModel, debouncerCmd) =
            model.Debouncer
            |> Debouncer.bounce (TimeSpan.FromSeconds 1) "user_input" ParseCode

        {
            model with
                FSharpCode = code
                Debouncer = debouncerModel
        },
        Cmd.map DebouncerSelfMsg debouncerCmd
    | SetMarkers markers ->
        let model = { model with Markers = markers }
        let cmd = Cmd.ofEffect (fun _ -> setModelMarkers model.Editor model.Markers)
        model, cmd
    | AddConsoleLog(level, output) ->
        let logs = model.Logs @ [ (output, level) ]
        { model with Logs = logs }, Cmd.none
    | Compiled(code, _, errors, _) ->
        let logs =
            if errors.Length = 0 then
                model.Logs
            else
                let errorLogs =
                    errors
                    |> Array.map (fun error -> error.Message, errorToLogLevel error)
                    |> Array.toList

                model.Logs @ errorLogs

        let model = {
            model with
                CompiledJavaScript = code
                Logs = logs
        }

        model,
        Cmd.batch [
            Cmd.ofEffect (fun _ -> toastNotificationFromErrors errors |> ignore)
            errors |> Editor.mapErrorToMarker |> SetMarkers |> Cmd.ofMsg
            Cmd.OfFunc.perform Iframe.generateHtmlBlobUrl model.CompiledJavaScript SetIFrameUrl
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
        let fsharpCode = calculateFSharpCodeValue model.CurrentPage
        let markdown = calculateMarkdownValue model.TableOfContents model.CurrentPage
        // clear the logs when we calculate new values.
        { model with Logs = [] },
        // this has useful side-effects like triggering an initial parse through the `SetFSharpCode` msg.
        Cmd.batch [ Cmd.ofMsg (SetFSharpCode fsharpCode); Cmd.ofMsg (SetMarkdown markdown) ]
    | CalculateDocEntryNavigation ->
        let allEntries = Documentation.TableOfContents.allPages model.TableOfContents

        let currentEntry =
            match model.CurrentPage with
            | Page.DocPage docPage -> Entry docPage
            | _ -> NotViewingEntry

        {
            model with
                DocEntryNavigation = getDocEntryNavigation currentEntry allEntries
        },
        Cmd.none
    | DebouncerSelfMsg debouncerMsg ->
        let (debouncerModel, debouncerCmd) = Debouncer.update debouncerMsg model.Debouncer

        {
            model with
                Debouncer = debouncerModel
        },
        debouncerCmd

let (|DesktopSize|MobileSize|) (screenSize: ScreenSize) =
    match screenSize with
    | ScreenSize.Desktop
    | ScreenSize.WideScreen
    | ScreenSize.Tablet -> DesktopSize // Tablets should be fine... but this may need to be changed in the future.
    | ScreenSize.Mobile
    | ScreenSize.MobileLandscape -> MobileSize

let mobileNavbar =
    Html.ul [ Html.li [ Html.a [ prop.href (Router.format []); prop.text "F# For You" ] ] ]

let desktopNavbar = [
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

    Html.ul [
        Html.li [
            Html.a [
                prop.href "https://fable.io"
                prop.target "_blank"
                prop.children [
                    Html.img [ prop.src "img/fable.png"; prop.width 40; prop.height 40 ]
                    Html.small " Powered by Fable"
                ]
            ]
        ]
        Html.li [
            Html.a [
                prop.href "https://github.com/fsharpforyou/tour"
                prop.target "_blank"
                prop.children [
                    Html.img [ prop.src "img/github.png"; prop.width 40; prop.height 40 ]
                    Html.small " View Source Code"
                ]
            ]
        ]
    ]
]

module View =
    [<ReactComponent>]
    let AppView () =
        let model, dispatch = React.useElmish (init, update)
        let screenSize = React.useResponsive Breakpoints.defaults

        React.router [
            router.onUrlChanged (SetUrl >> dispatch)
            router.children [
                Html.header [
                    if screenSize = ScreenSize.Desktop || screenSize = ScreenSize.WideScreen then
                        prop.className "container-fluid"
                    prop.style [ style.height (length.percent 10) ]
                    prop.children [
                        Html.nav [
                            match screenSize with
                            | MobileSize -> mobileNavbar
                            | DesktopSize -> yield! desktopNavbar

                            Html.ul [ Html.button [ prop.text "Run"; prop.onClick (fun _ -> dispatch Compile) ] ]
                        ]
                    ]
                ]
                Html.main [
                    prop.style [
                        style.display.flex

                        match screenSize with
                        | DesktopSize ->
                            style.height (length.percent 90)
                            style.flexDirection.row
                        | MobileSize ->
                            style.height (length.percent 100)
                            style.flexDirection.column
                    ]
                    prop.children [
                        Html.section [
                            prop.style [
                                match screenSize with
                                | MobileSize -> style.width (length.percent 100)
                                | DesktopSize ->
                                    style.overflow.scroll
                                    style.overflowX.hidden
                                    style.width (length.percent 50)
                            ]
                            prop.children [
                                Markdown.markdown [
                                    markdown.children model.Markdown
                                    markdown.components [
                                        markdown.components.code (fun props ->
                                            if props.isInline then
                                                Html.code props.children
                                            else
                                                let style =
                                                    import "vs" "react-syntax-highlighter/dist/esm/styles/prism"

                                                let language = props.className.Replace("language-", "")

                                                SyntaxHighlighter.highlighter [
                                                    SyntaxHighlighter.language language
                                                    SyntaxHighlighter.style style
                                                    SyntaxHighlighter.children props.children
                                                ])
                                    ]
                                ]

                                Html.nav [
                                    Html.ul [
                                        match model.DocEntryNavigation.PreviousEntry with
                                        | None -> Html.none
                                        | Some entry ->
                                            Html.li [
                                                Html.a [
                                                    prop.href (Router.format entry.Route)
                                                    prop.text $"< {entry.Title}"
                                                ]
                                            ]
                                        Html.li [
                                            Html.a [
                                                prop.href (Router.format [ "table-of-contents" ])
                                                prop.text "Table of Contents"
                                            ]
                                        ]
                                        match model.DocEntryNavigation.NextEntry with
                                        | None -> Html.none
                                        | Some entry ->
                                            Html.li [
                                                Html.a [
                                                    prop.href (Router.format entry.Route)
                                                    prop.text $"{entry.Title} >"
                                                ]
                                            ]
                                    ]
                                ]
                            ]
                        ]
                        Html.section [
                            prop.style [
                                match screenSize with
                                | DesktopSize -> style.width (length.percent 50)
                                | MobileSize -> style.width (length.percent 100)
                            ]
                            prop.children [
                                Html.section [
                                    prop.style [ style.height (length.percent 70) ]
                                    prop.children [
                                        MonacoEditor.editor [
                                            MonacoEditor.defaultLanguage "fsharp"
                                            MonacoEditor.value model.FSharpCode
                                            MonacoEditor.theme "vs"
                                            MonacoEditor.onChange (SetFSharpCode >> dispatch)
                                            MonacoEditor.onMount (
                                                Editor.onFSharpEditorDidMount model.Worker (SetEditor >> dispatch)
                                            )
                                        ]
                                    ]
                                ]
                                Html.article [
                                    prop.style [
                                        style.height (length.percent 30)
                                        style.overflow.scroll
                                        style.overflowX.hidden
                                    ]
                                    prop.children [
                                        Html.h4 "Output"
                                        for (log, level) in model.Logs do
                                            Html.p [
                                                prop.style [ style.color (LogLevel.toCssColor level) ]
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
