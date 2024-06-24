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

[<RequireQualifiedAccess>]
module WebWorker =
    open MonacoEditor.Monaco.Editor

    let create () = Worker.Create Constants.worker

    let command
        (setMarkersMsg: IMarkerData array -> 'msg)
        (compiledMsg: string * string * Error array * CompileStats -> 'msg)
        (worker: ObservableWorker<_>)
        =
        let handler dispatch =
            worker
            |> Observable.add (function
                | Loaded version -> ()
                | LoadFailed -> ()
                | ParsedCode errors -> errors |> Editor.mapErrorToMarker |> setMarkersMsg |> dispatch
                | CompilationFinished(code, lang, errors, stats) -> dispatch (compiledMsg (code, lang, errors, stats))
                | CompilationsFinished(code, lang, errors, stats) -> ()
                | CompilerCrashed msg -> ()
                | FoundTooltip _ -> ()
                | FoundCompletions _ -> ()
                | FoundDeclarationLocation _ -> ())

        [ handler ]

[<RequireQualifiedAccess>]
module EditorInstance =
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

        let fromCompilerError (error: Error) =
            if error.IsWarning then LogLevel.Warn else LogLevel.Error

    let toastNotificationFromErrors (errors: Error array) =
        match errors with
        | [||] -> Toastify.success "Compiled Successfully."
        | _ -> Toastify.error "Failed to Compile."

    let setModelMarkers (editor: Monaco.Editor.IStandaloneCodeEditor) (markers: Monaco.Editor.IMarkerData array) =
        match editor.getModel () with
        | None -> ()
        | Some textModel -> Monaco.editor.setModelMarkers (textModel, "FSharpErrors", ResizeArray markers)

    type Model = {
        Logs: (string * LogLevel) list
        FSharpCode: string
        CompiledJavaScript: string
        IFrameIdentifier: string
        IFrameUrl: string
        Worker: ObservableWorker<WorkerAnswer>
        Editor: Monaco.Editor.IStandaloneCodeEditor
        Markers: Monaco.Editor.IMarkerData array
        Debouncer: Debouncer.State
    }

    [<RequireQualifiedAccess>]
    type Msg =
        | Compile
        | ParseCode
        | Compiled of code: string * language: string * errors: Error array * stats: CompileStats
        | AddConsoleLog of string * LogLevel
        | SetIFrameUrl of string
        | SetFSharpCode of string
        | DebouncerSelfMsg of Debouncer.SelfMessage<Msg>
        | SetMarkers of Monaco.Editor.IMarkerData array
        | SetEditor of Monaco.Editor.IStandaloneCodeEditor

    let compile model =
        let language = "javascript"
        let fsharpOptions = [||]
        CompileCode(model.FSharpCode, language, fsharpOptions) |> model.Worker.Post

    let init initialFsharpCode =
        fun () ->
            let randomIdentifier = Guid.NewGuid().ToString()
            let fsharpOptions = [| "--define:FABLE_COMPILER"; "--langversion:preview" |]

            let worker =
                ObservableWorker(WebWorker.create (), WorkerAnswer.Decoder, randomIdentifier)

            CreateChecker(Constants.metadata, [||], Some ".txt", fsharpOptions)
            |> worker.Post

            {
                Logs = []
                FSharpCode = initialFsharpCode
                CompiledJavaScript = ""
                IFrameIdentifier = randomIdentifier
                IFrameUrl = ""
                Worker = worker
                Editor = Unchecked.defaultof<_>
                Markers = [||]
                Debouncer = Debouncer.create ()
            },
            Cmd.batch [
                WebWorker.command Msg.SetMarkers Msg.Compiled worker
                Iframe.command randomIdentifier {
                    ConsoleLog = fun text -> Msg.AddConsoleLog(text, LogLevel.Log)
                    ConsoleWarn = fun text -> Msg.AddConsoleLog(text, LogLevel.Warn)
                    ConsoleError = fun text -> Msg.AddConsoleLog(text, LogLevel.Error)
                }
            ]

    let update msg model =
        match msg with
        | Msg.Compile -> { model with Logs = [] }, Cmd.ofEffect (fun _ -> compile model)
        | Msg.ParseCode ->
            model, Cmd.ofEffect (fun _ -> WorkerRequest.ParseCode(model.FSharpCode, [||]) |> model.Worker.Post)
        | Msg.SetIFrameUrl url -> { model with IFrameUrl = url }, Cmd.none
        | Msg.SetEditor editor -> { model with Editor = editor }, Cmd.none
        | Msg.AddConsoleLog(logText, logLevel) ->
            let logs = model.Logs @ [ (logText, logLevel) ]
            { model with Logs = logs }, Cmd.none
        | Msg.SetMarkers markers ->
            let model = { model with Markers = markers }
            let cmd = Cmd.ofEffect (fun _ -> setModelMarkers model.Editor model.Markers)
            model, cmd
        | Msg.SetFSharpCode code ->
            let (debouncerModel, debouncerCmd) =
                model.Debouncer
                |> Debouncer.bounce (TimeSpan.FromSeconds 1) "user_input" Msg.ParseCode

            {
                model with
                    FSharpCode = code
                    Debouncer = debouncerModel
            },
            Cmd.map Msg.DebouncerSelfMsg debouncerCmd
        | Msg.Compiled(code, language, errors, stats) ->
            let logs =
                if errors.Length = 0 then
                    model.Logs
                else
                    let errorLogs =
                        errors
                        |> Array.map (fun error -> error.Message, LogLevel.fromCompilerError error)
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
                errors |> Editor.mapErrorToMarker |> Msg.SetMarkers |> Cmd.ofMsg
                Cmd.OfFunc.perform Iframe.generateHtmlBlobUrl model.CompiledJavaScript Msg.SetIFrameUrl
            ]
        | Msg.DebouncerSelfMsg debouncerMsg ->
            let (debouncerModel, debouncerCmd) = Debouncer.update debouncerMsg model.Debouncer

            {
                model with
                    Debouncer = debouncerModel
            },
            debouncerCmd

    [<ReactComponent>]
    let Component initialFsharpCode =
        let model, dispatch = React.useElmish (init initialFsharpCode, update)

        Html.div [
            MonacoEditor.editor [
                MonacoEditor.height "350px"
                prop.className "monaco-editor"
                MonacoEditor.defaultLanguage "fsharp"
                MonacoEditor.value model.FSharpCode
                MonacoEditor.theme "vs"
                MonacoEditor.onChange (Msg.SetFSharpCode >> dispatch)
                MonacoEditor.onMount (Editor.onFSharpEditorDidMount model.Worker (Msg.SetEditor >> dispatch))
            ]

            Html.button [ prop.text "Compile"; prop.onClick (fun _ -> dispatch Msg.Compile) ]
            Html.button [ prop.text "Open in Playground" ]

            match model.Logs with
            | [] -> Html.none
            | logs ->
                Html.article [
                    prop.style [ style.height (length.percent 30); style.overflow.scroll ]
                    prop.children [
                        // Html.h4 "Output"
                        for (log, level) in logs do
                            Html.p [ prop.style [ style.color (LogLevel.toCssColor level) ]; prop.text log ]

                            Html.hr []
                    ]
                ]
            Html.iframe [
                prop.id model.IFrameIdentifier
                prop.src model.IFrameUrl
                prop.style [
                    style.position.absolute
                    style.width 0
                    style.height 0
                    style.border (0, borderStyle.hidden, "")
                ]
            ]
        ]

[<RequireQualifiedAccess>]
module App =
    type Model = {
        Markdown: string
        TableOfContents: Documentation.TableOfContents
        CurrentPage: Navigation.Page
        DocEntryNavigation: DocEntryNavigation
    }

    [<RequireQualifiedAccess>]
    type Msg =
        | SetMarkdown of string
        | FetchedTableOfContents of Documentation.TableOfContents
        | SetUrl of string list
        | GetMarkdownValue
        | GetDocEntryNavigation

    let private getCurrentPage tableOfContents url =
        let docPages = Documentation.TableOfContents.allPages tableOfContents
        Page.fromUrl docPages url

    let (|DesktopSize|MobileSize|) (screenSize: ScreenSize) =
        match screenSize with
        | ScreenSize.Desktop
        | ScreenSize.WideScreen -> DesktopSize
        | ScreenSize.Tablet
        | ScreenSize.Mobile
        | ScreenSize.MobileLandscape -> MobileSize

    let calculateMarkdownValue (tableOfContents: Documentation.TableOfContents) currentPage =
        match currentPage with
        | Page.DocPage docPage -> docPage.MarkdownDocumentation
        | Page.NotFound
        | Page.Homepage -> tableOfContents.RootMarkdown
        | Page.TableOfContents -> Documentation.TableOfContents.toMarkdownString tableOfContents

    let scrollToTopOfMarkdown () =
        let markdownElement = document.getElementById "markdown-content"
        markdownElement.scrollTo (0, 0)

    let mobileNavbar =
        Html.ul [ Html.li [ Html.a [ prop.href (Router.format []); prop.text "F# For You" ] ] ]

    let imageLink href src text =
        Html.a [
            prop.href href
            prop.target "_blank"
            prop.children [
                Html.img [
                    prop.src src
                    prop.width 40
                    prop.height 40
                    prop.style [ style.marginRight (length.px 5) ]
                ]
                Html.small (text: string)
            ]
        ]

    let desktopNavbar = [
        Html.ul [ Html.li [ imageLink (Router.format []) "img/fsharp.png" "F# For You!" ] ]

        Html.ul [
            Html.li [ imageLink "https://fable.io" "img/fable.png" "Powered by Fable" ]
            Html.li [
                imageLink "https://github.com/fsharpforyou/tour" "img/github.png" "View Source Code"
            ]
        ]
    ]

    let init () =
        let currentUrl = Router.currentUrl ()
        let tableOfContents = Documentation.emptyTableOfContents
        let currentPage = getCurrentPage tableOfContents currentUrl

        {
            Markdown = ""
            TableOfContents = tableOfContents
            CurrentPage = currentPage
            DocEntryNavigation = {
                PreviousEntry = None
                NextEntry = None
            }
        },
        Cmd.OfPromise.perform Documentation.loadTableOfContents () Msg.FetchedTableOfContents

    let update msg model =
        match msg with
        | Msg.SetMarkdown doc -> { model with Markdown = doc }, Cmd.none
        | Msg.FetchedTableOfContents tableOfContents ->
            let url = Router.currentUrl ()
            let currentPage = getCurrentPage tableOfContents url

            {
                model with
                    CurrentPage = currentPage
                    TableOfContents = tableOfContents
            },
            Cmd.batch [ Cmd.ofMsg Msg.GetMarkdownValue; Cmd.ofMsg Msg.GetDocEntryNavigation ]
        | Msg.SetUrl url ->
            let currentPage = getCurrentPage model.TableOfContents url

            { model with CurrentPage = currentPage },
            Cmd.batch [ Cmd.ofMsg Msg.GetMarkdownValue; Cmd.ofMsg Msg.GetDocEntryNavigation ]
        | Msg.GetMarkdownValue ->
            let markdown = calculateMarkdownValue model.TableOfContents model.CurrentPage
            { model with Markdown = markdown }, Cmd.batch [ Cmd.ofEffect (fun _ -> scrollToTopOfMarkdown ()) ]
        | Msg.GetDocEntryNavigation ->
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

    [<ReactComponent>]
    let Component () =
        let model, dispatch = React.useElmish (init, update)
        let screenSize = React.useResponsive Breakpoints.defaults

        React.router [
            router.onUrlChanged (Msg.SetUrl >> dispatch)
            router.children [
                Html.main [
                    Html.header [
                        Html.nav [
                            match screenSize with
                            | MobileSize -> mobileNavbar
                            | DesktopSize -> yield! desktopNavbar
                        ]
                    ]
                    Html.section [
                        prop.id "markdown-content"
                        prop.children [
                            Markdown.markdown [
                                markdown.children model.Markdown
                                markdown.components [
                                    markdown.components.pre (fun props -> React.fragment props.children) // This doesn't wrap our editor instance in a `pre`
                                    markdown.components.code (fun props ->
                                        if props.isInline then
                                            Html.code props.children
                                        else
                                            // this is an interesting way to get the value of a code block
                                            props.children
                                            |> Seq.tryHead
                                            |> Option.map (string >> EditorInstance.Component)
                                            |> Option.defaultValue Html.none)
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
                ]
                Toastify.container [
                    ContainerOption.autoClose 2000
                    ContainerOption.position Position.BottomRight
                    ContainerOption.theme Theme.Light
                ]
            ]
        ]

ReactDOM.createRoot(document.getElementById "app").render (App.Component())
