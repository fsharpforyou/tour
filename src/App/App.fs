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

let fsharpOptions = [|
    "--define:FABLE_COMPILER"
    "--define:FABLE_COMPILER_4"
    "--define:FABLE_COMPILER_JAVASCRIPT"
    "--langversion:preview"
|]


let monacoEditorOptions = {|
    minimap = {| enabled = false |}
    fontSize = 16
    fontFamily = "JetBrains Mono"
|}

importSideEffects "react-toastify/dist/ReactToastify.css"
importSideEffects "./monaco-vite.js"

[<Erase>]
type LzString =
    [<Import("compressToEncodedURIComponent", "lz-string")>]
    static member compressToEncodedURIComponent(input: string) : string = nativeOnly

    [<Import("decompressFromEncodedURIComponent", "lz-string")>]
    static member decompressFromEncodedURIComponent(compressed: string) : string = nativeOnly

[<Erase>]
type MonacoEditor =
    static member inline onChange(f: string -> unit) = Interop.mkAttr "onChange" f
    static member inline theme(value: string) = Interop.mkAttr "theme" value
    static member inline defaultLanguage(value: string) = Interop.mkAttr "defaultLanguage" value
    static member inline value(value: string) = Interop.mkAttr "value" value
    static member inline width(value: string) = Interop.mkAttr "width" value
    static member inline height(value: string) = Interop.mkAttr "height" value

    static member inline options(value: obj) = Interop.mkAttr "options" value

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
module EditorUtils =
    [<RequireQualifiedAccess>]
    type LogLevel =
        | Log
        | Warn
        | Error


    type Model = {
        Logs: (string * LogLevel) list
        FSharpCode: string
        CompiledJavaScript: string
        IFrameIdentifier: string
        IFrameUrl: string
        PlaygroundUrl: string
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

    // TODO: move this to navigation???
    let playgroundUrlComponents fsharpCode =
        let data = LzString.compressToEncodedURIComponent fsharpCode
        [ "playground"; $"?data={data}" ]

    let createPlaygroundUrl fsharpCode =
        Router.format (playgroundUrlComponents fsharpCode)

    [<RequireQualifiedAccess>]
    module LogLevel =
        let toCssColor logLevel =
            match logLevel with
            | LogLevel.Log -> "inherit"
            | LogLevel.Warn -> "darkorange"
            | LogLevel.Error -> "red"

        let fromCompilerError (error: Error) =
            if error.IsWarning then LogLevel.Warn else LogLevel.Error

    let setModelMarkers (editor: Monaco.Editor.IStandaloneCodeEditor) (markers: Monaco.Editor.IMarkerData array) =
        match editor.getModel () with
        | None -> ()
        | Some textModel -> Monaco.editor.setModelMarkers (textModel, "FSharpErrors", ResizeArray markers)

    let toastNotificationFromErrors (errors: Error array) =
        match errors with
        | [||] -> Toastify.success "Compiled successfully."
        | errors when errors |> Array.forall _.IsWarning -> Toastify.warn "Compiled with warnings."
        | _ -> Toastify.error "Failed to compile."

    let initialCommand model =
        Cmd.batch [
            WebWorker.command Msg.SetMarkers Msg.Compiled model.Worker
            Iframe.command model.IFrameIdentifier {
                ConsoleLog = fun text -> Msg.AddConsoleLog(text, LogLevel.Log)
                ConsoleWarn = fun text -> Msg.AddConsoleLog(text, LogLevel.Warn)
                ConsoleError = fun text -> Msg.AddConsoleLog(text, LogLevel.Error)
            }
            Cmd.ofMsg Msg.ParseCode
        ]

    let compile model =
        CompileCode(model.FSharpCode, "javascript", fsharpOptions) |> model.Worker.Post

    let update msg model =
        match msg with
        | Msg.Compile -> { model with Logs = [] }, Cmd.ofEffect (fun _ -> compile model)
        | Msg.ParseCode ->
            model, Cmd.ofEffect (fun _ -> WorkerRequest.ParseCode(model.FSharpCode, fsharpOptions) |> model.Worker.Post)
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
                    PlaygroundUrl = createPlaygroundUrl code // TODO: ??? do we want to do this every time you type?
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

    let init initialFsharpCode =
        fun () ->
            let randomIdentifier = Guid.NewGuid().ToString()

            let worker =
                ObservableWorker(WebWorker.create (), WorkerAnswer.Decoder, randomIdentifier)

            CreateChecker(Constants.metadata, [||], Some ".txt", fsharpOptions)
            |> worker.Post

            let model = {
                Logs = []
                FSharpCode = initialFsharpCode
                CompiledJavaScript = ""
                IFrameIdentifier = randomIdentifier
                IFrameUrl = ""
                PlaygroundUrl = createPlaygroundUrl initialFsharpCode
                Worker = worker
                Editor = Unchecked.defaultof<_>
                Markers = [||]
                Debouncer = Debouncer.create ()
            }

            model, Cmd.batch [ initialCommand model ]

[<RequireQualifiedAccess>]
module Playground =
    [<ReactComponent>]
    let Component editorTheme initialFSharpCode =
        let model, dispatch =
            React.useElmish (EditorUtils.init initialFSharpCode, EditorUtils.update)

        Html.div [
            prop.className "playground"
            prop.children [
                Html.aside [
                    prop.className "sidebar"
                    prop.children [
                        Html.button [
                            prop.onClick (fun _ -> dispatch EditorUtils.Msg.Compile)
                            prop.children [ Html.i [ prop.className "fa-solid fa-play" ] ]
                        ]
                        Html.button [
                            prop.onClick (fun _ ->
                                Router.nav
                                    (EditorUtils.playgroundUrlComponents model.FSharpCode)
                                    HistoryMode.PushState
                                    RouteMode.Hash)
                            prop.children [ Html.i [ prop.className "fa-solid fa-share-from-square" ] ]
                        ]
                        // TODO: Implement this (clear playground)
                        Html.button [ Html.i [ prop.className "fa-solid fa-trash" ] ]
                    ]
                ]
                Html.section [
                    prop.className "editor-group"
                    prop.children [
                        MonacoEditor.editor [
                            MonacoEditor.height "1000px"
                            MonacoEditor.defaultLanguage "fsharp"
                            MonacoEditor.value model.FSharpCode
                            MonacoEditor.theme editorTheme
                            MonacoEditor.onChange (EditorUtils.Msg.SetFSharpCode >> dispatch)
                            MonacoEditor.options monacoEditorOptions
                            MonacoEditor.onMount (
                                Editor.onFSharpEditorDidMount model.Worker (EditorUtils.Msg.SetEditor >> dispatch)
                            )
                        ]
                        match model.Logs with
                        | [] -> Html.none
                        | logs ->
                            Html.article [
                                for (log, level) in logs do
                                    Html.p [ prop.text log ]
                            ]
                        Html.iframe [ prop.id model.IFrameIdentifier; prop.src model.IFrameUrl ]
                    ]
                ]
            ]
        ]

[<RequireQualifiedAccess>]
module DocumentationEditorInstance =
    [<ReactComponent>]
    let Component editorTheme initialFsharpCode =
        let isOutputExpanded, setOutputExpanded = React.useState false

        let model, dispatch =
            React.useElmish (EditorUtils.init initialFsharpCode, EditorUtils.update)

        Html.div [
            prop.className "editor-group"
            prop.children [
                MonacoEditor.editor [
                    MonacoEditor.height "350px"
                    MonacoEditor.defaultLanguage "fsharp"
                    MonacoEditor.value model.FSharpCode
                    MonacoEditor.theme editorTheme
                    MonacoEditor.onChange (EditorUtils.Msg.SetFSharpCode >> dispatch)
                    MonacoEditor.options monacoEditorOptions
                    MonacoEditor.onMount (
                        Editor.onFSharpEditorDidMount model.Worker (EditorUtils.Msg.SetEditor >> dispatch)
                    )
                ]
                Html.div [
                    prop.className "output-group"
                    prop.children [
                        if isOutputExpanded then
                            Html.div [
                                prop.className "output-logs"
                                prop.children [
                                    if List.isEmpty model.Logs then
                                        Html.p "No output."
                                    else
                                        for (log, level) in model.Logs do
                                            Html.p [
                                                prop.className (
                                                    match level with
                                                    | EditorUtils.LogLevel.Log -> "log-level-success"
                                                    | EditorUtils.LogLevel.Warn -> "log-level-warning"
                                                    | EditorUtils.LogLevel.Error -> "log-level-error"
                                                )
                                                prop.text log
                                            ]
                                ]
                            ]
                        else
                            Html.none

                        Html.div [
                            prop.className "output-controls"
                            prop.children [
                                Html.div [
                                    Html.p "Output"
                                    Html.i [
                                        prop.className (
                                            if isOutputExpanded then
                                                "fa-solid fa-caret-down"
                                            else
                                                "fa-solid fa-caret-up"
                                        )
                                        prop.onClick (fun _ -> setOutputExpanded (not isOutputExpanded))
                                    ]
                                ]
                                Html.div [
                                    Html.button [
                                        prop.text "Compile"
                                        prop.onClick (fun _ -> dispatch EditorUtils.Msg.Compile)
                                    ]
                                    Html.a [
                                        prop.href (EditorUtils.createPlaygroundUrl model.FSharpCode)
                                        prop.text "Open in Playground"
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]

                Html.iframe [ prop.id model.IFrameIdentifier; prop.src model.IFrameUrl ]
            ]
        ]

[<RequireQualifiedAccess>]
module Documentation =
    [<RequireQualifiedAccess>]
    type CurrentEntry =
        | Root
        | Entry of Documentation.Entry

    let private formatDocRoute (route: string list) = Router.format ("docs" :: route)

    [<ReactComponent>]
    let Component editorTheme currentEntry tableOfContents =
        let allEntries = Documentation.TableOfContents.allEntries tableOfContents

        let markdownDocumentation, githubUrl, docEntryNavigation =
            match currentEntry with
            | CurrentEntry.Root ->
                tableOfContents.RootMarkdown,
                tableOfContents.RootGitHubUrl,
                {
                    PreviousEntry = None
                    NextEntry = allEntries |> List.tryHead |> Option.map NavigationEntry.fromDocEntry
                }
            | CurrentEntry.Entry entry ->
                let navigation =
                    match getDocEntryNavigation entry allEntries with
                    | {
                          PreviousEntry = None
                          NextEntry = nextEntry
                      } -> {
                        PreviousEntry = Some { Title = "Cover"; Route = [] }
                        NextEntry = nextEntry
                      }
                    | navigation -> navigation

                entry.MarkdownDocumentation, entry.GitHubUrl, navigation

        Html.div [
            prop.className "documentation"
            prop.children [
                Html.aside [
                    prop.className "sidebar"
                    prop.children [
                        Html.nav [
                            for category in tableOfContents.Categories do
                                Html.details [
                                    Html.summary [ Html.strong category.Title ]
                                    Html.ul [
                                        for entry in category.Entries do
                                            Html.li [
                                                Html.a [ prop.href (formatDocRoute entry.Route); prop.text entry.Title ]
                                            ]
                                    ]
                                ]
                        ]
                    ]
                ]
                Html.section [
                    prop.className "markdown-content"
                    prop.children [
                        Markdown.markdown [
                            markdown.children markdownDocumentation
                            markdown.components [
                                // Put the "edit on github" link below the title
                                markdown.components.h1 (fun props ->
                                    React.fragment [
                                        Html.h1 props.children
                                        Html.span [
                                            prop.className "edit-this-page"
                                            prop.children [
                                                Html.i [ prop.className "fa-brands fa-github" ]
                                                Html.a [
                                                    prop.target.blank
                                                    prop.href githubUrl
                                                    prop.children [ Html.small "Edit This Page on GitHub" ]
                                                ]
                                            ]
                                        ]
                                    ])

                                markdown.components.pre (fun props -> React.fragment props.children) // This doesn't wrap our editor instance in a `pre`
                                markdown.components.code (fun props ->
                                    if props.isInline || props.className <> "language-fsharp" then
                                        Html.code props.children
                                    else
                                        // this is an interesting way to get the value of a code block
                                        props.children
                                        |> Seq.tryHead
                                        |> Option.map (string >> DocumentationEditorInstance.Component editorTheme)
                                        |> Option.defaultValue Html.none)
                            ]
                        ]

                        Html.nav [
                            Html.ul [
                                match docEntryNavigation.PreviousEntry with
                                | None -> Html.none
                                | Some entry ->
                                    Html.li [
                                        Html.a [ prop.href (formatDocRoute entry.Route); prop.text $"< {entry.Title}" ]
                                    ]

                                match docEntryNavigation.NextEntry with
                                | None -> Html.none
                                | Some entry ->
                                    Html.li [
                                        Html.a [ prop.href (formatDocRoute entry.Route); prop.text $"{entry.Title} >" ]
                                    ]
                            ]
                        ]
                    ]
                ]
            ]
        ]

[<RequireQualifiedAccess>]
module App =
    [<RequireQualifiedAccess>]
    type Theme =
        | Light
        | Dark

    [<RequireQualifiedAccess>]
    module Theme =
        let toString theme =
            match theme with
            | Theme.Light -> "light"
            | Theme.Dark -> "dark"

        let fromLocalStorage () =
            match localStorage.getItem "theme" with
            | "light" -> Theme.Light
            | "dark"
            | _ -> Theme.Dark

        let saveToLocalStorage theme =
            localStorage.setItem ("theme", toString theme)

        let apply theme =
            document.documentElement.setAttribute ("data-theme", toString theme)

    type Model = {
        CurrentUrl: string list
        CurrentPage: Navigation.Page
        CurrentTheme: Theme
        TableOfContents: Documentation.TableOfContents
    }

    [<RequireQualifiedAccess>]
    type Msg =
        | SetUrl of string list
        | FetchedTableOfContents of Documentation.TableOfContents
        | ToggleTheme

    let getPageFromUrl tableOfContents url =
        Page.fromUrl (Documentation.TableOfContents.allEntries tableOfContents) url

    let init () =
        let currentTheme = Theme.fromLocalStorage ()
        let tableOfContents = Documentation.emptyTableOfContents

        {
            CurrentUrl = []
            CurrentPage = Page.Homepage
            CurrentTheme = currentTheme
            TableOfContents = tableOfContents
        },
        Cmd.batch [
            Cmd.ofEffect (fun _ -> Theme.apply currentTheme)
            Cmd.OfPromise.perform Documentation.loadTableOfContents () Msg.FetchedTableOfContents
        ]

    let update msg model =
        match msg with
        | Msg.SetUrl url ->
            let currentPage = getPageFromUrl model.TableOfContents url

            {
                model with
                    CurrentUrl = url
                    CurrentPage = currentPage
            },
            Cmd.none
        | Msg.FetchedTableOfContents tableOfContents ->
            let currentPage = getPageFromUrl tableOfContents model.CurrentUrl

            {
                model with
                    CurrentPage = currentPage
                    TableOfContents = tableOfContents
            },
            Cmd.none
        | Msg.ToggleTheme ->
            let nextTheme =
                match model.CurrentTheme with
                | Theme.Light -> Theme.Dark
                | Theme.Dark -> Theme.Light

            { model with CurrentTheme = nextTheme },
            Cmd.ofEffect (fun _ ->
                Theme.saveToLocalStorage nextTheme
                Theme.apply nextTheme)

    [<ReactComponent>]
    let Component () =
        let model, dispatch = React.useElmish (init, update)

        let editorTheme =
            match model.CurrentTheme with
            | Theme.Light -> "vs"
            | Theme.Dark -> "vs-dark"

        React.router [
            router.onUrlChanged (Msg.SetUrl >> dispatch)
            router.children [
                Html.header [
                    Html.nav [
                        Html.ul [
                            prop.className "branding"
                            prop.children [
                                Html.li [
                                    Html.img [ prop.src "img/fsharp.png" ]
                                    Html.a [ prop.href (Router.format []); prop.text "F# For You" ]
                                ]
                            ]
                        ]
                        Html.ul [
                            Html.li [ Html.a [ prop.href (Router.format [ "docs" ]); prop.text "Docs" ] ]
                            Html.li [
                                Html.a [ prop.href (Router.format [ "playground" ]); prop.text "Playground" ]
                            ]
                        ]
                        Html.ul [
                            Html.li [
                                Html.button [
                                    Html.a [
                                        prop.target.blank
                                        prop.href "https://github.com/fsharpforyou/tour"
                                        prop.children [ Html.i [ prop.className "fa-brands fa-github" ] ]
                                    ]
                                ]
                            ]
                            Html.li [
                                Html.button [
                                    prop.onClick (fun _ -> dispatch Msg.ToggleTheme)
                                    prop.children [
                                        Html.i [
                                            prop.className (
                                                match model.CurrentTheme with
                                                | Theme.Light -> "fa-solid fa-moon"
                                                | Theme.Dark -> "fa-solid fa-sun"
                                            )
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
                Html.main [
                    match model.CurrentPage with
                    | Page.Docs ->
                        Documentation.Component editorTheme Documentation.CurrentEntry.Root model.TableOfContents
                    | Page.DocsEntry entry ->
                        Documentation.Component
                            editorTheme
                            (Documentation.CurrentEntry.Entry entry)
                            model.TableOfContents
                    | Page.Homepage -> Html.p "Homepage"
                    | Page.Playground data ->
                        let initialCode =
                            data
                            |> Option.map LzString.decompressFromEncodedURIComponent
                            |> Option.defaultValue ""

                        Playground.Component editorTheme initialCode
                    | Page.NotFound -> Html.p "Not found"

                    Toastify.container [
                        ContainerOption.autoClose 2000
                        ContainerOption.position Position.BottomRight
                        ContainerOption.theme (
                            match model.CurrentTheme with
                            | Theme.Light -> Fable.ReactToastify.Theme.Light
                            | Theme.Dark -> Fable.ReactToastify.Theme.Dark
                        )
                    ]
                ]
            ]
        ]

ReactDOM.createRoot(document.getElementById "app").render (App.Component())
