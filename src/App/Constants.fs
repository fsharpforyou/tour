module Constants

let host =
#if DEBUG
    Browser.Dom.window.location.href
#else
    // TODO: set this.
    Browser.Dom.window.location.href
#endif

let worker = host + "js/repl/worker.min.js"
let metadata = host + "metadata"
let fableLibrary = host + "js/repl/fable-library"
