open Feliz
open Feliz.UseElmish
open Feliz.Router
open Thoth.Elmish
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
open System

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
    Debouncer: Debouncer.State
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
    | SetMarkers of Monaco.Editor.IMarkerData array
    | SetEditor of Monaco.Editor.IStandaloneCodeEditor
    | DebouncerSelfMsg of Debouncer.SelfMessage<Msg>
    | ParseCode

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

// Source: https://github.com/fable-compiler/repl/blob/main/src/App/Editor.fs
module MonacoEditor =
    open System.Text.RegularExpressions

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

    let inline As<'T> (x: obj) = x :?> 'T

    let private stringReplacePatterns = [
        "&lt;", "<"
        "&gt;", ">"
        "&quot;", "\""
        "&apos;", "'"
        "&amp;", "&"
        "<summary>", "**Description**\n\n"
        "</summary>", "\n"
        "<para>", "\n"
        "</para>", "\n"
        "<remarks>", ""
        "</remarks>", "\n"
    ]

    let private regexReplacePatterns =
        let r pat = Regex(pat, RegexOptions.IgnoreCase)

        let code (strings: string array) =
            let str = strings.[0]

            if str.Contains("\n") then
                "```forceNoHighlight" + str + "```"
            else
                "`" + str + "`"

        let returns = Array.item 0 >> sprintf "\n**Returns**\n\n%s"

        let param (s: string[]) =
            sprintf "* `%s`: %s" (s.[0].Substring(1, s.[0].Length - 2)) s.[1]

        [
            r "<c>((?:(?!<c>)(?!<\/c>)[\s\S])*)<\/c>", code
            r """<see\s+cref=(?:'[^']*'|"[^"]*")>((?:(?!<\/see>)[\s\S])*)<\/see>""", code
            r """<param\s+name=('[^']*'|"[^"]*")>((?:(?!<\/param>)[\s\S])*)<\/param>""", param
            r """<typeparam\s+name=('[^']*'|"[^"]*")>((?:(?!<\/typeparam>)[\s\S])*)<\/typeparam>""", param
            r """<exception\s+cref=('[^']*'|"[^"]*")>((?:(?!<\/exception>)[\s\S])*)<\/exception>""", param
            r """<a\s+href=('[^']*'|"[^"]*")>((?:(?!<\/a>)[\s\S])*)<\/a>""",
            fun s -> (s.[0].Substring(1, s.[0].Length - 2))

            r "<returns>((?:(?!<\/returns>)[\s\S])*)<\/returns>", returns
        ]

    /// Helpers to create a new section in the markdown comment
    let private suffixXmlKey (tag: string) (value: string) (str: string) =
        match str.IndexOf(tag) with
        | x when x <> -1 ->
            let insertAt = if str.Chars(x - 1) = ' ' then x - 1 else x

            str.Insert(insertAt, value)
        | _ -> str

    let private suffixTypeparam = suffixXmlKey "<typeparam" "\n**Type parameters**\n\n"

    let private suffixException = suffixXmlKey "<exception" "\n**Exceptions**\n\n"

    let private suffixParam = suffixXmlKey "<param" "\n**Parameters**\n\n"

    /// Replaces XML tags with Markdown equivalents.
    /// List of standard tags: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/xml-documentation
    let replaceXml (str: string) : string =
        let str = str |> suffixTypeparam |> suffixException |> suffixParam

        let res =
            regexReplacePatterns
            |> List.fold
                (fun res (regex: Regex, formatter: string[] -> string) ->
                    // repeat replacing with same pattern to handle nested tags, like `<c>..<c>..</c>..</c>`
                    let rec loop res : string =
                        match regex.Match res with
                        | m when m.Success ->
                            m.Groups
                            |> Seq.cast<Group>
                            |> Seq.map (fun g -> g.Value)
                            |> Seq.toArray
                            |> Array.splitAt 1
                            |> function
                                | [| firstGroup |], otherGroups ->
                                    loop <| res.Replace(firstGroup, formatter otherGroups)
                                | _ -> res
                        | _ -> res

                    loop res)
                str

        stringReplacePatterns
        |> List.fold (fun (res: string) (oldValue, newValue) -> res.Replace(oldValue, newValue)) res

    let convertGlyph glyph =
        match glyph with
        | Glyph.Class -> Monaco.Languages.CompletionItemKind.Class
        | Glyph.Enum -> Monaco.Languages.CompletionItemKind.Enum
        | Glyph.Value -> Monaco.Languages.CompletionItemKind.Value
        | Glyph.Variable -> Monaco.Languages.CompletionItemKind.Variable
        | Glyph.Interface -> Monaco.Languages.CompletionItemKind.Interface
        | Glyph.Module -> Monaco.Languages.CompletionItemKind.Module
        | Glyph.Method -> Monaco.Languages.CompletionItemKind.Method
        | Glyph.Property -> Monaco.Languages.CompletionItemKind.Property
        | Glyph.Field -> Monaco.Languages.CompletionItemKind.Field
        | Glyph.Function -> Monaco.Languages.CompletionItemKind.Function
        | Glyph.Error
        | Glyph.Event -> Monaco.Languages.CompletionItemKind.Text
        | Glyph.TypeParameter -> Monaco.Languages.CompletionItemKind.TypeParameter


    let inline completionList suggestions =
        jsOptions<Monaco.Languages.CompletionList> (fun o -> o.suggestions <- suggestions)

    let inline createRange startLineNumber startColumn endLineNumber endColumn =
        {|
            startLineNumber = startLineNumber
            startColumn = startColumn
            endLineNumber = endLineNumber
            endColumn = endColumn
        |}
        |> As<Monaco.IRange>

    let createCompletionProvider getCompletions =
        { new Monaco.Languages.CompletionItemProvider with
            member this.provideCompletionItems
                (
                    model: Monaco.Editor.ITextModel,
                    position: Monaco.Position,
                    context: Monaco.Languages.CompletionContext,
                    token: Monaco.CancellationToken
                ) : Monaco.Languages.ProviderResult<Monaco.Languages.CompletionList> =
                async {
                    let lineText = model.getLineContent (position.lineNumber)
                    let! completions = getCompletions position.lineNumber position.column lineText

                    return
                        completions
                        |> Array.map (fun (c: Fable.Standalone.Completion) ->
                            jsOptions<Monaco.Languages.CompletionItem> (fun ci ->
                                ci.label <- U2.Case1 c.Name
                                ci.kind <- convertGlyph c.Glyph
                                ci.insertText <- c.Name))
                        |> ResizeArray
                        |> completionList
                }
                |> Async.StartAsPromise
                |> Promise.map Some
                |> U2.Case2
                |> Some

            member this.resolveCompletionItem
                (
                    item: Monaco.Languages.CompletionItem,
                    token: Monaco.CancellationToken
                ) : Monaco.Languages.ProviderResult<Monaco.Languages.CompletionItem> =
                item |> U2.Case1 |> Some

            member this.triggerCharacters
                with get (): ResizeArray<string> option = Some(ResizeArray [| "." |])
                and set (v: ResizeArray<string> option): unit = failwith "Not Implemented"
        }

    let createDefinitionProvider getDeclarationLocation =
        { new Monaco.Languages.DefinitionProvider with
            member this.provideDefinition
                (
                    model: Monaco.Editor.ITextModel,
                    position: Monaco.Position,
                    token: Monaco.CancellationToken
                ) : Monaco.Languages.ProviderResult<U2<Monaco.Languages.Definition, ResizeArray<Monaco.Languages.LocationLink>>> =
                async {
                    let lineText = model.getLineContent (position.lineNumber)
                    let! loc = getDeclarationLocation position.lineNumber position.column lineText

                    match loc with
                    | Some(uri, startLine, startColumn, endLine, endColumn) ->
                        return
                            U2.Case1(
                                U3.Case1(
                                    jsOptions (fun (loc2: Monaco.Languages.Location) ->
                                        loc2.uri <- uri

                                        loc2.range <-
                                            createRange
                                                startLine
                                                (float startColumn + 1.)
                                                endLine
                                                (float endColumn + 1.))
                                )
                            )
                    | None -> return U2.Case1(U3.Case1(null))
                }
                |> Async.StartAsPromise
                |> Promise.map Some
                |> U2.Case2
                |> Some

        }

    let createTooltipProvider getTooltip =
        { new Monaco.Languages.HoverProvider with
            member __.provideHover(doc, pos, _) =
                async {
                    match doc.getWordAtPosition (pos :?> Monaco.IPosition) with
                    | Some w ->
                        let lineText = doc.getLineContent (pos.lineNumber)
                        let! lines = getTooltip pos.lineNumber pos.column lineText

                        let range: Monaco.IRange =
                            createRange pos.lineNumber w.startColumn pos.lineNumber w.endColumn

                        return
                            jsOptions<Monaco.Languages.Hover> (fun h ->
                                h.contents <-
                                    lines
                                    |> Seq.map replaceXml
                                    |> Seq.mapi (fun i line ->
                                        {|
                                            value = if i = 0 then "```fsharp\n" + line + "\n```\n" else line
                                        |}
                                        |> As<Monaco.IMarkdownString>)
                                    |> Seq.toArray
                                    |> ResizeArray

                                h.range <- Some range)
                    | None -> return createEmpty<Monaco.Languages.Hover>
                }
                |> Async.StartAsPromise
                |> Promise.map Some
                |> U2.Case2
                |> Some
        }


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

