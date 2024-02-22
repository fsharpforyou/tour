open Feliz
open Feliz.UseElmish
open Elmish
open Browser
open Fable.Core
open Fable.Core.JsInterop
open Thoth.Json
open Fable.WebWorker
open Fable.Standalone
open Fable.ReactToastify

importSideEffects "react-toastify/dist/ReactToastify.css"

[<RequireQualifiedAccess>]
type Theme =
    | Light
    | Dark

[<RequireQualifiedAccess>]
module Theme =
    let toggle theme =
        match theme with
        | Theme.Light -> Theme.Dark
        | Theme.Dark -> Theme.Light

    let toMonacoTheme theme =
        match theme with
        | Theme.Light -> "vs"
        | Theme.Dark -> "vs-dark"

    let loadFromLocalStorage () =
        match localStorage.getItem "theme" with
        | "dark" -> Theme.Dark
        | "light"
        | _ -> Theme.Light

    let saveToLocalStorage theme =
        localStorage.setItem (
            "theme",
            match theme with
            | Theme.Light -> "light"
            | Theme.Dark -> "dark"
        )

type Model = {
    Logs: string list
    FSharpCode: string
    CompiledJavaScript: string
    IFrameUrl: string
    Theme: Theme
    Worker: ObservableWorker<WorkerAnswer>
}

[<RequireQualifiedAccess>]
type LogLevel =
    | Log
    | Warn
    | Error

type Msg =
    | Compile
    | SetIFrameUrl of string
    | SetFSharpCode of string
    | AddConsoleLog of LogLevel * string
    | Compiled of code: string * language: string * errors: Error array * stats: CompileStats
    | ToggleTheme

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
                | Loaded version -> printfn "Loaded %s" version
                | LoadFailed -> printfn "Load failed"
                | ParsedCode errors -> printfn "Parsed code: %A" errors
                | CompilationFinished(code, lang, errors, stats) -> dispatch (Compiled(code, lang, errors, stats))
                | CompilationsFinished(code, lang, errors, stats) -> printfn "Compilations finished: %A" code
                | CompilerCrashed msg -> printfn "Compiler crashed: %s" msg
                | FoundTooltip _ -> ()
                | FoundCompletions _ -> ()
                | FoundDeclarationLocation _ -> ())

        [ handler ]

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
        ]

    {
        Logs = []
        FSharpCode = ""
        CompiledJavaScript = ""
        IFrameUrl = ""
        Theme = Theme.loadFromLocalStorage ()
        Worker = worker
    },
    cmd

let compile model =
    let language = "javascript"
    let fsharpOptions = [||]
    CompileCode(model.FSharpCode, language, fsharpOptions) |> model.Worker.Post

let update msg model =
    match msg with
    | Compile -> { model with Logs = [] }, Cmd.ofEffect (fun _ -> compile model)
    | SetIFrameUrl url -> { model with IFrameUrl = url }, Cmd.none
    | SetFSharpCode code -> { model with FSharpCode = code }, Cmd.none
    | AddConsoleLog(level, output) ->
        let logs = output :: model.Logs
        { model with Logs = logs }, Cmd.none
    | Compiled(code, lang, errors, stats) ->
        let compiledSuccessfully = errors.Length = 0

        let toastCommand =
            Cmd.ofEffect (fun _ ->
                if compiledSuccessfully then
                    Toastify.success "Compiled Successfully." |> ignore
                else
                    Toastify.error "There were errors :(" |> ignore)

        // TODO: Handle errors and stats.
        printfn "Code: %s, Lang: %s, Errors: %A, Stats: %A" code lang errors stats
        let model = { model with CompiledJavaScript = code }

        model,
        Cmd.batch [
            toastCommand
            Cmd.OfFunc.perform Generator.generateHtmlBlobUrl model.CompiledJavaScript SetIFrameUrl
        ]
    | ToggleTheme ->
        let newTheme = Theme.toggle model.Theme
        { model with Theme = newTheme }, Cmd.ofEffect (fun _ -> Theme.saveToLocalStorage newTheme)

module MonacoEditor =
    [<Erase>]
    type props =
        static member inline onChange(f: string -> unit) = Interop.mkAttr "onChange" f
        static member inline theme(value: string) = Interop.mkAttr "theme" value
        static member inline defaultLanguage(value: string) = Interop.mkAttr "defaultLanguage" value
        static member inline value(value: string) = Interop.mkAttr "value" value
        static member inline width(value: string) = Interop.mkAttr "width" value
        static member inline height(value: string) = Interop.mkAttr "height" value

    [<Erase>]
    type editor =
        static member inline editor(properties: IReactProperty list) =
            Interop.reactApi.createElement (import "Editor" "@monaco-editor/react", createObj !!properties)

module View =
    [<ReactComponent>]
    let AppView () =
        let model, dispatch = React.useElmish (init, update)

        Html.div [
            Html.button [ prop.text "Toggle theme"; prop.onClick (fun _ -> dispatch ToggleTheme) ]
            Html.button [ prop.text "Compile Code"; prop.onClick (fun _ -> dispatch Compile) ]
            MonacoEditor.editor.editor [
                MonacoEditor.props.defaultLanguage "fsharp"
                MonacoEditor.props.height "90vh"
                MonacoEditor.props.width "90vw"
                MonacoEditor.props.value model.FSharpCode
                MonacoEditor.props.theme (Theme.toMonacoTheme model.Theme)
                MonacoEditor.props.onChange (SetFSharpCode >> dispatch)
            ]
            Html.div [
                Html.h4 "Output:"
                for log in model.Logs do
                    Html.p log
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
                ContainerOption.theme (
                    match model.Theme with
                    | Theme.Light -> Fable.ReactToastify.Theme.Light
                    | Theme.Dark -> Fable.ReactToastify.Theme.Dark
                )
            ]
        ]

ReactDOM.createRoot(document.getElementById ("app")).render (View.AppView())
