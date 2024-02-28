module Constants

let host =
#if DEBUG
    "http://localhost:5173/"
#else
    "https://fsharpforyou.github.io/tour/"
#endif

let worker = host + "js/repl/worker.min.js"
let metadata = host + "metadata"
let fableLibrary = host + "js/repl/fable-library-js"
let documentation = host + "documentation/"
let tableOfContents = documentation + "table-of-contents.json"