module WebWorker =
    let create () = Worker.Create(Constants.worker)

    let command (worker: ObservableWorker<_>) =
        let handler dispatch =
            worker
            |> Observable.add (function
                | Loaded version -> ()
                | LoadFailed -> ()
                | ParsedCode errors -> errors |> MonacoEditor.mapErrorToMarker |> SetMarkers |> dispatch
                | CompilationFinished(code, lang, errors, stats) -> dispatch (Compiled(code, lang, errors, stats))
                | CompilationsFinished(code, lang, errors, stats) -> ()
                | CompilerCrashed msg -> ()
                | FoundTooltip _ -> ()
                | FoundCompletions _ -> ()
                | FoundDeclarationLocation _ -> ())

        [ handler ]

let getCurrentPage tableOfContents url =
    let flattenCategories = List.collect _.Pages
    let pages = flattenCategories tableOfContents.Categories
    Page.fromUrl pages url

// Source: https://github.com/fable-compiler/repl/blob/372c55b5063c0310433dbb167cd27c540dac2f65/src/App/Main.fs#L878
let onFSharpEditorDidMount model dispatch =
    System.Func<_, _, _>(fun (editor: Monaco.Editor.IStandaloneCodeEditor) (_: Monaco.IExports) ->
        if not (isNull editor) then
            dispatch (SetEditor editor)

            // Because we have access to the monacoModule here,
            // register the different provider needed for F# editor
            let getTooltip line column lineText =
                async {
                    let id = System.Guid.NewGuid()

                    return!
                        model.Worker.PostAndAwaitResponse(
                            GetTooltip(id, int line, int column, lineText),
                            function
                            | FoundTooltip(id2, lines) when id = id2 -> Some lines
                            | _ -> None
                        )
                }

            let tooltipProvider = MonacoEditor.createTooltipProvider getTooltip

            Monaco.languages.registerHoverProvider (U3.Case1 "fsharp", tooltipProvider)
            |> ignore

            let getDeclarationLocation uri line column lineText =
                async {
                    let id = System.Guid.NewGuid()

                    return!
                        model.Worker.PostAndAwaitResponse(
                            GetDeclarationLocation(id, int line, int column, lineText),
                            function
                            | FoundDeclarationLocation(id2, res) when id = id2 ->
                                res
                                |> Option.map (fun (line1, col1, line2, col2) ->
                                    uri, float line1, col1, float line2, col2)
                                |> Some
                            | _ -> None
                        )
                }

            let editorUri =
                editor.getModel ()
                |> Option.map (fun x -> x.uri)
                |> Option.defaultValue Unchecked.defaultof<_>

            let definitionProvider =
                MonacoEditor.createDefinitionProvider (getDeclarationLocation editorUri)

            Monaco.languages.registerDefinitionProvider (U3.Case1 "fsharp", definitionProvider)
            |> ignore

            let getCompletion line column lineText =
                async {
                    let id = System.Guid.NewGuid()

                    return!
                        model.Worker.PostAndAwaitResponse(
                            GetCompletions(id, int line, int column, lineText),
                            function
                            | FoundCompletions(id2, lines) when id = id2 -> Some lines
                            | _ -> None
                        )
                }

            let completionProvider = MonacoEditor.createCompletionProvider getCompletion

            Monaco.languages.registerCompletionItemProvider (U3.Case1 "fsharp", completionProvider)
            |> ignore)

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
        Debouncer = Debouncer.create ()
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

