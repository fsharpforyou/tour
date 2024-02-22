// Source: https://github.com/fable-compiler/repl/blob/main/src/App/Generator.fs
[<RequireQualifiedAccess>]
module Generator

open System.Text.RegularExpressions
open Fable.Core
open Fable.Core.JsInterop
open Browser.Types
open Browser

[<Global>]
let URL: obj = jsNative

let private bubbleEvents =
    """
<body>
    <script>
    (function () {
        var nativeConsoleLog = console.log;
        var nativeConsoleWarn = console.warn;
        var nativeConsoleError = console.error;
        var origin = window.location.origin;

        console.log = function() {
            var firstArg = arguments[0];
            if (arguments.length === 1 && typeof firstArg === 'string') {
                parent.postMessage({
                    type: 'console_log',
                    content: firstArg
                }, origin);
            }
            nativeConsoleLog.apply(this, arguments);
        };

        console.warn = function() {
            var firstArg = arguments[0];
            if (arguments.length === 1 && typeof firstArg === 'string') {
                parent.postMessage({
                    type: 'console_warn',
                    content: firstArg
                }, origin);
            }
            nativeConsoleWarn.apply(this, arguments);
        };

        console.error = function() {
            var firstArg = arguments[0];
            if (arguments.length === 1 && typeof firstArg === 'string') {
                parent.postMessage({
                    type: 'console_error',
                    content: firstArg
                }, origin);
            }
            nativeConsoleError.apply(this, arguments);
        };
    })();
    </script>
    """
        .Trim()

let generateHtmlBlobUrl (jsCode: string) =
    // We need to convert import paths to absolute urls and add .js at the end if necessary
    let reg = Regex(@"^import (.*)""(fable-library)(.*)""(.*)$", RegexOptions.Multiline)

    let jsCode =
        reg.Replace(
            jsCode,
            fun m ->
                let baseDir = Constants.fableLibrary
                let filename = Regex.Replace(m.Groups.[3].Value, "\.fs$", ".js")

                sprintf
                    "import %s\"%s%s%s\"%s"
                    m.Groups.[1].Value
                    baseDir
                    filename
                    (if filename.EndsWith(".js") then "" else ".js")
                    m.Groups.[4].Value
        )

    let code =
        $"""
{bubbleEvents}
<script type="module">
  {jsCode}
</script>
      """
            .Trim()

    let options = jsOptions<BlobPropertyBag> (fun o -> o.``type`` <- "text/html")
    URL?createObjectURL(Blob.Create([| code |], options))
