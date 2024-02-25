// Source: https://github.com/fable-compiler/repl/blob/main/src/App/Editor.fs
module Editor

open MonacoEditor
open Fable.Standalone
open Fable.WebWorker
open Fable.Core
open Fable.Core.JsInterop

// Source: https://github.com/fable-compiler/repl/blob/main/src/App/Helpers.fs#L14
[<RequireQualifiedAccess>]
module Tooltip =
    open System.Text.RegularExpressions

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
                                        createRange startLine (float startColumn + 1.) endLine (float endColumn + 1.))
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
                                |> Seq.map Tooltip.replaceXml
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

// Source: https://github.com/fable-compiler/repl/blob/372c55b5063c0310433dbb167cd27c540dac2f65/src/App/Main.fs#L878
let onFSharpEditorDidMount
    (worker: ObservableWorker<WorkerAnswer>)
    (setEditor: Monaco.Editor.IStandaloneCodeEditor -> unit)
    =
    System.Func<_, _, _>(fun (editor: Monaco.Editor.IStandaloneCodeEditor) (_: Monaco.IExports) ->
        if not (isNull editor) then
            setEditor editor

            // Because we have access to the monacoModule here,
            // register the different provider needed for F# editor
            let getTooltip line column lineText =
                async {
                    let id = System.Guid.NewGuid()

                    return!
                        worker.PostAndAwaitResponse(
                            GetTooltip(id, int line, int column, lineText),
                            function
                            | FoundTooltip(id2, lines) when id = id2 -> Some lines
                            | _ -> None
                        )
                }

            let tooltipProvider = createTooltipProvider getTooltip

            Monaco.languages.registerHoverProvider (U3.Case1 "fsharp", tooltipProvider)
            |> ignore

            let getDeclarationLocation uri line column lineText =
                async {
                    let id = System.Guid.NewGuid()

                    return!
                        worker.PostAndAwaitResponse(
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

            let definitionProvider = createDefinitionProvider (getDeclarationLocation editorUri)

            Monaco.languages.registerDefinitionProvider (U3.Case1 "fsharp", definitionProvider)
            |> ignore

            let getCompletion line column lineText =
                async {
                    let id = System.Guid.NewGuid()

                    return!
                        worker.PostAndAwaitResponse(
                            GetCompletions(id, int line, int column, lineText),
                            function
                            | FoundCompletions(id2, lines) when id = id2 -> Some lines
                            | _ -> None
                        )
                }

            let completionProvider = createCompletionProvider getCompletion

            Monaco.languages.registerCompletionItemProvider (U3.Case1 "fsharp", completionProvider)
            |> ignore)