let setModelMarkers (model: Model) =
    match model.Editor.getModel () with
    | None -> ()
    | Some textModel -> Monaco.editor.setModelMarkers (textModel, "FSharpErrors", ResizeArray model.Markers)

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
        let cmd = Cmd.ofEffect (fun _ -> setModelMarkers model)
        model, cmd
    | AddConsoleLog(level, output) ->
        let logs = (output, level) :: model.Logs
        { model with Logs = logs }, Cmd.none
    | Compiled(code, _, errors, stats) ->
        let isSuccess = errors.Length = 0

        let toastCmd: Cmd<Msg> =
            Cmd.ofEffect (fun _ ->
                if isSuccess then
                    Toastify.success "Compiled Successfully."
                else
                    Toastify.error "Failed to Compile."
                |> ignore)

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
        }

        model,
        Cmd.batch [
            toastCmd
            Cmd.ofMsg (SetMarkers(MonacoEditor.mapErrorToMarker errors))
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
        let fsharpCode = calculateFSharpCodeValue model.CurrentPage
        let markdown = calculateMarkdownValue model.TableOfContents model.CurrentPage
        // clear the logs when we calculate new values.
        { model with Logs = [] },
        // this has useful side-effects like triggering an initial parse through the `SetFSharpCode` msg.
        Cmd.batch [ Cmd.ofMsg (SetFSharpCode fsharpCode); Cmd.ofMsg (SetMarkdown markdown) ]
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
    | DebouncerSelfMsg debouncerMsg ->
        let (debouncerModel, debouncerCmd) = Debouncer.update debouncerMsg model.Debouncer

        {
            model with
                Debouncer = debouncerModel
        },
        debouncerCmd

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
                                            MonacoEditor.props.onMount (onFSharpEditorDidMount model dispatch)
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
