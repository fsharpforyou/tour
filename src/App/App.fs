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
open Feliz.Markdown
open Navigation
open MonacoEditor
open System

importSideEffects "./monaco-vite.js"
importSideEffects "./styles.css"

let supressedWarningMessages = [| "https://aka.ms/fsharp-implicit-convs" |]

let shouldBeSupressed (error: Error) =
    supressedWarningMessages
    |> Array.exists (fun supressedMessage -> error.Message.Contains supressedMessage)

module Helper =
    let inline mkProperty<'t> (key: string) (value: obj) : 't = (key, box value) |> unbox<'t>

[<RequireQualifiedAccess>]
type LogLevel =
    | Log
    | Warn
    | Error

[<RequireQualifiedAccess>]
module LogLevel =
    let cssClass =
        function
        | LogLevel.Log -> "log"
        | LogLevel.Warn -> "warn"
        | LogLevel.Error -> "error"

type Theme =
    | Light
    | Dark

type CompileState =
    | Ready
    | Compiling

[<RequireQualifiedAccess>]
module Theme =
    let fromStorage () =
        match localStorage.getItem "theme" with
        | "light" -> Light
        | "dark"
        | _ -> Dark

    let saveToStorage theme =
        let value =
            match theme with
            | Light -> "light"
            | Dark -> "dark"

        localStorage.setItem ("theme", value)

type Model = {
    Logs: (string * LogLevel) list
    FSharpCode: string
    Markdown: string
    IFrameUrl: string
    Worker: ObservableWorker<WorkerAnswer>
    TableOfContents: Documentation.TableOfContents
    CurrentPage: Navigation.Page
    DocEntryNavigation: DocEntryNavigation
    Editor: Monaco.Editor.IStandaloneCodeEditor option
    Markers: Monaco.Editor.IMarkerData array
    Debouncer: Debouncer.State
    Theme: Theme
    CodeRevision: int
    CompilingRevision: int option
    CompileState: CompileState
    IsLoadingDocumentation: bool
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
    | ToggleTheme

[<Erase>]
type SyntaxHighlighter =
    static member inline language(value: string) = Helper.mkProperty "language" value
    static member inline style(value: string) = Helper.mkProperty "style" value
    static member inline customStyle(value: obj) = Helper.mkProperty "customStyle" value
    static member inline className(value: string) = Helper.mkProperty "className" value
    static member inline children(value: ReactElement seq) = Helper.mkProperty "children" value

    static member inline highlighter(properties: seq<IReactProperty>) =
        ReactLegacy.createElement (
            unbox<ReactElement> (import "Prism as ReactSyntaxHighlighter" "react-syntax-highlighter"),
            createObj !!properties
        )

[<Erase>]
type MonacoEditor =
    static member inline onChange(f: string -> unit) = Helper.mkProperty "onChange" f
    static member inline theme(value: string) = Helper.mkProperty "theme" value

    static member inline defaultLanguage(value: string) =
        Helper.mkProperty "defaultLanguage" value

    static member inline value(value: string) = Helper.mkProperty "value" value
    static member inline options(value: obj) = Helper.mkProperty "options" value

    static member inline onMount(f: System.Func<Monaco.Editor.IStandaloneCodeEditor, Monaco.IExports, unit>) =
        Helper.mkProperty "onMount" f

    static member inline editor(properties: IReactProperty list) =
        ReactLegacy.createElement (unbox<ReactElement> (import "Editor" "@monaco-editor/react"), createObj !!properties)

module WebWorker =
    let create () = Worker.Create(Constants.worker)

    let command (worker: ObservableWorker<_>) =
        let handler dispatch =
            worker
            |> Observable.add (function
                | Loaded _ -> ()
                | LoadFailed -> dispatch (AddConsoleLog(LogLevel.Error, "The F# compiler could not load."))
                | ParsedCode errors ->
                    errors
                    |> Array.filter (shouldBeSupressed >> not)
                    |> Editor.mapErrorToMarker
                    |> SetMarkers
                    |> dispatch
                | CompilationFinished(code, lang, errors, stats) -> dispatch (Compiled(code, lang, errors, stats))
                | CompilationsFinished(code, lang, errors, stats) -> ()
                | CompilerCrashed msg -> dispatch (AddConsoleLog(LogLevel.Error, "Compiler failed: " + msg))
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
        Markdown = ""
        IFrameUrl = ""
        Worker = worker
        TableOfContents = tableOfContents
        CurrentPage = currentPage
        DocEntryNavigation = {
            PreviousEntry = None
            NextEntry = None
        }
        Editor = None
        Markers = [||]
        Debouncer = Debouncer.create ()
        Theme = Theme.fromStorage ()
        CodeRevision = 0
        CompilingRevision = None
        CompileState = Ready
        IsLoadingDocumentation = true
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

let setModelMarkers (editor: Monaco.Editor.IStandaloneCodeEditor option) (markers: Monaco.Editor.IMarkerData array) =
    match editor with
    | None -> ()
    | Some editor ->
        match editor.getModel () with
        | None -> ()
        | Some textModel -> Monaco.editor.setModelMarkers (textModel, "FSharpErrors", ResizeArray markers)

let errorToLogLevel (error: Error) =
    if error.IsWarning then LogLevel.Warn else LogLevel.Error

let scrollToTopOfMarkdown () =
    let markdownElement = document.getElementById "markdown-content"
    markdownElement.scrollTo (0, 0)

let update msg model =
    match msg with
    | Compile when model.CompileState = Compiling -> model, Cmd.none
    | Compile ->
        {
            model with
                Logs = []
                CompilingRevision = Some model.CodeRevision
                CompileState = Compiling
        },
        Cmd.ofEffect (fun _ -> compile model)
    | SetIFrameUrl url -> { model with IFrameUrl = url }, Cmd.none
    | SetMarkdown doc -> { model with Markdown = doc }, Cmd.none
    | SetEditor editor ->
        let model = { model with Editor = Some editor }
        model, Cmd.ofEffect (fun _ -> setModelMarkers model.Editor model.Markers)
    | ParseCode -> model, Cmd.ofEffect (fun _ -> WorkerRequest.ParseCode(model.FSharpCode, [||]) |> model.Worker.Post)
    | SetFSharpCode code ->
        let debouncerModel, debouncerCmd =
            model.Debouncer
            |> Debouncer.bounce (TimeSpan.FromSeconds 1L) "user_input" ParseCode

        {
            model with
                FSharpCode = code
                Debouncer = debouncerModel
                CodeRevision = model.CodeRevision + 1
                CompileState = Ready
        },
        Cmd.map DebouncerSelfMsg debouncerCmd
    | SetMarkers markers ->
        let model = { model with Markers = markers }
        let cmd = Cmd.ofEffect (fun _ -> setModelMarkers model.Editor model.Markers)
        model, cmd
    | AddConsoleLog(level, output) ->
        let logs = model.Logs @ [ (output, level) ]
        { model with Logs = logs }, Cmd.none
    | Compiled(_, _, _, _) when model.CompilingRevision <> Some model.CodeRevision -> model, Cmd.none
    | Compiled(code, _, errors, _) ->
        let errors = errors |> Array.filter (shouldBeSupressed >> not)

        let logs =
            if errors.Length = 0 then
                model.Logs
            else
                let errorLogs =
                    errors
                    |> Array.map (fun error -> error.Message, errorToLogLevel error)
                    |> Array.toList

                errorLogs @ model.Logs

        let model = {
            model with
                Logs = logs
                CompilingRevision = None
                CompileState = Ready
        }

        model,
        Cmd.batch [
            errors |> Editor.mapErrorToMarker |> SetMarkers |> Cmd.ofMsg
            if errors.Length = 0 || Array.forall _.IsWarning errors then
                Cmd.OfFunc.perform Iframe.generateHtmlBlobUrl code SetIFrameUrl
            else
                Cmd.none
        ]
    | FetchedTableOfContents tableOfContents ->
        let url = Router.currentUrl ()
        let currentPage = getCurrentPage tableOfContents url

        {
            model with
                CurrentPage = currentPage
                TableOfContents = tableOfContents
                IsLoadingDocumentation = false
        },
        Cmd.batch [
            Cmd.ofMsg CalculateMarkdownAndCodeValues
            Cmd.ofMsg CalculateDocEntryNavigation
        ]
    | FetchTableOfContentsExn _ ->
        {
            model with
                IsLoadingDocumentation = false
        },
        Cmd.none
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
        Cmd.batch [
            Cmd.ofMsg (SetFSharpCode fsharpCode)
            Cmd.ofMsg (SetMarkdown markdown)
            Cmd.ofEffect (fun _ -> scrollToTopOfMarkdown ())
        ]
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
        let debouncerModel, debouncerCmd = Debouncer.update debouncerMsg model.Debouncer

        {
            model with
                Debouncer = debouncerModel
        },
        debouncerCmd
    | ToggleTheme ->
        let theme =
            match model.Theme with
            | Light -> Dark
            | Dark -> Light

        { model with Theme = theme }, Cmd.ofEffect (fun _ -> Theme.saveToStorage theme)

module TourView =
    [<ReactComponent>]
    let AppView () =
        let model, dispatch = React.useElmish (init, update)

        React.router [
            router.onUrlChanged (SetUrl >> dispatch)
            router.children [
                Html.div [
                    let themeClassName =
                        match model.Theme with
                        | Light -> "light"
                        | Dark -> "dark"

                    prop.className $"tour-app {themeClassName}"

                    prop.children [
                        Html.a [
                            prop.href "#main-content"
                            prop.className "skip-link"
                            prop.text "Skip to lesson"
                        ]
                        Html.header [
                            prop.className "tour-header"
                            prop.children [
                                Html.a [
                                    prop.href (Router.format [])
                                    prop.className "tour-brand"
                                    prop.children [
                                        Html.img [
                                            prop.src "img/fsharp.png"
                                            prop.alt "F#"
                                            prop.className "tour-logo"
                                        ]
                                        Html.text "Language Tour"
                                    ]
                                ]
                                Html.nav [
                                    prop.ariaLabel "Primary navigation"
                                    prop.children [
                                        Html.button [
                                            prop.className "run-button header-run-button"
                                            prop.text "Run"
                                            prop.disabled (model.CompileState = Compiling)
                                            prop.onClick (fun _ -> dispatch Compile)
                                        ]
                                        Html.a [
                                            prop.href "https://github.com/fsharpforyou/tour"
                                            prop.target "_blank"
                                            prop.rel "noreferrer"
                                            prop.ariaLabel "View source on GitHub"
                                            prop.title "View source on GitHub"
                                            prop.children [
                                                Html.i [ prop.className "fa-brands fa-github"; prop.ariaHidden true ]
                                            ]
                                        ]
                                        Html.button [
                                            let label =
                                                match model.Theme with
                                                | Light -> "Switch to dark mode"
                                                | Dark -> "Switch to light mode"

                                            prop.className "icon-button"
                                            prop.onClick (fun _ -> dispatch ToggleTheme)
                                            prop.ariaPressed (model.Theme = Dark)
                                            prop.ariaLabel label
                                            prop.title label

                                            prop.children [
                                                Html.i [
                                                    prop.className (
                                                        match model.Theme with
                                                        | Light -> "fa-solid fa-moon"
                                                        | Dark -> "fa-solid fa-sun"
                                                    )
                                                    prop.ariaHidden true
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                        Html.main [
                            prop.id "main-content"
                            prop.className "tour-main"
                            prop.children [
                                Html.article [
                                    prop.id "markdown-content"
                                    prop.className "reading-pane"
                                    prop.children [
                                        if model.IsLoadingDocumentation then
                                            Html.div [
                                                prop.className "documentation-loading"
                                                prop.role "status"
                                                prop.children [
                                                    Html.span [ prop.className "loading-spinner"; prop.ariaHidden true ]
                                                    Html.p "Loading lesson…"
                                                ]
                                            ]
                                        else
                                            Markdown.markdown [
                                                markdown.children model.Markdown
                                                markdown.components [
                                                    markdown.components.code (fun props ->
                                                        if props.isInline then
                                                            Html.code props.children
                                                        else
                                                            let syntaxStyle =
                                                                match model.Theme with
                                                                | Light ->
                                                                    import
                                                                        "vs"
                                                                        "react-syntax-highlighter/dist/esm/styles/prism"
                                                                | Dark ->
                                                                    import
                                                                        "vscDarkPlus"
                                                                        "react-syntax-highlighter/dist/esm/styles/prism"

                                                            let language = props.className.Replace("language-", "")

                                                            SyntaxHighlighter.highlighter [
                                                                SyntaxHighlighter.className "markdown-code-block"
                                                                SyntaxHighlighter.language language
                                                                SyntaxHighlighter.style syntaxStyle
                                                                SyntaxHighlighter.customStyle (
                                                                    createObj [
                                                                        "border"
                                                                        ==> match model.Theme with
                                                                            | Light -> "1px solid #b9d8e9"
                                                                            | Dark -> "1px solid #41647d"
                                                                        "background"
                                                                        ==> match model.Theme with
                                                                            | Light -> "#f5f9fc"
                                                                            | Dark -> "#111827"
                                                                        "borderRadius" ==> "0"
                                                                    ]
                                                                )
                                                                SyntaxHighlighter.children props.children
                                                            ])
                                                ]
                                            ]
                                    ]
                                ]
                                Html.section [
                                    prop.className "coding-pane"
                                    prop.children [
                                        Html.div [
                                            prop.className "code-editor"
                                            prop.children [
                                                MonacoEditor.editor [
                                                    MonacoEditor.defaultLanguage "fsharp"
                                                    MonacoEditor.value model.FSharpCode
                                                    MonacoEditor.theme (
                                                        match model.Theme with
                                                        | Light -> "vs"
                                                        | Dark -> "vs-dark"
                                                    )
                                                    MonacoEditor.options (
                                                        createObj [
                                                            "fontSize" ==> 16
                                                            "lineHeight" ==> 27
                                                            "fontFamily"
                                                            ==> "ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace"
                                                            "fontLigatures" ==> false
                                                            "automaticLayout" ==> true
                                                            "minimap" ==> createObj [ "enabled" ==> false ]
                                                            "padding" ==> createObj [ "top" ==> 18; "bottom" ==> 18 ]
                                                            "scrollBeyondLastLine" ==> false
                                                            "renderLineHighlight" ==> "gutter"
                                                        ]
                                                    )
                                                    MonacoEditor.onChange (SetFSharpCode >> dispatch)
                                                    MonacoEditor.onMount (
                                                        Editor.onFSharpEditorDidMount
                                                            model.Worker
                                                            (SetEditor >> dispatch)
                                                    )
                                                ]
                                            ]
                                        ]
                                        Html.section [
                                            prop.className "program-output"
                                            prop.ariaLive.polite
                                            prop.ariaLabel "Program output"
                                            prop.children [
                                                Html.header [
                                                    prop.className "output-header"
                                                    prop.children [ Html.h2 "OUTPUT" ]
                                                ]
                                                Html.div [
                                                    prop.className "output-body"
                                                    prop.role "log"
                                                    prop.ariaLabel "Program output"
                                                    prop.children [
                                                        if not (List.isEmpty model.Logs) then
                                                            for log, level in model.Logs do
                                                                Html.p [
                                                                    prop.className ("line " + LogLevel.cssClass level)
                                                                    prop.text (
                                                                        match level with
                                                                        | LogLevel.Log -> log
                                                                        | LogLevel.Warn -> "WARNING: " + log
                                                                        | LogLevel.Error -> "ERROR: " + log
                                                                    )
                                                                ]
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                        Html.footer [
                            prop.className "tour-footer"
                            prop.children [
                                match model.DocEntryNavigation.PreviousEntry with
                                | None -> Html.span ""
                                | Some entry ->
                                    Html.a [ prop.href (Router.format entry.Route); prop.text $"← {entry.Title}" ]
                                Html.a [
                                    prop.href (Router.format [ "table-of-contents" ])
                                    prop.text "TABLE OF CONTENTS"
                                ]
                                match model.DocEntryNavigation.NextEntry with
                                | None -> Html.span ""
                                | Some entry ->
                                    Html.a [ prop.href (Router.format entry.Route); prop.text $"{entry.Title} →" ]
                            ]
                        ]
                    ]
                ]
                Html.iframe [ prop.src model.IFrameUrl; prop.className "execution-frame" ]
            ]
        ]

ReactDOM.createRoot(document.getElementById "app").render (TourView.AppView())
