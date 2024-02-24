// ts2fable 0.9.0
module rec MonacoEditor

#nowarn "3390" // disable warnings for invalid XML comments
#nowarn "0044" // disable warnings for `Obsolete` usage

open System
open Fable.Core
open Fable.Core.JS
open Browser.Types

type Array<'T> = System.Collections.Generic.IList<'T>
type PromiseLike<'T> = Fable.Core.JS.Promise<'T>
type ReadonlyArray<'T> = System.Collections.Generic.IReadOnlyList<'T>
type RegExp = System.Text.RegularExpressions.Regex

[<Erase>]
type Record<'k, 't> =
    interface
    end

[<Import("MonacoEnvironment", "module")>]
let MonacoEnvironment: Monaco.Environment option = jsNative

[<AllowNullLiteral>]
type Window =
    abstract MonacoEnvironment: Monaco.Environment option with get, set

module Monaco =
    [<Import("editor", "monaco-editor")>]
    let editor: Editor.IExports = jsNative

    [<Import("languages", "monaco-editor")>]
    let languages: Languages.IExports = jsNative

    [<AllowNullLiteral>]
    type IExports =
        /// A helper that allows to emit and listen to typed events
        abstract Emitter: EmitterStatic
        abstract CancellationTokenSource: CancellationTokenSourceStatic
        /// <summary>
        /// Uniform Resource Identifier (Uri) <see href="http://tools.ietf.org/html/rfc3986." />
        /// This class is a simple parser which creates the basic component parts
        /// (<see href="http://tools.ietf.org/html/rfc3986#section-3)" /> with minimal validation
        /// and encoding.
        ///
        /// <code lang="txt">
        ///       foo://example.com:8042/over/there?name=ferret#nose
        ///       \_/   \______________/\_________/ \_________/ \__/
        ///        |           |            |            |        |
        ///     scheme     authority       path        query   fragment
        ///        |   _____________________|__
        ///       / \ /                        \
        ///       urn:example:animal:ferret:nose
        /// </code>
        /// </summary>
        abstract Uri: UriStatic
        abstract KeyMod: KeyModStatic
        /// A position in the editor.
        abstract Position: PositionStatic
        /// A range in the editor. (startLineNumber,startColumn) is <= (endLineNumber,endColumn)
        abstract Range: RangeStatic
        /// A selection in the editor.
        /// The selection is a range that has an orientation.
        abstract Selection: SelectionStatic
        abstract Token: TokenStatic

    type Thenable<'T> = PromiseLike<'T>

    [<AllowNullLiteral>]
    type Environment =
        /// <summary>
        /// Define a global <c>monaco</c> symbol.
        /// This is true by default in AMD and false by default in ESM.
        /// </summary>
        abstract globalAPI: bool option with get, set
        /// The base url where the editor sources are found (which contains the vs folder)
        abstract baseUrl: string option with get, set
        /// <summary>
        /// A web worker factory.
        /// NOTE: If <c>getWorker</c> is defined, <c>getWorkerUrl</c> is not invoked.
        /// </summary>
        abstract getWorker: workerId: string * label: string -> U2<Promise<Worker>, Worker>
        /// <summary>
        /// Return the location for web worker scripts.
        /// NOTE: If <c>getWorker</c> is defined, <c>getWorkerUrl</c> is not invoked.
        /// </summary>
        abstract getWorkerUrl: workerId: string * label: string -> string

        /// Create a trusted types policy (same API as window.trustedTypes.createPolicy)
        abstract createTrustedTypesPolicy:
            policyName: string * ?policyOptions: ITrustedTypePolicyOptions -> ITrustedTypePolicy option

    [<AllowNullLiteral>]
    type ITrustedTypePolicyOptions =
        abstract createHTML: (string -> ResizeArray<obj option> -> string) option with get, set
        abstract createScript: (string -> ResizeArray<obj option> -> string) option with get, set
        abstract createScriptURL: (string -> ResizeArray<obj option> -> string) option with get, set

    [<AllowNullLiteral>]
    type ITrustedTypePolicy =
        abstract name: string
        abstract createHTML: input: string -> obj option
        abstract createScript: input: string -> obj option
        abstract createScriptURL: input: string -> obj option

    [<AllowNullLiteral>]
    type IDisposable =
        abstract dispose: unit -> unit

    [<AllowNullLiteral>]
    type IEvent<'T> =
        [<Emit("$0($1...)")>]
        abstract Invoke: listener: ('T -> obj option) * ?thisArg: obj -> IDisposable

    /// A helper that allows to emit and listen to typed events
    [<AllowNullLiteral>]
    type Emitter<'T> =
        abstract ``event``: IEvent<'T>
        abstract fire: ``event``: 'T -> unit
        abstract dispose: unit -> unit

    /// A helper that allows to emit and listen to typed events
    [<AllowNullLiteral>]
    type EmitterStatic =
        [<EmitConstructor>]
        abstract Create: unit -> Emitter<'T>

    [<RequireQualifiedAccess>]
    type MarkerTag =
        | Unnecessary = 1
        | Deprecated = 2

    [<RequireQualifiedAccess>]
    type MarkerSeverity =
        | Hint = 1
        | Info = 2
        | Warning = 4
        | Error = 8

    [<AllowNullLiteral>]
    type CancellationTokenSource =
        abstract token: CancellationToken
        abstract cancel: unit -> unit
        abstract dispose: ?cancel: bool -> unit

    [<AllowNullLiteral>]
    type CancellationTokenSourceStatic =
        [<EmitConstructor>]
        abstract Create: ?parent: CancellationToken -> CancellationTokenSource

    [<AllowNullLiteral>]
    type CancellationToken =
        /// A flag signalling is cancellation has been requested.
        abstract isCancellationRequested: bool

        /// <summary>
        /// An event which fires when cancellation is requested. This event
        /// only ever fires <c>once</c> as cancellation can only happen once. Listeners
        /// that are registered after cancellation will be called (next event loop run),
        /// but also only once.
        /// </summary>
        abstract onCancellationRequested:
            (obj option -> obj option) -> (obj) option -> (ResizeArray<IDisposable>) option -> IDisposable

    /// <summary>
    /// Uniform Resource Identifier (Uri) <see href="http://tools.ietf.org/html/rfc3986." />
    /// This class is a simple parser which creates the basic component parts
    /// (<see href="http://tools.ietf.org/html/rfc3986#section-3)" /> with minimal validation
    /// and encoding.
    ///
    /// <code lang="txt">
    ///       foo://example.com:8042/over/there?name=ferret#nose
    ///       \_/   \______________/\_________/ \_________/ \__/
    ///        |           |            |            |        |
    ///     scheme     authority       path        query   fragment
    ///        |   _____________________|__
    ///       / \ /                        \
    ///       urn:example:animal:ferret:nose
    /// </code>
    /// </summary>
    [<AllowNullLiteral>]
    type Uri =
        inherit UriComponents
        /// <summary>
        /// scheme is the 'http' part of '<see href="http://www.example.com/some/path?query#fragment'." />
        /// The part before the first colon.
        /// </summary>
        abstract scheme: string
        /// <summary>
        /// authority is the 'www.example.com' part of '<see href="http://www.example.com/some/path?query#fragment'." />
        /// The part between the first double slashes and the next slash.
        /// </summary>
        abstract authority: string
        /// <summary>path is the '/some/path' part of '<see href="http://www.example.com/some/path?query#fragment'." /></summary>
        abstract path: string
        /// <summary>query is the 'query' part of '<see href="http://www.example.com/some/path?query#fragment'." /></summary>
        abstract query: string
        /// <summary>fragment is the 'fragment' part of '<see href="http://www.example.com/some/path?query#fragment'." /></summary>
        abstract fragment: string
        /// <summary>
        /// Returns a string representing the corresponding file system path of this Uri.
        /// Will handle UNC paths, normalizes windows drive letters to lower-case, and uses the
        /// platform specific path separator.
        ///
        /// * Will *not* validate the path for invalid characters and semantics.
        /// * Will *not* look at the scheme of this Uri.
        /// * The result shall *not* be used for display purposes but for accessing a file on disk.
        ///
        ///
        /// The *difference* to <c>Uri#path</c> is the use of the platform specific separator and the handling
        /// of UNC paths. See the below sample of a file-uri with an authority (UNC path).
        ///
        /// <code lang="ts">
        ///  const u = Uri.parse('file://server/c$/folder/file.txt')
        ///  u.authority === 'server'
        ///  u.path === '/shares/c$/file.txt'
        ///  u.fsPath === '\\server\c$\folder\file.txt'
        /// </code>
        ///
        /// Using <c>Uri#path</c> to read a file (using fs-apis) would not be enough because parts of the path,
        /// namely the server name, would be missing. Therefore <c>Uri#fsPath</c> exists - it's sugar to ease working
        /// with URIs that represent files on disk (<c>file</c> scheme).
        /// </summary>
        abstract fsPath: string
        abstract ``with``: change: UriWithChange -> Uri
        /// <summary>
        /// Creates a string representation for this Uri. It's guaranteed that calling
        /// <c>Uri.parse</c> with the result of this function creates an Uri which is equal
        /// to this Uri.
        ///
        /// * The result shall *not* be used for display purposes but for externalization or transport.
        /// * The result will be encoded using the percentage encoding and encoding happens mostly
        /// ignore the scheme-specific encoding rules.
        /// </summary>
        /// <param name="skipEncoding">Do not encode the result, default is <c>false</c></param>
        abstract toString: ?skipEncoding: bool -> string
        abstract toJSON: unit -> UriComponents

    [<AllowNullLiteral>]
    type UriWithChange =
        abstract scheme: string option with get, set
        abstract authority: string option with get, set
        abstract path: string option with get, set
        abstract query: string option with get, set
        abstract fragment: string option with get, set

    /// <summary>
    /// Uniform Resource Identifier (Uri) <see href="http://tools.ietf.org/html/rfc3986." />
    /// This class is a simple parser which creates the basic component parts
    /// (<see href="http://tools.ietf.org/html/rfc3986#section-3)" /> with minimal validation
    /// and encoding.
    ///
    /// <code lang="txt">
    ///       foo://example.com:8042/over/there?name=ferret#nose
    ///       \_/   \______________/\_________/ \_________/ \__/
    ///        |           |            |            |        |
    ///     scheme     authority       path        query   fragment
    ///        |   _____________________|__
    ///       / \ /                        \
    ///       urn:example:animal:ferret:nose
    /// </code>
    /// </summary>
    [<AllowNullLiteral>]
    type UriStatic =
        [<EmitConstructor>]
        abstract Create: unit -> Uri

        abstract isUri: thing: obj option -> bool
        /// <summary>
        /// Creates a new Uri from a string, e.g. <c>http://www.example.com/some/path</c>,
        /// <c>file:///usr/home</c>, or <c>scheme:with/path</c>.
        /// </summary>
        /// <param name="value">A string which represents an Uri (see <c>Uri#toString</c>).</param>
        abstract parse: value: string * ?_strict: bool -> Uri
        /// <summary>
        /// Creates a new Uri from a file system path, e.g. <c>c:\my\files</c>,
        /// <c>/usr/home</c>, or <c>\\server\share\some\path</c>.
        ///
        /// The *difference* between <c>Uri#parse</c> and <c>Uri#file</c> is that the latter treats the argument
        /// as path, not as stringified-uri. E.g. <c>Uri.file(path)</c> is **not the same as**
        /// <c>Uri.parse('file://' + path)</c> because the path might contain characters that are
        /// interpreted (# and ?). See the following sample:
        /// <code lang="ts">
        /// const good = Uri.file('/coding/c#/project1');
        /// good.scheme === 'file';
        /// good.path === '/coding/c#/project1';
        /// good.fragment === '';
        /// const bad = Uri.parse('file://' + '/coding/c#/project1');
        /// bad.scheme === 'file';
        /// bad.path === '/coding/c'; // path is now broken
        /// bad.fragment === '/project1';
        /// </code>
        /// </summary>
        /// <param name="path">A file system path (see <c>Uri#fsPath</c>)</param>
        abstract file: path: string -> Uri
        /// <summary>
        /// Creates new Uri from uri components.
        ///
        /// Unless <c>strict</c> is <c>true</c> the scheme is defaults to be <c>file</c>. This function performs
        /// validation and should be used for untrusted uri components retrieved from storage,
        /// user input, command arguments etc
        /// </summary>
        abstract from: components: UriComponents * ?strict: bool -> Uri
        /// <summary>Join a Uri path with path fragments and normalizes the resulting path.</summary>
        /// <param name="uri">The input Uri.</param>
        /// <param name="pathFragment">The path fragment to add to the Uri path.</param>
        /// <returns>The resulting Uri.</returns>
        abstract joinPath: uri: Uri * [<ParamArray>] pathFragment: string[] -> Uri
        /// <summary>
        /// A helper function to revive URIs.
        ///
        /// **Note** that this function should only be used when receiving Uri#toJSON generated data
        /// and that it doesn't do any validation. Use <see cref="Uri.from" /> when received "untrusted"
        /// uri components such as command arguments or data from storage.
        /// </summary>
        /// <param name="data">The Uri components or Uri to revive.</param>
        /// <returns>The revived Uri or undefined or null.</returns>
        abstract revive: data: U2<UriComponents, Uri> -> Uri
        abstract revive: data: U2<UriComponents, Uri> option -> Uri option

    [<AllowNullLiteral>]
    type UriComponents =
        abstract scheme: string with get, set
        abstract authority: string option with get, set
        abstract path: string option with get, set
        abstract query: string option with get, set
        abstract fragment: string option with get, set

    /// <summary>
    /// Virtual Key Codes, the value does not hold any inherent meaning.
    /// Inspired somewhat from <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx" />
    /// But these are "more general", as they should work across browsers &amp; OS`s.
    /// </summary>
    [<RequireQualifiedAccess>]
    type KeyCode =
        | DependsOnKbLayout = -1
        /// Placed first to cover the 0 value of the enum.
        | Unknown = 0
        | Backspace = 1
        | Tab = 2
        | Enter = 3
        | Shift = 4
        | Ctrl = 5
        | Alt = 6
        | PauseBreak = 7
        | CapsLock = 8
        | Escape = 9
        | Space = 10
        | PageUp = 11
        | PageDown = 12
        | End = 13
        | Home = 14
        | LeftArrow = 15
        | UpArrow = 16
        | RightArrow = 17
        | DownArrow = 18
        | Insert = 19
        | Delete = 20
        | Digit0 = 21
        | Digit1 = 22
        | Digit2 = 23
        | Digit3 = 24
        | Digit4 = 25
        | Digit5 = 26
        | Digit6 = 27
        | Digit7 = 28
        | Digit8 = 29
        | Digit9 = 30
        | KeyA = 31
        | KeyB = 32
        | KeyC = 33
        | KeyD = 34
        | KeyE = 35
        | KeyF = 36
        | KeyG = 37
        | KeyH = 38
        | KeyI = 39
        | KeyJ = 40
        | KeyK = 41
        | KeyL = 42
        | KeyM = 43
        | KeyN = 44
        | KeyO = 45
        | KeyP = 46
        | KeyQ = 47
        | KeyR = 48
        | KeyS = 49
        | KeyT = 50
        | KeyU = 51
        | KeyV = 52
        | KeyW = 53
        | KeyX = 54
        | KeyY = 55
        | KeyZ = 56
        | Meta = 57
        | ContextMenu = 58
        | F1 = 59
        | F2 = 60
        | F3 = 61
        | F4 = 62
        | F5 = 63
        | F6 = 64
        | F7 = 65
        | F8 = 66
        | F9 = 67
        | F10 = 68
        | F11 = 69
        | F12 = 70
        | F13 = 71
        | F14 = 72
        | F15 = 73
        | F16 = 74
        | F17 = 75
        | F18 = 76
        | F19 = 77
        | F20 = 78
        | F21 = 79
        | F22 = 80
        | F23 = 81
        | F24 = 82
        | NumLock = 83
        | ScrollLock = 84
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// For the US standard keyboard, the ';:' key
        | Semicolon = 85
        /// For any country/region, the '+' key
        /// For the US standard keyboard, the '=+' key
        | Equal = 86
        /// For any country/region, the ',' key
        /// For the US standard keyboard, the ',<' key
        | Comma = 87
        /// For any country/region, the '-' key
        /// For the US standard keyboard, the '-_' key
        | Minus = 88
        /// For any country/region, the '.' key
        /// For the US standard keyboard, the '.>' key
        | Period = 89
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// For the US standard keyboard, the '/?' key
        | Slash = 90
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// For the US standard keyboard, the '`~' key
        | Backquote = 91
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// For the US standard keyboard, the '[{' key
        | BracketLeft = 92
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// For the US standard keyboard, the '\|' key
        | Backslash = 93
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// For the US standard keyboard, the ']}' key
        | BracketRight = 94
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// For the US standard keyboard, the ''"' key
        | Quote = 95
        /// Used for miscellaneous characters; it can vary by keyboard.
        | OEM_8 = 96
        /// Either the angle bracket key or the backslash key on the RT 102-key keyboard.
        | IntlBackslash = 97
        | Numpad0 = 98
        | Numpad1 = 99
        | Numpad2 = 100
        | Numpad3 = 101
        | Numpad4 = 102
        | Numpad5 = 103
        | Numpad6 = 104
        | Numpad7 = 105
        | Numpad8 = 106
        | Numpad9 = 107
        | NumpadMultiply = 108
        | NumpadAdd = 109
        | NUMPAD_SEPARATOR = 110
        | NumpadSubtract = 111
        | NumpadDecimal = 112
        | NumpadDivide = 113
        /// Cover all key codes when IME is processing input.
        | KEY_IN_COMPOSITION = 114
        | ABNT_C1 = 115
        | ABNT_C2 = 116
        | AudioVolumeMute = 117
        | AudioVolumeUp = 118
        | AudioVolumeDown = 119
        | BrowserSearch = 120
        | BrowserHome = 121
        | BrowserBack = 122
        | BrowserForward = 123
        | MediaTrackNext = 124
        | MediaTrackPrevious = 125
        | MediaStop = 126
        | MediaPlayPause = 127
        | LaunchMediaPlayer = 128
        | LaunchMail = 129
        | LaunchApp2 = 130
        /// VK_CLEAR, 0x0C, CLEAR key
        | Clear = 131
        /// Placed last to cover the length of the enum.
        /// Please do not depend on this value!
        | MAX_VALUE = 132

    [<AllowNullLiteral>]
    type KeyMod =
        interface
        end

    [<AllowNullLiteral>]
    type KeyModStatic =
        [<EmitConstructor>]
        abstract Create: unit -> KeyMod

        abstract CtrlCmd: float
        abstract Shift: float
        abstract Alt: float
        abstract WinCtrl: float
        abstract chord: firstPart: float * secondPart: float -> float

    [<AllowNullLiteral>]
    type IMarkdownString =
        abstract value: string
        abstract isTrusted: U2<bool, MarkdownStringTrustedOptions> option
        abstract supportThemeIcons: bool option
        abstract supportHtml: bool option
        abstract baseUri: UriComponents option
        abstract uris: IMarkdownStringUris option with get, set

    [<AllowNullLiteral>]
    type MarkdownStringTrustedOptions =
        abstract enabledCommands: ResizeArray<string>

    [<AllowNullLiteral>]
    type IKeyboardEvent =
        abstract _standardKeyboardEventBrand: bool
        abstract browserEvent: KeyboardEvent
        abstract target: HTMLElement
        abstract ctrlKey: bool
        abstract shiftKey: bool
        abstract altKey: bool
        abstract metaKey: bool
        abstract altGraphKey: bool
        abstract keyCode: KeyCode
        abstract code: string
        abstract equals: keybinding: float -> bool
        abstract preventDefault: unit -> unit
        abstract stopPropagation: unit -> unit

    [<AllowNullLiteral>]
    type IMouseEvent =
        abstract browserEvent: MouseEvent
        abstract leftButton: bool
        abstract middleButton: bool
        abstract rightButton: bool
        abstract buttons: float
        abstract target: HTMLElement
        abstract detail: float
        abstract posx: float
        abstract posy: float
        abstract ctrlKey: bool
        abstract shiftKey: bool
        abstract altKey: bool
        abstract metaKey: bool
        abstract timestamp: float
        abstract preventDefault: unit -> unit
        abstract stopPropagation: unit -> unit

    [<AllowNullLiteral>]
    type IScrollEvent =
        abstract scrollTop: float
        abstract scrollLeft: float
        abstract scrollWidth: float
        abstract scrollHeight: float
        abstract scrollTopChanged: bool
        abstract scrollLeftChanged: bool
        abstract scrollWidthChanged: bool
        abstract scrollHeightChanged: bool

    /// A position in the editor. This interface is suitable for serialization.
    [<AllowNullLiteral>]
    type IPosition =
        /// line number (starts at 1)
        abstract lineNumber: float
        /// column (the first character in a line is between column 1 and column 2)
        abstract column: float

    /// A position in the editor.
    [<AllowNullLiteral>]
    type Position =
        /// line number (starts at 1)
        abstract lineNumber: float
        /// column (the first character in a line is between column 1 and column 2)
        abstract column: float
        /// <summary>Create a new position from this position.</summary>
        /// <param name="newLineNumber">new line number</param>
        /// <param name="newColumn">new column</param>
        abstract ``with``: ?newLineNumber: float * ?newColumn: float -> Position
        /// <summary>Derive a new position from this position.</summary>
        /// <param name="deltaLineNumber">line number delta</param>
        /// <param name="deltaColumn">column delta</param>
        abstract delta: ?deltaLineNumber: float * ?deltaColumn: float -> Position
        /// Test if this position equals other position
        abstract equals: other: IPosition -> bool
        /// Test if this position is before other position.
        /// If the two positions are equal, the result will be false.
        abstract isBefore: other: IPosition -> bool
        /// Test if this position is before other position.
        /// If the two positions are equal, the result will be true.
        abstract isBeforeOrEqual: other: IPosition -> bool
        /// Clone this position.
        abstract clone: unit -> Position
        /// Convert to a human-readable representation.
        abstract toString: unit -> string

    /// A position in the editor.
    [<AllowNullLiteral>]
    type PositionStatic =
        [<EmitConstructor>]
        abstract Create: lineNumber: float * column: float -> Position

        /// <summary>Test if position <c>a</c> equals position <c>b</c></summary>
        abstract equals: a: IPosition option * b: IPosition option -> bool
        /// <summary>
        /// Test if position <c>a</c> is before position <c>b</c>.
        /// If the two positions are equal, the result will be false.
        /// </summary>
        abstract isBefore: a: IPosition * b: IPosition -> bool
        /// <summary>
        /// Test if position <c>a</c> is before position <c>b</c>.
        /// If the two positions are equal, the result will be true.
        /// </summary>
        abstract isBeforeOrEqual: a: IPosition * b: IPosition -> bool
        /// A function that compares positions, useful for sorting
        abstract compare: a: IPosition * b: IPosition -> float
        /// <summary>Create a <c>Position</c> from an <c>IPosition</c>.</summary>
        abstract lift: pos: IPosition -> Position
        /// <summary>Test if <c>obj</c> is an <c>IPosition</c>.</summary>
        abstract isIPosition: obj: obj option -> bool

    /// A range in the editor. This interface is suitable for serialization.
    [<AllowNullLiteral>]
    type IRange =
        /// Line number on which the range starts (starts at 1).
        abstract startLineNumber: float
        /// <summary>Column on which the range starts in line <c>startLineNumber</c> (starts at 1).</summary>
        abstract startColumn: float
        /// Line number on which the range ends.
        abstract endLineNumber: float
        /// <summary>Column on which the range ends in line <c>endLineNumber</c>.</summary>
        abstract endColumn: float

    /// A range in the editor. (startLineNumber,startColumn) is <= (endLineNumber,endColumn)
    [<AllowNullLiteral>]
    type Range =
        /// Line number on which the range starts (starts at 1).
        abstract startLineNumber: float
        /// <summary>Column on which the range starts in line <c>startLineNumber</c> (starts at 1).</summary>
        abstract startColumn: float
        /// Line number on which the range ends.
        abstract endLineNumber: float
        /// <summary>Column on which the range ends in line <c>endLineNumber</c>.</summary>
        abstract endColumn: float
        /// Test if this range is empty.
        abstract isEmpty: unit -> bool
        /// Test if position is in this range. If the position is at the edges, will return true.
        abstract containsPosition: position: IPosition -> bool
        /// Test if range is in this range. If the range is equal to this range, will return true.
        abstract containsRange: range: IRange -> bool
        /// <summary>Test if <c>range</c> is strictly in this range. <c>range</c> must start after and end before this range for the result to be true.</summary>
        abstract strictContainsRange: range: IRange -> bool
        /// A reunion of the two ranges.
        /// The smallest position will be used as the start point, and the largest one as the end point.
        abstract plusRange: range: IRange -> Range
        /// A intersection of the two ranges.
        abstract intersectRanges: range: IRange -> Range option
        /// Test if this range equals other.
        abstract equalsRange: other: IRange option -> bool
        /// Return the end position (which will be after or equal to the start position)
        abstract getEndPosition: unit -> Position
        /// Return the start position (which will be before or equal to the end position)
        abstract getStartPosition: unit -> Position
        /// Transform to a user presentable string representation.
        abstract toString: unit -> string
        /// Create a new range using this range's start position, and using endLineNumber and endColumn as the end position.
        abstract setEndPosition: endLineNumber: float * endColumn: float -> Range
        /// Create a new range using this range's end position, and using startLineNumber and startColumn as the start position.
        abstract setStartPosition: startLineNumber: float * startColumn: float -> Range
        /// Create a new empty range using this range's start position.
        abstract collapseToStart: unit -> Range
        /// Create a new empty range using this range's end position.
        abstract collapseToEnd: unit -> Range
        /// Moves the range by the given amount of lines.
        abstract delta: lineCount: float -> Range
        abstract toJSON: unit -> IRange

    /// A range in the editor. (startLineNumber,startColumn) is <= (endLineNumber,endColumn)
    [<AllowNullLiteral>]
    type RangeStatic =
        [<EmitConstructor>]
        abstract Create: startLineNumber: float * startColumn: float * endLineNumber: float * endColumn: float -> Range

        /// <summary>Test if <c>range</c> is empty.</summary>
        abstract isEmpty: range: IRange -> bool
        /// <summary>Test if <c>position</c> is in <c>range</c>. If the position is at the edges, will return true.</summary>
        abstract containsPosition: range: IRange * position: IPosition -> bool
        /// <summary>Test if <c>otherRange</c> is in <c>range</c>. If the ranges are equal, will return true.</summary>
        abstract containsRange: range: IRange * otherRange: IRange -> bool
        /// <summary>Test if <c>otherRange</c> is strictly in <c>range</c> (must start after, and end before). If the ranges are equal, will return false.</summary>
        abstract strictContainsRange: range: IRange * otherRange: IRange -> bool
        /// A reunion of the two ranges.
        /// The smallest position will be used as the start point, and the largest one as the end point.
        abstract plusRange: a: IRange * b: IRange -> Range
        /// A intersection of the two ranges.
        abstract intersectRanges: a: IRange * b: IRange -> Range option
        /// <summary>Test if range <c>a</c> equals <c>b</c>.</summary>
        abstract equalsRange: a: IRange option * b: IRange option -> bool
        /// Return the end position (which will be after or equal to the start position)
        abstract getEndPosition: range: IRange -> Position
        /// Return the start position (which will be before or equal to the end position)
        abstract getStartPosition: range: IRange -> Position
        /// Create a new empty range using this range's start position.
        abstract collapseToStart: range: IRange -> Range
        /// Create a new empty range using this range's end position.
        abstract collapseToEnd: range: IRange -> Range
        abstract fromPositions: start: IPosition * ?``end``: IPosition -> Range
        /// <summary>Create a <c>Range</c> from an <c>IRange</c>.</summary>
        abstract lift: range: obj -> obj
        abstract lift: range: IRange -> Range
        abstract lift: range: IRange option -> Range option
        /// <summary>Test if <c>obj</c> is an <c>IRange</c>.</summary>
        abstract isIRange: obj: obj option -> bool
        /// Test if the two ranges are touching in any way.
        abstract areIntersectingOrTouching: a: IRange * b: IRange -> bool
        /// Test if the two ranges are intersecting. If the ranges are touching it returns true.
        abstract areIntersecting: a: IRange * b: IRange -> bool
        /// A function that compares ranges, useful for sorting ranges
        /// It will first compare ranges on the startPosition and then on the endPosition
        abstract compareRangesUsingStarts: a: IRange option * b: IRange option -> float
        /// A function that compares ranges, useful for sorting ranges
        /// It will first compare ranges on the endPosition and then on the startPosition
        abstract compareRangesUsingEnds: a: IRange * b: IRange -> float
        /// Test if the range spans multiple lines.
        abstract spansMultipleLines: range: IRange -> bool

    /// A selection in the editor.
    /// The selection is a range that has an orientation.
    [<AllowNullLiteral>]
    type ISelection =
        /// The line number on which the selection has started.
        abstract selectionStartLineNumber: float
        /// <summary>The column on <c>selectionStartLineNumber</c> where the selection has started.</summary>
        abstract selectionStartColumn: float
        /// The line number on which the selection has ended.
        abstract positionLineNumber: float
        /// <summary>The column on <c>positionLineNumber</c> where the selection has ended.</summary>
        abstract positionColumn: float

    /// A selection in the editor.
    /// The selection is a range that has an orientation.
    [<AllowNullLiteral>]
    type Selection =
        inherit Range
        /// The line number on which the selection has started.
        abstract selectionStartLineNumber: float
        /// <summary>The column on <c>selectionStartLineNumber</c> where the selection has started.</summary>
        abstract selectionStartColumn: float
        /// The line number on which the selection has ended.
        abstract positionLineNumber: float
        /// <summary>The column on <c>positionLineNumber</c> where the selection has ended.</summary>
        abstract positionColumn: float
        /// Transform to a human-readable representation.
        abstract toString: unit -> string
        /// Test if equals other selection.
        abstract equalsSelection: other: ISelection -> bool
        /// Get directions (LTR or RTL).
        abstract getDirection: unit -> SelectionDirection
        /// <summary>Create a new selection with a different <c>positionLineNumber</c> and <c>positionColumn</c>.</summary>
        abstract setEndPosition: endLineNumber: float * endColumn: float -> Selection
        /// <summary>Get the position at <c>positionLineNumber</c> and <c>positionColumn</c>.</summary>
        abstract getPosition: unit -> Position
        /// Get the position at the start of the selection.
        abstract getSelectionStart: unit -> Position
        /// <summary>Create a new selection with a different <c>selectionStartLineNumber</c> and <c>selectionStartColumn</c>.</summary>
        abstract setStartPosition: startLineNumber: float * startColumn: float -> Selection

    /// A selection in the editor.
    /// The selection is a range that has an orientation.
    [<AllowNullLiteral>]
    type SelectionStatic =
        [<EmitConstructor>]
        abstract Create:
            selectionStartLineNumber: float *
            selectionStartColumn: float *
            positionLineNumber: float *
            positionColumn: float ->
                Selection

        /// Test if the two selections are equal.
        abstract selectionsEqual: a: ISelection * b: ISelection -> bool
        /// <summary>Create a <c>Selection</c> from one or two positions</summary>
        abstract fromPositions: start: IPosition * ?``end``: IPosition -> Selection
        /// <summary>Creates a <c>Selection</c> from a range, given a direction.</summary>
        abstract fromRange: range: Range * direction: SelectionDirection -> Selection
        /// <summary>Create a <c>Selection</c> from an <c>ISelection</c>.</summary>
        abstract liftSelection: sel: ISelection -> Selection
        /// <summary><c>a</c> equals <c>b</c>.</summary>
        abstract selectionsArrEqual: a: ResizeArray<ISelection> * b: ResizeArray<ISelection> -> bool
        /// <summary>Test if <c>obj</c> is an <c>ISelection</c>.</summary>
        abstract isISelection: obj: obj option -> bool

        /// Create with a direction.
        abstract createWithDirection:
            startLineNumber: float *
            startColumn: float *
            endLineNumber: float *
            endColumn: float *
            direction: SelectionDirection ->
                Selection

    /// The direction of a selection.
    [<RequireQualifiedAccess>]
    type SelectionDirection =
        /// The selection starts above where it ends.
        | LTR = 0
        /// The selection starts below where it ends.
        | RTL = 1

    [<AllowNullLiteral>]
    type Token =
        abstract offset: float
        abstract ``type``: string
        abstract language: string
        // abstract _tokenBrand: unit with get, set
        abstract toString: unit -> string

    [<AllowNullLiteral>]
    type TokenStatic =
        [<EmitConstructor>]
        abstract Create: offset: float * ``type``: string * language: string -> Token

    module Editor =

        [<AllowNullLiteral>]
        type IExports =
            /// <summary>
            /// Create a new editor under <c>domElement</c>.
            /// <c>domElement</c> should be empty (not contain other dom nodes).
            /// The editor will read the size of <c>domElement</c>.
            /// </summary>
            abstract create:
                domElement: HTMLElement *
                ?options: IStandaloneEditorConstructionOptions *
                ?``override``: IEditorOverrideServices ->
                    IStandaloneCodeEditor

            /// <summary>
            /// Emitted when an editor is created.
            /// Creating a diff editor might cause this listener to be invoked with the two editors.
            /// </summary>
            abstract onDidCreateEditor: listener: (ICodeEditor -> unit) -> IDisposable
            /// <summary>Emitted when an diff editor is created.</summary>
            abstract onDidCreateDiffEditor: listener: (IDiffEditor -> unit) -> IDisposable
            /// Get all the created editors.
            abstract getEditors: unit -> ResizeArray<ICodeEditor>
            /// Get all the created diff editors.
            abstract getDiffEditors: unit -> ResizeArray<IDiffEditor>

            /// <summary>
            /// Create a new diff editor under <c>domElement</c>.
            /// <c>domElement</c> should be empty (not contain other dom nodes).
            /// The editor will read the size of <c>domElement</c>.
            /// </summary>
            abstract createDiffEditor:
                domElement: HTMLElement *
                ?options: IStandaloneDiffEditorConstructionOptions *
                ?``override``: IEditorOverrideServices ->
                    IStandaloneDiffEditor

            /// Add a command.
            abstract addCommand: descriptor: ICommandDescriptor -> IDisposable
            /// Add an action to all editors.
            abstract addEditorAction: descriptor: IActionDescriptor -> IDisposable
            /// Add a keybinding rule.
            abstract addKeybindingRule: rule: IKeybindingRule -> IDisposable
            /// Add keybinding rules.
            abstract addKeybindingRules: rules: ResizeArray<IKeybindingRule> -> IDisposable
            /// <summary>
            /// Create a new editor model.
            /// You can specify the language that should be set for this model or let the language be inferred from the <c>uri</c>.
            /// </summary>
            abstract createModel: value: string * ?language: string * ?uri: Uri -> ITextModel
            /// Change the language for a model.
            abstract setModelLanguage: model: ITextModel * mimeTypeOrLanguageId: string -> unit
            /// Set the markers for a model.
            abstract setModelMarkers: model: ITextModel * owner: string * markers: ResizeArray<IMarkerData> -> unit
            /// Remove all markers of an owner.
            abstract removeAllMarkers: owner: string -> unit

            /// <summary>Get markers for owner and/or resource</summary>
            /// <returns>list of markers</returns>
            abstract getModelMarkers:
                filter:
                    {|
                        owner: string option
                        resource: Uri option
                        take: float option
                    |} ->
                    ResizeArray<IMarker>

            /// <summary>Emitted when markers change for a model.</summary>
            abstract onDidChangeMarkers: listener: (ResizeArray<Uri> -> unit) -> IDisposable
            /// <summary>Get the model that has <c>uri</c> if it exists.</summary>
            abstract getModel: uri: Uri -> ITextModel option
            /// Get all the created models.
            abstract getModels: unit -> ResizeArray<ITextModel>
            /// <summary>Emitted when a model is created.</summary>
            abstract onDidCreateModel: listener: (ITextModel -> unit) -> IDisposable
            /// <summary>Emitted right before a model is disposed.</summary>
            abstract onWillDisposeModel: listener: (ITextModel -> unit) -> IDisposable

            /// <summary>Emitted when a different language is set to a model.</summary>
            abstract onDidChangeModelLanguage:
                listener:
                    ({|
                        model: ITextModel
                        oldLanguage: string
                    |}
                        -> unit) ->
                    IDisposable

            /// <summary>
            /// Create a new web worker that has model syncing capabilities built in.
            /// Specify an AMD module to load that will <c>create</c> an object that will be proxied.
            /// </summary>
            abstract createWebWorker: opts: IWebWorkerOptions -> MonacoWebWorker<'T> when 'T :> obj
            /// <summary>Colorize the contents of <c>domNode</c> using attribute <c>data-lang</c>.</summary>
            abstract colorizeElement: domNode: HTMLElement * options: IColorizerElementOptions -> Promise<unit>
            /// <summary>Colorize <c>text</c> using language <c>languageId</c>.</summary>
            abstract colorize: text: string * languageId: string * options: IColorizerOptions -> Promise<string>
            /// Colorize a line in a model.
            abstract colorizeModelLine: model: ITextModel * lineNumber: float * ?tabSize: float -> string
            /// <summary>Tokenize <c>text</c> using language <c>languageId</c></summary>
            abstract tokenize: text: string * languageId: string -> ResizeArray<ResizeArray<Token>>
            /// Define a new theme or update an existing theme.
            abstract defineTheme: themeName: string * themeData: IStandaloneThemeData -> unit
            /// Switches to a theme.
            abstract setTheme: themeName: string -> unit
            /// Clears all cached font measurements and triggers re-measurement.
            abstract remeasureFonts: unit -> unit

            /// Register a command.
            abstract registerCommand:
                id: string * handler: (obj option -> ResizeArray<obj option> -> unit) -> IDisposable

            /// <summary>
            /// Registers a handler that is called when a link is opened in any editor. The handler callback should return <c>true</c> if the link was handled and <c>false</c> otherwise.
            /// The handler that was registered last will be called first when a link is opened.
            ///
            /// Returns a disposable that can unregister the opener again.
            /// </summary>
            abstract registerLinkOpener: opener: ILinkOpener -> IDisposable
            /// <summary>
            /// Registers a handler that is called when a resource other than the current model should be opened in the editor (e.g. "go to definition").
            /// The handler callback should return <c>true</c> if the request was handled and <c>false</c> otherwise.
            ///
            /// Returns a disposable that can unregister the opener again.
            ///
            /// If no handler is registered the default behavior is to do nothing for models other than the currently attached one.
            /// </summary>
            abstract registerEditorOpener: opener: ICodeEditorOpener -> IDisposable
            abstract TextModelResolvedOptions: TextModelResolvedOptionsStatic
            abstract FindMatch: FindMatchStatic

            /// <summary>The type of the <c>IEditor</c>.</summary>
            abstract EditorType:
                {|
                    ICodeEditor: string
                    IDiffEditor: string
                |}

            /// An event describing that the configuration of the editor has changed.
            abstract ConfigurationChangedEvent: ConfigurationChangedEventStatic
            abstract ApplyUpdateResult: ApplyUpdateResultStatic
            abstract EditorOptions: IExportsEditorOptions
            abstract FontInfo: FontInfoStatic
            abstract BareFontInfo: BareFontInfoStatic
            abstract EditorZoom: IEditorZoom

        /// Description of a command contribution
        [<AllowNullLiteral>]
        type ICommandDescriptor =
            /// An unique identifier of the contributed command.
            abstract id: string with get, set
            /// Callback that will be executed when the command is triggered.
            abstract run: ICommandHandler with get, set

        /// A keybinding rule.
        [<AllowNullLiteral>]
        type IKeybindingRule =
            abstract keybinding: float with get, set
            abstract command: string option with get, set
            abstract commandArgs: obj option with get, set
            abstract ``when``: string option with get, set

        [<AllowNullLiteral>]
        type ILinkOpener =
            abstract ``open``: resource: Uri -> U2<bool, Promise<bool>>

        /// Represents an object that can handle editor open operations (e.g. when "go to definition" is called
        /// with a resource other than the current model).
        [<AllowNullLiteral>]
        type ICodeEditorOpener =
            /// <summary>
            /// Callback that is invoked when a resource other than the current model should be opened (e.g. when "go to definition" is called).
            /// The callback should return <c>true</c> if the request was handled and <c>false</c> otherwise.
            /// </summary>
            /// <param name="source">The code editor instance that initiated the request.</param>
            /// <param name="resource">The Uri of the resource that should be opened.</param>
            /// <param name="selectionOrPosition">An optional position or selection inside the model corresponding to <c>resource</c> that can be used to set the cursor.</param>
            abstract openCodeEditor:
                source: ICodeEditor * resource: Uri * ?selectionOrPosition: U2<IRange, IPosition> ->
                    U2<bool, Promise<bool>>

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type BuiltinTheme =
            | Vs
            | [<CompiledName("vs-dark")>] VsDark
            | [<CompiledName("hc-black")>] HcBlack
            | [<CompiledName("hc-light")>] HcLight

        [<AllowNullLiteral>]
        type IStandaloneThemeData =
            abstract ``base``: BuiltinTheme with get, set
            abstract ``inherit``: bool with get, set
            abstract rules: ResizeArray<ITokenThemeRule> with get, set
            abstract encodedTokensColors: ResizeArray<string> option with get, set
            abstract colors: IColors with get, set

        [<AllowNullLiteral>]
        type IColors =
            [<EmitIndexer>]
            abstract Item: colorId: string -> string with get, set

        [<AllowNullLiteral>]
        type ITokenThemeRule =
            abstract token: string with get, set
            abstract foreground: string option with get, set
            abstract background: string option with get, set
            abstract fontStyle: string option with get, set

        /// A web worker that can provide a proxy to an arbitrary file.
        [<AllowNullLiteral>]
        type MonacoWebWorker<'T> =
            /// Terminate the web worker, thus invalidating the returned proxy.
            abstract dispose: unit -> unit
            /// Get a proxy to the arbitrary loaded code.
            abstract getProxy: unit -> Promise<'T>
            /// <summary>
            /// Synchronize (send) the models at <c>resources</c> to the web worker,
            /// making them available in the monaco.worker.getMirrorModels().
            /// </summary>
            abstract withSyncedResources: resources: ResizeArray<Uri> -> Promise<'T>

        [<AllowNullLiteral>]
        type IWebWorkerOptions =
            /// <summary>
            /// The AMD moduleId to load.
            /// It should export a function <c>create</c> that should return the exported proxy.
            /// </summary>
            abstract moduleId: string with get, set
            /// The data to send over when calling create on the module.
            abstract createData: obj option with get, set
            /// A label to be used to identify the web worker for debugging purposes.
            abstract label: string option with get, set
            /// An object that can be used by the web worker to make calls back to the main thread.
            abstract host: obj option with get, set
            /// Keep idle models.
            /// Defaults to false, which means that idle models will stop syncing after a while.
            abstract keepIdleModels: bool option with get, set

        /// Description of an action contribution
        [<AllowNullLiteral>]
        type IActionDescriptor =
            /// An unique identifier of the contributed action.
            abstract id: string with get, set
            /// A label of the action that will be presented to the user.
            abstract label: string with get, set
            /// Precondition rule.
            abstract precondition: string option with get, set
            /// An array of keybindings for the action.
            abstract keybindings: ResizeArray<float> option with get, set
            /// The keybinding rule (condition on top of precondition).
            abstract keybindingContext: string option with get, set
            /// Control if the action should show up in the context menu and where.
            /// The context menu of the editor has these default:
            ///   navigation - The navigation group comes first in all cases.
            ///   1_modification - This group comes next and contains commands that modify your code.
            ///   9_cutcopypaste - The last default group with the basic editing commands.
            /// You can also create your own group.
            /// Defaults to null (don't show in context menu).
            abstract contextMenuGroupId: string option with get, set
            /// Control the order in the context menu group.
            abstract contextMenuOrder: float option with get, set
            /// <summary>Method that will be executed when the action is triggered.</summary>
            /// <param name="editor">The editor instance is passed in as a convenience</param>
            abstract run: editor: ICodeEditor * [<ParamArray>] args: obj option[] -> U2<unit, Promise<unit>>

        /// Options which apply for all editors.
        [<AllowNullLiteral>]
        type IGlobalEditorOptions =
            /// <summary>
            /// The number of spaces a tab is equal to.
            /// This setting is overridden based on the file contents when <c>detectIndentation</c> is on.
            /// Defaults to 4.
            /// </summary>
            abstract tabSize: float option with get, set
            /// <summary>
            /// Insert spaces when pressing <c>Tab</c>.
            /// This setting is overridden based on the file contents when <c>detectIndentation</c> is on.
            /// Defaults to true.
            /// </summary>
            abstract insertSpaces: bool option with get, set
            /// <summary>
            /// Controls whether <c>tabSize</c> and <c>insertSpaces</c> will be automatically detected when a file is opened based on the file contents.
            /// Defaults to true.
            /// </summary>
            abstract detectIndentation: bool option with get, set
            /// Remove trailing auto inserted whitespace.
            /// Defaults to true.
            abstract trimAutoWhitespace: bool option with get, set
            /// Special handling for large files to disable certain memory intensive features.
            /// Defaults to true.
            abstract largeFileOptimizations: bool option with get, set
            /// Controls whether completions should be computed based on words in the document.
            /// Defaults to true.
            abstract wordBasedSuggestions: bool option with get, set
            /// Controls whether word based completions should be included from opened documents of the same language or any language.
            abstract wordBasedSuggestionsOnlySameLanguage: bool option with get, set
            /// Controls whether the semanticHighlighting is shown for the languages that support it.
            /// true: semanticHighlighting is enabled for all themes
            /// false: semanticHighlighting is disabled for all themes
            /// 'configuredByTheme': semanticHighlighting is controlled by the current color theme's semanticHighlighting setting.
            /// Defaults to 'byTheme'.
            abstract ``semanticHighlighting.enabled``: IGlobalEditorOptionsSemanticHighlightingEnabled option with get, set
            /// <summary>
            /// Keep peek editors open even when double-clicking their content or when hitting <c>Escape</c>.
            /// Defaults to false.
            /// </summary>
            abstract stablePeek: bool option with get, set
            /// Lines above this length will not be tokenized for performance reasons.
            /// Defaults to 20000.
            abstract maxTokenizationLineLength: float option with get, set
            /// <summary>
            /// Theme to be used for rendering.
            /// The current out-of-the-box available themes are: 'vs' (default), 'vs-dark', 'hc-black', 'hc-light'.
            /// You can create custom themes via <c>monaco.editor.defineTheme</c>.
            /// To switch a theme, use <c>monaco.editor.setTheme</c>.
            /// **NOTE**: The theme might be overwritten if the OS is in high contrast mode, unless <c>autoDetectHighContrast</c> is set to false.
            /// </summary>
            abstract theme: string option with get, set
            /// If enabled, will automatically change to high contrast theme if the OS is using a high contrast theme.
            /// Defaults to true.
            abstract autoDetectHighContrast: bool option with get, set

        /// The options to create an editor.
        [<AllowNullLiteral>]
        type IStandaloneEditorConstructionOptions =
            inherit IEditorConstructionOptions
            inherit IGlobalEditorOptions
            /// The initial model associated with this code editor.
            abstract model: ITextModel option with get, set
            /// <summary>
            /// The initial value of the auto created model in the editor.
            /// To not automatically create a model, use <c>model: null</c>.
            /// </summary>
            abstract value: string option with get, set
            /// <summary>
            /// The initial language of the auto created model in the editor.
            /// To not automatically create a model, use <c>model: null</c>.
            /// </summary>
            abstract language: string option with get, set
            /// <summary>
            /// Initial theme to be used for rendering.
            /// The current out-of-the-box available themes are: 'vs' (default), 'vs-dark', 'hc-black', 'hc-light.
            /// You can create custom themes via <c>monaco.editor.defineTheme</c>.
            /// To switch a theme, use <c>monaco.editor.setTheme</c>.
            /// **NOTE**: The theme might be overwritten if the OS is in high contrast mode, unless <c>autoDetectHighContrast</c> is set to false.
            /// </summary>
            abstract theme: string option with get, set
            /// If enabled, will automatically change to high contrast theme if the OS is using a high contrast theme.
            /// Defaults to true.
            abstract autoDetectHighContrast: bool option with get, set
            /// <summary>
            /// An URL to open when Ctrl+H (Windows and Linux) or Cmd+H (OSX) is pressed in
            /// the accessibility help dialog in the editor.
            ///
            /// Defaults to "<see href="https://go.microsoft.com/fwlink/?linkid=852450"" />
            /// </summary>
            abstract accessibilityHelpUrl: string option with get, set
            /// Container element to use for ARIA messages.
            /// Defaults to document.body.
            abstract ariaContainerElement: HTMLElement option with get, set

        /// The options to create a diff editor.
        [<AllowNullLiteral>]
        type IStandaloneDiffEditorConstructionOptions =
            inherit IDiffEditorConstructionOptions
            /// <summary>
            /// Initial theme to be used for rendering.
            /// The current out-of-the-box available themes are: 'vs' (default), 'vs-dark', 'hc-black', 'hc-light.
            /// You can create custom themes via <c>monaco.editor.defineTheme</c>.
            /// To switch a theme, use <c>monaco.editor.setTheme</c>.
            /// **NOTE**: The theme might be overwritten if the OS is in high contrast mode, unless <c>autoDetectHighContrast</c> is set to false.
            /// </summary>
            abstract theme: string option with get, set
            /// If enabled, will automatically change to high contrast theme if the OS is using a high contrast theme.
            /// Defaults to true.
            abstract autoDetectHighContrast: bool option with get, set

        [<AllowNullLiteral>]
        type IStandaloneCodeEditor =
            inherit ICodeEditor
            /// Update the editor's options after the editor has been created.
            abstract updateOptions: newOptions: obj -> unit
            abstract addCommand: keybinding: float * handler: ICommandHandler * ?context: string -> string option
            abstract createContextKey: key: string * defaultValue: 'T -> IContextKey<'T> when 'T :> ContextKeyValue
            abstract addAction: descriptor: IActionDescriptor -> IDisposable

        [<AllowNullLiteral>]
        type IStandaloneDiffEditor =
            inherit IDiffEditor
            abstract addCommand: keybinding: float * handler: ICommandHandler * ?context: string -> string option
            abstract createContextKey: key: string * defaultValue: 'T -> IContextKey<'T> when 'T :> ContextKeyValue
            abstract addAction: descriptor: IActionDescriptor -> IDisposable
            /// <summary>Get the <c>original</c> editor.</summary>
            abstract getOriginalEditor: unit -> IStandaloneCodeEditor
            /// <summary>Get the <c>modified</c> editor.</summary>
            abstract getModifiedEditor: unit -> IStandaloneCodeEditor

        [<AllowNullLiteral>]
        type ICommandHandler =
            [<Emit("$0($1...)")>]
            abstract Invoke: [<ParamArray>] args: obj option[] -> unit

        type IContextKey = IContextKey<ContextKeyValue>

        [<AllowNullLiteral>]
        type IContextKey<'T when 'T :> ContextKeyValue> =
            abstract set: value: 'T -> unit
            abstract reset: unit -> unit
            abstract get: unit -> 'T option

        type ContextKeyValue =
            interface
            end

        [<AllowNullLiteral>]
        type IEditorOverrideServices =
            [<EmitIndexer>]
            abstract Item: index: string -> obj option with get, set

        [<AllowNullLiteral>]
        type IMarker =
            abstract owner: string with get, set
            abstract resource: Uri with get, set
            abstract severity: MarkerSeverity with get, set
            abstract code: U2<string, {| value: string; target: Uri |}> option with get, set
            abstract message: string with get, set
            abstract source: string option with get, set
            abstract startLineNumber: float with get, set
            abstract startColumn: float with get, set
            abstract endLineNumber: float with get, set
            abstract endColumn: float with get, set
            abstract modelVersionId: float option with get, set
            abstract relatedInformation: ResizeArray<IRelatedInformation> option with get, set
            abstract tags: ResizeArray<MarkerTag> option with get, set

        /// A structure defining a problem/warning/etc.
        [<AllowNullLiteral>]
        type IMarkerData =
            abstract code: U2<string, {| value: string; target: Uri |}> option with get, set
            abstract severity: MarkerSeverity with get, set
            abstract message: string with get, set
            abstract source: string option with get, set
            abstract startLineNumber: float with get, set
            abstract startColumn: float with get, set
            abstract endLineNumber: float with get, set
            abstract endColumn: float with get, set
            abstract modelVersionId: float option with get, set
            abstract relatedInformation: ResizeArray<IRelatedInformation> option with get, set
            abstract tags: ResizeArray<MarkerTag> option with get, set

        [<AllowNullLiteral>]
        type IRelatedInformation =
            abstract resource: Uri with get, set
            abstract message: string with get, set
            abstract startLineNumber: float with get, set
            abstract startColumn: float with get, set
            abstract endLineNumber: float with get, set
            abstract endColumn: float with get, set

        [<AllowNullLiteral>]
        type IColorizerOptions =
            abstract tabSize: float option with get, set

        [<AllowNullLiteral>]
        type IColorizerElementOptions =
            inherit IColorizerOptions
            abstract theme: string option with get, set
            abstract mimeType: string option with get, set

        [<RequireQualifiedAccess>]
        type ScrollbarVisibility =
            | Auto = 1
            | Hidden = 2
            | Visible = 3

        [<AllowNullLiteral>]
        type ThemeColor =
            abstract id: string with get, set

        /// <summary>
        /// A single edit operation, that acts as a simple replace.
        /// i.e. Replace text at <c>range</c> with <c>text</c> in model.
        /// </summary>
        [<AllowNullLiteral>]
        type ISingleEditOperation =
            /// The range to replace. This can be empty to emulate a simple insert.
            abstract range: IRange with get, set
            /// The text to replace with. This can be null to emulate a simple delete.
            abstract text: string option with get, set
            /// <summary>
            /// This indicates that this operation has "insert" semantics.
            /// i.e. forceMoveMarkers = true =&gt; if <c>range</c> is collapsed, all markers at the position will be moved.
            /// </summary>
            abstract forceMoveMarkers: bool option with get, set

        /// Word inside a model.
        [<AllowNullLiteral>]
        type IWordAtPosition =
            /// The word.
            abstract word: string
            /// The column where the word starts.
            abstract startColumn: float
            /// The column where the word ends.
            abstract endColumn: float

        /// Vertical Lane in the overview ruler of the editor.
        [<RequireQualifiedAccess>]
        type OverviewRulerLane =
            | Left = 1
            | Center = 2
            | Right = 4
            | Full = 7

        /// Vertical Lane in the glyph margin of the editor.
        [<RequireQualifiedAccess>]
        type GlyphMarginLane =
            | Left = 1
            | Right = 2

        /// Position in the minimap to render the decoration.
        [<RequireQualifiedAccess>]
        type MinimapPosition =
            | Inline = 1
            | Gutter = 2

        [<AllowNullLiteral>]
        type IDecorationOptions =
            /// CSS color to render.
            /// e.g.: rgba(100, 100, 100, 0.5) or a color from the color registry
            abstract color: U2<string, ThemeColor> option with get, set
            /// CSS color to render.
            /// e.g.: rgba(100, 100, 100, 0.5) or a color from the color registry
            abstract darkColor: U2<string, ThemeColor> option with get, set

        [<AllowNullLiteral>]
        type IModelDecorationGlyphMarginOptions =
            /// The position in the glyph margin.
            abstract position: GlyphMarginLane with get, set

        /// Options for rendering a model decoration in the overview ruler.
        [<AllowNullLiteral>]
        type IModelDecorationOverviewRulerOptions =
            inherit IDecorationOptions
            /// The position in the overview ruler.
            abstract position: OverviewRulerLane with get, set

        /// Options for rendering a model decoration in the minimap.
        [<AllowNullLiteral>]
        type IModelDecorationMinimapOptions =
            inherit IDecorationOptions
            /// The position in the minimap.
            abstract position: MinimapPosition with get, set

        /// Options for a model decoration.
        [<AllowNullLiteral>]
        type IModelDecorationOptions =
            /// Customize the growing behavior of the decoration when typing at the edges of the decoration.
            /// Defaults to TrackedRangeStickiness.AlwaysGrowsWhenTypingAtEdges
            abstract stickiness: TrackedRangeStickiness option with get, set
            /// CSS class name describing the decoration.
            abstract className: string option with get, set
            /// Indicates whether the decoration should span across the entire line when it continues onto the next line.
            abstract shouldFillLineOnLineBreak: bool option with get, set
            abstract blockClassName: string option with get, set
            /// Indicates if this block should be rendered after the last line.
            /// In this case, the range must be empty and set to the last line.
            abstract blockIsAfterEnd: bool option with get, set
            abstract blockDoesNotCollapse: bool option with get, set
            abstract blockPadding: obj * obj * obj * obj option with get, set
            /// Message to be rendered when hovering over the glyph margin decoration.
            abstract glyphMarginHoverMessage: U2<IMarkdownString, ResizeArray<IMarkdownString>> option with get, set
            /// Array of MarkdownString to render as the decoration message.
            abstract hoverMessage: U2<IMarkdownString, ResizeArray<IMarkdownString>> option with get, set
            /// Should the decoration expand to encompass a whole line.
            abstract isWholeLine: bool option with get, set
            /// Always render the decoration (even when the range it encompasses is collapsed).
            abstract showIfCollapsed: bool option with get, set
            /// Specifies the stack order of a decoration.
            /// A decoration with greater stack order is always in front of a decoration with
            /// a lower stack order when the decorations are on the same line.
            abstract zIndex: float option with get, set
            /// If set, render this decoration in the overview ruler.
            abstract overviewRuler: IModelDecorationOverviewRulerOptions option with get, set
            /// If set, render this decoration in the minimap.
            abstract minimap: IModelDecorationMinimapOptions option with get, set
            /// If set, the decoration will be rendered in the glyph margin with this CSS class name.
            abstract glyphMarginClassName: string option with get, set
            /// <summary>
            /// If set and the decoration has <see cref="glyphMarginClassName" /> set, render this decoration
            /// with the specified <see cref="IModelDecorationGlyphMarginOptions" /> in the glyph margin.
            /// </summary>
            abstract glyphMargin: IModelDecorationGlyphMarginOptions option with get, set
            /// If set, the decoration will be rendered in the lines decorations with this CSS class name.
            abstract linesDecorationsClassName: string option with get, set
            /// If set, the decoration will be rendered in the lines decorations with this CSS class name, but only for the first line in case of line wrapping.
            abstract firstLineDecorationClassName: string option with get, set
            /// If set, the decoration will be rendered in the margin (covering its full width) with this CSS class name.
            abstract marginClassName: string option with get, set
            /// <summary>
            /// If set, the decoration will be rendered inline with the text with this CSS class name.
            /// Please use this only for CSS rules that must impact the text. For example, use <c>className</c>
            /// to have a background color decoration.
            /// </summary>
            abstract inlineClassName: string option with get, set
            /// <summary>If there is an <c>inlineClassName</c> which affects letter spacing.</summary>
            abstract inlineClassNameAffectsLetterSpacing: bool option with get, set
            /// If set, the decoration will be rendered before the text with this CSS class name.
            abstract beforeContentClassName: string option with get, set
            /// If set, the decoration will be rendered after the text with this CSS class name.
            abstract afterContentClassName: string option with get, set
            /// If set, text will be injected in the view after the range.
            abstract after: InjectedTextOptions option with get, set
            /// If set, text will be injected in the view before the range.
            abstract before: InjectedTextOptions option with get, set

        /// Configures text that is injected into the view without changing the underlying document.
        [<AllowNullLiteral>]
        type InjectedTextOptions =
            /// Sets the text to inject. Must be a single line.
            abstract content: string
            /// If set, the decoration will be rendered inline with the text with this CSS class name.
            abstract inlineClassName: string option
            /// <summary>If there is an <c>inlineClassName</c> which affects letter spacing.</summary>
            abstract inlineClassNameAffectsLetterSpacing: bool option
            /// This field allows to attach data to this injected text.
            /// The data can be read when injected texts at a given position are queried.
            abstract attachedData: obj option
            /// <summary>
            /// Configures cursor stops around injected text.
            /// Defaults to <see cref="InjectedTextCursorStops.Both" />.
            /// </summary>
            abstract cursorStops: InjectedTextCursorStops option

        [<RequireQualifiedAccess>]
        type InjectedTextCursorStops =
            | Both = 0
            | Right = 1
            | Left = 2
            | None = 3

        /// New model decorations.
        [<AllowNullLiteral>]
        type IModelDeltaDecoration =
            /// Range that this decoration covers.
            abstract range: IRange with get, set
            /// Options associated with this decoration.
            abstract options: IModelDecorationOptions with get, set

        /// A decoration in the model.
        [<AllowNullLiteral>]
        type IModelDecoration =
            /// Identifier for a decoration.
            abstract id: string
            /// Identifier for a decoration's owner.
            abstract ownerId: float
            /// Range that this decoration covers.
            abstract range: Range
            /// Options associated with this decoration.
            abstract options: IModelDecorationOptions

        /// End of line character preference.
        [<RequireQualifiedAccess>]
        type EndOfLinePreference =
            /// Use the end of line character identified in the text buffer.
            | TextDefined = 0
            /// Use line feed (\n) as the end of line character.
            | LF = 1
            /// Use carriage return and line feed (\r\n) as the end of line character.
            | CRLF = 2

        /// The default end of line to use when instantiating models.
        [<RequireQualifiedAccess>]
        type DefaultEndOfLine =
            /// Use line feed (\n) as the end of line character.
            | LF = 1
            /// Use carriage return and line feed (\r\n) as the end of line character.
            | CRLF = 2

        /// End of line character preference.
        [<RequireQualifiedAccess>]
        type EndOfLineSequence =
            /// Use line feed (\n) as the end of line character.
            | LF = 0
            /// Use carriage return and line feed (\r\n) as the end of line character.
            | CRLF = 1

        /// A single edit operation, that has an identifier.
        [<AllowNullLiteral>]
        type IIdentifiedSingleEditOperation =
            inherit ISingleEditOperation

        [<AllowNullLiteral>]
        type IValidEditOperation =
            /// The range to replace. This can be empty to emulate a simple insert.
            abstract range: Range with get, set
            /// The text to replace with. This can be empty to emulate a simple delete.
            abstract text: string with get, set

        /// A callback that can compute the cursor state after applying a series of edit operations.
        [<AllowNullLiteral>]
        type ICursorStateComputer =
            /// A callback that can compute the resulting cursors state after some edit operations have been executed.
            [<Emit("$0($1...)")>]
            abstract Invoke: inverseEditOperations: ResizeArray<IValidEditOperation> -> ResizeArray<Selection> option

        [<AllowNullLiteral>]
        type TextModelResolvedOptions =
            // abstract _textModelResolvedOptionsBrand: unit with get, set
            abstract tabSize: float
            abstract indentSize: float
            abstract insertSpaces: bool
            abstract defaultEOL: DefaultEndOfLine
            abstract trimAutoWhitespace: bool
            abstract bracketPairColorizationOptions: BracketPairColorizationOptions
            abstract originalIndentSize: U2<float, string>

        [<AllowNullLiteral>]
        type TextModelResolvedOptionsStatic =
            [<EmitConstructor>]
            abstract Create: unit -> TextModelResolvedOptions

        [<AllowNullLiteral>]
        type BracketPairColorizationOptions =
            abstract enabled: bool with get, set
            abstract independentColorPoolPerBracketType: bool with get, set

        [<AllowNullLiteral>]
        type ITextModelUpdateOptions =
            abstract tabSize: float option with get, set
            abstract indentSize: U2<float, string> option with get, set
            abstract insertSpaces: bool option with get, set
            abstract trimAutoWhitespace: bool option with get, set
            abstract bracketColorizationOptions: BracketPairColorizationOptions option with get, set

        [<AllowNullLiteral>]
        type FindMatch =
            // abstract _findMatchBrand: unit with get, set
            abstract range: Range
            abstract matches: ResizeArray<string> option

        [<AllowNullLiteral>]
        type FindMatchStatic =
            [<EmitConstructor>]
            abstract Create: unit -> FindMatch

        /// <summary>
        /// Describes the behavior of decorations when typing/editing near their edges.
        /// Note: Please do not edit the values, as they very carefully match <c>DecorationRangeBehavior</c>
        /// </summary>
        [<RequireQualifiedAccess>]
        type TrackedRangeStickiness =
            | AlwaysGrowsWhenTypingAtEdges = 0
            | NeverGrowsWhenTypingAtEdges = 1
            | GrowsOnlyWhenTypingBefore = 2
            | GrowsOnlyWhenTypingAfter = 3

        /// Text snapshot that works like an iterator.
        /// Will try to return chunks of roughly ~64KB size.
        /// Will return null when finished.
        [<AllowNullLiteral>]
        type ITextSnapshot =
            abstract read: unit -> string option

        /// A model.
        [<AllowNullLiteral>]
        type ITextModel =
            /// Gets the resource associated with this editor model.
            abstract uri: Uri
            /// A unique identifier associated with this model.
            abstract id: string
            /// Get the resolved options for this model.
            abstract getOptions: unit -> TextModelResolvedOptions
            /// Get the current version id of the model.
            /// Anytime a change happens to the model (even undo/redo),
            /// the version id is incremented.
            abstract getVersionId: unit -> float
            /// Get the alternative version id of the model.
            /// This alternative version id is not always incremented,
            /// it will return the same values in the case of undo-redo.
            abstract getAlternativeVersionId: unit -> float
            /// Replace the entire text buffer value contained in this model.
            abstract setValue: newValue: U2<string, ITextSnapshot> -> unit
            /// <summary>Get the text stored in this model.</summary>
            /// <param name="eol">The end of line character preference. Defaults to <c>EndOfLinePreference.TextDefined</c>.</param>
            /// <param name="preserverBOM">Preserve a BOM character if it was detected when the model was constructed.</param>
            /// <returns>The text.</returns>
            abstract getValue: ?eol: EndOfLinePreference * ?preserveBOM: bool -> string
            /// <summary>Get the text stored in this model.</summary>
            /// <param name="preserverBOM">Preserve a BOM character if it was detected when the model was constructed.</param>
            /// <returns>The text snapshot (it is safe to consume it asynchronously).</returns>
            abstract createSnapshot: ?preserveBOM: bool -> ITextSnapshot
            /// Get the length of the text stored in this model.
            abstract getValueLength: ?eol: EndOfLinePreference * ?preserveBOM: bool -> float
            /// <summary>Get the text in a certain range.</summary>
            /// <param name="range">The range describing what text to get.</param>
            /// <param name="eol">The end of line character preference. This will only be used for multiline ranges. Defaults to <c>EndOfLinePreference.TextDefined</c>.</param>
            /// <returns>The text.</returns>
            abstract getValueInRange: range: IRange * ?eol: EndOfLinePreference -> string
            /// <summary>Get the length of text in a certain range.</summary>
            /// <param name="range">The range describing what text length to get.</param>
            /// <returns>The text length.</returns>
            abstract getValueLengthInRange: range: IRange * ?eol: EndOfLinePreference -> float
            /// <summary>Get the character count of text in a certain range.</summary>
            /// <param name="range">The range describing what text length to get.</param>
            abstract getCharacterCountInRange: range: IRange * ?eol: EndOfLinePreference -> float
            /// Get the number of lines in the model.
            abstract getLineCount: unit -> float
            /// Get the text for a certain line.
            abstract getLineContent: lineNumber: float -> string
            /// Get the text length for a certain line.
            abstract getLineLength: lineNumber: float -> float
            /// Get the text for all lines.
            abstract getLinesContent: unit -> ResizeArray<string>
            /// <summary>Get the end of line sequence predominantly used in the text buffer.</summary>
            /// <returns>EOL char sequence (e.g.: '\n' or '\r\n').</returns>
            abstract getEOL: unit -> string
            /// Get the end of line sequence predominantly used in the text buffer.
            abstract getEndOfLineSequence: unit -> EndOfLineSequence
            /// <summary>Get the minimum legal column for line at <c>lineNumber</c></summary>
            abstract getLineMinColumn: lineNumber: float -> float
            /// <summary>Get the maximum legal column for line at <c>lineNumber</c></summary>
            abstract getLineMaxColumn: lineNumber: float -> float
            /// <summary>
            /// Returns the column before the first non whitespace character for line at <c>lineNumber</c>.
            /// Returns 0 if line is empty or contains only whitespace.
            /// </summary>
            abstract getLineFirstNonWhitespaceColumn: lineNumber: float -> float
            /// <summary>
            /// Returns the column after the last non whitespace character for line at <c>lineNumber</c>.
            /// Returns 0 if line is empty or contains only whitespace.
            /// </summary>
            abstract getLineLastNonWhitespaceColumn: lineNumber: float -> float
            /// Create a valid position.
            abstract validatePosition: position: IPosition -> Position
            /// Advances the given position by the given offset (negative offsets are also accepted)
            /// and returns it as a new valid position.
            ///
            /// If the offset and position are such that their combination goes beyond the beginning or
            /// end of the model, throws an exception.
            ///
            /// If the offset is such that the new position would be in the middle of a multi-byte
            /// line terminator, throws an exception.
            abstract modifyPosition: position: IPosition * offset: float -> Position
            /// Create a valid range.
            abstract validateRange: range: IRange -> Range
            /// <summary>
            /// Converts the position to a zero-based offset.
            ///
            /// The position will be <see cref="TextDocument.validatePosition">adjusted</see>.
            /// </summary>
            /// <param name="position">A position.</param>
            /// <returns>A valid zero-based offset.</returns>
            abstract getOffsetAt: position: IPosition -> float
            /// <summary>Converts a zero-based offset to a position.</summary>
            /// <param name="offset">A zero-based offset.</param>
            /// <returns>A valid <see cref="Position">position</see>.</returns>
            abstract getPositionAt: offset: float -> Position
            /// Get a range covering the entire model.
            abstract getFullModelRange: unit -> Range
            /// Returns if the model was disposed or not.
            abstract isDisposed: unit -> bool

            /// <summary>Search the model.</summary>
            /// <param name="searchString">The string used to search. If it is a regular expression, set <c>isRegex</c> to true.</param>
            /// <param name="searchOnlyEditableRange">Limit the searching to only search inside the editable range of the model.</param>
            /// <param name="isRegex">Used to indicate that <c>searchString</c> is a regular expression.</param>
            /// <param name="matchCase">Force the matching to match lower/upper case exactly.</param>
            /// <param name="wordSeparators">Force the matching to match entire words only. Pass null otherwise.</param>
            /// <param name="captureMatches">The result will contain the captured groups.</param>
            /// <param name="limitResultCount">Limit the number of results</param>
            /// <returns>The ranges where the matches are. It is empty if not matches have been found.</returns>
            abstract findMatches:
                searchString: string *
                searchOnlyEditableRange: bool *
                isRegex: bool *
                matchCase: bool *
                wordSeparators: string option *
                captureMatches: bool *
                ?limitResultCount: float ->
                    ResizeArray<FindMatch>

            /// <summary>Search the model.</summary>
            /// <param name="searchString">The string used to search. If it is a regular expression, set <c>isRegex</c> to true.</param>
            /// <param name="searchScope">Limit the searching to only search inside these ranges.</param>
            /// <param name="isRegex">Used to indicate that <c>searchString</c> is a regular expression.</param>
            /// <param name="matchCase">Force the matching to match lower/upper case exactly.</param>
            /// <param name="wordSeparators">Force the matching to match entire words only. Pass null otherwise.</param>
            /// <param name="captureMatches">The result will contain the captured groups.</param>
            /// <param name="limitResultCount">Limit the number of results</param>
            /// <returns>The ranges where the matches are. It is empty if no matches have been found.</returns>
            abstract findMatches:
                searchString: string *
                searchScope: U2<IRange, ResizeArray<IRange>> *
                isRegex: bool *
                matchCase: bool *
                wordSeparators: string option *
                captureMatches: bool *
                ?limitResultCount: float ->
                    ResizeArray<FindMatch>

            /// <summary>Search the model for the next match. Loops to the beginning of the model if needed.</summary>
            /// <param name="searchString">The string used to search. If it is a regular expression, set <c>isRegex</c> to true.</param>
            /// <param name="searchStart">Start the searching at the specified position.</param>
            /// <param name="isRegex">Used to indicate that <c>searchString</c> is a regular expression.</param>
            /// <param name="matchCase">Force the matching to match lower/upper case exactly.</param>
            /// <param name="wordSeparators">Force the matching to match entire words only. Pass null otherwise.</param>
            /// <param name="captureMatches">The result will contain the captured groups.</param>
            /// <returns>The range where the next match is. It is null if no next match has been found.</returns>
            abstract findNextMatch:
                searchString: string *
                searchStart: IPosition *
                isRegex: bool *
                matchCase: bool *
                wordSeparators: string option *
                captureMatches: bool ->
                    FindMatch option

            /// <summary>Search the model for the previous match. Loops to the end of the model if needed.</summary>
            /// <param name="searchString">The string used to search. If it is a regular expression, set <c>isRegex</c> to true.</param>
            /// <param name="searchStart">Start the searching at the specified position.</param>
            /// <param name="isRegex">Used to indicate that <c>searchString</c> is a regular expression.</param>
            /// <param name="matchCase">Force the matching to match lower/upper case exactly.</param>
            /// <param name="wordSeparators">Force the matching to match entire words only. Pass null otherwise.</param>
            /// <param name="captureMatches">The result will contain the captured groups.</param>
            /// <returns>The range where the previous match is. It is null if no previous match has been found.</returns>
            abstract findPreviousMatch:
                searchString: string *
                searchStart: IPosition *
                isRegex: bool *
                matchCase: bool *
                wordSeparators: string option *
                captureMatches: bool ->
                    FindMatch option

            /// Get the language associated with this model.
            abstract getLanguageId: unit -> string
            /// <summary>Get the word under or besides <c>position</c>.</summary>
            /// <param name="position">The position to look for a word.</param>
            /// <returns>The word under or besides <c>position</c>. Might be null.</returns>
            abstract getWordAtPosition: position: IPosition -> IWordAtPosition option
            /// <summary>Get the word under or besides <c>position</c> trimmed to <c>position</c>.column</summary>
            /// <param name="position">The position to look for a word.</param>
            /// <returns>The word under or besides <c>position</c>. Will never be null.</returns>
            abstract getWordUntilPosition: position: IPosition -> IWordAtPosition

            /// <summary>
            /// Perform a minimum amount of operations, in order to transform the decorations
            /// identified by <c>oldDecorations</c> to the decorations described by <c>newDecorations</c>
            /// and returns the new identifiers associated with the resulting decorations.
            /// </summary>
            /// <param name="oldDecorations">Array containing previous decorations identifiers.</param>
            /// <param name="newDecorations">Array describing what decorations should result after the call.</param>
            /// <param name="ownerId">Identifies the editor id in which these decorations should appear. If no <c>ownerId</c> is provided, the decorations will appear in all editors that attach this model.</param>
            /// <returns>An array containing the new decorations identifiers.</returns>
            abstract deltaDecorations:
                oldDecorations: ResizeArray<string> *
                newDecorations: ResizeArray<IModelDeltaDecoration> *
                ?ownerId: float ->
                    ResizeArray<string>

            /// <summary>Get the options associated with a decoration.</summary>
            /// <param name="id">The decoration id.</param>
            /// <returns>The decoration options or null if the decoration was not found.</returns>
            abstract getDecorationOptions: id: string -> IModelDecorationOptions option
            /// <summary>Get the range associated with a decoration.</summary>
            /// <param name="id">The decoration id.</param>
            /// <returns>The decoration range or null if the decoration was not found.</returns>
            abstract getDecorationRange: id: string -> Range option

            /// <summary>Gets all the decorations for the line <c>lineNumber</c> as an array.</summary>
            /// <param name="lineNumber">The line number</param>
            /// <param name="ownerId">If set, it will ignore decorations belonging to other owners.</param>
            /// <param name="filterOutValidation">If set, it will ignore decorations specific to validation (i.e. warnings, errors).</param>
            /// <returns>An array with the decorations</returns>
            abstract getLineDecorations:
                lineNumber: float * ?ownerId: float * ?filterOutValidation: bool -> ResizeArray<IModelDecoration>

            /// <summary>Gets all the decorations for the lines between <c>startLineNumber</c> and <c>endLineNumber</c> as an array.</summary>
            /// <param name="startLineNumber">The start line number</param>
            /// <param name="endLineNumber">The end line number</param>
            /// <param name="ownerId">If set, it will ignore decorations belonging to other owners.</param>
            /// <param name="filterOutValidation">If set, it will ignore decorations specific to validation (i.e. warnings, errors).</param>
            /// <returns>An array with the decorations</returns>
            abstract getLinesDecorations:
                startLineNumber: float * endLineNumber: float * ?ownerId: float * ?filterOutValidation: bool ->
                    ResizeArray<IModelDecoration>

            /// <summary>
            /// Gets all the decorations in a range as an array. Only <c>startLineNumber</c> and <c>endLineNumber</c> from <c>range</c> are used for filtering.
            /// So for now it returns all the decorations on the same line as <c>range</c>.
            /// </summary>
            /// <param name="range">The range to search in</param>
            /// <param name="ownerId">If set, it will ignore decorations belonging to other owners.</param>
            /// <param name="filterOutValidation">If set, it will ignore decorations specific to validation (i.e. warnings, errors).</param>
            /// <param name="onlyMinimapDecorations">If set, it will return only decorations that render in the minimap.</param>
            /// <param name="onlyMarginDecorations">If set, it will return only decorations that render in the glyph margin.</param>
            /// <returns>An array with the decorations</returns>
            abstract getDecorationsInRange:
                range: IRange *
                ?ownerId: float *
                ?filterOutValidation: bool *
                ?onlyMinimapDecorations: bool *
                ?onlyMarginDecorations: bool ->
                    ResizeArray<IModelDecoration>

            /// <summary>Gets all the decorations as an array.</summary>
            /// <param name="ownerId">If set, it will ignore decorations belonging to other owners.</param>
            /// <param name="filterOutValidation">If set, it will ignore decorations specific to validation (i.e. warnings, errors).</param>
            abstract getAllDecorations: ?ownerId: float * ?filterOutValidation: bool -> ResizeArray<IModelDecoration>
            /// <summary>Gets all decorations that render in the glyph margin as an array.</summary>
            /// <param name="ownerId">If set, it will ignore decorations belonging to other owners.</param>
            abstract getAllMarginDecorations: ?ownerId: float -> ResizeArray<IModelDecoration>

            /// <summary>Gets all the decorations that should be rendered in the overview ruler as an array.</summary>
            /// <param name="ownerId">If set, it will ignore decorations belonging to other owners.</param>
            /// <param name="filterOutValidation">If set, it will ignore decorations specific to validation (i.e. warnings, errors).</param>
            abstract getOverviewRulerDecorations:
                ?ownerId: float * ?filterOutValidation: bool -> ResizeArray<IModelDecoration>

            /// <summary>Gets all the decorations that contain injected text.</summary>
            /// <param name="ownerId">If set, it will ignore decorations belonging to other owners.</param>
            abstract getInjectedTextDecorations: ?ownerId: float -> ResizeArray<IModelDecoration>
            /// Normalize a string containing whitespace according to indentation rules (converts to spaces or to tabs).
            abstract normalizeIndentation: str: string -> string
            /// Change the options of this model.
            abstract updateOptions: newOpts: ITextModelUpdateOptions -> unit
            /// Detect the indentation options for this model from its content.
            abstract detectIndentation: defaultInsertSpaces: bool * defaultTabSize: float -> unit
            /// Close the current undo-redo element.
            /// This offers a way to create an undo/redo stop point.
            abstract pushStackElement: unit -> unit
            /// Open the current undo-redo element.
            /// This offers a way to remove the current undo/redo stop point.
            abstract popStackElement: unit -> unit

            /// <summary>
            /// Push edit operations, basically editing the model. This is the preferred way
            /// of editing the model. The edit operations will land on the undo stack.
            /// </summary>
            /// <param name="beforeCursorState">The cursor state before the edit operations. This cursor state will be returned when <c>undo</c> or <c>redo</c> are invoked.</param>
            /// <param name="editOperations">The edit operations.</param>
            /// <param name="cursorStateComputer">A callback that can compute the resulting cursors state after the edit operations have been executed.</param>
            /// <returns>The cursor state returned by the <c>cursorStateComputer</c>.</returns>
            abstract pushEditOperations:
                beforeCursorState: ResizeArray<Selection> option *
                editOperations: ResizeArray<IIdentifiedSingleEditOperation> *
                cursorStateComputer: ICursorStateComputer ->
                    ResizeArray<Selection> option

            /// Change the end of line sequence. This is the preferred way of
            /// changing the eol sequence. This will land on the undo stack.
            abstract pushEOL: eol: EndOfLineSequence -> unit
            /// <summary>
            /// Edit the model without adding the edits to the undo stack.
            /// This can have dire consequences on the undo stack! See
            /// </summary>
            /// <param name="operations">The edit operations.</param>
            /// <returns>If desired, the inverse edit operations, that, when applied, will bring the model back to the previous state.</returns>
            abstract applyEdits: operations: ResizeArray<IIdentifiedSingleEditOperation> -> unit

            [<Emit("$0.applyEdits($1,false)")>]
            abstract applyEdits_false: operations: ResizeArray<IIdentifiedSingleEditOperation> -> unit

            [<Emit("$0.applyEdits($1,true)")>]
            abstract applyEdits_true:
                operations: ResizeArray<IIdentifiedSingleEditOperation> -> ResizeArray<IValidEditOperation>

            /// <summary>
            /// Change the end of line sequence without recording in the undo stack.
            /// This can have dire consequences on the undo stack! See
            /// </summary>
            abstract setEOL: eol: EndOfLineSequence -> unit
            /// <summary>An event emitted when the contents of the model have changed.</summary>
            abstract onDidChangeContent: listener: (IModelContentChangedEvent -> unit) -> IDisposable
            /// <summary>An event emitted when decorations of the model have changed.</summary>
            abstract onDidChangeDecorations: IEvent<IModelDecorationsChangedEvent>
            /// <summary>An event emitted when the model options have changed.</summary>
            abstract onDidChangeOptions: IEvent<IModelOptionsChangedEvent>
            /// <summary>An event emitted when the language associated with the model has changed.</summary>
            abstract onDidChangeLanguage: IEvent<IModelLanguageChangedEvent>
            /// <summary>An event emitted when the language configuration associated with the model has changed.</summary>
            abstract onDidChangeLanguageConfiguration: IEvent<IModelLanguageConfigurationChangedEvent>
            /// <summary>An event emitted when the model has been attached to the first editor or detached from the last editor.</summary>
            abstract onDidChangeAttached: IEvent<unit>
            /// <summary>An event emitted right before disposing the model.</summary>
            abstract onWillDispose: IEvent<unit>
            /// Destroy this model.
            abstract dispose: unit -> unit
            /// Returns if this model is attached to an editor or not.
            abstract isAttachedToEditor: unit -> bool

        [<RequireQualifiedAccess>]
        type PositionAffinity =
            /// Prefers the left most position.
            | Left = 0
            /// Prefers the right most position.
            | Right = 1
            /// No preference.
            | None = 2
            /// If the given position is on injected text, prefers the position left of it.
            | LeftOfInjectedText = 3
            /// If the given position is on injected text, prefers the position right of it.
            | RightOfInjectedText = 4

        /// A change
        [<AllowNullLiteral>]
        type IChange =
            abstract originalStartLineNumber: float
            abstract originalEndLineNumber: float
            abstract modifiedStartLineNumber: float
            abstract modifiedEndLineNumber: float

        /// A character level change.
        [<AllowNullLiteral>]
        type ICharChange =
            inherit IChange
            abstract originalStartColumn: float
            abstract originalEndColumn: float
            abstract modifiedStartColumn: float
            abstract modifiedEndColumn: float

        /// A line change
        [<AllowNullLiteral>]
        type ILineChange =
            inherit IChange
            abstract charChanges: ResizeArray<ICharChange> option

        [<AllowNullLiteral>]
        type IDimension =
            abstract width: float with get, set
            abstract height: float with get, set

        /// A builder and helper for edit operations for a command.
        [<AllowNullLiteral>]
        type IEditOperationBuilder =
            /// <summary>Add a new edit operation (a replace operation).</summary>
            /// <param name="range">The range to replace (delete). May be empty to represent a simple insert.</param>
            /// <param name="text">The text to replace with. May be null to represent a simple delete.</param>
            abstract addEditOperation: range: IRange * text: string option * ?forceMoveMarkers: bool -> unit
            /// <summary>
            /// Add a new edit operation (a replace operation).
            /// The inverse edits will be accessible in <c>ICursorStateComputerData.getInverseEditOperations()</c>
            /// </summary>
            /// <param name="range">The range to replace (delete). May be empty to represent a simple insert.</param>
            /// <param name="text">The text to replace with. May be null to represent a simple delete.</param>
            abstract addTrackedEditOperation: range: IRange * text: string option * ?forceMoveMarkers: bool -> unit
            /// <summary>
            /// Track <c>selection</c> when applying edit operations.
            /// A best effort will be made to not grow/expand the selection.
            /// An empty selection will clamp to a nearby character.
            /// </summary>
            /// <param name="selection">The selection to track.</param>
            /// <param name="trackPreviousOnEmpty">
            /// If set, and the selection is empty, indicates whether the selection
            /// should clamp to the previous or the next character.
            /// </param>
            /// <returns>A unique identifier.</returns>
            abstract trackSelection: selection: Selection * ?trackPreviousOnEmpty: bool -> string

        /// A helper for computing cursor state after a command.
        [<AllowNullLiteral>]
        type ICursorStateComputerData =
            /// Get the inverse edit operations of the added edit operations.
            abstract getInverseEditOperations: unit -> ResizeArray<IValidEditOperation>
            /// <summary>Get a previously tracked selection.</summary>
            /// <param name="id">The unique identifier returned by <c>trackSelection</c>.</param>
            /// <returns>The selection.</returns>
            abstract getTrackedSelection: id: string -> Selection

        /// A command that modifies text / cursor state on a model.
        [<AllowNullLiteral>]
        type ICommand =
            /// <summary>Get the edit operations needed to execute this command.</summary>
            /// <param name="model">The model the command will execute on.</param>
            /// <param name="builder">A helper to collect the needed edit operations and to track selections.</param>
            abstract getEditOperations: model: ITextModel * builder: IEditOperationBuilder -> unit
            /// <summary>Compute the cursor state after the edit operations were applied.</summary>
            /// <param name="model">The model the command has executed on.</param>
            /// <param name="helper">A helper to get inverse edit operations and to get previously tracked selections.</param>
            /// <returns>The cursor state after the command executed.</returns>
            abstract computeCursorState: model: ITextModel * helper: ICursorStateComputerData -> Selection

        /// A model for the diff editor.
        [<AllowNullLiteral>]
        type IDiffEditorModel =
            /// Original model.
            abstract original: ITextModel with get, set
            /// Modified model.
            abstract modified: ITextModel with get, set

        [<AllowNullLiteral>]
        type IDiffEditorViewModel =
            abstract model: IDiffEditorModel
            abstract waitForDiff: unit -> Promise<unit>

        /// <summary>An event describing that an editor has had its model reset (i.e. <c>editor.setModel()</c>).</summary>
        [<AllowNullLiteral>]
        type IModelChangedEvent =
            /// <summary>The <c>uri</c> of the previous model or null.</summary>
            abstract oldModelUrl: Uri option
            /// <summary>The <c>uri</c> of the new model or null.</summary>
            abstract newModelUrl: Uri option

        [<AllowNullLiteral>]
        type IContentSizeChangedEvent =
            abstract contentWidth: float
            abstract contentHeight: float
            abstract contentWidthChanged: bool
            abstract contentHeightChanged: bool

        [<AllowNullLiteral>]
        type INewScrollPosition =
            abstract scrollLeft: float option with get, set
            abstract scrollTop: float option with get, set

        [<AllowNullLiteral>]
        type IEditorAction =
            abstract id: string
            abstract label: string
            abstract alias: string
            abstract isSupported: unit -> bool
            abstract run: ?args: obj -> Promise<unit>

        type IEditorModel = U3<ITextModel, IDiffEditorModel, IDiffEditorViewModel>

        /// A (serializable) state of the cursors.
        [<AllowNullLiteral>]
        type ICursorState =
            abstract inSelectionMode: bool with get, set
            abstract selectionStart: IPosition with get, set
            abstract position: IPosition with get, set

        /// A (serializable) state of the view.
        [<AllowNullLiteral>]
        type IViewState =
            /// written by previous versions
            abstract scrollTop: float option with get, set
            /// written by previous versions
            abstract scrollTopWithoutViewZones: float option with get, set
            abstract scrollLeft: float with get, set
            abstract firstPosition: IPosition with get, set
            abstract firstPositionDeltaTop: float with get, set

        /// A (serializable) state of the code editor.
        [<AllowNullLiteral>]
        type ICodeEditorViewState =
            abstract cursorState: ResizeArray<ICursorState> with get, set
            abstract viewState: IViewState with get, set
            abstract contributionsState: ICodeEditorViewStateContributionsState with get, set

        /// (Serializable) View state for the diff editor.
        [<AllowNullLiteral>]
        type IDiffEditorViewState =
            abstract original: ICodeEditorViewState option with get, set
            abstract modified: ICodeEditorViewState option with get, set
            abstract modelState: obj option with get, set

        /// An editor view state.
        type IEditorViewState = U2<ICodeEditorViewState, IDiffEditorViewState>

        [<RequireQualifiedAccess>]
        type ScrollType =
            | Smooth = 0
            | Immediate = 1

        /// An editor.
        [<AllowNullLiteral>]
        type IEditor =
            /// <summary>An event emitted when the editor has been disposed.</summary>
            abstract onDidDispose: listener: (unit -> unit) -> IDisposable
            /// Dispose the editor.
            abstract dispose: unit -> unit
            /// Get a unique id for this editor instance.
            abstract getId: unit -> string
            /// <summary>
            /// Get the editor type. Please see <c>EditorType</c>.
            /// This is to avoid an instanceof check
            /// </summary>
            abstract getEditorType: unit -> string
            /// Update the editor's options after the editor has been created.
            abstract updateOptions: newOptions: IEditorOptions -> unit
            /// Instructs the editor to remeasure its container. This method should
            /// be called when the container of the editor gets resized.
            ///
            /// If a dimension is passed in, the passed in value will be used.
            abstract layout: ?dimension: IDimension -> unit
            /// Brings browser focus to the editor text
            abstract focus: unit -> unit
            /// Returns true if the text inside this editor is focused (i.e. cursor is blinking).
            abstract hasTextFocus: unit -> bool
            /// Returns all actions associated with this editor.
            abstract getSupportedActions: unit -> ResizeArray<IEditorAction>
            /// Saves current view state of the editor in a serializable object.
            abstract saveViewState: unit -> IEditorViewState option
            /// <summary>Restores the view state of the editor from a serializable object generated by <c>saveViewState</c>.</summary>
            abstract restoreViewState: state: IEditorViewState option -> unit
            /// Given a position, returns a column number that takes tab-widths into account.
            abstract getVisibleColumnFromPosition: position: IPosition -> float
            /// Returns the primary position of the cursor.
            abstract getPosition: unit -> Position option
            /// <summary>Set the primary position of the cursor. This will remove any secondary cursors.</summary>
            /// <param name="position">New primary cursor's position</param>
            /// <param name="source">Source of the call that caused the position</param>
            abstract setPosition: position: IPosition * ?source: string -> unit
            /// Scroll vertically as necessary and reveal a line.
            abstract revealLine: lineNumber: float * ?scrollType: ScrollType -> unit
            /// Scroll vertically as necessary and reveal a line centered vertically.
            abstract revealLineInCenter: lineNumber: float * ?scrollType: ScrollType -> unit
            /// Scroll vertically as necessary and reveal a line centered vertically only if it lies outside the viewport.
            abstract revealLineInCenterIfOutsideViewport: lineNumber: float * ?scrollType: ScrollType -> unit
            /// Scroll vertically as necessary and reveal a line close to the top of the viewport,
            /// optimized for viewing a code definition.
            abstract revealLineNearTop: lineNumber: float * ?scrollType: ScrollType -> unit
            /// Scroll vertically or horizontally as necessary and reveal a position.
            abstract revealPosition: position: IPosition * ?scrollType: ScrollType -> unit
            /// Scroll vertically or horizontally as necessary and reveal a position centered vertically.
            abstract revealPositionInCenter: position: IPosition * ?scrollType: ScrollType -> unit
            /// Scroll vertically or horizontally as necessary and reveal a position centered vertically only if it lies outside the viewport.
            abstract revealPositionInCenterIfOutsideViewport: position: IPosition * ?scrollType: ScrollType -> unit
            /// Scroll vertically or horizontally as necessary and reveal a position close to the top of the viewport,
            /// optimized for viewing a code definition.
            abstract revealPositionNearTop: position: IPosition * ?scrollType: ScrollType -> unit
            /// Returns the primary selection of the editor.
            abstract getSelection: unit -> Selection option
            /// Returns all the selections of the editor.
            abstract getSelections: unit -> ResizeArray<Selection> option
            /// <summary>Set the primary selection of the editor. This will remove any secondary cursors.</summary>
            /// <param name="selection">The new selection</param>
            /// <param name="source">Source of the call that caused the selection</param>
            abstract setSelection: selection: IRange * ?source: string -> unit
            /// <summary>Set the primary selection of the editor. This will remove any secondary cursors.</summary>
            /// <param name="selection">The new selection</param>
            /// <param name="source">Source of the call that caused the selection</param>
            abstract setSelection: selection: Range * ?source: string -> unit
            /// <summary>Set the primary selection of the editor. This will remove any secondary cursors.</summary>
            /// <param name="selection">The new selection</param>
            /// <param name="source">Source of the call that caused the selection</param>
            abstract setSelection: selection: ISelection * ?source: string -> unit
            /// <summary>Set the primary selection of the editor. This will remove any secondary cursors.</summary>
            /// <param name="selection">The new selection</param>
            /// <param name="source">Source of the call that caused the selection</param>
            abstract setSelection: selection: Selection * ?source: string -> unit
            /// <summary>
            /// Set the selections for all the cursors of the editor.
            /// Cursors will be removed or added, as necessary.
            /// </summary>
            /// <param name="selections">The new selection</param>
            /// <param name="source">Source of the call that caused the selection</param>
            abstract setSelections: selections: ResizeArray<ISelection> * ?source: string -> unit
            /// Scroll vertically as necessary and reveal lines.
            abstract revealLines: startLineNumber: float * endLineNumber: float * ?scrollType: ScrollType -> unit
            /// Scroll vertically as necessary and reveal lines centered vertically.
            abstract revealLinesInCenter: lineNumber: float * endLineNumber: float * ?scrollType: ScrollType -> unit

            /// Scroll vertically as necessary and reveal lines centered vertically only if it lies outside the viewport.
            abstract revealLinesInCenterIfOutsideViewport:
                lineNumber: float * endLineNumber: float * ?scrollType: ScrollType -> unit

            /// Scroll vertically as necessary and reveal lines close to the top of the viewport,
            /// optimized for viewing a code definition.
            abstract revealLinesNearTop: lineNumber: float * endLineNumber: float * ?scrollType: ScrollType -> unit
            /// Scroll vertically or horizontally as necessary and reveal a range.
            abstract revealRange: range: IRange * ?scrollType: ScrollType -> unit
            /// Scroll vertically or horizontally as necessary and reveal a range centered vertically.
            abstract revealRangeInCenter: range: IRange * ?scrollType: ScrollType -> unit
            /// Scroll vertically or horizontally as necessary and reveal a range at the top of the viewport.
            abstract revealRangeAtTop: range: IRange * ?scrollType: ScrollType -> unit
            /// Scroll vertically or horizontally as necessary and reveal a range centered vertically only if it lies outside the viewport.
            abstract revealRangeInCenterIfOutsideViewport: range: IRange * ?scrollType: ScrollType -> unit
            /// Scroll vertically or horizontally as necessary and reveal a range close to the top of the viewport,
            /// optimized for viewing a code definition.
            abstract revealRangeNearTop: range: IRange * ?scrollType: ScrollType -> unit
            /// Scroll vertically or horizontally as necessary and reveal a range close to the top of the viewport,
            /// optimized for viewing a code definition. Only if it lies outside the viewport.
            abstract revealRangeNearTopIfOutsideViewport: range: IRange * ?scrollType: ScrollType -> unit
            /// <summary>Directly trigger a handler or an editor action.</summary>
            /// <param name="source">The source of the call.</param>
            /// <param name="handlerId">The id of the handler or the id of a contribution.</param>
            /// <param name="payload">Extra data to be sent to the handler.</param>
            abstract trigger: source: string option * handlerId: string * payload: obj option -> unit
            /// Gets the current model attached to this editor.
            abstract getModel: unit -> IEditorModel option
            /// Sets the current model attached to this editor.
            /// If the previous model was created by the editor via the value key in the options
            /// literal object, it will be destroyed. Otherwise, if the previous model was set
            /// via setModel, or the model key in the options literal object, the previous model
            /// will not be destroyed.
            /// It is safe to call setModel(null) to simply detach the current model from the editor.
            abstract setModel: model: IEditorModel option -> unit

            /// Create a collection of decorations. All decorations added through this collection
            /// will get the ownerId of the editor (meaning they will not show up in other editors).
            /// These decorations will be automatically cleared when the editor's model changes.
            abstract createDecorationsCollection:
                ?decorations: ResizeArray<IModelDeltaDecoration> -> IEditorDecorationsCollection

        /// A collection of decorations
        [<AllowNullLiteral>]
        type IEditorDecorationsCollection =
            /// An event emitted when decorations change in the editor,
            /// but the change is not caused by us setting or clearing the collection.
            abstract onDidChange: IEvent<IModelDecorationsChangedEvent> with get, set
            /// Get the decorations count.
            abstract length: float with get, set
            /// Get the range for a decoration.
            abstract getRange: index: float -> Range option
            /// Get all ranges for decorations.
            abstract getRanges: unit -> ResizeArray<Range>
            /// Determine if a decoration is in this collection.
            abstract has: decoration: IModelDecoration -> bool
            /// <summary>Replace all previous decorations with <c>newDecorations</c>.</summary>
            abstract set: newDecorations: ResizeArray<IModelDeltaDecoration> -> ResizeArray<string>
            /// Remove all previous decorations.
            abstract clear: unit -> unit

        /// An editor contribution that gets created every time a new editor gets created and gets disposed when the editor gets disposed.
        [<AllowNullLiteral>]
        type IEditorContribution =
            /// Dispose this contribution.
            abstract dispose: unit -> unit
            /// Store view state.
            abstract saveViewState: unit -> obj option
            /// Restore view state.
            abstract restoreViewState: state: obj option -> unit

        /// An event describing that the current language associated with a model has changed.
        [<AllowNullLiteral>]
        type IModelLanguageChangedEvent =
            /// Previous language
            abstract oldLanguage: string
            /// New language
            abstract newLanguage: string
            /// Source of the call that caused the event.
            abstract source: string

        /// An event describing that the language configuration associated with a model has changed.
        [<AllowNullLiteral>]
        type IModelLanguageConfigurationChangedEvent =
            interface
            end

        [<AllowNullLiteral>]
        type IModelContentChange =
            /// The range that got replaced.
            abstract range: IRange
            /// The offset of the range that got replaced.
            abstract rangeOffset: float
            /// The length of the range that got replaced.
            abstract rangeLength: float
            /// The new text for the range.
            abstract text: string

        /// An event describing a change in the text of a model.
        [<AllowNullLiteral>]
        type IModelContentChangedEvent =
            abstract changes: ResizeArray<IModelContentChange>
            /// The (new) end-of-line character.
            abstract eol: string
            /// The new version id the model has transitioned to.
            abstract versionId: float
            /// Flag that indicates that this event was generated while undoing.
            abstract isUndoing: bool
            /// Flag that indicates that this event was generated while redoing.
            abstract isRedoing: bool
            /// Flag that indicates that all decorations were lost with this edit.
            /// The model has been reset to a new value.
            abstract isFlush: bool
            /// Flag that indicates that this event describes an eol change.
            abstract isEolChange: bool

        /// An event describing that model decorations have changed.
        [<AllowNullLiteral>]
        type IModelDecorationsChangedEvent =
            abstract affectsMinimap: bool
            abstract affectsOverviewRuler: bool
            abstract affectsGlyphMargin: bool

        [<AllowNullLiteral>]
        type IModelOptionsChangedEvent =
            abstract tabSize: bool
            abstract indentSize: bool
            abstract insertSpaces: bool
            abstract trimAutoWhitespace: bool

        /// Describes the reason the cursor has changed its position.
        [<RequireQualifiedAccess>]
        type CursorChangeReason =
            /// Unknown or not set.
            | NotSet = 0
            /// <summary>A <c>model.setValue()</c> was called.</summary>
            | ContentFlush = 1
            /// <summary>The <c>model</c> has been changed outside of this cursor and the cursor recovers its position from associated markers.</summary>
            | RecoverFromMarkers = 2
            /// There was an explicit user gesture.
            | Explicit = 3
            /// There was a Paste.
            | Paste = 4
            /// There was an Undo.
            | Undo = 5
            /// There was a Redo.
            | Redo = 6

        /// An event describing that the cursor position has changed.
        [<AllowNullLiteral>]
        type ICursorPositionChangedEvent =
            /// Primary cursor's position.
            abstract position: Position
            /// Secondary cursors' position.
            abstract secondaryPositions: ResizeArray<Position>
            /// Reason.
            abstract reason: CursorChangeReason
            /// Source of the call that caused the event.
            abstract source: string

        /// An event describing that the cursor selection has changed.
        [<AllowNullLiteral>]
        type ICursorSelectionChangedEvent =
            /// The primary selection.
            abstract selection: Selection
            /// The secondary selections.
            abstract secondarySelections: ResizeArray<Selection>
            /// The model version id.
            abstract modelVersionId: float
            /// The old selections.
            abstract oldSelections: ResizeArray<Selection> option
            /// <summary>The model version id the that <c>oldSelections</c> refer to.</summary>
            abstract oldModelVersionId: float
            /// Source of the call that caused the event.
            abstract source: string
            /// Reason.
            abstract reason: CursorChangeReason

        [<RequireQualifiedAccess>]
        type AccessibilitySupport =
            /// This should be the browser case where it is not known if a screen reader is attached or no.
            | Unknown = 0
            | Disabled = 1
            | Enabled = 2

        /// Configuration options for auto closing quotes and brackets
        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type EditorAutoClosingStrategy =
            | Always
            | LanguageDefined
            | BeforeWhitespace
            | Never

        /// Configuration options for auto wrapping quotes and brackets
        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type EditorAutoSurroundStrategy =
            | LanguageDefined
            | Quotes
            | Brackets
            | Never

        /// Configuration options for typing over closing quotes or brackets
        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type EditorAutoClosingEditStrategy =
            | Always
            | Auto
            | Never

        /// Configuration options for auto indentation in the editor
        [<RequireQualifiedAccess>]
        type EditorAutoIndentStrategy =
            | None = 0
            | Keep = 1
            | Brackets = 2
            | Advanced = 3
            | Full = 4

        /// Configuration options for the editor.
        [<AllowNullLiteral>]
        type IEditorOptions =
            /// This editor is used inside a diff editor.
            abstract inDiffEditor: bool option with get, set
            /// The aria label for the editor's textarea (when it is focused).
            abstract ariaLabel: string option with get, set
            /// Whether the aria-required attribute should be set on the editors textarea.
            abstract ariaRequired: bool option with get, set
            /// Control whether a screen reader announces inline suggestion content immediately.
            abstract screenReaderAnnounceInlineSuggestion: bool option with get, set
            /// <summary>The <c>tabindex</c> property of the editor's textarea</summary>
            abstract tabIndex: float option with get, set
            /// Render vertical lines at the specified columns.
            /// Defaults to empty array.
            abstract rulers: ResizeArray<U2<float, IRulerOption>> option with get, set
            /// A string containing the word separators used when doing word navigation.
            /// Defaults to `~!@#$%^&*()-=+[{]}\\|;:\'",.<>/?
            abstract wordSeparators: string option with get, set
            /// Enable Linux primary clipboard.
            /// Defaults to true.
            abstract selectionClipboard: bool option with get, set
            /// <summary>
            /// Control the rendering of line numbers.
            /// If it is a function, it will be invoked when rendering a line number and the return value will be rendered.
            /// Otherwise, if it is a truthy, line numbers will be rendered normally (equivalent of using an identity function).
            /// Otherwise, line numbers will not be rendered.
            /// Defaults to <c>on</c>.
            /// </summary>
            abstract lineNumbers: LineNumbersType option with get, set
            /// Controls the minimal number of visible leading and trailing lines surrounding the cursor.
            /// Defaults to 0.
            abstract cursorSurroundingLines: float option with get, set
            /// <summary>
            /// Controls when <c>cursorSurroundingLines</c> should be enforced
            /// Defaults to <c>default</c>, <c>cursorSurroundingLines</c> is not enforced when cursor position is changed
            /// by mouse.
            /// </summary>
            abstract cursorSurroundingLinesStyle: IExportsEditorOptionsCursorSurroundingLinesStyleIEditorOption option with get, set
            /// Render last line number when the file ends with a newline.
            /// Defaults to 'on' for Windows and macOS and 'dimmed' for Linux.
            abstract renderFinalNewline: IExportsEditorOptionsRenderFinalNewlineIEditorOption option with get, set
            /// Remove unusual line terminators like LINE SEPARATOR (LS), PARAGRAPH SEPARATOR (PS).
            /// Defaults to 'prompt'.
            abstract unusualLineTerminators: IExportsEditorOptionsUnusualLineTerminatorsIEditorOption option with get, set
            /// Should the corresponding line be selected when clicking on the line number?
            /// Defaults to true.
            abstract selectOnLineNumbers: bool option with get, set
            /// Control the width of line numbers, by reserving horizontal space for rendering at least an amount of digits.
            /// Defaults to 5.
            abstract lineNumbersMinChars: float option with get, set
            /// Enable the rendering of the glyph margin.
            /// Defaults to true in vscode and to false in monaco-editor.
            abstract glyphMargin: bool option with get, set
            /// The width reserved for line decorations (in px).
            /// Line decorations are placed between line numbers and the editor content.
            /// You can pass in a string in the format floating point followed by "ch". e.g. 1.3ch.
            /// Defaults to 10.
            abstract lineDecorationsWidth: U2<float, string> option with get, set
            /// When revealing the cursor, a virtual padding (px) is added to the cursor, turning it into a rectangle.
            /// This virtual padding ensures that the cursor gets revealed before hitting the edge of the viewport.
            /// Defaults to 30 (px).
            abstract revealHorizontalRightPadding: float option with get, set
            /// Render the editor selection with rounded borders.
            /// Defaults to true.
            abstract roundedSelection: bool option with get, set
            /// Class name to be added to the editor.
            abstract extraEditorClassName: string option with get, set
            /// <summary>
            /// Should the editor be read only. See also <c>domReadOnly</c>.
            /// Defaults to false.
            /// </summary>
            abstract readOnly: bool option with get, set
            /// The message to display when the editor is readonly.
            abstract readOnlyMessage: IMarkdownString option with get, set
            /// <summary>
            /// Should the textarea used for input use the DOM <c>readonly</c> attribute.
            /// Defaults to false.
            /// </summary>
            abstract domReadOnly: bool option with get, set
            /// Enable linked editing.
            /// Defaults to false.
            abstract linkedEditing: bool option with get, set
            /// deprecated, use linkedEditing instead
            abstract renameOnType: bool option with get, set
            /// Should the editor render validation decorations.
            /// Defaults to editable.
            abstract renderValidationDecorations: IEditorOptionsRenderValidationDecorations option with get, set
            /// Control the behavior and rendering of the scrollbars.
            abstract scrollbar: IEditorScrollbarOptions option with get, set
            /// Control the behavior of sticky scroll options
            abstract stickyScroll: IEditorStickyScrollOptions option with get, set
            /// Control the behavior and rendering of the minimap.
            abstract minimap: IEditorMinimapOptions option with get, set
            /// Control the behavior of the find widget.
            abstract find: IEditorFindOptions option with get, set
            /// <summary>
            /// Display overflow widgets as <c>fixed</c>.
            /// Defaults to <c>false</c>.
            /// </summary>
            abstract fixedOverflowWidgets: bool option with get, set
            /// The number of vertical lanes the overview ruler should render.
            /// Defaults to 3.
            abstract overviewRulerLanes: float option with get, set
            /// <summary>
            /// Controls if a border should be drawn around the overview ruler.
            /// Defaults to <c>true</c>.
            /// </summary>
            abstract overviewRulerBorder: bool option with get, set
            /// Control the cursor animation style, possible values are 'blink', 'smooth', 'phase', 'expand' and 'solid'.
            /// Defaults to 'blink'.
            abstract cursorBlinking: IEditorOptionsCursorBlinking option with get, set
            /// Zoom the font in the editor when using the mouse wheel in combination with holding Ctrl.
            /// Defaults to false.
            abstract mouseWheelZoom: bool option with get, set
            /// Control the mouse pointer style, either 'text' or 'default' or 'copy'
            /// Defaults to 'text'
            abstract mouseStyle: IEditorOptionsMouseStyle option with get, set
            /// Enable smooth caret animation.
            /// Defaults to 'off'.
            abstract cursorSmoothCaretAnimation: IEditorOptionsCursorSmoothCaretAnimation option with get, set
            /// Control the cursor style, either 'block' or 'line'.
            /// Defaults to 'line'.
            abstract cursorStyle: IEditorOptionsCursorStyle option with get, set
            /// Control the width of the cursor when cursorStyle is set to 'line'
            abstract cursorWidth: float option with get, set
            /// Enable font ligatures.
            /// Defaults to false.
            abstract fontLigatures: U2<bool, string> option with get, set
            /// Enable font variations.
            /// Defaults to false.
            abstract fontVariations: U2<bool, string> option with get, set
            /// Controls whether to use default color decorations or not using the default document color provider
            abstract defaultColorDecorators: bool option with get, set
            /// <summary>
            /// Disable the use of <c>transform: translate3d(0px, 0px, 0px)</c> for the editor margin and lines layers.
            /// The usage of <c>transform: translate3d(0px, 0px, 0px)</c> acts as a hint for browsers to create an extra layer.
            /// Defaults to false.
            /// </summary>
            abstract disableLayerHinting: bool option with get, set
            /// Disable the optimizations for monospace fonts.
            /// Defaults to false.
            abstract disableMonospaceOptimizations: bool option with get, set
            /// Should the cursor be hidden in the overview ruler.
            /// Defaults to false.
            abstract hideCursorInOverviewRuler: bool option with get, set
            /// Enable that scrolling can go one screen size after the last line.
            /// Defaults to true.
            abstract scrollBeyondLastLine: bool option with get, set
            /// Enable that scrolling can go beyond the last column by a number of columns.
            /// Defaults to 5.
            abstract scrollBeyondLastColumn: float option with get, set
            /// Enable that the editor animates scrolling to a position.
            /// Defaults to false.
            abstract smoothScrolling: bool option with get, set
            /// Enable that the editor will install a ResizeObserver to check if its container dom node size has changed.
            /// Defaults to false.
            abstract automaticLayout: bool option with get, set
            /// <summary>
            /// Control the wrapping of the editor.
            /// When <c>wordWrap</c> = "off", the lines will never wrap.
            /// When <c>wordWrap</c> = "on", the lines will wrap at the viewport width.
            /// When <c>wordWrap</c> = "wordWrapColumn", the lines will wrap at <c>wordWrapColumn</c>.
            /// When <c>wordWrap</c> = "bounded", the lines will wrap at min(viewport width, wordWrapColumn).
            /// Defaults to "off".
            /// </summary>
            abstract wordWrap: IEditorOptionsWordWrap option with get, set
            /// <summary>Override the <c>wordWrap</c> setting.</summary>
            abstract wordWrapOverride1: IEditorOptionsWordWrapOverride1 option with get, set
            /// <summary>Override the <c>wordWrapOverride1</c> setting.</summary>
            abstract wordWrapOverride2: IEditorOptionsWordWrapOverride1 option with get, set
            /// <summary>
            /// Control the wrapping of the editor.
            /// When <c>wordWrap</c> = "off", the lines will never wrap.
            /// When <c>wordWrap</c> = "on", the lines will wrap at the viewport width.
            /// When <c>wordWrap</c> = "wordWrapColumn", the lines will wrap at <c>wordWrapColumn</c>.
            /// When <c>wordWrap</c> = "bounded", the lines will wrap at min(viewport width, wordWrapColumn).
            /// Defaults to 80.
            /// </summary>
            abstract wordWrapColumn: float option with get, set
            /// Control indentation of wrapped lines. Can be: 'none', 'same', 'indent' or 'deepIndent'.
            /// Defaults to 'same' in vscode and to 'none' in monaco-editor.
            abstract wrappingIndent: IEditorOptionsWrappingIndent option with get, set
            /// Controls the wrapping strategy to use.
            /// Defaults to 'simple'.
            abstract wrappingStrategy: IExportsEditorOptionsWrappingStrategyIEditorOption option with get, set
            /// Configure word wrapping characters. A break will be introduced before these characters.
            abstract wordWrapBreakBeforeCharacters: string option with get, set
            /// Configure word wrapping characters. A break will be introduced after these characters.
            abstract wordWrapBreakAfterCharacters: string option with get, set
            /// Sets whether line breaks appear wherever the text would otherwise overflow its content box.
            /// When wordBreak = 'normal', Use the default line break rule.
            /// When wordBreak = 'keepAll', Word breaks should not be used for Chinese/Japanese/Korean (CJK) text. Non-CJK text behavior is the same as for normal.
            abstract wordBreak: IExportsEditorOptionsWordBreakIEditorOption option with get, set
            /// Performance guard: Stop rendering a line after x characters.
            /// Defaults to 10000.
            /// Use -1 to never stop rendering
            abstract stopRenderingLineAfter: float option with get, set
            /// Configure the editor's hover.
            abstract hover: IEditorHoverOptions option with get, set
            /// Enable detecting links and making them clickable.
            /// Defaults to true.
            abstract links: bool option with get, set
            /// Enable inline color decorators and color picker rendering.
            abstract colorDecorators: bool option with get, set
            /// Controls what is the condition to spawn a color picker from a color dectorator
            abstract colorDecoratorsActivatedOn: IExportsEditorOptionsColorDecoratorActivatedOnIEditorOption option with get, set
            /// Controls the max number of color decorators that can be rendered in an editor at once.
            abstract colorDecoratorsLimit: float option with get, set
            /// Control the behaviour of comments in the editor.
            abstract comments: IEditorCommentsOptions option with get, set
            /// Enable custom contextmenu.
            /// Defaults to true.
            abstract contextmenu: bool option with get, set
            /// <summary>
            /// A multiplier to be used on the <c>deltaX</c> and <c>deltaY</c> of mouse wheel scroll events.
            /// Defaults to 1.
            /// </summary>
            abstract mouseWheelScrollSensitivity: float option with get, set
            /// <summary>
            /// FastScrolling mulitplier speed when pressing <c>Alt</c>
            /// Defaults to 5.
            /// </summary>
            abstract fastScrollSensitivity: float option with get, set
            /// Enable that the editor scrolls only the predominant axis. Prevents horizontal drift when scrolling vertically on a trackpad.
            /// Defaults to true.
            abstract scrollPredominantAxis: bool option with get, set
            /// Enable that the selection with the mouse and keys is doing column selection.
            /// Defaults to false.
            abstract columnSelection: bool option with get, set
            /// The modifier to be used to add multiple cursors with the mouse.
            /// Defaults to 'alt'
            abstract multiCursorModifier: IEditorOptionsMultiCursorModifier option with get, set
            /// Merge overlapping selections.
            /// Defaults to true
            abstract multiCursorMergeOverlapping: bool option with get, set
            /// Configure the behaviour when pasting a text with the line count equal to the cursor count.
            /// Defaults to 'spread'.
            abstract multiCursorPaste: IExportsEditorOptionsMultiCursorPasteIEditorOption option with get, set
            /// Controls the max number of text cursors that can be in an active editor at once.
            abstract multiCursorLimit: float option with get, set
            /// Configure the editor's accessibility support.
            /// Defaults to 'auto'. It is best to leave this to 'auto'.
            abstract accessibilitySupport: IEditorOptionsAccessibilitySupport option with get, set
            /// Controls the number of lines in the editor that can be read out by a screen reader
            abstract accessibilityPageSize: float option with get, set
            /// Suggest options.
            abstract suggest: ISuggestOptions option with get, set
            abstract inlineSuggest: IInlineSuggestOptions option with get, set
            /// Smart select options.
            abstract smartSelect: ISmartSelectOptions option with get, set
            abstract gotoLocation: IGotoLocationOptions option with get, set
            /// Enable quick suggestions (shadow suggestions)
            /// Defaults to true.
            abstract quickSuggestions: U2<bool, IQuickSuggestionsOptions> option with get, set
            /// Quick suggestions show delay (in ms)
            /// Defaults to 10 (ms)
            abstract quickSuggestionsDelay: float option with get, set
            /// Controls the spacing around the editor.
            abstract padding: IEditorPaddingOptions option with get, set
            /// Parameter hint options.
            abstract parameterHints: IEditorParameterHintOptions option with get, set
            /// Options for auto closing brackets.
            /// Defaults to language defined behavior.
            abstract autoClosingBrackets: EditorAutoClosingStrategy option with get, set
            /// Options for auto closing comments.
            /// Defaults to language defined behavior.
            abstract autoClosingComments: EditorAutoClosingStrategy option with get, set
            /// Options for auto closing quotes.
            /// Defaults to language defined behavior.
            abstract autoClosingQuotes: EditorAutoClosingStrategy option with get, set
            /// Options for pressing backspace near quotes or bracket pairs.
            abstract autoClosingDelete: EditorAutoClosingEditStrategy option with get, set
            /// Options for typing over closing quotes or brackets.
            abstract autoClosingOvertype: EditorAutoClosingEditStrategy option with get, set
            /// Options for auto surrounding.
            /// Defaults to always allowing auto surrounding.
            abstract autoSurround: EditorAutoSurroundStrategy option with get, set
            /// Controls whether the editor should automatically adjust the indentation when users type, paste, move or indent lines.
            /// Defaults to advanced.
            abstract autoIndent: IEditorOptionsAutoIndent option with get, set
            /// Emulate selection behaviour of tab characters when using spaces for indentation.
            /// This means selection will stick to tab stops.
            abstract stickyTabStops: bool option with get, set
            /// Enable format on type.
            /// Defaults to false.
            abstract formatOnType: bool option with get, set
            /// Enable format on paste.
            /// Defaults to false.
            abstract formatOnPaste: bool option with get, set
            /// Controls if the editor should allow to move selections via drag and drop.
            /// Defaults to false.
            abstract dragAndDrop: bool option with get, set
            /// Enable the suggestion box to pop-up on trigger characters.
            /// Defaults to true.
            abstract suggestOnTriggerCharacters: bool option with get, set
            /// Accept suggestions on ENTER.
            /// Defaults to 'on'.
            abstract acceptSuggestionOnEnter: IEditorOptionsAcceptSuggestionOnEnter option with get, set
            /// Accept suggestions on provider defined characters.
            /// Defaults to true.
            abstract acceptSuggestionOnCommitCharacter: bool option with get, set
            /// Enable snippet suggestions. Default to 'true'.
            abstract snippetSuggestions: IEditorOptionsSnippetSuggestions option with get, set
            /// Copying without a selection copies the current line.
            abstract emptySelectionClipboard: bool option with get, set
            /// Syntax highlighting is copied.
            abstract copyWithSyntaxHighlighting: bool option with get, set
            /// The history mode for suggestions.
            abstract suggestSelection: IExportsEditorOptionsSuggestSelectionIEditorOption option with get, set
            /// The font size for the suggest widget.
            /// Defaults to the editor font size.
            abstract suggestFontSize: float option with get, set
            /// The line height for the suggest widget.
            /// Defaults to the editor line height.
            abstract suggestLineHeight: float option with get, set
            /// Enable tab completion.
            abstract tabCompletion: IExportsEditorOptionsTabCompletionIEditorOption option with get, set
            /// Enable selection highlight.
            /// Defaults to true.
            abstract selectionHighlight: bool option with get, set
            /// Enable semantic occurrences highlight.
            /// Defaults to true.
            abstract occurrencesHighlight: bool option with get, set
            /// Show code lens
            /// Defaults to true.
            abstract codeLens: bool option with get, set
            /// Code lens font family. Defaults to editor font family.
            abstract codeLensFontFamily: string option with get, set
            /// Code lens font size. Default to 90% of the editor font size
            abstract codeLensFontSize: float option with get, set
            /// Control the behavior and rendering of the code action lightbulb.
            abstract lightbulb: IEditorLightbulbOptions option with get, set
            /// Timeout for running code actions on save.
            abstract codeActionsOnSaveTimeout: float option with get, set
            /// Enable code folding.
            /// Defaults to true.
            abstract folding: bool option with get, set
            /// Selects the folding strategy. 'auto' uses the strategies contributed for the current document, 'indentation' uses the indentation based folding strategy.
            /// Defaults to 'auto'.
            abstract foldingStrategy: IExportsEditorOptionsFoldingStrategyIEditorOption option with get, set
            /// Enable highlight for folded regions.
            /// Defaults to true.
            abstract foldingHighlight: bool option with get, set
            /// Auto fold imports folding regions.
            /// Defaults to true.
            abstract foldingImportsByDefault: bool option with get, set
            /// Maximum number of foldable regions.
            /// Defaults to 5000.
            abstract foldingMaximumRegions: float option with get, set
            /// Controls whether the fold actions in the gutter stay always visible or hide unless the mouse is over the gutter.
            /// Defaults to 'mouseover'.
            abstract showFoldingControls: IExportsEditorOptionsShowFoldingControlsIEditorOption option with get, set
            /// Controls whether clicking on the empty content after a folded line will unfold the line.
            /// Defaults to false.
            abstract unfoldOnClickAfterEndOfLine: bool option with get, set
            /// Enable highlighting of matching brackets.
            /// Defaults to 'always'.
            abstract matchBrackets: IEditorOptionsMatchBrackets option with get, set
            /// Enable experimental whitespace rendering.
            /// Defaults to 'svg'.
            abstract experimentalWhitespaceRendering: IEditorOptionsExperimentalWhitespaceRendering option with get, set
            /// Enable rendering of whitespace.
            /// Defaults to 'selection'.
            abstract renderWhitespace: IEditorOptionsRenderWhitespace option with get, set
            /// Enable rendering of control characters.
            /// Defaults to true.
            abstract renderControlCharacters: bool option with get, set
            /// Enable rendering of current line highlight.
            /// Defaults to all.
            abstract renderLineHighlight: IEditorOptionsRenderLineHighlight option with get, set
            /// Control if the current line highlight should be rendered only the editor is focused.
            /// Defaults to false.
            abstract renderLineHighlightOnlyWhenFocus: bool option with get, set
            /// Inserting and deleting whitespace follows tab stops.
            abstract useTabStops: bool option with get, set
            /// The font family
            abstract fontFamily: string option with get, set
            /// The font weight
            abstract fontWeight: string option with get, set
            /// The font size
            abstract fontSize: float option with get, set
            /// The line height
            abstract lineHeight: float option with get, set
            /// The letter spacing
            abstract letterSpacing: float option with get, set
            /// Controls fading out of unused variables.
            abstract showUnused: bool option with get, set
            /// Controls whether to focus the inline editor in the peek widget by default.
            /// Defaults to false.
            abstract peekWidgetDefaultFocus: IExportsEditorOptionsPeekWidgetDefaultFocusIEditorOption option with get, set
            /// Controls whether the definition link opens element in the peek widget.
            /// Defaults to false.
            abstract definitionLinkOpensInPeek: bool option with get, set
            /// Controls strikethrough deprecated variables.
            abstract showDeprecated: bool option with get, set
            /// Controls whether suggestions allow matches in the middle of the word instead of only at the beginning
            abstract matchOnWordStartOnly: bool option with get, set
            /// Control the behavior and rendering of the inline hints.
            abstract inlayHints: IEditorInlayHintsOptions option with get, set
            /// Control if the editor should use shadow DOM.
            abstract useShadowDOM: bool option with get, set
            /// Controls the behavior of editor guides.
            abstract guides: IGuidesOptions option with get, set
            /// Controls the behavior of the unicode highlight feature
            /// (by default, ambiguous and invisible characters are highlighted).
            abstract unicodeHighlight: IUnicodeHighlightOptions option with get, set
            /// Configures bracket pair colorization (disabled by default).
            abstract bracketPairColorization: IBracketPairColorizationOptions option with get, set
            /// <summary>
            /// Controls dropping into the editor from an external source.
            ///
            /// When enabled, this shows a preview of the drop location and triggers an <c>onDropIntoEditor</c> event.
            /// </summary>
            abstract dropIntoEditor: IDropIntoEditorOptions option with get, set
            /// Controls support for changing how content is pasted into the editor.
            abstract pasteAs: IPasteAsOptions option with get, set
            /// Controls whether the editor / terminal receives tabs or defers them to the workbench for navigation.
            abstract tabFocusMode: bool option with get, set
            /// Controls whether the accessibility hint should be provided to screen reader users when an inline completion is shown.
            abstract inlineCompletionsAccessibilityVerbose: bool option with get, set

        [<AllowNullLiteral>]
        type IDiffEditorBaseOptions =
            /// Allow the user to resize the diff editor split view.
            /// Defaults to true.
            abstract enableSplitViewResizing: bool option with get, set
            /// The default ratio when rendering side-by-side editors.
            /// Must be a number between 0 and 1, min sizes apply.
            /// Defaults to 0.5
            abstract splitViewDefaultRatio: float option with get, set
            /// Render the differences in two side-by-side editors.
            /// Defaults to true.
            abstract renderSideBySide: bool option with get, set
            /// <summary>
            /// When <c>renderSideBySide</c> is enabled, <c>useInlineViewWhenSpaceIsLimited</c> is set,
            /// and the diff editor has a width less than <c>renderSideBySideInlineBreakpoint</c>, the inline view is used.
            /// </summary>
            abstract renderSideBySideInlineBreakpoint: float option with get, set
            /// <summary>
            /// When <c>renderSideBySide</c> is enabled, <c>useInlineViewWhenSpaceIsLimited</c> is set,
            /// and the diff editor has a width less than <c>renderSideBySideInlineBreakpoint</c>, the inline view is used.
            /// </summary>
            abstract useInlineViewWhenSpaceIsLimited: bool option with get, set
            /// Timeout in milliseconds after which diff computation is cancelled.
            /// Defaults to 5000.
            abstract maxComputationTime: float option with get, set
            /// Maximum supported file size in MB.
            /// Defaults to 50.
            abstract maxFileSize: float option with get, set
            /// Compute the diff by ignoring leading/trailing whitespace
            /// Defaults to true.
            abstract ignoreTrimWhitespace: bool option with get, set
            /// Render +/- indicators for added/deleted changes.
            /// Defaults to true.
            abstract renderIndicators: bool option with get, set
            /// Shows icons in the glyph margin to revert changes.
            /// Default to true.
            abstract renderMarginRevertIcon: bool option with get, set
            /// Original model should be editable?
            /// Defaults to false.
            abstract originalEditable: bool option with get, set
            /// Should the diff editor enable code lens?
            /// Defaults to false.
            abstract diffCodeLens: bool option with get, set
            /// Is the diff editor should render overview ruler
            /// Defaults to true
            abstract renderOverviewRuler: bool option with get, set
            /// Control the wrapping of the diff editor.
            abstract diffWordWrap: IEditorOptionsWordWrapOverride1 option with get, set
            /// Diff Algorithm
            abstract diffAlgorithm: IDiffEditorBaseOptionsDiffAlgorithm option with get, set
            /// Whether the diff editor aria label should be verbose.
            abstract accessibilityVerbose: bool option with get, set

            abstract experimental:
                {|
                    showMoves: bool option
                    showEmptyDecorations: bool option
                |} option with get, set

            /// Is the diff editor inside another editor
            /// Defaults to false
            abstract isInEmbeddedEditor: bool option with get, set
            /// If the diff editor should only show the difference review mode.
            abstract onlyShowAccessibleDiffViewer: bool option with get, set

            abstract hideUnchangedRegions:
                {|
                    enabled: bool option
                    revealLineCount: float option
                    minimumLineCount: float option
                    contextLineCount: float option
                |} option with get, set

        /// Configuration options for the diff editor.
        [<AllowNullLiteral>]
        type IDiffEditorOptions =
            inherit IEditorOptions
            inherit IDiffEditorBaseOptions

        /// An event describing that the configuration of the editor has changed.
        [<AllowNullLiteral>]
        type ConfigurationChangedEvent =
            abstract hasChanged: id: EditorOption -> bool

        /// An event describing that the configuration of the editor has changed.
        [<AllowNullLiteral>]
        type ConfigurationChangedEventStatic =
            [<EmitConstructor>]
            abstract Create: unit -> ConfigurationChangedEvent

        /// All computed editor options.
        [<AllowNullLiteral>]
        type IComputedEditorOptions =
            abstract get: id: 'T -> FindComputedEditorOptionValueById<'T>

        [<AllowNullLiteral>]
        type IEditorOption<'K, 'V> =
            abstract id: 'K
            abstract name: string
            abstract defaultValue: 'V with get, set
            /// <summary>Might modify <c>value</c>.</summary>
            abstract applyUpdate: value: 'V option * update: 'V -> ApplyUpdateResult<'V>

        [<AllowNullLiteral>]
        type ApplyUpdateResult<'T> =
            abstract newValue: 'T
            abstract didChange: bool

        [<AllowNullLiteral>]
        type ApplyUpdateResultStatic =
            [<EmitConstructor>]
            abstract Create: newValue: 'T * didChange: bool -> ApplyUpdateResult<'T>

        /// Configuration options for editor comments
        [<AllowNullLiteral>]
        type IEditorCommentsOptions =
            /// Insert a space after the line comment token and inside the block comments tokens.
            /// Defaults to true.
            abstract insertSpace: bool option with get, set
            /// Ignore empty lines when inserting line comments.
            /// Defaults to true.
            abstract ignoreEmptyLines: bool option with get, set

        /// The kind of animation in which the editor's cursor should be rendered.
        [<RequireQualifiedAccess>]
        type TextEditorCursorBlinkingStyle =
            /// Hidden
            | Hidden = 0
            /// Blinking
            | Blink = 1
            /// Blinking with smooth fading
            | Smooth = 2
            /// Blinking with prolonged filled state and smooth fading
            | Phase = 3
            /// Expand collapse animation on the y axis
            | Expand = 4
            /// No-Blinking
            | Solid = 5

        /// The style in which the editor's cursor should be rendered.
        [<RequireQualifiedAccess>]
        type TextEditorCursorStyle =
            /// As a vertical line (sitting between two characters).
            | Line = 1
            /// As a block (sitting on top of a character).
            | Block = 2
            /// As a horizontal line (sitting under a character).
            | Underline = 3
            /// As a thin vertical line (sitting between two characters).
            | LineThin = 4
            /// As an outlined block (sitting on top of a character).
            | BlockOutline = 5
            /// As a thin horizontal line (sitting under a character).
            | UnderlineThin = 6

        /// Configuration options for editor find widget
        [<AllowNullLiteral>]
        type IEditorFindOptions =
            /// Controls whether the cursor should move to find matches while typing.
            abstract cursorMoveOnType: bool option with get, set
            /// Controls if we seed search string in the Find Widget with editor selection.
            abstract seedSearchStringFromSelection: IEditorFindOptionsSeedSearchStringFromSelection option with get, set
            /// Controls if Find in Selection flag is turned on in the editor.
            abstract autoFindInSelection: IEditorFindOptionsAutoFindInSelection option with get, set
            abstract addExtraSpaceOnTop: bool option with get, set
            /// Controls whether the search result and diff result automatically restarts from the beginning (or the end) when no further matches can be found
            abstract loop: bool option with get, set

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type GoToLocationValues =
            | Peek
            | GotoAndPeek
            | Goto

        /// Configuration options for go to location
        [<AllowNullLiteral>]
        type IGotoLocationOptions =
            abstract multiple: GoToLocationValues option with get, set
            abstract multipleDefinitions: GoToLocationValues option with get, set
            abstract multipleTypeDefinitions: GoToLocationValues option with get, set
            abstract multipleDeclarations: GoToLocationValues option with get, set
            abstract multipleImplementations: GoToLocationValues option with get, set
            abstract multipleReferences: GoToLocationValues option with get, set
            abstract alternativeDefinitionCommand: string option with get, set
            abstract alternativeTypeDefinitionCommand: string option with get, set
            abstract alternativeDeclarationCommand: string option with get, set
            abstract alternativeImplementationCommand: string option with get, set
            abstract alternativeReferenceCommand: string option with get, set

        /// Configuration options for editor hover
        [<AllowNullLiteral>]
        type IEditorHoverOptions =
            /// Enable the hover.
            /// Defaults to true.
            abstract enabled: bool option with get, set
            /// Delay for showing the hover.
            /// Defaults to 300.
            abstract delay: float option with get, set
            /// Is the hover sticky such that it can be clicked and its contents selected?
            /// Defaults to true.
            abstract sticky: bool option with get, set
            /// Controls how long the hover is visible after you hovered out of it.
            /// Require sticky setting to be true.
            abstract hidingDelay: float option with get, set
            /// Should the hover be shown above the line if possible?
            /// Defaults to false.
            abstract above: bool option with get, set

        /// A description for the overview ruler position.
        [<AllowNullLiteral>]
        type OverviewRulerPosition =
            /// Width of the overview ruler
            abstract width: float
            /// Height of the overview ruler
            abstract height: float
            /// Top position for the overview ruler
            abstract top: float
            /// Right position for the overview ruler
            abstract right: float

        [<RequireQualifiedAccess>]
        type RenderMinimap =
            | None = 0
            | Text = 1
            | Blocks = 2

        /// The internal layout details of the editor.
        [<AllowNullLiteral>]
        type EditorLayoutInfo =
            /// Full editor width.
            abstract width: float
            /// Full editor height.
            abstract height: float
            /// Left position for the glyph margin.
            abstract glyphMarginLeft: float
            /// The width of the glyph margin.
            abstract glyphMarginWidth: float
            /// The number of decoration lanes to render in the glyph margin.
            abstract glyphMarginDecorationLaneCount: float
            /// Left position for the line numbers.
            abstract lineNumbersLeft: float
            /// The width of the line numbers.
            abstract lineNumbersWidth: float
            /// Left position for the line decorations.
            abstract decorationsLeft: float
            /// The width of the line decorations.
            abstract decorationsWidth: float
            /// Left position for the content (actual text)
            abstract contentLeft: float
            /// The width of the content (actual text)
            abstract contentWidth: float
            /// Layout information for the minimap
            abstract minimap: EditorMinimapLayoutInfo
            /// The number of columns (of typical characters) fitting on a viewport line.
            abstract viewportColumn: float
            abstract isWordWrapMinified: bool
            abstract isViewportWrapping: bool
            abstract wrappingColumn: float
            /// The width of the vertical scrollbar.
            abstract verticalScrollbarWidth: float
            /// The height of the horizontal scrollbar.
            abstract horizontalScrollbarHeight: float
            /// The position of the overview ruler.
            abstract overviewRuler: OverviewRulerPosition

        /// The internal layout details of the editor.
        [<AllowNullLiteral>]
        type EditorMinimapLayoutInfo =
            abstract renderMinimap: RenderMinimap
            abstract minimapLeft: float
            abstract minimapWidth: float
            abstract minimapHeightIsEditorHeight: bool
            abstract minimapIsSampling: bool
            abstract minimapScale: float
            abstract minimapLineHeight: float
            abstract minimapCanvasInnerWidth: float
            abstract minimapCanvasInnerHeight: float
            abstract minimapCanvasOuterWidth: float
            abstract minimapCanvasOuterHeight: float

        /// Configuration options for editor lightbulb
        [<AllowNullLiteral>]
        type IEditorLightbulbOptions =
            /// Enable the lightbulb code action.
            /// Defaults to true.
            abstract enabled: bool option with get, set

        [<AllowNullLiteral>]
        type IEditorStickyScrollOptions =
            /// Enable the sticky scroll
            abstract enabled: bool option with get, set
            /// Maximum number of sticky lines to show
            abstract maxLineCount: float option with get, set
            /// Model to choose for sticky scroll by default
            abstract defaultModel: IEditorStickyScrollOptionsDefaultModel option with get, set
            /// Define whether to scroll sticky scroll with editor horizontal scrollbae
            abstract scrollWithEditor: bool option with get, set

        /// Configuration options for editor inlayHints
        [<AllowNullLiteral>]
        type IEditorInlayHintsOptions =
            /// Enable the inline hints.
            /// Defaults to true.
            abstract enabled: IEditorInlayHintsOptionsEnabled option with get, set
            /// Font size of inline hints.
            /// Default to 90% of the editor font size.
            abstract fontSize: float option with get, set
            /// Font family of inline hints.
            /// Defaults to editor font family.
            abstract fontFamily: string option with get, set
            /// Enables the padding around the inlay hint.
            /// Defaults to false.
            abstract padding: bool option with get, set

        /// Configuration options for editor minimap
        [<AllowNullLiteral>]
        type IEditorMinimapOptions =
            /// Enable the rendering of the minimap.
            /// Defaults to true.
            abstract enabled: bool option with get, set
            /// Control the rendering of minimap.
            abstract autohide: bool option with get, set
            /// Control the side of the minimap in editor.
            /// Defaults to 'right'.
            abstract side: IEditorMinimapOptionsSide option with get, set
            /// Control the minimap rendering mode.
            /// Defaults to 'actual'.
            abstract size: IEditorMinimapOptionsSize option with get, set
            /// Control the rendering of the minimap slider.
            /// Defaults to 'mouseover'.
            abstract showSlider: IEditorMinimapOptionsShowSlider option with get, set
            /// Render the actual text on a line (as opposed to color blocks).
            /// Defaults to true.
            abstract renderCharacters: bool option with get, set
            /// Limit the width of the minimap to render at most a certain number of columns.
            /// Defaults to 120.
            abstract maxColumn: float option with get, set
            /// Relative size of the font in the minimap. Defaults to 1.
            abstract scale: float option with get, set

        /// Configuration options for editor padding
        [<AllowNullLiteral>]
        type IEditorPaddingOptions =
            /// Spacing between top edge of editor and first line.
            abstract top: float option with get, set
            /// Spacing between bottom edge of editor and last line.
            abstract bottom: float option with get, set

        /// Configuration options for parameter hints
        [<AllowNullLiteral>]
        type IEditorParameterHintOptions =
            /// Enable parameter hints.
            /// Defaults to true.
            abstract enabled: bool option with get, set
            /// Enable cycling of parameter hints.
            /// Defaults to false.
            abstract cycle: bool option with get, set

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type QuickSuggestionsValue =
            | On
            | Inline
            | Off

        /// Configuration options for quick suggestions
        [<AllowNullLiteral>]
        type IQuickSuggestionsOptions =
            abstract other: U2<bool, QuickSuggestionsValue> option with get, set
            abstract comments: U2<bool, QuickSuggestionsValue> option with get, set
            abstract strings: U2<bool, QuickSuggestionsValue> option with get, set

        [<AllowNullLiteral>]
        type InternalQuickSuggestionsOptions =
            abstract other: QuickSuggestionsValue
            abstract comments: QuickSuggestionsValue
            abstract strings: QuickSuggestionsValue

        type LineNumbersType = U2<(float -> string), string>

        [<RequireQualifiedAccess>]
        type RenderLineNumbersType =
            | Off = 0
            | On = 1
            | Relative = 2
            | Interval = 3
            | Custom = 4

        [<AllowNullLiteral>]
        type InternalEditorRenderLineNumbersOptions =
            abstract renderType: RenderLineNumbersType
            abstract renderFn: (float -> string) option

        [<AllowNullLiteral>]
        type IRulerOption =
            abstract column: float
            abstract color: string option

        /// Configuration options for editor scrollbars
        [<AllowNullLiteral>]
        type IEditorScrollbarOptions =
            /// <summary>
            /// The size of arrows (if displayed).
            /// Defaults to 11.
            /// **NOTE**: This option cannot be updated using <c>updateOptions()</c>
            /// </summary>
            abstract arrowSize: float option with get, set
            /// Render vertical scrollbar.
            /// Defaults to 'auto'.
            abstract vertical: IEditorScrollbarOptionsVertical option with get, set
            /// Render horizontal scrollbar.
            /// Defaults to 'auto'.
            abstract horizontal: IEditorScrollbarOptionsVertical option with get, set
            /// <summary>
            /// Cast horizontal and vertical shadows when the content is scrolled.
            /// Defaults to true.
            /// **NOTE**: This option cannot be updated using <c>updateOptions()</c>
            /// </summary>
            abstract useShadows: bool option with get, set
            /// <summary>
            /// Render arrows at the top and bottom of the vertical scrollbar.
            /// Defaults to false.
            /// **NOTE**: This option cannot be updated using <c>updateOptions()</c>
            /// </summary>
            abstract verticalHasArrows: bool option with get, set
            /// <summary>
            /// Render arrows at the left and right of the horizontal scrollbar.
            /// Defaults to false.
            /// **NOTE**: This option cannot be updated using <c>updateOptions()</c>
            /// </summary>
            abstract horizontalHasArrows: bool option with get, set
            /// Listen to mouse wheel events and react to them by scrolling.
            /// Defaults to true.
            abstract handleMouseWheel: bool option with get, set
            /// <summary>
            /// Always consume mouse wheel events (always call preventDefault() and stopPropagation() on the browser events).
            /// Defaults to true.
            /// **NOTE**: This option cannot be updated using <c>updateOptions()</c>
            /// </summary>
            abstract alwaysConsumeMouseWheel: bool option with get, set
            /// Height in pixels for the horizontal scrollbar.
            /// Defaults to 10 (px).
            abstract horizontalScrollbarSize: float option with get, set
            /// Width in pixels for the vertical scrollbar.
            /// Defaults to 10 (px).
            abstract verticalScrollbarSize: float option with get, set
            /// <summary>
            /// Width in pixels for the vertical slider.
            /// Defaults to <c>verticalScrollbarSize</c>.
            /// **NOTE**: This option cannot be updated using <c>updateOptions()</c>
            /// </summary>
            abstract verticalSliderSize: float option with get, set
            /// <summary>
            /// Height in pixels for the horizontal slider.
            /// Defaults to <c>horizontalScrollbarSize</c>.
            /// **NOTE**: This option cannot be updated using <c>updateOptions()</c>
            /// </summary>
            abstract horizontalSliderSize: float option with get, set
            /// Scroll gutter clicks move by page vs jump to position.
            /// Defaults to false.
            abstract scrollByPage: bool option with get, set

        [<AllowNullLiteral>]
        type InternalEditorScrollbarOptions =
            abstract arrowSize: float
            abstract vertical: ScrollbarVisibility
            abstract horizontal: ScrollbarVisibility
            abstract useShadows: bool
            abstract verticalHasArrows: bool
            abstract horizontalHasArrows: bool
            abstract handleMouseWheel: bool
            abstract alwaysConsumeMouseWheel: bool
            abstract horizontalScrollbarSize: float
            abstract horizontalSliderSize: float
            abstract verticalScrollbarSize: float
            abstract verticalSliderSize: float
            abstract scrollByPage: bool

        type InUntrustedWorkspace = string

        /// Configuration options for unicode highlighting.
        [<AllowNullLiteral>]
        type IUnicodeHighlightOptions =
            /// Controls whether all non-basic ASCII characters are highlighted. Only characters between U+0020 and U+007E, tab, line-feed and carriage-return are considered basic ASCII.
            abstract nonBasicASCII: U2<bool, InUntrustedWorkspace> option with get, set
            /// Controls whether characters that just reserve space or have no width at all are highlighted.
            abstract invisibleCharacters: bool option with get, set
            /// Controls whether characters are highlighted that can be confused with basic ASCII characters, except those that are common in the current user locale.
            abstract ambiguousCharacters: bool option with get, set
            /// Controls whether characters in comments should also be subject to unicode highlighting.
            abstract includeComments: U2<bool, InUntrustedWorkspace> option with get, set
            /// Controls whether characters in strings should also be subject to unicode highlighting.
            abstract includeStrings: U2<bool, InUntrustedWorkspace> option with get, set
            /// Defines allowed characters that are not being highlighted.
            abstract allowedCharacters: Record<string, bool> option with get, set
            /// Unicode characters that are common in allowed locales are not being highlighted.
            abstract allowedLocales: Record<U2<string, string>, bool> option with get, set

        [<AllowNullLiteral>]
        type IInlineSuggestOptions =
            /// Enable or disable the rendering of automatic inline completions.
            abstract enabled: bool option with get, set
            /// <summary>
            /// Configures the mode.
            /// Use <c>prefix</c> to only show ghost text if the text to replace is a prefix of the suggestion text.
            /// Use <c>subword</c> to only show ghost text if the replace text is a subword of the suggestion text.
            /// Use <c>subwordSmart</c> to only show ghost text if the replace text is a subword of the suggestion text, but the subword must start after the cursor position.
            /// Defaults to <c>prefix</c>.
            /// </summary>
            abstract mode: IInlineSuggestOptionsMode option with get, set
            abstract showToolbar: IInlineSuggestOptionsShowToolbar option with get, set
            abstract suppressSuggestions: bool option with get, set
            /// Does not clear active inline suggestions when the editor loses focus.
            abstract keepOnBlur: bool option with get, set

        [<AllowNullLiteral>]
        type IBracketPairColorizationOptions =
            /// Enable or disable bracket pair colorization.
            abstract enabled: bool option with get, set
            /// Use independent color pool per bracket type.
            abstract independentColorPoolPerBracketType: bool option with get, set

        [<AllowNullLiteral>]
        type IGuidesOptions =
            /// Enable rendering of bracket pair guides.
            /// Defaults to false.
            abstract bracketPairs: U2<bool, string> option with get, set
            /// Enable rendering of vertical bracket pair guides.
            /// Defaults to 'active'.
            abstract bracketPairsHorizontal: U2<bool, string> option with get, set
            /// Enable highlighting of the active bracket pair.
            /// Defaults to true.
            abstract highlightActiveBracketPair: bool option with get, set
            /// Enable rendering of indent guides.
            /// Defaults to true.
            abstract indentation: bool option with get, set
            /// Enable highlighting of the active indent guide.
            /// Defaults to true.
            abstract highlightActiveIndentation: U2<bool, string> option with get, set

        /// Configuration options for editor suggest widget
        [<AllowNullLiteral>]
        type ISuggestOptions =
            /// Overwrite word ends on accept. Default to false.
            abstract insertMode: ISuggestOptionsInsertMode option with get, set
            /// Enable graceful matching. Defaults to true.
            abstract filterGraceful: bool option with get, set
            /// Prevent quick suggestions when a snippet is active. Defaults to true.
            abstract snippetsPreventQuickSuggestions: bool option with get, set
            /// Favors words that appear close to the cursor.
            abstract localityBonus: bool option with get, set
            /// Enable using global storage for remembering suggestions.
            abstract shareSuggestSelections: bool option with get, set
            /// Select suggestions when triggered via quick suggest or trigger characters
            abstract selectionMode: ISuggestOptionsSelectionMode option with get, set
            /// Enable or disable icons in suggestions. Defaults to true.
            abstract showIcons: bool option with get, set
            /// Enable or disable the suggest status bar.
            abstract showStatusBar: bool option with get, set
            /// Enable or disable the rendering of the suggestion preview.
            abstract preview: bool option with get, set
            /// Configures the mode of the preview.
            abstract previewMode: IInlineSuggestOptionsMode option with get, set
            /// Show details inline with the label. Defaults to true.
            abstract showInlineDetails: bool option with get, set
            /// Show method-suggestions.
            abstract showMethods: bool option with get, set
            /// Show function-suggestions.
            abstract showFunctions: bool option with get, set
            /// Show constructor-suggestions.
            abstract showConstructors: bool option with get, set
            /// Show deprecated-suggestions.
            abstract showDeprecated: bool option with get, set
            /// Controls whether suggestions allow matches in the middle of the word instead of only at the beginning
            abstract matchOnWordStartOnly: bool option with get, set
            /// Show field-suggestions.
            abstract showFields: bool option with get, set
            /// Show variable-suggestions.
            abstract showVariables: bool option with get, set
            /// Show class-suggestions.
            abstract showClasses: bool option with get, set
            /// Show struct-suggestions.
            abstract showStructs: bool option with get, set
            /// Show interface-suggestions.
            abstract showInterfaces: bool option with get, set
            /// Show module-suggestions.
            abstract showModules: bool option with get, set
            /// Show property-suggestions.
            abstract showProperties: bool option with get, set
            /// Show event-suggestions.
            abstract showEvents: bool option with get, set
            /// Show operator-suggestions.
            abstract showOperators: bool option with get, set
            /// Show unit-suggestions.
            abstract showUnits: bool option with get, set
            /// Show value-suggestions.
            abstract showValues: bool option with get, set
            /// Show constant-suggestions.
            abstract showConstants: bool option with get, set
            /// Show enum-suggestions.
            abstract showEnums: bool option with get, set
            /// Show enumMember-suggestions.
            abstract showEnumMembers: bool option with get, set
            /// Show keyword-suggestions.
            abstract showKeywords: bool option with get, set
            /// Show text-suggestions.
            abstract showWords: bool option with get, set
            /// Show color-suggestions.
            abstract showColors: bool option with get, set
            /// Show file-suggestions.
            abstract showFiles: bool option with get, set
            /// Show reference-suggestions.
            abstract showReferences: bool option with get, set
            /// Show folder-suggestions.
            abstract showFolders: bool option with get, set
            /// Show typeParameter-suggestions.
            abstract showTypeParameters: bool option with get, set
            /// Show issue-suggestions.
            abstract showIssues: bool option with get, set
            /// Show user-suggestions.
            abstract showUsers: bool option with get, set
            /// Show snippet-suggestions.
            abstract showSnippets: bool option with get, set

        [<AllowNullLiteral>]
        type ISmartSelectOptions =
            abstract selectLeadingAndTrailingWhitespace: bool option with get, set
            abstract selectSubwords: bool option with get, set

        /// Describes how to indent wrapped lines.
        [<RequireQualifiedAccess>]
        type WrappingIndent =
            /// No indentation => wrapped lines begin at column 1.
            | None = 0
            /// Same => wrapped lines get the same indentation as the parent.
            | Same = 1
            /// Indent => wrapped lines get +1 indentation toward the parent.
            | Indent = 2
            /// DeepIndent => wrapped lines get +2 indentation toward the parent.
            | DeepIndent = 3

        [<AllowNullLiteral>]
        type EditorWrappingInfo =
            abstract isDominatedByLongLines: bool
            abstract isWordWrapMinified: bool
            abstract isViewportWrapping: bool
            abstract wrappingColumn: float

        /// Configuration options for editor drop into behavior
        [<AllowNullLiteral>]
        type IDropIntoEditorOptions =
            /// Enable dropping into editor.
            /// Defaults to true.
            abstract enabled: bool option with get, set
            /// Controls if a widget is shown after a drop.
            /// Defaults to 'afterDrop'.
            abstract showDropSelector: IDropIntoEditorOptionsShowDropSelector option with get, set

        /// Configuration options for editor pasting as into behavior
        [<AllowNullLiteral>]
        type IPasteAsOptions =
            /// Enable paste as functionality in editors.
            /// Defaults to true.
            abstract enabled: bool option with get, set
            /// Controls if a widget is shown after a drop.
            /// Defaults to 'afterPaste'.
            abstract showPasteSelector: IPasteAsOptionsShowPasteSelector option with get, set

        [<RequireQualifiedAccess>]
        type EditorOption =
            | AcceptSuggestionOnCommitCharacter = 0
            | AcceptSuggestionOnEnter = 1
            | AccessibilitySupport = 2
            | AccessibilityPageSize = 3
            | AriaLabel = 4
            | AriaRequired = 5
            | AutoClosingBrackets = 6
            | AutoClosingComments = 7
            | ScreenReaderAnnounceInlineSuggestion = 8
            | AutoClosingDelete = 9
            | AutoClosingOvertype = 10
            | AutoClosingQuotes = 11
            | AutoIndent = 12
            | AutomaticLayout = 13
            | AutoSurround = 14
            | BracketPairColorization = 15
            | Guides = 16
            | CodeLens = 17
            | CodeLensFontFamily = 18
            | CodeLensFontSize = 19
            | ColorDecorators = 20
            | ColorDecoratorsLimit = 21
            | ColumnSelection = 22
            | Comments = 23
            | Contextmenu = 24
            | CopyWithSyntaxHighlighting = 25
            | CursorBlinking = 26
            | CursorSmoothCaretAnimation = 27
            | CursorStyle = 28
            | CursorSurroundingLines = 29
            | CursorSurroundingLinesStyle = 30
            | CursorWidth = 31
            | DisableLayerHinting = 32
            | DisableMonospaceOptimizations = 33
            | DomReadOnly = 34
            | DragAndDrop = 35
            | DropIntoEditor = 36
            | EmptySelectionClipboard = 37
            | ExperimentalWhitespaceRendering = 38
            | ExtraEditorClassName = 39
            | FastScrollSensitivity = 40
            | Find = 41
            | FixedOverflowWidgets = 42
            | Folding = 43
            | FoldingStrategy = 44
            | FoldingHighlight = 45
            | FoldingImportsByDefault = 46
            | FoldingMaximumRegions = 47
            | UnfoldOnClickAfterEndOfLine = 48
            | FontFamily = 49
            | FontInfo = 50
            | FontLigatures = 51
            | FontSize = 52
            | FontWeight = 53
            | FontVariations = 54
            | FormatOnPaste = 55
            | FormatOnType = 56
            | GlyphMargin = 57
            | GotoLocation = 58
            | HideCursorInOverviewRuler = 59
            | Hover = 60
            | InDiffEditor = 61
            | InlineSuggest = 62
            | LetterSpacing = 63
            | Lightbulb = 64
            | LineDecorationsWidth = 65
            | LineHeight = 66
            | LineNumbers = 67
            | LineNumbersMinChars = 68
            | LinkedEditing = 69
            | Links = 70
            | MatchBrackets = 71
            | Minimap = 72
            | MouseStyle = 73
            | MouseWheelScrollSensitivity = 74
            | MouseWheelZoom = 75
            | MultiCursorMergeOverlapping = 76
            | MultiCursorModifier = 77
            | MultiCursorPaste = 78
            | MultiCursorLimit = 79
            | OccurrencesHighlight = 80
            | OverviewRulerBorder = 81
            | OverviewRulerLanes = 82
            | Padding = 83
            | PasteAs = 84
            | ParameterHints = 85
            | PeekWidgetDefaultFocus = 86
            | DefinitionLinkOpensInPeek = 87
            | QuickSuggestions = 88
            | QuickSuggestionsDelay = 89
            | ReadOnly = 90
            | ReadOnlyMessage = 91
            | RenameOnType = 92
            | RenderControlCharacters = 93
            | RenderFinalNewline = 94
            | RenderLineHighlight = 95
            | RenderLineHighlightOnlyWhenFocus = 96
            | RenderValidationDecorations = 97
            | RenderWhitespace = 98
            | RevealHorizontalRightPadding = 99
            | RoundedSelection = 100
            | Rulers = 101
            | Scrollbar = 102
            | ScrollBeyondLastColumn = 103
            | ScrollBeyondLastLine = 104
            | ScrollPredominantAxis = 105
            | SelectionClipboard = 106
            | SelectionHighlight = 107
            | SelectOnLineNumbers = 108
            | ShowFoldingControls = 109
            | ShowUnused = 110
            | SnippetSuggestions = 111
            | SmartSelect = 112
            | SmoothScrolling = 113
            | StickyScroll = 114
            | StickyTabStops = 115
            | StopRenderingLineAfter = 116
            | Suggest = 117
            | SuggestFontSize = 118
            | SuggestLineHeight = 119
            | SuggestOnTriggerCharacters = 120
            | SuggestSelection = 121
            | TabCompletion = 122
            | TabIndex = 123
            | UnicodeHighlighting = 124
            | UnusualLineTerminators = 125
            | UseShadowDOM = 126
            | UseTabStops = 127
            | WordBreak = 128
            | WordSeparators = 129
            | WordWrap = 130
            | WordWrapBreakAfterCharacters = 131
            | WordWrapBreakBeforeCharacters = 132
            | WordWrapColumn = 133
            | WordWrapOverride1 = 134
            | WordWrapOverride2 = 135
            | WrappingIndent = 136
            | WrappingStrategy = 137
            | ShowDeprecated = 138
            | InlayHints = 139
            | EditorClassName = 140
            | PixelRatio = 141
            | TabFocusMode = 142
            | LayoutInfo = 143
            | WrappingInfo = 144
            | DefaultColorDecorators = 145
            | ColorDecoratorsActivatedOn = 146
            | InlineCompletionsAccessibilityVerbose = 147

        type EditorOptionsType = obj

        type FindEditorOptionsKeyById<'T> =
            interface
            end

        type ComputedEditorOptionValue<'T when 'T :> IEditorOption<obj option, obj option>> =
            interface
            end

        type FindComputedEditorOptionValueById<'T> =
            interface
            end

        [<AllowNullLiteral>]
        type IEditorConstructionOptions =
            inherit IEditorOptions
            /// The initial editor dimension (to avoid measuring the container).
            abstract dimension: IDimension option with get, set
            /// Place overflow widgets inside an external DOM node.
            /// Defaults to an internal DOM node.
            abstract overflowWidgetsDomNode: HTMLElement option with get, set

        /// A view zone is a full horizontal rectangle that 'pushes' text down.
        /// The editor reserves space for view zones when rendering.
        [<AllowNullLiteral>]
        type IViewZone =
            /// The line number after which this zone should appear.
            /// Use 0 to place a view zone before the first line number.
            abstract afterLineNumber: float with get, set
            /// <summary>
            /// The column after which this zone should appear.
            /// If not set, the maxLineColumn of <c>afterLineNumber</c> will be used.
            /// This is relevant for wrapped lines.
            /// </summary>
            abstract afterColumn: float option with get, set
            /// <summary>If the <c>afterColumn</c> has multiple view columns, the affinity specifies which one to use. Defaults to <c>none</c>.</summary>
            abstract afterColumnAffinity: PositionAffinity option with get, set
            /// Render the zone even when its line is hidden.
            abstract showInHiddenAreas: bool option with get, set
            /// <summary>
            /// Tiebreaker that is used when multiple view zones want to be after the same line.
            /// Defaults to <c>afterColumn</c> otherwise 10000;
            /// </summary>
            abstract ordinal: float option with get, set
            /// Suppress mouse down events.
            /// If set, the editor will attach a mouse down listener to the view zone and .preventDefault on it.
            /// Defaults to false
            abstract suppressMouseDown: bool option with get, set
            /// <summary>
            /// The height in lines of the view zone.
            /// If specified, <c>heightInPx</c> will be used instead of this.
            /// If neither <c>heightInPx</c> nor <c>heightInLines</c> is specified, a default of <c>heightInLines</c> = 1 will be chosen.
            /// </summary>
            abstract heightInLines: float option with get, set
            /// <summary>
            /// The height in px of the view zone.
            /// If this is set, the editor will give preference to it rather than <c>heightInLines</c> above.
            /// If neither <c>heightInPx</c> nor <c>heightInLines</c> is specified, a default of <c>heightInLines</c> = 1 will be chosen.
            /// </summary>
            abstract heightInPx: float option with get, set
            /// The minimum width in px of the view zone.
            /// If this is set, the editor will ensure that the scroll width is >= than this value.
            abstract minWidthInPx: float option with get, set
            /// The dom node of the view zone
            abstract domNode: HTMLElement with get, set
            /// An optional dom node for the view zone that will be placed in the margin area.
            abstract marginDomNode: HTMLElement option with get, set
            /// Callback which gives the relative top of the view zone as it appears (taking scrolling into account).
            abstract onDomNodeTop: (float -> unit) option with get, set
            /// Callback which gives the height in pixels of the view zone.
            abstract onComputedHeight: (float -> unit) option with get, set

        /// An accessor that allows for zones to be added or removed.
        [<AllowNullLiteral>]
        type IViewZoneChangeAccessor =
            /// <summary>Create a new view zone.</summary>
            /// <param name="zone">Zone to create</param>
            /// <returns>A unique identifier to the view zone.</returns>
            abstract addZone: zone: IViewZone -> string
            /// <summary>Remove a zone</summary>
            /// <param name="id">A unique identifier to the view zone, as returned by the <c>addZone</c> call.</param>
            abstract removeZone: id: string -> unit
            /// <summary>
            /// Change a zone's position.
            /// The editor will rescan the <c>afterLineNumber</c> and <c>afterColumn</c> properties of a view zone.
            /// </summary>
            abstract layoutZone: id: string -> unit

        /// A positioning preference for rendering content widgets.
        [<RequireQualifiedAccess>]
        type ContentWidgetPositionPreference =
            /// Place the content widget exactly at a position
            | EXACT = 0
            /// Place the content widget above a position
            | ABOVE = 1
            /// Place the content widget below a position
            | BELOW = 2

        /// A position for rendering content widgets.
        [<AllowNullLiteral>]
        type IContentWidgetPosition =
            /// <summary>
            /// Desired position which serves as an anchor for placing the content widget.
            /// The widget will be placed above, at, or below the specified position, based on the
            /// provided preference. The widget will always touch this position.
            ///
            /// Given sufficient horizontal space, the widget will be placed to the right of the
            /// passed in position. This can be tweaked by providing a <c>secondaryPosition</c>.
            /// </summary>
            /// <seealso cref="preference"></seealso>
            /// <seealso cref="secondaryPosition" />
            abstract position: IPosition option with get, set
            /// Optionally, a secondary position can be provided to further define the placing of
            /// the content widget. The secondary position must have the same line number as the
            /// primary position. If possible, the widget will be placed such that it also touches
            /// the secondary position.
            abstract secondaryPosition: IPosition option with get, set
            /// Placement preference for position, in order of preference.
            abstract preference: ResizeArray<ContentWidgetPositionPreference> with get, set
            /// Placement preference when multiple view positions refer to the same (model) position.
            /// This plays a role when injected text is involved.
            abstract positionAffinity: PositionAffinity option with get, set

        /// A content widget renders inline with the text and can be easily placed 'near' an editor position.
        [<AllowNullLiteral>]
        type IContentWidget =
            /// Render this content widget in a location where it could overflow the editor's view dom node.
            abstract allowEditorOverflow: bool option with get, set
            /// Call preventDefault() on mousedown events that target the content widget.
            abstract suppressMouseDown: bool option with get, set
            /// Get a unique identifier of the content widget.
            abstract getId: unit -> string
            /// Get the dom node of the content widget.
            abstract getDomNode: unit -> HTMLElement
            /// Get the placement of the content widget.
            /// If null is returned, the content widget will be placed off screen.
            abstract getPosition: unit -> IContentWidgetPosition option
            /// Optional function that is invoked before rendering
            /// the content widget. If a dimension is returned the editor will
            /// attempt to use it.
            abstract beforeRender: unit -> IDimension option
            /// <summary>
            /// Optional function that is invoked after rendering the content
            /// widget. Is being invoked with the selected position preference
            /// or <c>null</c> if not rendered.
            /// </summary>
            abstract afterRender: position: ContentWidgetPositionPreference option -> unit

        /// A positioning preference for rendering overlay widgets.
        [<RequireQualifiedAccess>]
        type OverlayWidgetPositionPreference =
            /// Position the overlay widget in the top right corner
            | TOP_RIGHT_CORNER = 0
            /// Position the overlay widget in the bottom right corner
            | BOTTOM_RIGHT_CORNER = 1
            /// Position the overlay widget in the top center
            | TOP_CENTER = 2

        /// A position for rendering overlay widgets.
        [<AllowNullLiteral>]
        type IOverlayWidgetPosition =
            /// The position preference for the overlay widget.
            abstract preference: OverlayWidgetPositionPreference option with get, set

        /// An overlay widgets renders on top of the text.
        [<AllowNullLiteral>]
        type IOverlayWidget =
            /// Get a unique identifier of the overlay widget.
            abstract getId: unit -> string
            /// Get the dom node of the overlay widget.
            abstract getDomNode: unit -> HTMLElement
            /// Get the placement of the overlay widget.
            /// If null is returned, the overlay widget is responsible to place itself.
            abstract getPosition: unit -> IOverlayWidgetPosition option
            /// The editor will ensure that the scroll width is >= than this value.
            abstract getMinContentWidthInPx: unit -> float

        /// A glyph margin widget renders in the editor glyph margin.
        [<AllowNullLiteral>]
        type IGlyphMarginWidget =
            /// Get a unique identifier of the glyph widget.
            abstract getId: unit -> string
            /// Get the dom node of the glyph widget.
            abstract getDomNode: unit -> HTMLElement
            /// Get the placement of the glyph widget.
            abstract getPosition: unit -> IGlyphMarginWidgetPosition

        /// A position for rendering glyph margin widgets.
        [<AllowNullLiteral>]
        type IGlyphMarginWidgetPosition =
            /// The glyph margin lane where the widget should be shown.
            abstract lane: GlyphMarginLane with get, set
            /// The priority order of the widget, used for determining which widget
            /// to render when there are multiple.
            abstract zIndex: float with get, set
            /// The editor range that this widget applies to.
            abstract range: IRange with get, set

        /// Type of hit element with the mouse in the editor.
        [<RequireQualifiedAccess>]
        type MouseTargetType =
            /// Mouse is on top of an unknown element.
            | UNKNOWN = 0
            /// Mouse is on top of the textarea used for input.
            | TEXTAREA = 1
            /// Mouse is on top of the glyph margin
            | GUTTER_GLYPH_MARGIN = 2
            /// Mouse is on top of the line numbers
            | GUTTER_LINE_NUMBERS = 3
            /// Mouse is on top of the line decorations
            | GUTTER_LINE_DECORATIONS = 4
            /// Mouse is on top of the whitespace left in the gutter by a view zone.
            | GUTTER_VIEW_ZONE = 5
            /// Mouse is on top of text in the content.
            | CONTENT_TEXT = 6
            /// Mouse is on top of empty space in the content (e.g. after line text or below last line)
            | CONTENT_EMPTY = 7
            /// Mouse is on top of a view zone in the content.
            | CONTENT_VIEW_ZONE = 8
            /// Mouse is on top of a content widget.
            | CONTENT_WIDGET = 9
            /// Mouse is on top of the decorations overview ruler.
            | OVERVIEW_RULER = 10
            /// Mouse is on top of a scrollbar.
            | SCROLLBAR = 11
            /// Mouse is on top of an overlay widget.
            | OVERLAY_WIDGET = 12
            /// Mouse is outside of the editor.
            | OUTSIDE_EDITOR = 13

        [<AllowNullLiteral>]
        type IBaseMouseTarget =
            /// The target element
            abstract element: HTMLElement option
            /// The 'approximate' editor position
            abstract position: Position option
            /// Desired mouse column (e.g. when position.column gets clamped to text length -- clicking after text on a line).
            abstract mouseColumn: float
            /// The 'approximate' editor range
            abstract range: Range option

        [<AllowNullLiteral>]
        type IMouseTargetUnknown =
            inherit IBaseMouseTarget
            abstract ``type``: MouseTargetType

        [<AllowNullLiteral>]
        type IMouseTargetTextarea =
            inherit IBaseMouseTarget
            abstract ``type``: MouseTargetType
            /// The 'approximate' editor position
            abstract position: obj
            /// The 'approximate' editor range
            abstract range: obj

        [<AllowNullLiteral>]
        type IMouseTargetMarginData =
            abstract isAfterLines: bool
            abstract glyphMarginLeft: float
            abstract glyphMarginWidth: float
            abstract lineNumbersWidth: float
            abstract offsetX: float

        [<AllowNullLiteral>]
        type IMouseTargetMargin =
            inherit IBaseMouseTarget
            abstract ``type``: MouseTargetType
            /// The 'approximate' editor position
            abstract position: Position
            /// The 'approximate' editor range
            abstract range: Range
            abstract detail: IMouseTargetMarginData

        [<AllowNullLiteral>]
        type IMouseTargetViewZoneData =
            abstract viewZoneId: string
            abstract positionBefore: Position option
            abstract positionAfter: Position option
            abstract position: Position
            abstract afterLineNumber: float

        [<AllowNullLiteral>]
        type IMouseTargetViewZone =
            inherit IBaseMouseTarget
            abstract ``type``: MouseTargetType
            /// The 'approximate' editor position
            abstract position: Position
            /// The 'approximate' editor range
            abstract range: Range
            abstract detail: IMouseTargetViewZoneData

        [<AllowNullLiteral>]
        type IMouseTargetContentTextData =
            abstract mightBeForeignElement: bool

        [<AllowNullLiteral>]
        type IMouseTargetContentText =
            inherit IBaseMouseTarget
            abstract ``type``: MouseTargetType
            /// The 'approximate' editor position
            abstract position: Position
            /// The 'approximate' editor range
            abstract range: Range
            abstract detail: IMouseTargetContentTextData

        [<AllowNullLiteral>]
        type IMouseTargetContentEmptyData =
            abstract isAfterLines: bool
            abstract horizontalDistanceToText: float option

        [<AllowNullLiteral>]
        type IMouseTargetContentEmpty =
            inherit IBaseMouseTarget
            abstract ``type``: MouseTargetType
            /// The 'approximate' editor position
            abstract position: Position
            /// The 'approximate' editor range
            abstract range: Range
            abstract detail: IMouseTargetContentEmptyData

        [<AllowNullLiteral>]
        type IMouseTargetContentWidget =
            inherit IBaseMouseTarget
            abstract ``type``: MouseTargetType
            /// The 'approximate' editor position
            abstract position: obj
            /// The 'approximate' editor range
            abstract range: obj
            abstract detail: string

        [<AllowNullLiteral>]
        type IMouseTargetOverlayWidget =
            inherit IBaseMouseTarget
            abstract ``type``: MouseTargetType
            /// The 'approximate' editor position
            abstract position: obj
            /// The 'approximate' editor range
            abstract range: obj
            abstract detail: string

        [<AllowNullLiteral>]
        type IMouseTargetScrollbar =
            inherit IBaseMouseTarget
            abstract ``type``: MouseTargetType
            /// The 'approximate' editor position
            abstract position: Position
            /// The 'approximate' editor range
            abstract range: Range

        [<AllowNullLiteral>]
        type IMouseTargetOverviewRuler =
            inherit IBaseMouseTarget
            abstract ``type``: MouseTargetType

        [<AllowNullLiteral>]
        type IMouseTargetOutsideEditor =
            inherit IBaseMouseTarget
            abstract ``type``: MouseTargetType
            abstract outsidePosition: IMouseTargetOutsideEditorOutsidePosition
            abstract outsideDistance: float

        /// Target hit with the mouse in the editor.
        type IMouseTarget = obj

        /// A mouse event originating from the editor.
        [<AllowNullLiteral>]
        type IEditorMouseEvent =
            abstract ``event``: IMouseEvent
            abstract target: IMouseTarget

        [<AllowNullLiteral>]
        type IPartialEditorMouseEvent =
            abstract ``event``: IMouseEvent
            abstract target: IMouseTarget option

        /// A paste event originating from the editor.
        [<AllowNullLiteral>]
        type IPasteEvent =
            abstract range: Range
            abstract languageId: string option

        [<AllowNullLiteral>]
        type IDiffEditorConstructionOptions =
            inherit IDiffEditorOptions
            inherit IEditorConstructionOptions
            /// Place overflow widgets inside an external DOM node.
            /// Defaults to an internal DOM node.
            abstract overflowWidgetsDomNode: HTMLElement option with get, set
            /// Aria label for original editor.
            abstract originalAriaLabel: string option with get, set
            /// Aria label for modified editor.
            abstract modifiedAriaLabel: string option with get, set

        /// A rich code editor.
        [<AllowNullLiteral>]
        type ICodeEditor =
            inherit IEditor
            /// <summary>An event emitted when the content of the current model has changed.</summary>
            abstract onDidChangeModelContent: IEvent<IModelContentChangedEvent>
            /// <summary>An event emitted when the language of the current model has changed.</summary>
            abstract onDidChangeModelLanguage: IEvent<IModelLanguageChangedEvent>
            /// <summary>An event emitted when the language configuration of the current model has changed.</summary>
            abstract onDidChangeModelLanguageConfiguration: IEvent<IModelLanguageConfigurationChangedEvent>
            /// <summary>An event emitted when the options of the current model has changed.</summary>
            abstract onDidChangeModelOptions: IEvent<IModelOptionsChangedEvent>
            /// <summary>An event emitted when the configuration of the editor has changed. (e.g. <c>editor.updateOptions()</c>)</summary>
            abstract onDidChangeConfiguration: IEvent<ConfigurationChangedEvent>
            /// <summary>An event emitted when the cursor position has changed.</summary>
            abstract onDidChangeCursorPosition: IEvent<ICursorPositionChangedEvent>
            /// <summary>An event emitted when the cursor selection has changed.</summary>
            abstract onDidChangeCursorSelection: IEvent<ICursorSelectionChangedEvent>
            /// <summary>An event emitted when the model of this editor has changed (e.g. <c>editor.setModel()</c>).</summary>
            abstract onDidChangeModel: IEvent<IModelChangedEvent>
            /// <summary>An event emitted when the decorations of the current model have changed.</summary>
            abstract onDidChangeModelDecorations: IEvent<IModelDecorationsChangedEvent>
            /// <summary>An event emitted when the text inside this editor gained focus (i.e. cursor starts blinking).</summary>
            abstract onDidFocusEditorText: IEvent<unit>
            /// <summary>An event emitted when the text inside this editor lost focus (i.e. cursor stops blinking).</summary>
            abstract onDidBlurEditorText: IEvent<unit>
            /// <summary>An event emitted when the text inside this editor or an editor widget gained focus.</summary>
            abstract onDidFocusEditorWidget: IEvent<unit>
            /// <summary>An event emitted when the text inside this editor or an editor widget lost focus.</summary>
            abstract onDidBlurEditorWidget: IEvent<unit>
            /// An event emitted after composition has started.
            abstract onDidCompositionStart: IEvent<unit>
            /// An event emitted after composition has ended.
            abstract onDidCompositionEnd: IEvent<unit>
            /// <summary>An event emitted when editing failed because the editor is read-only.</summary>
            abstract onDidAttemptReadOnlyEdit: IEvent<unit>
            /// <summary>An event emitted when users paste text in the editor.</summary>
            abstract onDidPaste: IEvent<IPasteEvent>
            /// <summary>An event emitted on a "mouseup".</summary>
            abstract onMouseUp: IEvent<IEditorMouseEvent>
            /// <summary>An event emitted on a "mousedown".</summary>
            abstract onMouseDown: IEvent<IEditorMouseEvent>
            /// <summary>An event emitted on a "contextmenu".</summary>
            abstract onContextMenu: IEvent<IEditorMouseEvent>
            /// <summary>An event emitted on a "mousemove".</summary>
            abstract onMouseMove: IEvent<IEditorMouseEvent>
            /// <summary>An event emitted on a "mouseleave".</summary>
            abstract onMouseLeave: IEvent<IPartialEditorMouseEvent>
            /// <summary>An event emitted on a "keyup".</summary>
            abstract onKeyUp: IEvent<IKeyboardEvent>
            /// <summary>An event emitted on a "keydown".</summary>
            abstract onKeyDown: IEvent<IKeyboardEvent>
            /// <summary>An event emitted when the layout of the editor has changed.</summary>
            abstract onDidLayoutChange: IEvent<EditorLayoutInfo>
            /// <summary>An event emitted when the content width or content height in the editor has changed.</summary>
            abstract onDidContentSizeChange: IEvent<IContentSizeChangedEvent>
            /// <summary>An event emitted when the scroll in the editor has changed.</summary>
            abstract onDidScrollChange: IEvent<IScrollEvent>
            /// <summary>An event emitted when hidden areas change in the editor (e.g. due to folding).</summary>
            abstract onDidChangeHiddenAreas: IEvent<unit>
            /// Saves current view state of the editor in a serializable object.
            abstract saveViewState: unit -> ICodeEditorViewState option
            /// <summary>Restores the view state of the editor from a serializable object generated by <c>saveViewState</c>.</summary>
            abstract restoreViewState: state: ICodeEditorViewState option -> unit
            /// Returns true if the text inside this editor or an editor widget has focus.
            abstract hasWidgetFocus: unit -> bool
            /// <summary>Get a contribution of this editor.</summary>
            /// <returns>The contribution or null if contribution not found.</returns>
            abstract getContribution: id: string -> 'T option when 'T :> IEditorContribution
            /// Type the getModel() of IEditor.
            abstract getModel: unit -> ITextModel option
            /// Sets the current model attached to this editor.
            /// If the previous model was created by the editor via the value key in the options
            /// literal object, it will be destroyed. Otherwise, if the previous model was set
            /// via setModel, or the model key in the options literal object, the previous model
            /// will not be destroyed.
            /// It is safe to call setModel(null) to simply detach the current model from the editor.
            abstract setModel: model: ITextModel option -> unit
            /// Gets all the editor computed options.
            abstract getOptions: unit -> IComputedEditorOptions
            /// Gets a specific editor option.
            abstract getOption: id: 'T -> FindComputedEditorOptionValueById<'T>
            /// Returns the editor's configuration (without any validation or defaults).
            abstract getRawOptions: unit -> IEditorOptions

            /// <summary>Get value of the current model attached to this editor.</summary>
            /// <seealso cref="ITextModel.getValue" />
            abstract getValue:
                ?options:
                    {|
                        preserveBOM: bool
                        lineEnding: string
                    |} ->
                    string

            /// <summary>Set the value of the current model attached to this editor.</summary>
            /// <seealso cref="ITextModel.setValue" />
            abstract setValue: newValue: string -> unit
            /// <summary>
            /// Get the width of the editor's content.
            /// This is information that is "erased" when computing <c>scrollWidth = Math.max(contentWidth, width)</c>
            /// </summary>
            abstract getContentWidth: unit -> float
            /// Get the scrollWidth of the editor's viewport.
            abstract getScrollWidth: unit -> float
            /// Get the scrollLeft of the editor's viewport.
            abstract getScrollLeft: unit -> float
            /// <summary>
            /// Get the height of the editor's content.
            /// This is information that is "erased" when computing <c>scrollHeight = Math.max(contentHeight, height)</c>
            /// </summary>
            abstract getContentHeight: unit -> float
            /// Get the scrollHeight of the editor's viewport.
            abstract getScrollHeight: unit -> float
            /// Get the scrollTop of the editor's viewport.
            abstract getScrollTop: unit -> float
            /// Change the scrollLeft of the editor's viewport.
            abstract setScrollLeft: newScrollLeft: float * ?scrollType: ScrollType -> unit
            /// Change the scrollTop of the editor's viewport.
            abstract setScrollTop: newScrollTop: float * ?scrollType: ScrollType -> unit
            /// Change the scroll position of the editor's viewport.
            abstract setScrollPosition: position: INewScrollPosition * ?scrollType: ScrollType -> unit
            /// Check if the editor is currently scrolling towards a different scroll position.
            abstract hasPendingScrollAnimation: unit -> bool
            /// <summary>Get an action that is a contribution to this editor.</summary>
            /// <returns>The action or null if action not found.</returns>
            abstract getAction: id: string -> IEditorAction option
            /// <summary>
            /// Execute a command on the editor.
            /// The edits will land on the undo-redo stack, but no "undo stop" will be pushed.
            /// </summary>
            /// <param name="source">The source of the call.</param>
            /// <param name="command">The command to execute</param>
            abstract executeCommand: source: string option * command: ICommand -> unit
            /// Create an "undo stop" in the undo-redo stack.
            abstract pushUndoStop: unit -> bool
            /// Remove the "undo stop" in the undo-redo stack.
            abstract popUndoStop: unit -> bool

            /// <summary>
            /// Execute edits on the editor.
            /// The edits will land on the undo-redo stack, but no "undo stop" will be pushed.
            /// </summary>
            /// <param name="source">The source of the call.</param>
            /// <param name="edits">The edits to execute.</param>
            /// <param name="endCursorState">Cursor state after the edits were applied.</param>
            abstract executeEdits:
                source: string option *
                edits: ResizeArray<IIdentifiedSingleEditOperation> *
                ?endCursorState: U2<ICursorStateComputer, ResizeArray<Selection>> ->
                    bool

            /// <summary>Execute multiple (concomitant) commands on the editor.</summary>
            /// <param name="source">The source of the call.</param>
            /// <param name="command">The commands to execute</param>
            abstract executeCommands: source: string option * commands: ResizeArray<ICommand option> -> unit
            /// Get all the decorations on a line (filtering out decorations from other editors).
            abstract getLineDecorations: lineNumber: float -> ResizeArray<IModelDecoration> option
            /// Get all the decorations for a range (filtering out decorations from other editors).
            abstract getDecorationsInRange: range: Range -> ResizeArray<IModelDecoration> option

            /// <summary>All decorations added through this call will get the ownerId of this editor.</summary>
            /// <seealso cref="createDecorationsCollection" />
            [<Obsolete("Use `createDecorationsCollection`")>]
            abstract deltaDecorations:
                oldDecorations: ResizeArray<string> * newDecorations: ResizeArray<IModelDeltaDecoration> ->
                    ResizeArray<string>

            /// Remove previously added decorations.
            abstract removeDecorations: decorationIds: ResizeArray<string> -> unit
            /// Get the layout info for the editor.
            abstract getLayoutInfo: unit -> EditorLayoutInfo
            /// Returns the ranges that are currently visible.
            /// Does not account for horizontal scrolling.
            abstract getVisibleRanges: unit -> ResizeArray<Range>
            /// Get the vertical position (top offset) for the line's top w.r.t. to the first line.
            abstract getTopForLineNumber: lineNumber: float * ?includeViewZones: bool -> float
            /// Get the vertical position (top offset) for the line's bottom w.r.t. to the first line.
            abstract getBottomForLineNumber: lineNumber: float -> float
            /// Get the vertical position (top offset) for the position w.r.t. to the first line.
            abstract getTopForPosition: lineNumber: float * column: float -> float
            /// Write the screen reader content to be the current selection
            abstract writeScreenReaderContent: reason: string -> unit
            /// Returns the editor's container dom node
            abstract getContainerDomNode: unit -> HTMLElement
            /// Returns the editor's dom node
            abstract getDomNode: unit -> HTMLElement option
            /// Add a content widget. Widgets must have unique ids, otherwise they will be overwritten.
            abstract addContentWidget: widget: IContentWidget -> unit
            /// Layout/Reposition a content widget. This is a ping to the editor to call widget.getPosition()
            /// and update appropriately.
            abstract layoutContentWidget: widget: IContentWidget -> unit
            /// Remove a content widget.
            abstract removeContentWidget: widget: IContentWidget -> unit
            /// Add an overlay widget. Widgets must have unique ids, otherwise they will be overwritten.
            abstract addOverlayWidget: widget: IOverlayWidget -> unit
            /// Layout/Reposition an overlay widget. This is a ping to the editor to call widget.getPosition()
            /// and update appropriately.
            abstract layoutOverlayWidget: widget: IOverlayWidget -> unit
            /// Remove an overlay widget.
            abstract removeOverlayWidget: widget: IOverlayWidget -> unit
            /// Add a glyph margin widget. Widgets must have unique ids, otherwise they will be overwritten.
            abstract addGlyphMarginWidget: widget: IGlyphMarginWidget -> unit
            /// Layout/Reposition a glyph margin widget. This is a ping to the editor to call widget.getPosition()
            /// and update appropriately.
            abstract layoutGlyphMarginWidget: widget: IGlyphMarginWidget -> unit
            /// Remove a glyph margin widget.
            abstract removeGlyphMarginWidget: widget: IGlyphMarginWidget -> unit
            /// Change the view zones. View zones are lost when a new model is attached to the editor.
            abstract changeViewZones: callback: (IViewZoneChangeAccessor -> unit) -> unit
            /// <summary>
            /// Get the horizontal position (left offset) for the column w.r.t to the beginning of the line.
            /// This method works only if the line <c>lineNumber</c> is currently rendered (in the editor's viewport).
            /// Use this method with caution.
            /// </summary>
            abstract getOffsetForColumn: lineNumber: float * column: float -> float
            /// Force an editor render now.
            abstract render: ?forceRedraw: bool -> unit
            /// <summary>
            /// Get the hit test target at coordinates <c>clientX</c> and <c>clientY</c>.
            /// The coordinates are relative to the top-left of the viewport.
            /// </summary>
            /// <returns>Hit test target or null if the coordinates fall outside the editor or the editor has no model.</returns>
            abstract getTargetAtClientPoint: clientX: float * clientY: float -> IMouseTarget option

            /// <summary>
            /// Get the visible position for <c>position</c>.
            /// The result position takes scrolling into account and is relative to the top left corner of the editor.
            /// Explanation 1: the results of this method will change for the same <c>position</c> if the user scrolls the editor.
            /// Explanation 2: the results of this method will not change if the container of the editor gets repositioned.
            /// Warning: the results of this method are inaccurate for positions that are outside the current editor viewport.
            /// </summary>
            abstract getScrolledVisiblePosition:
                position: IPosition ->
                    {|
                        top: float
                        left: float
                        height: float
                    |} option

            /// <summary>Apply the same font settings as the editor to <c>target</c>.</summary>
            abstract applyFontInfo: target: HTMLElement -> unit
            abstract setBanner: bannerDomNode: HTMLElement option * height: float -> unit
            /// Is called when the model has been set, view state was restored and options are updated.
            /// This is the best place to compute data for the viewport (such as tokens).
            abstract handleInitialized: unit -> unit

        /// A rich diff editor.
        [<AllowNullLiteral>]
        type IDiffEditor =
            inherit IEditor
            /// <seealso cref="ICodeEditor.getContainerDomNode" />
            abstract getContainerDomNode: unit -> HTMLElement
            /// <summary>An event emitted when the diff information computed by this diff editor has been updated.</summary>
            abstract onDidUpdateDiff: IEvent<unit>
            /// <summary>An event emitted when the diff model is changed (i.e. the diff editor shows new content).</summary>
            abstract onDidChangeModel: IEvent<unit>
            /// Saves current view state of the editor in a serializable object.
            abstract saveViewState: unit -> IDiffEditorViewState option
            /// <summary>Restores the view state of the editor from a serializable object generated by <c>saveViewState</c>.</summary>
            abstract restoreViewState: state: IDiffEditorViewState option -> unit
            /// Type the getModel() of IEditor.
            abstract getModel: unit -> IDiffEditorModel option
            abstract createViewModel: model: IDiffEditorModel -> IDiffEditorViewModel
            /// Sets the current model attached to this editor.
            /// If the previous model was created by the editor via the value key in the options
            /// literal object, it will be destroyed. Otherwise, if the previous model was set
            /// via setModel, or the model key in the options literal object, the previous model
            /// will not be destroyed.
            /// It is safe to call setModel(null) to simply detach the current model from the editor.
            abstract setModel: model: U2<IDiffEditorModel, IDiffEditorViewModel> option -> unit
            /// <summary>Get the <c>original</c> editor.</summary>
            abstract getOriginalEditor: unit -> ICodeEditor
            /// <summary>Get the <c>modified</c> editor.</summary>
            abstract getModifiedEditor: unit -> ICodeEditor
            /// Get the computed diff information.
            abstract getLineChanges: unit -> ResizeArray<ILineChange> option
            /// Update the editor's options after the editor has been created.
            abstract updateOptions: newOptions: IDiffEditorOptions -> unit
            abstract accessibleDiffViewerNext: unit -> unit
            abstract accessibleDiffViewerPrev: unit -> unit

        [<AllowNullLiteral>]
        type FontInfo =
            inherit BareFontInfo
            abstract _editorStylingBrand: unit
            abstract version: float
            abstract isTrusted: bool
            abstract isMonospace: bool
            abstract typicalHalfwidthCharacterWidth: float
            abstract typicalFullwidthCharacterWidth: float
            abstract canUseHalfwidthRightwardsArrow: bool
            abstract spaceWidth: float
            abstract middotWidth: float
            abstract wsmiddotWidth: float
            abstract maxDigitWidth: float

        [<AllowNullLiteral>]
        type FontInfoStatic =
            [<EmitConstructor>]
            abstract Create: unit -> FontInfo

        [<AllowNullLiteral>]
        type BareFontInfo =
            abstract _bareFontInfoBrand: unit
            abstract pixelRatio: float
            abstract fontFamily: string
            abstract fontWeight: string
            abstract fontSize: float
            abstract fontFeatureSettings: string
            abstract fontVariationSettings: string
            abstract lineHeight: float
            abstract letterSpacing: float

        [<AllowNullLiteral>]
        type BareFontInfoStatic =
            [<EmitConstructor>]
            abstract Create: unit -> BareFontInfo

        [<AllowNullLiteral>]
        type IEditorZoom =
            abstract onDidChangeZoomLevel: IEvent<float> with get, set
            abstract getZoomLevel: unit -> float
            abstract setZoomLevel: zoomLevel: float -> unit

        type IReadOnlyModel = ITextModel

        type IModel = ITextModel

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsAcceptSuggestionOnEnterIEditorOption =
            | On
            | Off
            | Smart

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsAutoClosingBracketsIEditorOption =
            | Always
            | LanguageDefined
            | BeforeWhitespace
            | Never

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsAutoClosingDeleteIEditorOption =
            | Always
            | Never
            | Auto

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsAutoSurroundIEditorOption =
            | LanguageDefined
            | Never
            | Quotes
            | Brackets

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsColorDecoratorActivatedOnIEditorOption =
            | ClickAndHover
            | Click
            | Hover

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsCursorSmoothCaretAnimationIEditorOption =
            | On
            | Off
            | Explicit

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsCursorSurroundingLinesStyleIEditorOption =
            | Default
            | All

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsExperimentalWhitespaceRenderingIEditorOption =
            | Off
            | Svg
            | Font

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsFoldingStrategyIEditorOption =
            | Auto
            | Indentation

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsMatchBracketsIEditorOption =
            | Always
            | Never
            | Near

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsMouseStyleIEditorOption =
            | Default
            | Text
            | Copy

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsMultiCursorModifierIEditorOption =
            | AltKey
            | MetaKey
            | CtrlKey

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsMultiCursorPasteIEditorOption =
            | Spread
            | Full

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsPeekWidgetDefaultFocusIEditorOption =
            | Tree
            | Editor

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsRenderFinalNewlineIEditorOption =
            | On
            | Off
            | Dimmed

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsRenderLineHighlightIEditorOption =
            | All
            | Line
            | None
            | Gutter

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsRenderValidationDecorationsIEditorOption =
            | On
            | Off
            | Editable

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsRenderWhitespaceIEditorOption =
            | All
            | None
            | Boundary
            | Selection
            | Trailing

        [<AllowNullLiteral>]
        type IExportsEditorOptionsRulersIEditorOption =
            interface
            end

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsShowFoldingControlsIEditorOption =
            | Always
            | Never
            | Mouseover

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsSnippetSuggestionsIEditorOption =
            | None
            | Top
            | Bottom
            | Inline

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsSuggestSelectionIEditorOption =
            | First
            | RecentlyUsed
            | RecentlyUsedByPrefix

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsTabCompletionIEditorOption =
            | On
            | Off
            | OnlySnippets

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsUnusualLineTerminatorsIEditorOption =
            | Auto
            | Off
            | Prompt

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsWordBreakIEditorOption =
            | Normal
            | KeepAll

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsWordWrapIEditorOption =
            | On
            | Off
            | WordWrapColumn
            | Bounded

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsWordWrapOverride1IEditorOption =
            | On
            | Off
            | Inherit

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IExportsEditorOptionsWrappingStrategyIEditorOption =
            | Simple
            | Advanced

        [<AllowNullLiteral>]
        type IExportsEditorOptions =
            abstract acceptSuggestionOnCommitCharacter: IEditorOption<EditorOption, bool> with get, set

            abstract acceptSuggestionOnEnter:
                IEditorOption<EditorOption, IExportsEditorOptionsAcceptSuggestionOnEnterIEditorOption> with get, set

            abstract accessibilitySupport: IEditorOption<EditorOption, AccessibilitySupport> with get, set
            abstract accessibilityPageSize: IEditorOption<EditorOption, float> with get, set
            abstract ariaLabel: IEditorOption<EditorOption, string> with get, set
            abstract ariaRequired: IEditorOption<EditorOption, bool> with get, set
            abstract screenReaderAnnounceInlineSuggestion: IEditorOption<EditorOption, bool> with get, set

            abstract autoClosingBrackets:
                IEditorOption<EditorOption, IExportsEditorOptionsAutoClosingBracketsIEditorOption> with get, set

            abstract autoClosingComments:
                IEditorOption<EditorOption, IExportsEditorOptionsAutoClosingBracketsIEditorOption> with get, set

            abstract autoClosingDelete: IEditorOption<EditorOption, IExportsEditorOptionsAutoClosingDeleteIEditorOption> with get, set

            abstract autoClosingOvertype:
                IEditorOption<EditorOption, IExportsEditorOptionsAutoClosingDeleteIEditorOption> with get, set

            abstract autoClosingQuotes:
                IEditorOption<EditorOption, IExportsEditorOptionsAutoClosingBracketsIEditorOption> with get, set

            abstract autoIndent: IEditorOption<EditorOption, EditorAutoIndentStrategy> with get, set
            abstract automaticLayout: IEditorOption<EditorOption, bool> with get, set
            abstract autoSurround: IEditorOption<EditorOption, IExportsEditorOptionsAutoSurroundIEditorOption> with get, set
            abstract bracketPairColorization: IEditorOption<EditorOption, obj> with get, set
            abstract bracketPairGuides: IEditorOption<EditorOption, obj> with get, set
            abstract stickyTabStops: IEditorOption<EditorOption, bool> with get, set
            abstract codeLens: IEditorOption<EditorOption, bool> with get, set
            abstract codeLensFontFamily: IEditorOption<EditorOption, string> with get, set
            abstract codeLensFontSize: IEditorOption<EditorOption, float> with get, set
            abstract colorDecorators: IEditorOption<EditorOption, bool> with get, set

            abstract colorDecoratorActivatedOn:
                IEditorOption<EditorOption, IExportsEditorOptionsColorDecoratorActivatedOnIEditorOption> with get, set

            abstract colorDecoratorsLimit: IEditorOption<EditorOption, float> with get, set
            abstract columnSelection: IEditorOption<EditorOption, bool> with get, set
            abstract comments: IEditorOption<EditorOption, obj> with get, set
            abstract contextmenu: IEditorOption<EditorOption, bool> with get, set
            abstract copyWithSyntaxHighlighting: IEditorOption<EditorOption, bool> with get, set
            abstract cursorBlinking: IEditorOption<EditorOption, TextEditorCursorBlinkingStyle> with get, set

            abstract cursorSmoothCaretAnimation:
                IEditorOption<EditorOption, IExportsEditorOptionsCursorSmoothCaretAnimationIEditorOption> with get, set

            abstract cursorStyle: IEditorOption<EditorOption, TextEditorCursorStyle> with get, set
            abstract cursorSurroundingLines: IEditorOption<EditorOption, float> with get, set

            abstract cursorSurroundingLinesStyle:
                IEditorOption<EditorOption, IExportsEditorOptionsCursorSurroundingLinesStyleIEditorOption> with get, set

            abstract cursorWidth: IEditorOption<EditorOption, float> with get, set
            abstract disableLayerHinting: IEditorOption<EditorOption, bool> with get, set
            abstract disableMonospaceOptimizations: IEditorOption<EditorOption, bool> with get, set
            abstract domReadOnly: IEditorOption<EditorOption, bool> with get, set
            abstract dragAndDrop: IEditorOption<EditorOption, bool> with get, set
            abstract emptySelectionClipboard: IEditorOption<EditorOption, bool> with get, set
            abstract dropIntoEditor: IEditorOption<EditorOption, obj> with get, set
            abstract stickyScroll: IEditorOption<EditorOption, obj> with get, set

            abstract experimentalWhitespaceRendering:
                IEditorOption<EditorOption, IExportsEditorOptionsExperimentalWhitespaceRenderingIEditorOption> with get, set

            abstract extraEditorClassName: IEditorOption<EditorOption, string> with get, set
            abstract fastScrollSensitivity: IEditorOption<EditorOption, float> with get, set
            abstract find: IEditorOption<EditorOption, obj> with get, set
            abstract fixedOverflowWidgets: IEditorOption<EditorOption, bool> with get, set
            abstract folding: IEditorOption<EditorOption, bool> with get, set
            abstract foldingStrategy: IEditorOption<EditorOption, IExportsEditorOptionsFoldingStrategyIEditorOption> with get, set
            abstract foldingHighlight: IEditorOption<EditorOption, bool> with get, set
            abstract foldingImportsByDefault: IEditorOption<EditorOption, bool> with get, set
            abstract foldingMaximumRegions: IEditorOption<EditorOption, float> with get, set
            abstract unfoldOnClickAfterEndOfLine: IEditorOption<EditorOption, bool> with get, set
            abstract fontFamily: IEditorOption<EditorOption, string> with get, set
            abstract fontInfo: IEditorOption<EditorOption, FontInfo> with get, set
            abstract fontLigatures2: IEditorOption<EditorOption, string> with get, set
            abstract fontSize: IEditorOption<EditorOption, float> with get, set
            abstract fontWeight: IEditorOption<EditorOption, string> with get, set
            abstract fontVariations: IEditorOption<EditorOption, string> with get, set
            abstract formatOnPaste: IEditorOption<EditorOption, bool> with get, set
            abstract formatOnType: IEditorOption<EditorOption, bool> with get, set
            abstract glyphMargin: IEditorOption<EditorOption, bool> with get, set
            abstract gotoLocation: IEditorOption<EditorOption, obj> with get, set
            abstract hideCursorInOverviewRuler: IEditorOption<EditorOption, bool> with get, set
            abstract hover: IEditorOption<EditorOption, obj> with get, set
            abstract inDiffEditor: IEditorOption<EditorOption, bool> with get, set
            abstract letterSpacing: IEditorOption<EditorOption, float> with get, set
            abstract lightbulb: IEditorOption<EditorOption, obj> with get, set
            abstract lineDecorationsWidth: IEditorOption<EditorOption, float> with get, set
            abstract lineHeight: IEditorOption<EditorOption, float> with get, set
            abstract lineNumbers: IEditorOption<EditorOption, InternalEditorRenderLineNumbersOptions> with get, set
            abstract lineNumbersMinChars: IEditorOption<EditorOption, float> with get, set
            abstract linkedEditing: IEditorOption<EditorOption, bool> with get, set
            abstract links: IEditorOption<EditorOption, bool> with get, set
            abstract matchBrackets: IEditorOption<EditorOption, IExportsEditorOptionsMatchBracketsIEditorOption> with get, set
            abstract minimap: IEditorOption<EditorOption, obj> with get, set
            abstract mouseStyle: IEditorOption<EditorOption, IExportsEditorOptionsMouseStyleIEditorOption> with get, set
            abstract mouseWheelScrollSensitivity: IEditorOption<EditorOption, float> with get, set
            abstract mouseWheelZoom: IEditorOption<EditorOption, bool> with get, set
            abstract multiCursorMergeOverlapping: IEditorOption<EditorOption, bool> with get, set

            abstract multiCursorModifier:
                IEditorOption<EditorOption, IExportsEditorOptionsMultiCursorModifierIEditorOption> with get, set

            abstract multiCursorPaste: IEditorOption<EditorOption, IExportsEditorOptionsMultiCursorPasteIEditorOption> with get, set
            abstract multiCursorLimit: IEditorOption<EditorOption, float> with get, set
            abstract occurrencesHighlight: IEditorOption<EditorOption, bool> with get, set
            abstract overviewRulerBorder: IEditorOption<EditorOption, bool> with get, set
            abstract overviewRulerLanes: IEditorOption<EditorOption, float> with get, set
            abstract padding: IEditorOption<EditorOption, obj> with get, set
            abstract pasteAs: IEditorOption<EditorOption, obj> with get, set
            abstract parameterHints: IEditorOption<EditorOption, obj> with get, set

            abstract peekWidgetDefaultFocus:
                IEditorOption<EditorOption, IExportsEditorOptionsPeekWidgetDefaultFocusIEditorOption> with get, set

            abstract definitionLinkOpensInPeek: IEditorOption<EditorOption, bool> with get, set
            abstract quickSuggestions: IEditorOption<EditorOption, InternalQuickSuggestionsOptions> with get, set
            abstract quickSuggestionsDelay: IEditorOption<EditorOption, float> with get, set
            abstract readOnly: IEditorOption<EditorOption, bool> with get, set
            abstract readOnlyMessage: IEditorOption<EditorOption, obj option> with get, set
            abstract renameOnType: IEditorOption<EditorOption, bool> with get, set
            abstract renderControlCharacters: IEditorOption<EditorOption, bool> with get, set

            abstract renderFinalNewline:
                IEditorOption<EditorOption, IExportsEditorOptionsRenderFinalNewlineIEditorOption> with get, set

            abstract renderLineHighlight:
                IEditorOption<EditorOption, IExportsEditorOptionsRenderLineHighlightIEditorOption> with get, set

            abstract renderLineHighlightOnlyWhenFocus: IEditorOption<EditorOption, bool> with get, set

            abstract renderValidationDecorations:
                IEditorOption<EditorOption, IExportsEditorOptionsRenderValidationDecorationsIEditorOption> with get, set

            abstract renderWhitespace: IEditorOption<EditorOption, IExportsEditorOptionsRenderWhitespaceIEditorOption> with get, set
            abstract revealHorizontalRightPadding: IEditorOption<EditorOption, float> with get, set
            abstract roundedSelection: IEditorOption<EditorOption, bool> with get, set
            abstract rulers: IEditorOption<EditorOption, IExportsEditorOptionsRulersIEditorOption> with get, set
            abstract scrollbar: IEditorOption<EditorOption, InternalEditorScrollbarOptions> with get, set
            abstract scrollBeyondLastColumn: IEditorOption<EditorOption, float> with get, set
            abstract scrollBeyondLastLine: IEditorOption<EditorOption, bool> with get, set
            abstract scrollPredominantAxis: IEditorOption<EditorOption, bool> with get, set
            abstract selectionClipboard: IEditorOption<EditorOption, bool> with get, set
            abstract selectionHighlight: IEditorOption<EditorOption, bool> with get, set
            abstract selectOnLineNumbers: IEditorOption<EditorOption, bool> with get, set

            abstract showFoldingControls:
                IEditorOption<EditorOption, IExportsEditorOptionsShowFoldingControlsIEditorOption> with get, set

            abstract showUnused: IEditorOption<EditorOption, bool> with get, set
            abstract showDeprecated: IEditorOption<EditorOption, bool> with get, set
            abstract inlayHints: IEditorOption<EditorOption, obj> with get, set

            abstract snippetSuggestions:
                IEditorOption<EditorOption, IExportsEditorOptionsSnippetSuggestionsIEditorOption> with get, set

            abstract smartSelect: IEditorOption<EditorOption, obj> with get, set
            abstract smoothScrolling: IEditorOption<EditorOption, bool> with get, set
            abstract stopRenderingLineAfter: IEditorOption<EditorOption, float> with get, set
            abstract suggest: IEditorOption<EditorOption, obj> with get, set
            abstract inlineSuggest: IEditorOption<EditorOption, obj> with get, set
            abstract inlineCompletionsAccessibilityVerbose: IEditorOption<EditorOption, bool> with get, set
            abstract suggestFontSize: IEditorOption<EditorOption, float> with get, set
            abstract suggestLineHeight: IEditorOption<EditorOption, float> with get, set
            abstract suggestOnTriggerCharacters: IEditorOption<EditorOption, bool> with get, set
            abstract suggestSelection: IEditorOption<EditorOption, IExportsEditorOptionsSuggestSelectionIEditorOption> with get, set
            abstract tabCompletion: IEditorOption<EditorOption, IExportsEditorOptionsTabCompletionIEditorOption> with get, set
            abstract tabIndex: IEditorOption<EditorOption, float> with get, set
            abstract unicodeHighlight: IEditorOption<EditorOption, obj option> with get, set

            abstract unusualLineTerminators:
                IEditorOption<EditorOption, IExportsEditorOptionsUnusualLineTerminatorsIEditorOption> with get, set

            abstract useShadowDOM: IEditorOption<EditorOption, bool> with get, set
            abstract useTabStops: IEditorOption<EditorOption, bool> with get, set
            abstract wordBreak: IEditorOption<EditorOption, IExportsEditorOptionsWordBreakIEditorOption> with get, set
            abstract wordSeparators: IEditorOption<EditorOption, string> with get, set
            abstract wordWrap: IEditorOption<EditorOption, IExportsEditorOptionsWordWrapIEditorOption> with get, set
            abstract wordWrapBreakAfterCharacters: IEditorOption<EditorOption, string> with get, set
            abstract wordWrapBreakBeforeCharacters: IEditorOption<EditorOption, string> with get, set
            abstract wordWrapColumn: IEditorOption<EditorOption, float> with get, set
            abstract wordWrapOverride1: IEditorOption<EditorOption, IExportsEditorOptionsWordWrapOverride1IEditorOption> with get, set
            abstract wordWrapOverride2: IEditorOption<EditorOption, IExportsEditorOptionsWordWrapOverride1IEditorOption> with get, set
            abstract editorClassName: IEditorOption<EditorOption, string> with get, set
            abstract defaultColorDecorators: IEditorOption<EditorOption, bool> with get, set
            abstract pixelRatio: IEditorOption<EditorOption, float> with get, set
            abstract tabFocusMode: IEditorOption<EditorOption, bool> with get, set
            abstract layoutInfo: IEditorOption<EditorOption, EditorLayoutInfo> with get, set
            abstract wrappingInfo: IEditorOption<EditorOption, EditorWrappingInfo> with get, set
            abstract wrappingIndent: IEditorOption<EditorOption, WrappingIndent> with get, set
            abstract wrappingStrategy: IEditorOption<EditorOption, IExportsEditorOptionsWrappingStrategyIEditorOption> with get, set

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IGlobalEditorOptionsSemanticHighlightingEnabled = | ConfiguredByTheme

        [<AllowNullLiteral>]
        type ICodeEditorViewStateContributionsState =
            [<EmitIndexer>]
            abstract Item: id: string -> obj option with get, set

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsRenderValidationDecorations =
            | Editable
            | On
            | Off

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsCursorBlinking =
            | Blink
            | Smooth
            | Phase
            | Expand
            | Solid

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsMouseStyle =
            | Text
            | Default
            | Copy

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsCursorSmoothCaretAnimation =
            | Off
            | Explicit
            | On

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsCursorStyle =
            | Line
            | Block
            | Underline
            | [<CompiledName("line-thin")>] LineThin
            | [<CompiledName("block-outline")>] BlockOutline
            | [<CompiledName("underline-thin")>] UnderlineThin

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsWordWrap =
            | Off
            | On
            | WordWrapColumn
            | Bounded

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsWordWrapOverride1 =
            | Off
            | On
            | Inherit

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsWrappingIndent =
            | None
            | Same
            | Indent
            | DeepIndent

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsMultiCursorModifier =
            | CtrlCmd
            | Alt

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsAccessibilitySupport =
            | Auto
            | Off
            | On

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsAutoIndent =
            | None
            | Keep
            | Brackets
            | Advanced
            | Full

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsAcceptSuggestionOnEnter =
            | On
            | Smart
            | Off

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsSnippetSuggestions =
            | Top
            | Bottom
            | Inline
            | None

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsMatchBrackets =
            | Never
            | Near
            | Always

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsExperimentalWhitespaceRendering =
            | Svg
            | Font
            | Off

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsRenderWhitespace =
            | None
            | Boundary
            | Selection
            | Trailing
            | All

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorOptionsRenderLineHighlight =
            | None
            | Gutter
            | Line
            | All

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IDiffEditorBaseOptionsDiffAlgorithm =
            | Legacy
            | Advanced

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorFindOptionsSeedSearchStringFromSelection =
            | Never
            | Always
            | Selection

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorFindOptionsAutoFindInSelection =
            | Never
            | Always
            | Multiline

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorStickyScrollOptionsDefaultModel =
            | OutlineModel
            | FoldingProviderModel
            | IndentationModel

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorInlayHintsOptionsEnabled =
            | On
            | Off
            | OffUnlessPressed
            | OnUnlessPressed

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorMinimapOptionsSide =
            | Right
            | Left

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorMinimapOptionsSize =
            | Proportional
            | Fill
            | Fit

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorMinimapOptionsShowSlider =
            | Always
            | Mouseover

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IEditorScrollbarOptionsVertical =
            | Auto
            | Visible
            | Hidden

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IInlineSuggestOptionsMode =
            | Prefix
            | Subword
            | SubwordSmart

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IInlineSuggestOptionsShowToolbar =
            | Always
            | OnHover

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type ISuggestOptionsInsertMode =
            | Insert
            | Replace

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type ISuggestOptionsSelectionMode =
            | Always
            | Never
            | WhenTriggerCharacter
            | WhenQuickSuggestion

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IDropIntoEditorOptionsShowDropSelector =
            | AfterDrop
            | Never

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IPasteAsOptionsShowPasteSelector =
            | AfterPaste
            | Never

        [<StringEnum>]
        [<RequireQualifiedAccess>]
        type IMouseTargetOutsideEditorOutsidePosition =
            | Above
            | Below
            | Left
            | Right

    module Languages =
        [<Import("css", "module/monaco/languages")>]
        let css: Css.IExports = jsNative

        [<Import("html", "module/monaco/languages")>]
        let html: Html.IExports = jsNative

        [<Import("json", "module/monaco/languages")>]
        let json: Json.IExports = jsNative

        [<Import("typescript", "module/monaco/languages")>]
        let typescript: Typescript.IExports = jsNative

        [<AllowNullLiteral>]
        type IExports =
            /// Register information about a new language.
            abstract register: language: ILanguageExtensionPoint -> unit
            /// Get the information of all the registered languages.
            abstract getLanguages: unit -> ResizeArray<ILanguageExtensionPoint>
            abstract getEncodedLanguageId: languageId: string -> float
            /// <summary>An event emitted when a language is associated for the first time with a text model.</summary>
            abstract onLanguage: languageId: string * callback: (unit -> unit) -> IDisposable
            /// <summary>
            /// An event emitted when a language is associated for the first time with a text model or
            /// when a language is encountered during the tokenization of another language.
            /// </summary>
            abstract onLanguageEncountered: languageId: string * callback: (unit -> unit) -> IDisposable
            /// Set the editing configuration for a language.
            abstract setLanguageConfiguration: languageId: string * configuration: LanguageConfiguration -> IDisposable
            /// Change the color map that is used for token colors.
            /// Supported formats (hex): #RRGGBB, $RRGGBBAA, #RGB, #RGBA
            abstract setColorMap: colorMap: ResizeArray<string> option -> unit
            /// <summary>
            /// Register a tokens provider factory for a language. This tokenizer will be exclusive with a tokenizer
            /// set using <c>setTokensProvider</c> or one created using <c>setMonarchTokensProvider</c>, but will work together
            /// with a tokens provider set using <c>registerDocumentSemanticTokensProvider</c> or <c>registerDocumentRangeSemanticTokensProvider</c>.
            /// </summary>
            abstract registerTokensProviderFactory: languageId: string * factory: TokensProviderFactory -> IDisposable

            /// <summary>
            /// Set the tokens provider for a language (manual implementation). This tokenizer will be exclusive
            /// with a tokenizer created using <c>setMonarchTokensProvider</c>, or with <c>registerTokensProviderFactory</c>,
            /// but will work together with a tokens provider set using <c>registerDocumentSemanticTokensProvider</c>
            /// or <c>registerDocumentRangeSemanticTokensProvider</c>.
            /// </summary>
            abstract setTokensProvider:
                languageId: string *
                provider: U3<TokensProvider, EncodedTokensProvider, Thenable<U2<TokensProvider, EncodedTokensProvider>>> ->
                    IDisposable

            /// <summary>
            /// Set the tokens provider for a language (monarch implementation). This tokenizer will be exclusive
            /// with a tokenizer set using <c>setTokensProvider</c>, or with <c>registerTokensProviderFactory</c>, but will
            /// work together with a tokens provider set using <c>registerDocumentSemanticTokensProvider</c> or
            /// <c>registerDocumentRangeSemanticTokensProvider</c>.
            /// </summary>
            abstract setMonarchTokensProvider:
                languageId: string * languageDef: U2<IMonarchLanguage, Thenable<IMonarchLanguage>> -> IDisposable

            /// Register a reference provider (used by e.g. reference search).
            abstract registerReferenceProvider:
                languageSelector: LanguageSelector * provider: ReferenceProvider -> IDisposable

            /// Register a rename provider (used by e.g. rename symbol).
            abstract registerRenameProvider:
                languageSelector: LanguageSelector * provider: RenameProvider -> IDisposable

            /// Register a signature help provider (used by e.g. parameter hints).
            abstract registerSignatureHelpProvider:
                languageSelector: LanguageSelector * provider: SignatureHelpProvider -> IDisposable

            /// Register a hover provider (used by e.g. editor hover).
            abstract registerHoverProvider: languageSelector: LanguageSelector * provider: HoverProvider -> IDisposable

            /// Register a document symbol provider (used by e.g. outline).
            abstract registerDocumentSymbolProvider:
                languageSelector: LanguageSelector * provider: DocumentSymbolProvider -> IDisposable

            /// Register a document highlight provider (used by e.g. highlight occurrences).
            abstract registerDocumentHighlightProvider:
                languageSelector: LanguageSelector * provider: DocumentHighlightProvider -> IDisposable

            /// Register an linked editing range provider.
            abstract registerLinkedEditingRangeProvider:
                languageSelector: LanguageSelector * provider: LinkedEditingRangeProvider -> IDisposable

            /// Register a definition provider (used by e.g. go to definition).
            abstract registerDefinitionProvider:
                languageSelector: LanguageSelector * provider: DefinitionProvider -> IDisposable

            /// Register a implementation provider (used by e.g. go to implementation).
            abstract registerImplementationProvider:
                languageSelector: LanguageSelector * provider: ImplementationProvider -> IDisposable

            /// Register a type definition provider (used by e.g. go to type definition).
            abstract registerTypeDefinitionProvider:
                languageSelector: LanguageSelector * provider: TypeDefinitionProvider -> IDisposable

            /// Register a code lens provider (used by e.g. inline code lenses).
            abstract registerCodeLensProvider:
                languageSelector: LanguageSelector * provider: CodeLensProvider -> IDisposable

            /// Register a code action provider (used by e.g. quick fix).
            abstract registerCodeActionProvider:
                languageSelector: LanguageSelector *
                provider: CodeActionProvider *
                ?metadata: CodeActionProviderMetadata ->
                    IDisposable

            /// Register a formatter that can handle only entire models.
            abstract registerDocumentFormattingEditProvider:
                languageSelector: LanguageSelector * provider: DocumentFormattingEditProvider -> IDisposable

            /// Register a formatter that can handle a range inside a model.
            abstract registerDocumentRangeFormattingEditProvider:
                languageSelector: LanguageSelector * provider: DocumentRangeFormattingEditProvider -> IDisposable

            /// Register a formatter than can do formatting as the user types.
            abstract registerOnTypeFormattingEditProvider:
                languageSelector: LanguageSelector * provider: OnTypeFormattingEditProvider -> IDisposable

            /// Register a link provider that can find links in text.
            abstract registerLinkProvider: languageSelector: LanguageSelector * provider: LinkProvider -> IDisposable

            /// Register a completion item provider (use by e.g. suggestions).
            abstract registerCompletionItemProvider:
                languageSelector: LanguageSelector * provider: CompletionItemProvider -> IDisposable

            /// Register a document color provider (used by Color Picker, Color Decorator).
            abstract registerColorProvider:
                languageSelector: LanguageSelector * provider: DocumentColorProvider -> IDisposable

            /// Register a folding range provider
            abstract registerFoldingRangeProvider:
                languageSelector: LanguageSelector * provider: FoldingRangeProvider -> IDisposable

            /// Register a declaration provider
            abstract registerDeclarationProvider:
                languageSelector: LanguageSelector * provider: DeclarationProvider -> IDisposable

            /// Register a selection range provider
            abstract registerSelectionRangeProvider:
                languageSelector: LanguageSelector * provider: SelectionRangeProvider -> IDisposable

            /// <summary>
            /// Register a document semantic tokens provider. A semantic tokens provider will complement and enhance a
            /// simple top-down tokenizer. Simple top-down tokenizers can be set either via <c>setMonarchTokensProvider</c>
            /// or <c>setTokensProvider</c>.
            ///
            /// For the best user experience, register both a semantic tokens provider and a top-down tokenizer.
            /// </summary>
            abstract registerDocumentSemanticTokensProvider:
                languageSelector: LanguageSelector * provider: DocumentSemanticTokensProvider -> IDisposable

            /// <summary>
            /// Register a document range semantic tokens provider. A semantic tokens provider will complement and enhance a
            /// simple top-down tokenizer. Simple top-down tokenizers can be set either via <c>setMonarchTokensProvider</c>
            /// or <c>setTokensProvider</c>.
            ///
            /// For the best user experience, register both a semantic tokens provider and a top-down tokenizer.
            /// </summary>
            abstract registerDocumentRangeSemanticTokensProvider:
                languageSelector: LanguageSelector * provider: DocumentRangeSemanticTokensProvider -> IDisposable

            /// Register an inline completions provider.
            abstract registerInlineCompletionsProvider:
                languageSelector: LanguageSelector * provider: InlineCompletionsProvider -> IDisposable

            /// Register an inlay hints provider.
            abstract registerInlayHintsProvider:
                languageSelector: LanguageSelector * provider: InlayHintsProvider -> IDisposable

            abstract SelectedSuggestionInfo: SelectedSuggestionInfoStatic
            abstract FoldingRangeKind: FoldingRangeKindStatic

        [<AllowNullLiteral>]
        type IRelativePattern =
            /// A base file path to which this pattern will be matched against relatively.
            abstract ``base``: string
            /// <summary>
            /// A file glob pattern like <c>*.{ts,js}</c> that will be matched on file paths
            /// relative to the base path.
            ///
            /// Example: Given a base of <c>/home/work/folder</c> and a file path of <c>/home/work/folder/index.js</c>,
            /// the file glob pattern will match on <c>index.js</c>.
            /// </summary>
            abstract pattern: string

        type LanguageSelector = U3<string, LanguageFilter, ReadonlyArray<U2<string, LanguageFilter>>>

        [<AllowNullLiteral>]
        type LanguageFilter =
            abstract language: string option
            abstract scheme: string option
            abstract pattern: U2<string, IRelativePattern> option
            abstract notebookType: string option
            /// This provider is implemented in the UI thread.
            abstract hasAccessToAllModels: bool option
            abstract exclusive: bool option
            /// This provider comes from a builtin extension.
            abstract isBuiltin: bool option

        /// A token.
        [<AllowNullLiteral>]
        type IToken =
            abstract startIndex: float with get, set
            abstract scopes: string with get, set

        /// The result of a line tokenization.
        [<AllowNullLiteral>]
        type ILineTokens =
            /// The list of tokens on the line.
            abstract tokens: ResizeArray<IToken> with get, set
            /// The tokenization end state.
            /// A pointer will be held to this and the object should not be modified by the tokenizer after the pointer is returned.
            abstract endState: IState with get, set

        /// The result of a line tokenization.
        [<AllowNullLiteral>]
        type IEncodedLineTokens =
            /// <summary>
            /// The tokens on the line in a binary, encoded format. Each token occupies two array indices. For token i:
            ///  - at offset 2*i =&gt; startIndex
            ///  - at offset 2*i + 1 =&gt; metadata
            /// Meta data is in binary format:
            /// - -------------------------------------------
            ///     3322 2222 2222 1111 1111 1100 0000 0000
            ///     1098 7654 3210 9876 5432 1098 7654 3210
            /// - -------------------------------------------
            ///     bbbb bbbb bfff ffff ffFF FFTT LLLL LLLL
            /// - -------------------------------------------
            ///  - L = EncodedLanguageId (8 bits): Use <c>getEncodedLanguageId</c> to get the encoded ID of a language.
            ///  - T = StandardTokenType (2 bits): Other = 0, Comment = 1, String = 2, RegEx = 3.
            ///  - F = FontStyle (4 bits): None = 0, Italic = 1, Bold = 2, Underline = 4, Strikethrough = 8.
            ///  - f = foreground ColorId (9 bits)
            ///  - b = background ColorId (9 bits)
            ///  - The color value for each colorId is defined in IStandaloneThemeData.customTokenColors:
            /// e.g. colorId = 1 is stored in IStandaloneThemeData.customTokenColors[1]. Color id = 0 means no color,
            /// id = 1 is for the default foreground color, id = 2 for the default background.
            /// </summary>
            abstract tokens: Uint32Array with get, set
            /// The tokenization end state.
            /// A pointer will be held to this and the object should not be modified by the tokenizer after the pointer is returned.
            abstract endState: IState with get, set

        /// A factory for token providers.
        [<AllowNullLiteral>]
        type TokensProviderFactory =
            abstract create: unit -> ProviderResult<U3<TokensProvider, EncodedTokensProvider, IMonarchLanguage>>

        /// A "manual" provider of tokens.
        [<AllowNullLiteral>]
        type TokensProvider =
            /// The initial state of a language. Will be the state passed in to tokenize the first line.
            abstract getInitialState: unit -> IState
            /// Tokenize a line given the state at the beginning of the line.
            abstract tokenize: line: string * state: IState -> ILineTokens

        /// A "manual" provider of tokens, returning tokens in a binary form.
        [<AllowNullLiteral>]
        type EncodedTokensProvider =
            /// The initial state of a language. Will be the state passed in to tokenize the first line.
            abstract getInitialState: unit -> IState
            /// Tokenize a line given the state at the beginning of the line.
            abstract tokenizeEncoded: line: string * state: IState -> IEncodedLineTokens
            /// Tokenize a line given the state at the beginning of the line.
            abstract tokenize: line: string * state: IState -> ILineTokens

        /// <summary>
        /// Contains additional diagnostic information about the context in which
        /// a <see cref="CodeActionProvider.provideCodeActions">code action</see> is run.
        /// </summary>
        [<AllowNullLiteral>]
        type CodeActionContext =
            /// An array of diagnostics.
            abstract markers: ResizeArray<Editor.IMarkerData>
            /// Requested kind of actions to return.
            abstract only: string option
            /// The reason why code actions were requested.
            abstract trigger: CodeActionTriggerType

        /// <summary>
        /// The code action interface defines the contract between extensions and
        /// the <see href="https://code.visualstudio.com/docs/editor/editingevolved#_code-action">light bulb</see> feature.
        /// </summary>
        [<AllowNullLiteral>]
        type CodeActionProvider =
            /// Provide commands for the given document and range.
            abstract provideCodeActions:
                model: Editor.ITextModel * range: Range * context: CodeActionContext * token: CancellationToken ->
                    ProviderResult<CodeActionList>

            /// Given a code action fill in the edit. Will only invoked when missing.
            abstract resolveCodeAction: codeAction: CodeAction * token: CancellationToken -> ProviderResult<CodeAction>

        /// <summary>Metadata about the type of code actions that a <see cref="CodeActionProvider" /> provides.</summary>
        [<AllowNullLiteral>]
        type CodeActionProviderMetadata =
            /// <summary>
            /// List of code action kinds that a <see cref="CodeActionProvider" /> may return.
            ///
            /// This list is used to determine if a given <c>CodeActionProvider</c> should be invoked or not.
            /// To avoid unnecessary computation, every <c>CodeActionProvider</c> should list use <c>providedCodeActionKinds</c>. The
            /// list of kinds may either be generic, such as <c>["quickfix", "refactor", "source"]</c>, or list out every kind provided,
            /// such as <c>["quickfix.removeLine", "source.fixAll" ...]</c>.
            /// </summary>
            abstract providedCodeActionKinds: ResizeArray<string> option
            abstract documentation: ReadonlyArray<{| kind: string; command: Command |}> option

        /// Describes how comments for a language work.
        [<AllowNullLiteral>]
        type CommentRule =
            /// <summary>The line comment token, like <c>// this is a comment</c></summary>
            abstract lineComment: string option with get, set
            /// <summary>The block comment character pair, like <c>/* block comment *&amp;#47;</c></summary>
            abstract blockComment: CharacterPair option with get, set

        /// The language configuration interface defines the contract between extensions and
        /// various editor features, like automatic bracket insertion, automatic indentation etc.
        [<AllowNullLiteral>]
        type LanguageConfiguration =
            /// The language's comment settings.
            abstract comments: CommentRule option with get, set
            /// The language's brackets.
            /// This configuration implicitly affects pressing Enter around these brackets.
            abstract brackets: ResizeArray<CharacterPair> option with get, set
            /// The language's word definition.
            /// If the language supports Unicode identifiers (e.g. JavaScript), it is preferable
            /// to provide a word definition that uses exclusion of known separators.
            /// e.g.: A regex that matches anything except known separators (and dot is allowed to occur in a floating point number):
            ///   /(-?\d*\.\d\w*)|([^\`\~\!\@\#\%\^\&\*\(\)\-\=\+\[\{\]\}\\\|\;\:\'\"\,\.\<\>\/\?\s]+)/g
            abstract wordPattern: RegExp option with get, set
            /// The language's indentation settings.
            abstract indentationRules: IndentationRule option with get, set
            /// The language's rules to be evaluated when pressing Enter.
            abstract onEnterRules: ResizeArray<OnEnterRule> option with get, set
            /// The language's auto closing pairs. The 'close' character is automatically inserted with the
            /// 'open' character is typed. If not set, the configured brackets will be used.
            abstract autoClosingPairs: ResizeArray<IAutoClosingPairConditional> option with get, set
            /// The language's surrounding pairs. When the 'open' character is typed on a selection, the
            /// selected string is surrounded by the open and close characters. If not set, the autoclosing pairs
            /// settings will be used.
            abstract surroundingPairs: ResizeArray<IAutoClosingPair> option with get, set
            /// Defines a list of bracket pairs that are colorized depending on their nesting level.
            /// If not set, the configured brackets will be used.
            abstract colorizedBracketPairs: ResizeArray<CharacterPair> option with get, set
            /// Defines what characters must be after the cursor for bracket or quote autoclosing to occur when using the \'languageDefined\' autoclosing setting.
            ///
            /// This is typically the set of characters which can not start an expression, such as whitespace, closing brackets, non-unary operators, etc.
            abstract autoCloseBefore: string option with get, set
            /// The language's folding rules.
            abstract folding: FoldingRules option with get, set

            /// **Deprecated** Do not use.
            [<Obsolete("Will be replaced by a better API soon.")>]
            abstract __electricCharacterSupport: {| docComment: IDocComment option |} option with get, set

        /// Describes indentation rules for a language.
        [<AllowNullLiteral>]
        type IndentationRule =
            /// If a line matches this pattern, then all the lines after it should be unindented once (until another rule matches).
            abstract decreaseIndentPattern: RegExp with get, set
            /// If a line matches this pattern, then all the lines after it should be indented once (until another rule matches).
            abstract increaseIndentPattern: RegExp with get, set
            /// If a line matches this pattern, then **only the next line** after it should be indented once.
            abstract indentNextLinePattern: RegExp option with get, set
            /// If a line matches this pattern, then its indentation should not be changed and it should not be evaluated against the other rules.
            abstract unIndentedLinePattern: RegExp option with get, set

        /// Describes language specific folding markers such as '#region' and '#endregion'.
        /// The start and end regexes will be tested against the contents of all lines and must be designed efficiently:
        /// - the regex should start with '^'
        /// - regexp flags (i, g) are ignored
        [<AllowNullLiteral>]
        type FoldingMarkers =
            abstract start: RegExp with get, set
            abstract ``end``: RegExp with get, set

        /// Describes folding rules for a language.
        [<AllowNullLiteral>]
        type FoldingRules =
            /// <summary>
            /// Used by the indentation based strategy to decide whether empty lines belong to the previous or the next block.
            /// A language adheres to the off-side rule if blocks in that language are expressed by their indentation.
            /// See <see href="https://en.wikipedia.org/wiki/Off-side_rule">wikipedia</see> for more information.
            /// If not set, <c>false</c> is used and empty lines belong to the previous block.
            /// </summary>
            abstract offSide: bool option with get, set
            /// Region markers used by the language.
            abstract markers: FoldingMarkers option with get, set

        /// Describes a rule to be evaluated when pressing Enter.
        [<AllowNullLiteral>]
        type OnEnterRule =
            /// This rule will only execute if the text before the cursor matches this regular expression.
            abstract beforeText: RegExp with get, set
            /// This rule will only execute if the text after the cursor matches this regular expression.
            abstract afterText: RegExp option with get, set
            /// This rule will only execute if the text above the this line matches this regular expression.
            abstract previousLineText: RegExp option with get, set
            /// The action to execute.
            abstract action: EnterAction with get, set

        /// Definition of documentation comments (e.g. Javadoc/JSdoc)
        [<AllowNullLiteral>]
        type IDocComment =
            /// The string that starts a doc comment (e.g. '/**')
            abstract ``open``: string with get, set
            /// The string that appears on the last line and closes the doc comment (e.g. ' * /').
            abstract close: string option with get, set

        /// A tuple of two characters, like a pair of
        /// opening and closing brackets.
        type CharacterPair = string * string

        [<AllowNullLiteral>]
        type IAutoClosingPair =
            abstract ``open``: string with get, set
            abstract close: string with get, set

        [<AllowNullLiteral>]
        type IAutoClosingPairConditional =
            inherit IAutoClosingPair
            abstract notIn: ResizeArray<string> option with get, set

        /// Describes what to do with the indentation when pressing Enter.
        [<RequireQualifiedAccess>]
        type IndentAction =
            /// Insert new line and copy the previous line's indentation.
            | None = 0
            /// Insert new line and indent once (relative to the previous line's indentation).
            | Indent = 1
            /// Insert two new lines:
            ///  - the first one indented which will hold the cursor
            ///  - the second one at the same indentation level
            | IndentOutdent = 2
            /// Insert new line and outdent once (relative to the previous line's indentation).
            | Outdent = 3

        /// Describes what to do when pressing Enter.
        [<AllowNullLiteral>]
        type EnterAction =
            /// Describe what to do with the indentation.
            abstract indentAction: IndentAction with get, set
            /// Describes text to be appended after the new line and after the indentation.
            abstract appendText: string option with get, set
            /// Describes the number of characters to remove from the new line's indentation.
            abstract removeText: float option with get, set

        /// The state of the tokenizer between two lines.
        /// It is useful to store flags such as in multiline comment, etc.
        /// The model will clone the previous line's state and pass it in to tokenize the next line.
        [<AllowNullLiteral>]
        type IState =
            abstract clone: unit -> IState
            abstract equals: other: IState -> bool

        /// <summary>
        /// A provider result represents the values a provider, like the <see cref="HoverProvider" />,
        /// may return. For once this is the actual result type <c>T</c>, like <c>Hover</c>, or a thenable that resolves
        /// to that type <c>T</c>. In addition, <c>null</c> and <c>undefined</c> can be returned - either directly or from a
        /// thenable.
        /// </summary>
        type ProviderResult<'T> = U2<'T, Thenable<'T option>> option

        /// A hover represents additional information for a symbol or word. Hovers are
        /// rendered in a tooltip-like widget.
        [<AllowNullLiteral>]
        type Hover =
            /// The contents of this hover.
            abstract contents: ResizeArray<IMarkdownString> with get, set
            /// The range to which this hover applies. When missing, the
            /// editor will use the range at the current position or the
            /// current position itself.
            abstract range: IRange option with get, set

        /// <summary>
        /// The hover provider interface defines the contract between extensions and
        /// the <see href="https://code.visualstudio.com/docs/editor/intellisense">hover</see>-feature.
        /// </summary>
        [<AllowNullLiteral>]
        type HoverProvider =
            /// Provide a hover for the given position and document. Multiple hovers at the same
            /// position will be merged by the editor. A hover can have a range which defaults
            /// to the word range at the position when omitted.
            abstract provideHover:
                model: Editor.ITextModel * position: Position * token: CancellationToken -> ProviderResult<Hover>

        [<RequireQualifiedAccess>]
        type CompletionItemKind =
            | Method = 0
            | Function = 1
            | Constructor = 2
            | Field = 3
            | Variable = 4
            | Class = 5
            | Struct = 6
            | Interface = 7
            | Module = 8
            | Property = 9
            | Event = 10
            | Operator = 11
            | Unit = 12
            | Value = 13
            | Constant = 14
            | Enum = 15
            | EnumMember = 16
            | Keyword = 17
            | Text = 18
            | Color = 19
            | File = 20
            | Reference = 21
            | Customcolor = 22
            | Folder = 23
            | TypeParameter = 24
            | User = 25
            | Issue = 26
            | Snippet = 27

        [<AllowNullLiteral>]
        type CompletionItemLabel =
            abstract label: string with get, set
            abstract detail: string option with get, set
            abstract description: string option with get, set

        [<RequireQualifiedAccess>]
        type CompletionItemTag =
            | Deprecated = 1

        [<RequireQualifiedAccess>]
        type CompletionItemInsertTextRule =
            | None = 0
            /// Adjust whitespace/indentation of multiline insert texts to
            /// match the current line indentation.
            | KeepWhitespace = 1
            /// <summary><c>insertText</c> is a snippet.</summary>
            | InsertAsSnippet = 4

        [<AllowNullLiteral>]
        type CompletionItemRanges =
            abstract insert: IRange with get, set
            abstract replace: IRange with get, set

        /// A completion item represents a text snippet that is
        /// proposed to complete text that is being typed.
        [<AllowNullLiteral>]
        type CompletionItem =
            /// The label of this completion item. By default
            /// this is also the text that is inserted when selecting
            /// this completion.
            abstract label: U2<string, CompletionItemLabel> with get, set
            /// The kind of this completion item. Based on the kind
            /// an icon is chosen by the editor.
            abstract kind: CompletionItemKind with get, set
            /// <summary>
            /// A modifier to the <c>kind</c> which affect how the item
            /// is rendered, e.g. Deprecated is rendered with a strikeout
            /// </summary>
            abstract tags: ReadonlyArray<CompletionItemTag> option with get, set
            /// A human-readable string with additional information
            /// about this item, like type or symbol information.
            abstract detail: string option with get, set
            /// A human-readable string that represents a doc-comment.
            abstract documentation: U2<string, IMarkdownString> option with get, set
            /// <summary>
            /// A string that should be used when comparing this item
            /// with other items. When <c>falsy</c> the <see cref="CompletionItem.labellabel" />
            /// is used.
            /// </summary>
            abstract sortText: string option with get, set
            /// <summary>
            /// A string that should be used when filtering a set of
            /// completion items. When <c>falsy</c> the <see cref="CompletionItem.labellabel" />
            /// is used.
            /// </summary>
            abstract filterText: string option with get, set
            /// Select this item when showing. *Note* that only one completion item can be selected and
            /// that the editor decides which item that is. The rule is that the *first* item of those
            /// that match best is selected.
            abstract preselect: bool option with get, set
            /// A string or snippet that should be inserted in a document when selecting
            /// this completion.
            abstract insertText: string with get, set
            /// Additional rules (as bitmask) that should be applied when inserting
            /// this completion.
            abstract insertTextRules: CompletionItemInsertTextRule option with get, set
            /// <summary>
            /// A range of text that should be replaced by this completion item.
            ///
            /// Defaults to a range from the start of the <see cref="TextDocument.getWordRangeAtPosition">current word</see> to the
            /// current position.
            ///
            /// *Note:* The range must be a <see cref="Range.isSingleLine">single line</see> and it must
            /// <see cref="Range.contains">contain</see> the position at which completion has been <see cref="CompletionItemProvider.provideCompletionItemsrequested" />.
            /// </summary>
            abstract range: U2<IRange, CompletionItemRanges> with get, set
            /// <summary>
            /// An optional set of characters that when pressed while this completion is active will accept it first and
            /// then type that character. *Note* that all commit characters should have <c>length=1</c> and that superfluous
            /// characters will be ignored.
            /// </summary>
            abstract commitCharacters: ResizeArray<string> option with get, set
            /// An optional array of additional text edits that are applied when
            /// selecting this completion. Edits must not overlap with the main edit
            /// nor with themselves.
            abstract additionalTextEdits: ResizeArray<Editor.ISingleEditOperation> option with get, set
            /// A command that should be run upon acceptance of this item.
            abstract command: Command option with get, set

        [<AllowNullLiteral>]
        type CompletionList =
            abstract suggestions: ResizeArray<CompletionItem> with get, set
            abstract incomplete: bool option with get, set
            abstract dispose: unit -> unit

        /// How a suggest provider was triggered.
        [<RequireQualifiedAccess>]
        type CompletionTriggerKind =
            | Invoke = 0
            | TriggerCharacter = 1
            | TriggerForIncompleteCompletions = 2

        /// <summary>
        /// Contains additional information about the context in which
        /// <see cref="CompletionItemProvider.provideCompletionItemscompletion">provider</see> is triggered.
        /// </summary>
        [<AllowNullLiteral>]
        type CompletionContext =
            /// How the completion was triggered.
            abstract triggerKind: CompletionTriggerKind with get, set
            /// <summary>
            /// Character that triggered the completion item provider.
            ///
            /// <c>undefined</c> if provider was not triggered by a character.
            /// </summary>
            abstract triggerCharacter: string option with get, set

        /// <summary>
        /// The completion item provider interface defines the contract between extensions and
        /// the <see href="https://code.visualstudio.com/docs/editor/intellisense">IntelliSense</see>.
        ///
        /// When computing *complete* completion items is expensive, providers can optionally implement
        /// the <c>resolveCompletionItem</c>-function. In that case it is enough to return completion
        /// items with a <see cref="CompletionItem.labellabel" /> from the
        /// <see cref="CompletionItemProvider.provideCompletionItemsprovideCompletionItems" />-function. Subsequently,
        /// when a completion item is shown in the UI and gains focus this provider is asked to resolve
        /// the item, like adding <see cref="CompletionItem.documentationdoc-comment" /> or <see cref="CompletionItem.detaildetails" />.
        /// </summary>
        [<AllowNullLiteral>]
        type CompletionItemProvider =
            abstract triggerCharacters: ResizeArray<string> option with get, set

            /// Provide completion items for the given position and document.
            abstract provideCompletionItems:
                model: Editor.ITextModel * position: Position * context: CompletionContext * token: CancellationToken ->
                    ProviderResult<CompletionList>

            /// <summary>
            /// Given a completion item fill in more data, like <see cref="CompletionItem.documentationdoc-comment" />
            /// or <see cref="CompletionItem.detaildetails" />.
            ///
            /// The editor will only resolve a completion item once.
            /// </summary>
            abstract resolveCompletionItem:
                item: CompletionItem * token: CancellationToken -> ProviderResult<CompletionItem>

        /// <summary>How an <see cref="InlineCompletionsProviderinline">completion provider</see> was triggered.</summary>
        [<RequireQualifiedAccess>]
        type InlineCompletionTriggerKind =
            /// Completion was triggered automatically while editing.
            /// It is sufficient to return a single completion item in this case.
            | Automatic = 0
            /// Completion was triggered explicitly by a user gesture.
            /// Return multiple completion items to enable cycling through them.
            | Explicit = 1

        [<AllowNullLiteral>]
        type InlineCompletionContext =
            /// How the completion was triggered.
            abstract triggerKind: InlineCompletionTriggerKind
            abstract selectedSuggestionInfo: SelectedSuggestionInfo option

        [<AllowNullLiteral>]
        type SelectedSuggestionInfo =
            abstract range: IRange
            abstract text: string
            abstract completionKind: CompletionItemKind
            abstract isSnippetText: bool
            abstract equals: other: SelectedSuggestionInfo -> bool

        [<AllowNullLiteral>]
        type SelectedSuggestionInfoStatic =
            [<EmitConstructor>]
            abstract Create:
                range: IRange * text: string * completionKind: CompletionItemKind * isSnippetText: bool ->
                    SelectedSuggestionInfo

        [<AllowNullLiteral>]
        type InlineCompletion =
            /// The text to insert.
            /// If the text contains a line break, the range must end at the end of a line.
            /// If existing text should be replaced, the existing text must be a prefix of the text to insert.
            ///
            /// The text can also be a snippet. In that case, a preview with default parameters is shown.
            /// When accepting the suggestion, the full snippet is inserted.
            abstract insertText: U2<string, {| snippet: string |}>
            /// A text that is used to decide if this inline completion should be shown.
            /// An inline completion is shown if the text to replace is a subword of the filter text.
            abstract filterText: string option
            /// An optional array of additional text edits that are applied when
            /// selecting this completion. Edits must not overlap with the main edit
            /// nor with themselves.
            abstract additionalTextEdits: ResizeArray<Editor.ISingleEditOperation> option
            /// The range to replace.
            /// Must begin and end on the same line.
            abstract range: IRange option
            abstract command: Command option
            /// <summary>
            /// If set to <c>true</c>, unopened closing brackets are removed and unclosed opening brackets are closed.
            /// Defaults to <c>false</c>.
            /// </summary>
            abstract completeBracketPairs: bool option

        type InlineCompletions = InlineCompletions<InlineCompletion>

        [<AllowNullLiteral>]
        type InlineCompletions<'TItem when 'TItem :> InlineCompletion> =
            abstract items: ResizeArray<'TItem>
            /// A list of commands associated with the inline completions of this list.
            abstract commands: ResizeArray<Command> option
            abstract suppressSuggestions: bool option
            /// When set and the user types a suggestion without derivating from it, the inline suggestion is not updated.
            abstract enableForwardStability: bool option

        type InlineCompletionProviderGroupId = string

        type InlineCompletionsProvider = InlineCompletionsProvider<InlineCompletions>

        [<AllowNullLiteral>]
        type InlineCompletionsProvider<'T when 'T :> InlineCompletions> =
            abstract provideInlineCompletions:
                model: Editor.ITextModel *
                position: Position *
                context: InlineCompletionContext *
                token: CancellationToken ->
                    ProviderResult<'T>

            /// <summary>Will be called when an item is shown.</summary>
            /// <param name="updatedInsertText">Is useful to understand bracket completion.</param>
            abstract handleItemDidShow: completions: 'T * item: obj * updatedInsertText: string -> unit
            /// Will be called when an item is partially accepted.
            abstract handlePartialAccept: completions: 'T * item: obj * acceptedCharacters: float -> unit
            /// Will be called when a completions list is no longer in use and can be garbage-collected.
            abstract freeInlineCompletions: completions: 'T -> unit
            /// <summary>
            /// Only used for <see cref="yieldsToGroupIds" />.
            /// Multiple providers can have the same group id.
            /// </summary>
            abstract groupId: InlineCompletionProviderGroupId option with get, set
            /// <summary>
            /// Returns a list of preferred provider <see cref="groupId" />s.
            /// The current provider is only requested for completions if no provider with a preferred group id returned a result.
            /// </summary>
            abstract yieldsToGroupIds: ResizeArray<InlineCompletionProviderGroupId> option with get, set
            abstract toString: unit -> string

        [<AllowNullLiteral>]
        type CodeAction =
            abstract title: string with get, set
            abstract command: Command option with get, set
            abstract edit: WorkspaceEdit option with get, set
            abstract diagnostics: ResizeArray<Editor.IMarkerData> option with get, set
            abstract kind: string option with get, set
            abstract isPreferred: bool option with get, set
            abstract disabled: string option with get, set

        [<RequireQualifiedAccess>]
        type CodeActionTriggerType =
            | Invoke = 1
            | Auto = 2

        [<AllowNullLiteral>]
        type CodeActionList =
            inherit IDisposable
            abstract actions: ReadonlyArray<CodeAction>

        /// Represents a parameter of a callable-signature. A parameter can
        /// have a label and a doc-comment.
        [<AllowNullLiteral>]
        type ParameterInformation =
            /// The label of this signature. Will be shown in
            /// the UI.
            abstract label: U2<string, float * float> with get, set
            /// The human-readable doc-comment of this signature. Will be shown
            /// in the UI but can be omitted.
            abstract documentation: U2<string, IMarkdownString> option with get, set

        /// Represents the signature of something callable. A signature
        /// can have a label, like a function-name, a doc-comment, and
        /// a set of parameters.
        [<AllowNullLiteral>]
        type SignatureInformation =
            /// The label of this signature. Will be shown in
            /// the UI.
            abstract label: string with get, set
            /// The human-readable doc-comment of this signature. Will be shown
            /// in the UI but can be omitted.
            abstract documentation: U2<string, IMarkdownString> option with get, set
            /// The parameters of this signature.
            abstract parameters: ResizeArray<ParameterInformation> with get, set
            /// <summary>
            /// Index of the active parameter.
            ///
            /// If provided, this is used in place of <c>SignatureHelp.activeSignature</c>.
            /// </summary>
            abstract activeParameter: float option with get, set

        /// Signature help represents the signature of something
        /// callable. There can be multiple signatures but only one
        /// active and only one active parameter.
        [<AllowNullLiteral>]
        type SignatureHelp =
            /// One or more signatures.
            abstract signatures: ResizeArray<SignatureInformation> with get, set
            /// The active signature.
            abstract activeSignature: float with get, set
            /// The active parameter of the active signature.
            abstract activeParameter: float with get, set

        [<AllowNullLiteral>]
        type SignatureHelpResult =
            inherit IDisposable
            abstract value: SignatureHelp with get, set

        [<RequireQualifiedAccess>]
        type SignatureHelpTriggerKind =
            | Invoke = 1
            | TriggerCharacter = 2
            | ContentChange = 3

        [<AllowNullLiteral>]
        type SignatureHelpContext =
            abstract triggerKind: SignatureHelpTriggerKind
            abstract triggerCharacter: string option
            abstract isRetrigger: bool
            abstract activeSignatureHelp: SignatureHelp option

        /// <summary>
        /// The signature help provider interface defines the contract between extensions and
        /// the <see href="https://code.visualstudio.com/docs/editor/intellisense">parameter hints</see>-feature.
        /// </summary>
        [<AllowNullLiteral>]
        type SignatureHelpProvider =
            abstract signatureHelpTriggerCharacters: ReadonlyArray<string> option
            abstract signatureHelpRetriggerCharacters: ReadonlyArray<string> option

            /// Provide help for the signature at the given position and document.
            abstract provideSignatureHelp:
                model: Editor.ITextModel * position: Position * token: CancellationToken * context: SignatureHelpContext ->
                    ProviderResult<SignatureHelpResult>

        /// A document highlight kind.
        [<RequireQualifiedAccess>]
        type DocumentHighlightKind =
            /// A textual occurrence.
            | Text = 0
            /// Read-access of a symbol, like reading a variable.
            | Read = 1
            /// Write-access of a symbol, like writing to a variable.
            | Write = 2

        /// A document highlight is a range inside a text document which deserves
        /// special attention. Usually a document highlight is visualized by changing
        /// the background color of its range.
        [<AllowNullLiteral>]
        type DocumentHighlight =
            /// The range this highlight applies to.
            abstract range: IRange with get, set
            /// <summary>The highlight kind, default is <see cref="DocumentHighlightKind.Texttext" />.</summary>
            abstract kind: DocumentHighlightKind option with get, set

        /// The document highlight provider interface defines the contract between extensions and
        /// the word-highlight-feature.
        [<AllowNullLiteral>]
        type DocumentHighlightProvider =
            /// Provide a set of document highlights, like all occurrences of a variable or
            /// all exit-points of a function.
            abstract provideDocumentHighlights:
                model: Editor.ITextModel * position: Position * token: CancellationToken ->
                    ProviderResult<ResizeArray<DocumentHighlight>>

        /// The linked editing range provider interface defines the contract between extensions and
        /// the linked editing feature.
        [<AllowNullLiteral>]
        type LinkedEditingRangeProvider =
            /// Provide a list of ranges that can be edited together.
            abstract provideLinkedEditingRanges:
                model: Editor.ITextModel * position: Position * token: CancellationToken ->
                    ProviderResult<LinkedEditingRanges>

        /// Represents a list of ranges that can be edited together along with a word pattern to describe valid contents.
        [<AllowNullLiteral>]
        type LinkedEditingRanges =
            /// A list of ranges that can be edited together. The ranges must have
            /// identical length and text content. The ranges cannot overlap
            abstract ranges: ResizeArray<IRange> with get, set
            /// An optional word pattern that describes valid contents for the given ranges.
            /// If no pattern is provided, the language configuration's word pattern will be used.
            abstract wordPattern: RegExp option with get, set

        /// Value-object that contains additional information when
        /// requesting references.
        [<AllowNullLiteral>]
        type ReferenceContext =
            /// Include the declaration of the current symbol.
            abstract includeDeclaration: bool with get, set

        /// <summary>
        /// The reference provider interface defines the contract between extensions and
        /// the <see href="https://code.visualstudio.com/docs/editor/editingevolved#_peek">find references</see>-feature.
        /// </summary>
        [<AllowNullLiteral>]
        type ReferenceProvider =
            /// Provide a set of project-wide references for the given position and document.
            abstract provideReferences:
                model: Editor.ITextModel * position: Position * context: ReferenceContext * token: CancellationToken ->
                    ProviderResult<ResizeArray<Location>>

        /// Represents a location inside a resource, such as a line
        /// inside a text file.
        [<AllowNullLiteral>]
        type Location =
            /// The resource identifier of this location.
            abstract uri: Uri with get, set
            /// The document range of this locations.
            abstract range: IRange with get, set

        [<AllowNullLiteral>]
        type LocationLink =
            /// A range to select where this link originates from.
            abstract originSelectionRange: IRange option with get, set
            /// The target uri this link points to.
            abstract uri: Uri with get, set
            /// The full range this link points to.
            abstract range: IRange with get, set
            /// <summary>
            /// A range to select this link points to. Must be contained
            /// in <c>LocationLink.range</c>.
            /// </summary>
            abstract targetSelectionRange: IRange option with get, set

        type Definition = U3<Location, ResizeArray<Location>, ResizeArray<LocationLink>>

        /// <summary>
        /// The definition provider interface defines the contract between extensions and
        /// the <see href="https://code.visualstudio.com/docs/editor/editingevolved#_go-to-definition">go to definition</see>
        /// and peek definition features.
        /// </summary>
        [<AllowNullLiteral>]
        type DefinitionProvider =
            /// Provide the definition of the symbol at the given position and document.
            abstract provideDefinition:
                model: Editor.ITextModel * position: Position * token: CancellationToken ->
                    ProviderResult<U2<Definition, ResizeArray<LocationLink>>>

        /// <summary>
        /// The definition provider interface defines the contract between extensions and
        /// the <see href="https://code.visualstudio.com/docs/editor/editingevolved#_go-to-definition">go to definition</see>
        /// and peek definition features.
        /// </summary>
        [<AllowNullLiteral>]
        type DeclarationProvider =
            /// Provide the declaration of the symbol at the given position and document.
            abstract provideDeclaration:
                model: Editor.ITextModel * position: Position * token: CancellationToken ->
                    ProviderResult<U2<Definition, ResizeArray<LocationLink>>>

        /// The implementation provider interface defines the contract between extensions and
        /// the go to implementation feature.
        [<AllowNullLiteral>]
        type ImplementationProvider =
            /// Provide the implementation of the symbol at the given position and document.
            abstract provideImplementation:
                model: Editor.ITextModel * position: Position * token: CancellationToken ->
                    ProviderResult<U2<Definition, ResizeArray<LocationLink>>>

        /// The type definition provider interface defines the contract between extensions and
        /// the go to type definition feature.
        [<AllowNullLiteral>]
        type TypeDefinitionProvider =
            /// Provide the type definition of the symbol at the given position and document.
            abstract provideTypeDefinition:
                model: Editor.ITextModel * position: Position * token: CancellationToken ->
                    ProviderResult<U2<Definition, ResizeArray<LocationLink>>>

        /// A symbol kind.
        [<RequireQualifiedAccess>]
        type SymbolKind =
            | File = 0
            | Module = 1
            | Namespace = 2
            | Package = 3
            | Class = 4
            | Method = 5
            | Property = 6
            | Field = 7
            | Constructor = 8
            | Enum = 9
            | Interface = 10
            | Function = 11
            | Variable = 12
            | Constant = 13
            | String = 14
            | Number = 15
            | Boolean = 16
            | Array = 17
            | Object = 18
            | Key = 19
            | Null = 20
            | EnumMember = 21
            | Struct = 22
            | Event = 23
            | Operator = 24
            | TypeParameter = 25

        [<RequireQualifiedAccess>]
        type SymbolTag =
            | Deprecated = 1

        [<AllowNullLiteral>]
        type DocumentSymbol =
            abstract name: string with get, set
            abstract detail: string with get, set
            abstract kind: SymbolKind with get, set
            abstract tags: ReadonlyArray<SymbolTag> with get, set
            abstract containerName: string option with get, set
            abstract range: IRange with get, set
            abstract selectionRange: IRange with get, set
            abstract children: ResizeArray<DocumentSymbol> option with get, set

        /// <summary>
        /// The document symbol provider interface defines the contract between extensions and
        /// the <see href="https://code.visualstudio.com/docs/editor/editingevolved#_go-to-symbol">go to symbol</see>-feature.
        /// </summary>
        [<AllowNullLiteral>]
        type DocumentSymbolProvider =
            abstract displayName: string option with get, set

            /// Provide symbol information for the given document.
            abstract provideDocumentSymbols:
                model: Editor.ITextModel * token: CancellationToken -> ProviderResult<ResizeArray<DocumentSymbol>>

        [<AllowNullLiteral>]
        type TextEdit =
            abstract range: IRange with get, set
            abstract text: string with get, set
            abstract eol: Editor.EndOfLineSequence option with get, set

        /// Interface used to format a model
        [<AllowNullLiteral>]
        type FormattingOptions =
            /// Size of a tab in spaces.
            abstract tabSize: float with get, set
            /// Prefer spaces over tabs.
            abstract insertSpaces: bool with get, set

        /// The document formatting provider interface defines the contract between extensions and
        /// the formatting-feature.
        [<AllowNullLiteral>]
        type DocumentFormattingEditProvider =
            abstract displayName: string option

            /// Provide formatting edits for a whole document.
            abstract provideDocumentFormattingEdits:
                model: Editor.ITextModel * options: FormattingOptions * token: CancellationToken ->
                    ProviderResult<ResizeArray<TextEdit>>

        /// The document formatting provider interface defines the contract between extensions and
        /// the formatting-feature.
        [<AllowNullLiteral>]
        type DocumentRangeFormattingEditProvider =
            abstract displayName: string option

            /// Provide formatting edits for a range in a document.
            ///
            /// The given range is a hint and providers can decide to format a smaller
            /// or larger range. Often this is done by adjusting the start and end
            /// of the range to full syntax nodes.
            abstract provideDocumentRangeFormattingEdits:
                model: Editor.ITextModel * range: Range * options: FormattingOptions * token: CancellationToken ->
                    ProviderResult<ResizeArray<TextEdit>>

            abstract provideDocumentRangesFormattingEdits:
                model: Editor.ITextModel *
                ranges: ResizeArray<Range> *
                options: FormattingOptions *
                token: CancellationToken ->
                    ProviderResult<ResizeArray<TextEdit>>

        /// The document formatting provider interface defines the contract between extensions and
        /// the formatting-feature.
        [<AllowNullLiteral>]
        type OnTypeFormattingEditProvider =
            abstract autoFormatTriggerCharacters: ResizeArray<string> with get, set

            /// <summary>
            /// Provide formatting edits after a character has been typed.
            ///
            /// The given position and character should hint to the provider
            /// what range the position to expand to, like find the matching <c>{</c>
            /// when <c>}</c> has been entered.
            /// </summary>
            abstract provideOnTypeFormattingEdits:
                model: Editor.ITextModel *
                position: Position *
                ch: string *
                options: FormattingOptions *
                token: CancellationToken ->
                    ProviderResult<ResizeArray<TextEdit>>

        /// A link inside the editor.
        [<AllowNullLiteral>]
        type ILink =
            abstract range: IRange with get, set
            abstract url: U2<Uri, string> option with get, set
            abstract tooltip: string option with get, set

        [<AllowNullLiteral>]
        type ILinksList =
            abstract links: ResizeArray<ILink> with get, set
            abstract dispose: unit -> unit

        /// A provider of links.
        [<AllowNullLiteral>]
        type LinkProvider =
            abstract provideLinks: model: Editor.ITextModel * token: CancellationToken -> ProviderResult<ILinksList>
            abstract resolveLink: (ILink -> CancellationToken -> ProviderResult<ILink>) option with get, set

        /// A color in RGBA format.
        [<AllowNullLiteral>]
        type IColor =
            /// The red component in the range [0-1].
            abstract red: float
            /// The green component in the range [0-1].
            abstract green: float
            /// The blue component in the range [0-1].
            abstract blue: float
            /// The alpha component in the range [0-1].
            abstract alpha: float

        /// String representations for a color
        [<AllowNullLiteral>]
        type IColorPresentation =
            /// The label of this color presentation. It will be shown on the color
            /// picker header. By default this is also the text that is inserted when selecting
            /// this color presentation.
            abstract label: string with get, set
            /// <summary>
            /// An <see cref="TextEditedit" /> which is applied to a document when selecting
            /// this presentation for the color.
            /// </summary>
            abstract textEdit: TextEdit option with get, set
            /// <summary>
            /// An optional array of additional <see cref="TextEdittext">edits</see> that are applied when
            /// selecting this color presentation.
            /// </summary>
            abstract additionalTextEdits: ResizeArray<TextEdit> option with get, set

        /// A color range is a range in a text model which represents a color.
        [<AllowNullLiteral>]
        type IColorInformation =
            /// The range within the model.
            abstract range: IRange with get, set
            /// The color represented in this range.
            abstract color: IColor with get, set

        /// A provider of colors for editor models.
        [<AllowNullLiteral>]
        type DocumentColorProvider =
            /// Provides the color ranges for a specific model.
            abstract provideDocumentColors:
                model: Editor.ITextModel * token: CancellationToken -> ProviderResult<ResizeArray<IColorInformation>>

            /// Provide the string representations for a color.
            abstract provideColorPresentations:
                model: Editor.ITextModel * colorInfo: IColorInformation * token: CancellationToken ->
                    ProviderResult<ResizeArray<IColorPresentation>>

        [<AllowNullLiteral>]
        type SelectionRange =
            abstract range: IRange with get, set

        [<AllowNullLiteral>]
        type SelectionRangeProvider =
            /// Provide ranges that should be selected from the given position.
            abstract provideSelectionRanges:
                model: Editor.ITextModel * positions: ResizeArray<Position> * token: CancellationToken ->
                    ProviderResult<ResizeArray<ResizeArray<SelectionRange>>>

        [<AllowNullLiteral>]
        type FoldingContext =
            interface
            end

        /// A provider of folding ranges for editor models.
        [<AllowNullLiteral>]
        type FoldingRangeProvider =
            /// An optional event to signal that the folding ranges from this provider have changed.
            abstract onDidChange: IEvent<FoldingRangeProvider> option with get, set

            /// Provides the folding ranges for a specific model.
            abstract provideFoldingRanges:
                model: Editor.ITextModel * context: FoldingContext * token: CancellationToken ->
                    ProviderResult<ResizeArray<FoldingRange>>

        [<AllowNullLiteral>]
        type FoldingRange =
            /// The one-based start line of the range to fold. The folded area starts after the line's last character.
            abstract start: float with get, set
            /// The one-based end line of the range to fold. The folded area ends with the line's last character.
            abstract ``end``: float with get, set
            /// <summary>
            /// Describes the <see cref="FoldingRangeKindKind" /> of the folding range such as <see cref="FoldingRangeKind.CommentComment" /> or
            /// <see cref="FoldingRangeKind.RegionRegion" />. The kind is used to categorize folding ranges and used by commands
            /// like 'Fold all comments'. See
            /// <see cref="FoldingRangeKind" /> for an enumeration of standardized kinds.
            /// </summary>
            abstract kind: FoldingRangeKind option with get, set

        [<AllowNullLiteral>]
        type FoldingRangeKind =
            abstract value: string with get, set

        [<AllowNullLiteral>]
        type FoldingRangeKindStatic =
            /// Kind for folding range representing a comment. The value of the kind is 'comment'.
            abstract Comment: FoldingRangeKind
            /// Kind for folding range representing a import. The value of the kind is 'imports'.
            abstract Imports: FoldingRangeKind
            /// <summary>
            /// Kind for folding range representing regions (for example marked by <c>#region</c>, <c>#endregion</c>).
            /// The value of the kind is 'region'.
            /// </summary>
            abstract Region: FoldingRangeKind
            /// <summary>Returns a <see cref="FoldingRangeKind" /> for the given value.</summary>
            /// <param name="value">of the kind.</param>
            abstract fromValue: value: string -> FoldingRangeKind

            /// <summary>Creates a new <see cref="FoldingRangeKind" />.</summary>
            /// <param name="value">of the kind.</param>
            [<EmitConstructor>]
            abstract Create: value: string -> FoldingRangeKind

        [<AllowNullLiteral>]
        type WorkspaceEditMetadata =
            abstract needsConfirmation: bool with get, set
            abstract label: string with get, set
            abstract description: string option with get, set

        [<AllowNullLiteral>]
        type WorkspaceFileEditOptions =
            abstract overwrite: bool option with get, set
            abstract ignoreIfNotExists: bool option with get, set
            abstract ignoreIfExists: bool option with get, set
            abstract recursive: bool option with get, set
            abstract copy: bool option with get, set
            abstract folder: bool option with get, set
            abstract skipTrashBin: bool option with get, set
            abstract maxSize: float option with get, set

        [<AllowNullLiteral>]
        type IWorkspaceFileEdit =
            abstract oldResource: Uri option with get, set
            abstract newResource: Uri option with get, set
            abstract options: WorkspaceFileEditOptions option with get, set
            abstract metadata: WorkspaceEditMetadata option with get, set

        [<AllowNullLiteral>]
        type IWorkspaceTextEdit =
            abstract resource: Uri with get, set
            abstract textEdit: obj with get, set
            abstract versionId: float option with get, set
            abstract metadata: WorkspaceEditMetadata option with get, set

        [<AllowNullLiteral>]
        type WorkspaceEdit =
            abstract edits: Array<U2<IWorkspaceTextEdit, IWorkspaceFileEdit>> with get, set

        [<AllowNullLiteral>]
        type Rejection =
            abstract rejectReason: string option with get, set

        [<AllowNullLiteral>]
        type RenameLocation =
            abstract range: IRange with get, set
            abstract text: string with get, set

        [<AllowNullLiteral>]
        type RenameProvider =
            abstract provideRenameEdits:
                model: Editor.ITextModel * position: Position * newName: string * token: CancellationToken ->
                    ProviderResult<obj>

            abstract resolveRenameLocation:
                model: Editor.ITextModel * position: Position * token: CancellationToken -> ProviderResult<obj>

        [<AllowNullLiteral>]
        type Command =
            abstract id: string with get, set
            abstract title: string with get, set
            abstract tooltip: string option with get, set
            abstract arguments: ResizeArray<obj option> option with get, set

        [<AllowNullLiteral>]
        type PendingCommentThread =
            abstract body: string with get, set
            abstract range: IRange with get, set
            abstract uri: Uri with get, set
            abstract owner: string with get, set

        [<AllowNullLiteral>]
        type CodeLens =
            abstract range: IRange with get, set
            abstract id: string option with get, set
            abstract command: Command option with get, set

        [<AllowNullLiteral>]
        type CodeLensList =
            abstract lenses: ResizeArray<CodeLens> with get, set
            abstract dispose: unit -> unit

        [<AllowNullLiteral>]
        type CodeLensProvider =
            abstract onDidChange: IEvent<CodeLensProvider> option with get, set

            abstract provideCodeLenses:
                model: Editor.ITextModel * token: CancellationToken -> ProviderResult<CodeLensList>

            abstract resolveCodeLens:
                model: Editor.ITextModel * codeLens: CodeLens * token: CancellationToken -> ProviderResult<CodeLens>

        [<RequireQualifiedAccess>]
        type InlayHintKind =
            | Type = 1
            | Parameter = 2

        [<AllowNullLiteral>]
        type InlayHintLabelPart =
            abstract label: string with get, set
            abstract tooltip: U2<string, IMarkdownString> option with get, set
            abstract command: Command option with get, set
            abstract location: Location option with get, set

        [<AllowNullLiteral>]
        type InlayHint =
            abstract label: U2<string, ResizeArray<InlayHintLabelPart>> with get, set
            abstract tooltip: U2<string, IMarkdownString> option with get, set
            abstract textEdits: ResizeArray<TextEdit> option with get, set
            abstract position: IPosition with get, set
            abstract kind: InlayHintKind option with get, set
            abstract paddingLeft: bool option with get, set
            abstract paddingRight: bool option with get, set

        [<AllowNullLiteral>]
        type InlayHintList =
            abstract hints: ResizeArray<InlayHint> with get, set
            abstract dispose: unit -> unit

        [<AllowNullLiteral>]
        type InlayHintsProvider =
            abstract displayName: string option with get, set
            abstract onDidChangeInlayHints: IEvent<unit> option with get, set

            abstract provideInlayHints:
                model: Editor.ITextModel * range: Range * token: CancellationToken -> ProviderResult<InlayHintList>

            abstract resolveInlayHint: hint: InlayHint * token: CancellationToken -> ProviderResult<InlayHint>

        [<AllowNullLiteral>]
        type SemanticTokensLegend =
            abstract tokenTypes: ResizeArray<string>
            abstract tokenModifiers: ResizeArray<string>

        [<AllowNullLiteral>]
        type SemanticTokens =
            abstract resultId: string option
            abstract data: Uint32Array

        [<AllowNullLiteral>]
        type SemanticTokensEdit =
            abstract start: float
            abstract deleteCount: float
            abstract data: Uint32Array option

        [<AllowNullLiteral>]
        type SemanticTokensEdits =
            abstract resultId: string option
            abstract edits: ResizeArray<SemanticTokensEdit>

        [<AllowNullLiteral>]
        type DocumentSemanticTokensProvider =
            abstract onDidChange: IEvent<unit> option with get, set
            abstract getLegend: unit -> SemanticTokensLegend

            abstract provideDocumentSemanticTokens:
                model: Editor.ITextModel * lastResultId: string option * token: CancellationToken ->
                    ProviderResult<U2<SemanticTokens, SemanticTokensEdits>>

            abstract releaseDocumentSemanticTokens: resultId: string option -> unit

        [<AllowNullLiteral>]
        type DocumentRangeSemanticTokensProvider =
            abstract getLegend: unit -> SemanticTokensLegend

            abstract provideDocumentRangeSemanticTokens:
                model: Editor.ITextModel * range: Range * token: CancellationToken -> ProviderResult<SemanticTokens>

        [<AllowNullLiteral>]
        type DocumentContextItem =
            abstract uri: Uri
            abstract version: float
            abstract ranges: ResizeArray<IRange>

        [<AllowNullLiteral>]
        type MappedEditsContext =
            /// The outer array is sorted by priority - from highest to lowest. The inner arrays contain elements of the same priority.
            abstract documents: ResizeArray<ResizeArray<DocumentContextItem>> with get, set

        [<AllowNullLiteral>]
        type MappedEditsProvider =
            /// <summary>Provider maps code blocks from the chat into a workspace edit.</summary>
            /// <param name="document">The document to provide mapped edits for.</param>
            /// <param name="codeBlocks">
            /// Code blocks that come from an LLM's reply.
            /// "Insert at cursor" in the panel chat only sends one edit that the user clicks on, but inline chat can send multiple blocks and let the lang server decide what to do with them.
            /// </param>
            /// <param name="context">The context for providing mapped edits.</param>
            /// <param name="token">A cancellation token.</param>
            /// <returns>A provider result of text edits.</returns>
            abstract provideMappedEdits:
                document: Editor.ITextModel *
                codeBlocks: ResizeArray<string> *
                context: MappedEditsContext *
                token: CancellationToken ->
                    Promise<WorkspaceEdit option>

        [<AllowNullLiteral>]
        type ILanguageExtensionPoint =
            abstract id: string with get, set
            abstract extensions: ResizeArray<string> option with get, set
            abstract filenames: ResizeArray<string> option with get, set
            abstract filenamePatterns: ResizeArray<string> option with get, set
            abstract firstLine: string option with get, set
            abstract aliases: ResizeArray<string> option with get, set
            abstract mimetypes: ResizeArray<string> option with get, set
            abstract configuration: Uri option with get, set

        /// A Monarch language definition
        [<AllowNullLiteral>]
        type IMonarchLanguage =
            /// map from string to ILanguageRule[]
            abstract tokenizer: IMonarchLanguageTokenizer with get, set
            /// is the language case insensitive?
            abstract ignoreCase: bool option with get, set
            /// is the language unicode-aware? (i.e., /\u{1D306}/)
            abstract unicode: bool option with get, set
            /// if no match in the tokenizer assign this token class (default 'source')
            abstract defaultToken: string option with get, set
            /// for example [['{','}','delimiter.curly']]
            abstract brackets: ResizeArray<IMonarchLanguageBracket> option with get, set
            /// start symbol in the tokenizer (by default the first entry is used)
            abstract start: string option with get, set
            /// attach this to every token class (by default '.' + name)
            abstract tokenPostfix: string option with get, set
            /// include line feeds (in the form of a \n character) at the end of lines
            /// Defaults to false
            abstract includeLF: bool option with get, set

            /// Other keys that can be referred to by the tokenizer.
            [<EmitIndexer>]
            abstract Item: key: string -> obj option with get, set

        /// A rule is either a regular expression and an action
        /// 		shorthands: [reg,act] == { regex: reg, action: act}
        /// 	and       : [reg,act,nxt] == { regex: reg, action: act{ next: nxt }}
        type IShortMonarchLanguageRule1 = U2<string, RegExp> * IMonarchLanguageAction

        type IShortMonarchLanguageRule2 = U2<string, RegExp> * IMonarchLanguageAction * string

        [<AllowNullLiteral>]
        type IExpandedMonarchLanguageRule =
            /// match tokens
            abstract regex: U2<string, RegExp> option with get, set
            /// action to take on match
            abstract action: IMonarchLanguageAction option with get, set
            /// or an include rule. include all rules from the included state
            abstract ``include``: string option with get, set

        type IMonarchLanguageRule =
            U3<IShortMonarchLanguageRule1, IShortMonarchLanguageRule2, IExpandedMonarchLanguageRule>

        /// An action is either an array of actions...
        /// ... or a case statement with guards...
        /// ... or a basic action with a token value.
        type IShortMonarchLanguageAction = string

        [<AllowNullLiteral>]
        type IExpandedMonarchLanguageAction =
            /// array of actions for each parenthesized match group
            abstract group: ResizeArray<IMonarchLanguageAction> option with get, set
            /// map from string to ILanguageAction
            abstract cases: Object option with get, set
            /// token class (ie. css class) (or "@brackets" or "@rematch")
            abstract token: string option with get, set
            /// the next state to push, or "@push", "@pop", "@popall"
            abstract next: string option with get, set
            /// switch to this state
            abstract switchTo: string option with get, set
            /// go back n characters in the stream
            abstract goBack: float option with get, set
            abstract bracket: string option with get, set
            /// switch to embedded language (using the mimetype) or get out using "@pop"
            abstract nextEmbedded: string option with get, set
            /// log a message to the browser console window
            abstract log: string option with get, set

        type IMonarchLanguageAction =
            U3<IShortMonarchLanguageAction, IExpandedMonarchLanguageAction, ResizeArray<U2<IShortMonarchLanguageAction, IExpandedMonarchLanguageAction>>>

        /// This interface can be shortened as an array, ie. ['{','}','delimiter.curly']
        [<AllowNullLiteral>]
        type IMonarchLanguageBracket =
            /// open bracket
            abstract ``open``: string with get, set
            /// closing bracket
            abstract close: string with get, set
            /// token class
            abstract token: string with get, set

        module Css =

            [<AllowNullLiteral>]
            type IExports =
                abstract cssDefaults: LanguageServiceDefaults
                abstract scssDefaults: LanguageServiceDefaults
                abstract lessDefaults: LanguageServiceDefaults

            [<AllowNullLiteral>]
            type CSSFormatConfiguration =
                /// separate selectors with newline (e.g. "a,\nbr" or "a, br"): Default: true
                abstract newlineBetweenSelectors: bool option with get, set
                /// add a new line after every css rule: Default: true
                abstract newlineBetweenRules: bool option with get, set
                /// ensure space around selector separators:  '>', '+', '~' (e.g. "a>b" -> "a > b"): Default: false
                abstract spaceAroundSelectorSeparator: bool option with get, set
                /// <summary>put braces on the same line as rules (<c>collapse</c>), or put braces on own line, Allman / ANSI style (<c>expand</c>). Default <c>collapse</c></summary>
                abstract braceStyle: CSSFormatConfigurationBraceStyle option with get, set
                /// whether existing line breaks before elements should be preserved. Default: true
                abstract preserveNewLines: bool option with get, set
                /// maximum number of line breaks to be preserved in one chunk. Default: unlimited
                abstract maxPreserveNewLines: float option with get, set

            [<AllowNullLiteral>]
            type Options =
                abstract validate: bool option
                abstract lint: OptionsLint option
                /// Configures the CSS data types known by the langauge service.
                abstract data: CSSDataConfiguration option
                /// Settings for the CSS formatter.
                abstract format: CSSFormatConfiguration option

            [<AllowNullLiteral>]
            type ModeConfiguration =
                /// Defines whether the built-in completionItemProvider is enabled.
                abstract completionItems: bool option
                /// Defines whether the built-in hoverProvider is enabled.
                abstract hovers: bool option
                /// Defines whether the built-in documentSymbolProvider is enabled.
                abstract documentSymbols: bool option
                /// Defines whether the built-in definitions provider is enabled.
                abstract definitions: bool option
                /// Defines whether the built-in references provider is enabled.
                abstract references: bool option
                /// Defines whether the built-in references provider is enabled.
                abstract documentHighlights: bool option
                /// Defines whether the built-in rename provider is enabled.
                abstract rename: bool option
                /// Defines whether the built-in color provider is enabled.
                abstract colors: bool option
                /// Defines whether the built-in foldingRange provider is enabled.
                abstract foldingRanges: bool option
                /// Defines whether the built-in diagnostic provider is enabled.
                abstract diagnostics: bool option
                /// Defines whether the built-in selection range provider is enabled.
                abstract selectionRanges: bool option
                /// Defines whether the built-in document formatting edit provider is enabled.
                abstract documentFormattingEdits: bool option
                /// Defines whether the built-in document formatting range edit provider is enabled.
                abstract documentRangeFormattingEdits: bool option

            [<AllowNullLiteral>]
            type LanguageServiceDefaults =
                abstract languageId: string
                abstract onDidChange: IEvent<LanguageServiceDefaults>
                abstract modeConfiguration: ModeConfiguration
                abstract options: Options
                abstract setOptions: options: Options -> unit
                abstract setModeConfiguration: modeConfiguration: ModeConfiguration -> unit

                [<Obsolete("Use options instead")>]
                abstract diagnosticsOptions: DiagnosticsOptions

                [<Obsolete("Use setOptions instead")>]
                abstract setDiagnosticsOptions: options: DiagnosticsOptions -> unit

            [<Obsolete("Use Options instead")>]
            type DiagnosticsOptions = Options

            [<AllowNullLiteral>]
            type CSSDataConfiguration =
                /// Defines whether the standard CSS properties, at-directives, pseudoClasses and pseudoElements are shown.
                abstract useDefaultDataProvider: bool option with get, set
                /// Provides a set of custom data providers.
                abstract dataProviders: CSSDataConfigurationDataProviders option with get, set

            /// <summary>
            /// Custom CSS properties, at-directives, pseudoClasses and pseudoElements
            /// <see href="https://github.com/microsoft/vscode-css-languageservice/blob/main/docs/customData.md" />
            /// </summary>
            [<AllowNullLiteral>]
            type CSSDataV1 =
                abstract version: CSSDataV1Version with get, set
                abstract properties: ResizeArray<IPropertyData> option with get, set
                abstract atDirectives: ResizeArray<IAtDirectiveData> option with get, set
                abstract pseudoClasses: ResizeArray<IPseudoClassData> option with get, set
                abstract pseudoElements: ResizeArray<IPseudoElementData> option with get, set

            [<StringEnum>]
            [<RequireQualifiedAccess>]
            type EntryStatus =
                | Standard
                | Experimental
                | Nonstandard
                | Obsolete

            [<AllowNullLiteral>]
            type IReference =
                abstract name: string with get, set
                abstract url: string with get, set

            [<AllowNullLiteral>]
            type IPropertyData =
                abstract name: string with get, set
                abstract description: U2<string, MarkupContent> option with get, set
                abstract browsers: ResizeArray<string> option with get, set
                abstract restrictions: ResizeArray<string> option with get, set
                abstract status: EntryStatus option with get, set
                abstract syntax: string option with get, set
                abstract values: ResizeArray<IValueData> option with get, set
                abstract references: ResizeArray<IReference> option with get, set
                abstract relevance: float option with get, set

            [<AllowNullLiteral>]
            type IAtDirectiveData =
                abstract name: string with get, set
                abstract description: U2<string, MarkupContent> option with get, set
                abstract browsers: ResizeArray<string> option with get, set
                abstract status: EntryStatus option with get, set
                abstract references: ResizeArray<IReference> option with get, set

            [<AllowNullLiteral>]
            type IPseudoClassData =
                abstract name: string with get, set
                abstract description: U2<string, MarkupContent> option with get, set
                abstract browsers: ResizeArray<string> option with get, set
                abstract status: EntryStatus option with get, set
                abstract references: ResizeArray<IReference> option with get, set

            [<AllowNullLiteral>]
            type IPseudoElementData =
                abstract name: string with get, set
                abstract description: U2<string, MarkupContent> option with get, set
                abstract browsers: ResizeArray<string> option with get, set
                abstract status: EntryStatus option with get, set
                abstract references: ResizeArray<IReference> option with get, set

            [<AllowNullLiteral>]
            type IValueData =
                abstract name: string with get, set
                abstract description: U2<string, MarkupContent> option with get, set
                abstract browsers: ResizeArray<string> option with get, set
                abstract status: EntryStatus option with get, set
                abstract references: ResizeArray<IReference> option with get, set

            [<AllowNullLiteral>]
            type MarkupContent =
                abstract kind: MarkupKind with get, set
                abstract value: string with get, set

            [<StringEnum>]
            [<RequireQualifiedAccess>]
            type MarkupKind =
                | Plaintext
                | Markdown

            [<StringEnum>]
            [<RequireQualifiedAccess>]
            type CSSFormatConfigurationBraceStyle =
                | Collapse
                | Expand

            [<StringEnum>]
            [<RequireQualifiedAccess>]
            type OptionsLintCompatibleVendorPrefixes =
                | Ignore
                | Warning
                | Error

            [<AllowNullLiteral>]
            type OptionsLint =
                abstract compatibleVendorPrefixes: OptionsLintCompatibleVendorPrefixes option
                abstract vendorPrefix: OptionsLintCompatibleVendorPrefixes option
                abstract duplicateProperties: OptionsLintCompatibleVendorPrefixes option
                abstract emptyRules: OptionsLintCompatibleVendorPrefixes option
                abstract importStatement: OptionsLintCompatibleVendorPrefixes option
                abstract boxModel: OptionsLintCompatibleVendorPrefixes option
                abstract universalSelector: OptionsLintCompatibleVendorPrefixes option
                abstract zeroUnits: OptionsLintCompatibleVendorPrefixes option
                abstract fontFaceProperties: OptionsLintCompatibleVendorPrefixes option
                abstract hexColorLength: OptionsLintCompatibleVendorPrefixes option
                abstract argumentsInColorFunction: OptionsLintCompatibleVendorPrefixes option
                abstract unknownProperties: OptionsLintCompatibleVendorPrefixes option
                abstract ieHack: OptionsLintCompatibleVendorPrefixes option
                abstract unknownVendorSpecificProperties: OptionsLintCompatibleVendorPrefixes option
                abstract propertyIgnoredDueToDisplay: OptionsLintCompatibleVendorPrefixes option
                abstract important: OptionsLintCompatibleVendorPrefixes option
                abstract float: OptionsLintCompatibleVendorPrefixes option
                abstract idSelector: OptionsLintCompatibleVendorPrefixes option

            [<AllowNullLiteral>]
            type CSSDataConfigurationDataProviders =
                [<EmitIndexer>]
                abstract Item: providerId: string -> CSSDataV1 with get, set

            [<RequireQualifiedAccess>]
            type CSSDataV1Version =
                interface
                end

        module Html =

            [<AllowNullLiteral>]
            type IExports =
                abstract htmlLanguageService: LanguageServiceRegistration
                abstract htmlDefaults: LanguageServiceDefaults
                abstract handlebarLanguageService: LanguageServiceRegistration
                abstract handlebarDefaults: LanguageServiceDefaults
                abstract razorLanguageService: LanguageServiceRegistration
                abstract razorDefaults: LanguageServiceDefaults

                /// Registers a new HTML language service for the languageId.
                /// Note: 'html', 'handlebar' and 'razor' are registered by default.
                ///
                /// Use this method to register additional language ids with a HTML service.
                /// The language server has to be registered before an editor model is opened.
                abstract registerHTMLLanguageService:
                    languageId: string * ?options: Options * ?modeConfiguration: ModeConfiguration ->
                        LanguageServiceRegistration

            [<AllowNullLiteral>]
            type HTMLFormatConfiguration =
                abstract tabSize: float
                abstract insertSpaces: bool
                abstract wrapLineLength: float
                abstract unformatted: string
                abstract contentUnformatted: string
                abstract indentInnerHtml: bool
                abstract preserveNewLines: bool
                abstract maxPreserveNewLines: float option
                abstract indentHandlebars: bool
                abstract endWithNewline: bool
                abstract extraLiners: string
                abstract wrapAttributes: HTMLFormatConfigurationWrapAttributes

            [<AllowNullLiteral>]
            type CompletionConfiguration =
                [<EmitIndexer>]
                abstract Item: providerId: string -> bool

            [<AllowNullLiteral>]
            type Options =
                /// Settings for the HTML formatter.
                abstract format: HTMLFormatConfiguration option
                /// Code completion settings.
                abstract suggest: CompletionConfiguration option
                /// Configures the HTML data types known by the HTML langauge service.
                abstract data: HTMLDataConfiguration option

            [<AllowNullLiteral>]
            type ModeConfiguration =
                /// Defines whether the built-in completionItemProvider is enabled.
                abstract completionItems: bool option
                /// Defines whether the built-in hoverProvider is enabled.
                abstract hovers: bool option
                /// Defines whether the built-in documentSymbolProvider is enabled.
                abstract documentSymbols: bool option
                /// Defines whether the built-in definitions provider is enabled.
                abstract links: bool option
                /// Defines whether the built-in references provider is enabled.
                abstract documentHighlights: bool option
                /// Defines whether the built-in rename provider is enabled.
                abstract rename: bool option
                /// Defines whether the built-in color provider is enabled.
                abstract colors: bool option
                /// Defines whether the built-in foldingRange provider is enabled.
                abstract foldingRanges: bool option
                /// Defines whether the built-in diagnostic provider is enabled.
                abstract diagnostics: bool option
                /// Defines whether the built-in selection range provider is enabled.
                abstract selectionRanges: bool option
                /// Defines whether the built-in documentFormattingEdit provider is enabled.
                abstract documentFormattingEdits: bool option
                /// Defines whether the built-in documentRangeFormattingEdit provider is enabled.
                abstract documentRangeFormattingEdits: bool option

            [<AllowNullLiteral>]
            type LanguageServiceDefaults =
                abstract languageId: string
                abstract modeConfiguration: ModeConfiguration
                abstract onDidChange: IEvent<LanguageServiceDefaults>
                abstract options: Options
                abstract setOptions: options: Options -> unit
                abstract setModeConfiguration: modeConfiguration: ModeConfiguration -> unit

            [<AllowNullLiteral>]
            type LanguageServiceRegistration =
                inherit IDisposable
                abstract defaults: LanguageServiceDefaults

            [<AllowNullLiteral>]
            type HTMLDataConfiguration =
                /// Defines whether the standard HTML tags and attributes are shown
                abstract useDefaultDataProvider: bool option
                /// Provides a set of custom data providers.
                abstract dataProviders: HTMLDataConfigurationDataProviders option

            /// <summary>
            /// Custom HTML tags attributes and attribute values
            /// <see href="https://github.com/microsoft/vscode-html-languageservice/blob/main/docs/customData.md" />
            /// </summary>
            [<AllowNullLiteral>]
            type HTMLDataV1 =
                abstract version: HTMLDataV1Version
                abstract tags: ResizeArray<ITagData> option
                abstract globalAttributes: ResizeArray<IAttributeData> option
                abstract valueSets: ResizeArray<IValueSet> option

            [<AllowNullLiteral>]
            type IReference =
                abstract name: string
                abstract url: string

            [<AllowNullLiteral>]
            type ITagData =
                abstract name: string
                abstract description: U2<string, MarkupContent> option
                abstract attributes: ResizeArray<IAttributeData>
                abstract references: ResizeArray<IReference> option

            [<AllowNullLiteral>]
            type IAttributeData =
                abstract name: string
                abstract description: U2<string, MarkupContent> option
                abstract valueSet: string option
                abstract values: ResizeArray<IValueData> option
                abstract references: ResizeArray<IReference> option

            [<AllowNullLiteral>]
            type IValueData =
                abstract name: string
                abstract description: U2<string, MarkupContent> option
                abstract references: ResizeArray<IReference> option

            [<AllowNullLiteral>]
            type IValueSet =
                abstract name: string
                abstract values: ResizeArray<IValueData>

            [<AllowNullLiteral>]
            type MarkupContent =
                abstract kind: MarkupKind
                abstract value: string

            [<StringEnum>]
            [<RequireQualifiedAccess>]
            type MarkupKind =
                | Plaintext
                | Markdown

            [<StringEnum>]
            [<RequireQualifiedAccess>]
            type HTMLFormatConfigurationWrapAttributes =
                | Auto
                | Force
                | [<CompiledName("force-aligned")>] ForceAligned
                | [<CompiledName("force-expand-multiline")>] ForceExpandMultiline

            [<AllowNullLiteral>]
            type HTMLDataConfigurationDataProviders =
                [<EmitIndexer>]
                abstract Item: providerId: string -> HTMLDataV1 with get, set

            [<RequireQualifiedAccess>]
            type HTMLDataV1Version =
                interface
                end

        module Json =

            [<AllowNullLiteral>]
            type IExports =
                abstract jsonDefaults: LanguageServiceDefaults

            [<AllowNullLiteral>]
            type DiagnosticsOptions =
                /// <summary>
                /// If set, the validator will be enabled and perform syntax and schema based validation,
                /// unless <c>DiagnosticsOptions.schemaValidation</c> is set to <c>ignore</c>.
                /// </summary>
                abstract validate: bool option
                /// <summary>
                /// If set, comments are tolerated. If set to false, syntax errors will be emitted for comments.
                /// <c>DiagnosticsOptions.allowComments</c> will override this setting.
                /// </summary>
                abstract allowComments: bool option

                /// A list of known schemas and/or associations of schemas to file names.
                abstract schemas:
                    ResizeArray<{|
                        uri: string
                        fileMatch: ResizeArray<string> option
                        schema: obj option
                    |}> option

                /// If set, the schema service would load schema content on-demand with 'fetch' if available
                abstract enableSchemaRequest: bool option
                /// The severity of problems from schema validation. If set to 'ignore', schema validation will be skipped. If not set, 'warning' is used.
                abstract schemaValidation: SeverityLevel option
                /// The severity of problems that occurred when resolving and loading schemas. If set to 'ignore', schema resolving problems are not reported. If not set, 'warning' is used.
                abstract schemaRequest: SeverityLevel option
                /// The severity of reported trailing commas. If not set, trailing commas will be reported as errors.
                abstract trailingCommas: SeverityLevel option
                /// The severity of reported comments. If not set, 'DiagnosticsOptions.allowComments' defines whether comments are ignored or reported as errors.
                abstract comments: SeverityLevel option

            [<StringEnum>]
            [<RequireQualifiedAccess>]
            type SeverityLevel =
                | Error
                | Warning
                | Ignore

            [<AllowNullLiteral>]
            type ModeConfiguration =
                /// Defines whether the built-in documentFormattingEdit provider is enabled.
                abstract documentFormattingEdits: bool option
                /// Defines whether the built-in documentRangeFormattingEdit provider is enabled.
                abstract documentRangeFormattingEdits: bool option
                /// Defines whether the built-in completionItemProvider is enabled.
                abstract completionItems: bool option
                /// Defines whether the built-in hoverProvider is enabled.
                abstract hovers: bool option
                /// Defines whether the built-in documentSymbolProvider is enabled.
                abstract documentSymbols: bool option
                /// Defines whether the built-in tokens provider is enabled.
                abstract tokens: bool option
                /// Defines whether the built-in color provider is enabled.
                abstract colors: bool option
                /// Defines whether the built-in foldingRange provider is enabled.
                abstract foldingRanges: bool option
                /// Defines whether the built-in diagnostic provider is enabled.
                abstract diagnostics: bool option
                /// Defines whether the built-in selection range provider is enabled.
                abstract selectionRanges: bool option

            [<AllowNullLiteral>]
            type LanguageServiceDefaults =
                abstract languageId: string
                abstract onDidChange: IEvent<LanguageServiceDefaults>
                abstract diagnosticsOptions: DiagnosticsOptions
                abstract modeConfiguration: ModeConfiguration
                abstract setDiagnosticsOptions: options: DiagnosticsOptions -> unit
                abstract setModeConfiguration: modeConfiguration: ModeConfiguration -> unit

        module Typescript =

            [<AllowNullLiteral>]
            type IExports =
                abstract typescriptVersion: string
                abstract typescriptDefaults: LanguageServiceDefaults
                abstract javascriptDefaults: LanguageServiceDefaults
                abstract getTypeScriptWorker: unit -> Promise<(ResizeArray<Uri> -> Promise<TypeScriptWorker>)>
                abstract getJavaScriptWorker: unit -> Promise<(ResizeArray<Uri> -> Promise<TypeScriptWorker>)>

            [<RequireQualifiedAccess>]
            type ModuleKind =
                | None = 0
                | CommonJS = 1
                | AMD = 2
                | UMD = 3
                | System = 4
                | ES2015 = 5
                | ESNext = 99

            [<RequireQualifiedAccess>]
            type JsxEmit =
                | None = 0
                | Preserve = 1
                | React = 2
                | ReactNative = 3
                | ReactJSX = 4
                | ReactJSXDev = 5

            [<RequireQualifiedAccess>]
            type NewLineKind =
                | CarriageReturnLineFeed = 0
                | LineFeed = 1

            [<RequireQualifiedAccess>]
            type ScriptTarget =
                | ES3 = 0
                | ES5 = 1
                | ES2015 = 2
                | ES2016 = 3
                | ES2017 = 4
                | ES2018 = 5
                | ES2019 = 6
                | ES2020 = 7
                | ESNext = 99
                | JSON = 100
                | Latest = 99

            [<RequireQualifiedAccess>]
            type ModuleResolutionKind =
                | Classic = 1
                | NodeJs = 2

            [<AllowNullLiteral>]
            type MapLike<'T> =
                [<EmitIndexer>]
                abstract Item: index: string -> 'T with get, set

            type CompilerOptionsValue =
                U6<string, float, bool, ResizeArray<U2<string, float>>, ResizeArray<string>, MapLike<ResizeArray<string>>> option

            [<AllowNullLiteral>]
            type CompilerOptions =
                abstract allowJs: bool option with get, set
                abstract allowSyntheticDefaultImports: bool option with get, set
                abstract allowUmdGlobalAccess: bool option with get, set
                abstract allowUnreachableCode: bool option with get, set
                abstract allowUnusedLabels: bool option with get, set
                abstract alwaysStrict: bool option with get, set
                abstract baseUrl: string option with get, set
                abstract charset: string option with get, set
                abstract checkJs: bool option with get, set
                abstract declaration: bool option with get, set
                abstract declarationMap: bool option with get, set
                abstract emitDeclarationOnly: bool option with get, set
                abstract declarationDir: string option with get, set
                abstract disableSizeLimit: bool option with get, set
                abstract disableSourceOfProjectReferenceRedirect: bool option with get, set
                abstract downlevelIteration: bool option with get, set
                abstract emitBOM: bool option with get, set
                abstract emitDecoratorMetadata: bool option with get, set
                abstract experimentalDecorators: bool option with get, set
                abstract forceConsistentCasingInFileNames: bool option with get, set
                abstract importHelpers: bool option with get, set
                abstract inlineSourceMap: bool option with get, set
                abstract inlineSources: bool option with get, set
                abstract isolatedModules: bool option with get, set
                abstract jsx: JsxEmit option with get, set
                abstract keyofStringsOnly: bool option with get, set
                abstract lib: ResizeArray<string> option with get, set
                abstract locale: string option with get, set
                abstract mapRoot: string option with get, set
                abstract maxNodeModuleJsDepth: float option with get, set
                abstract ``module``: ModuleKind option with get, set
                abstract moduleResolution: ModuleResolutionKind option with get, set
                abstract newLine: NewLineKind option with get, set
                abstract noEmit: bool option with get, set
                abstract noEmitHelpers: bool option with get, set
                abstract noEmitOnError: bool option with get, set
                abstract noErrorTruncation: bool option with get, set
                abstract noFallthroughCasesInSwitch: bool option with get, set
                abstract noImplicitAny: bool option with get, set
                abstract noImplicitReturns: bool option with get, set
                abstract noImplicitThis: bool option with get, set
                abstract noStrictGenericChecks: bool option with get, set
                abstract noUnusedLocals: bool option with get, set
                abstract noUnusedParameters: bool option with get, set
                abstract noImplicitUseStrict: bool option with get, set
                abstract noLib: bool option with get, set
                abstract noResolve: bool option with get, set
                abstract out: string option with get, set
                abstract outDir: string option with get, set
                abstract outFile: string option with get, set
                abstract paths: MapLike<ResizeArray<string>> option with get, set
                abstract preserveConstEnums: bool option with get, set
                abstract preserveSymlinks: bool option with get, set
                abstract project: string option with get, set
                abstract reactNamespace: string option with get, set
                abstract jsxFactory: string option with get, set
                abstract composite: bool option with get, set
                abstract removeComments: bool option with get, set
                abstract rootDir: string option with get, set
                abstract rootDirs: ResizeArray<string> option with get, set
                abstract skipLibCheck: bool option with get, set
                abstract skipDefaultLibCheck: bool option with get, set
                abstract sourceMap: bool option with get, set
                abstract sourceRoot: string option with get, set
                abstract strict: bool option with get, set
                abstract strictFunctionTypes: bool option with get, set
                abstract strictBindCallApply: bool option with get, set
                abstract strictNullChecks: bool option with get, set
                abstract strictPropertyInitialization: bool option with get, set
                abstract stripInternal: bool option with get, set
                abstract suppressExcessPropertyErrors: bool option with get, set
                abstract suppressImplicitAnyIndexErrors: bool option with get, set
                abstract target: ScriptTarget option with get, set
                abstract traceResolution: bool option with get, set
                abstract resolveJsonModule: bool option with get, set
                abstract types: ResizeArray<string> option with get, set
                /// Paths used to compute primary types search locations
                abstract typeRoots: ResizeArray<string> option with get, set
                abstract esModuleInterop: bool option with get, set
                abstract useDefineForClassFields: bool option with get, set

                [<EmitIndexer>]
                abstract Item: option: string -> CompilerOptionsValue option with get, set

            [<AllowNullLiteral>]
            type DiagnosticsOptions =
                abstract noSemanticValidation: bool option with get, set
                abstract noSyntaxValidation: bool option with get, set
                abstract noSuggestionDiagnostics: bool option with get, set
                /// Limit diagnostic computation to only visible files.
                /// Defaults to false.
                abstract onlyVisible: bool option with get, set
                abstract diagnosticCodesToIgnore: ResizeArray<float> option with get, set

            [<AllowNullLiteral>]
            type WorkerOptions =
                /// <summary>A full HTTP path to a JavaScript file which adds a function <c>customTSWorkerFactory</c> to the self inside a web-worker</summary>
                abstract customWorkerPath: string option with get, set

            [<AllowNullLiteral>]
            type InlayHintsOptions =
                abstract includeInlayParameterNameHints: InlayHintsOptionsIncludeInlayParameterNameHints option
                abstract includeInlayParameterNameHintsWhenArgumentMatchesName: bool option
                abstract includeInlayFunctionParameterTypeHints: bool option
                abstract includeInlayVariableTypeHints: bool option
                abstract includeInlayPropertyDeclarationTypeHints: bool option
                abstract includeInlayFunctionLikeReturnTypeHints: bool option
                abstract includeInlayEnumMemberValueHints: bool option

            [<AllowNullLiteral>]
            type IExtraLib =
                abstract content: string with get, set
                abstract version: float with get, set

            [<AllowNullLiteral>]
            type IExtraLibs =
                [<EmitIndexer>]
                abstract Item: path: string -> IExtraLib with get, set

            /// A linked list of formatted diagnostic messages to be used as part of a multiline message.
            /// It is built from the bottom up, leaving the head to be the "main" diagnostic.
            [<AllowNullLiteral>]
            type DiagnosticMessageChain =
                abstract messageText: string with get, set
                /// Diagnostic category: warning = 0, error = 1, suggestion = 2, message = 3
                abstract category: DiagnosticMessageChainCategory with get, set
                abstract code: float with get, set
                abstract next: ResizeArray<DiagnosticMessageChain> option with get, set

            [<AllowNullLiteral>]
            type Diagnostic =
                inherit DiagnosticRelatedInformation
                /// <summary>May store more in future. For now, this will simply be <c>true</c> to indicate when a diagnostic is an unused-identifier diagnostic.</summary>
                abstract reportsUnnecessary: DiagnosticReportsUnnecessary option with get, set
                abstract reportsDeprecated: DiagnosticReportsUnnecessary option with get, set
                abstract source: string option with get, set
                abstract relatedInformation: ResizeArray<DiagnosticRelatedInformation> option with get, set

            [<AllowNullLiteral>]
            type DiagnosticRelatedInformation =
                /// Diagnostic category: warning = 0, error = 1, suggestion = 2, message = 3
                abstract category: DiagnosticMessageChainCategory with get, set
                abstract code: float with get, set
                /// <summary>TypeScriptWorker removes all but the <c>fileName</c> property to avoid serializing circular JSON structures.</summary>
                abstract file: {| fileName: string |} option with get, set
                abstract start: float option with get, set
                abstract length: float option with get, set
                abstract messageText: U2<string, DiagnosticMessageChain> with get, set

            [<AllowNullLiteral>]
            type EmitOutput =
                abstract outputFiles: ResizeArray<OutputFile> with get, set
                abstract emitSkipped: bool with get, set

            [<AllowNullLiteral>]
            type OutputFile =
                abstract name: string with get, set
                abstract writeByteOrderMark: bool with get, set
                abstract text: string with get, set

            [<AllowNullLiteral>]
            type ModeConfiguration =
                /// Defines whether the built-in completionItemProvider is enabled.
                abstract completionItems: bool option
                /// Defines whether the built-in hoverProvider is enabled.
                abstract hovers: bool option
                /// Defines whether the built-in documentSymbolProvider is enabled.
                abstract documentSymbols: bool option
                /// Defines whether the built-in definitions provider is enabled.
                abstract definitions: bool option
                /// Defines whether the built-in references provider is enabled.
                abstract references: bool option
                /// Defines whether the built-in references provider is enabled.
                abstract documentHighlights: bool option
                /// Defines whether the built-in rename provider is enabled.
                abstract rename: bool option
                /// Defines whether the built-in diagnostic provider is enabled.
                abstract diagnostics: bool option
                /// Defines whether the built-in document formatting range edit provider is enabled.
                abstract documentRangeFormattingEdits: bool option
                /// Defines whether the built-in signature help provider is enabled.
                abstract signatureHelp: bool option
                /// Defines whether the built-in onType formatting edit provider is enabled.
                abstract onTypeFormattingEdits: bool option
                /// Defines whether the built-in code actions provider is enabled.
                abstract codeActions: bool option
                /// Defines whether the built-in inlay hints provider is enabled.
                abstract inlayHints: bool option

            [<AllowNullLiteral>]
            type LanguageServiceDefaults =
                /// Event fired when compiler options or diagnostics options are changed.
                abstract onDidChange: IEvent<unit>
                /// Event fired when extra libraries registered with the language service change.
                abstract onDidExtraLibsChange: IEvent<unit>
                abstract workerOptions: WorkerOptions
                abstract inlayHintsOptions: InlayHintsOptions
                abstract modeConfiguration: ModeConfiguration
                abstract setModeConfiguration: modeConfiguration: ModeConfiguration -> unit
                /// Get the current extra libs registered with the language service.
                abstract getExtraLibs: unit -> IExtraLibs
                /// <summary>
                /// Add an additional source file to the language service. Use this
                /// for typescript (definition) files that won't be loaded as editor
                /// documents, like <c>jquery.d.ts</c>.
                /// </summary>
                /// <param name="content">The file content</param>
                /// <param name="filePath">An optional file path</param>
                /// <returns>
                /// A disposable which will remove the file from the
                /// language service upon disposal.
                /// </returns>
                abstract addExtraLib: content: string * ?filePath: string -> IDisposable

                /// <summary>
                /// Remove all existing extra libs and set the additional source
                /// files to the language service. Use this for typescript definition
                /// files that won't be loaded as editor documents, like <c>jquery.d.ts</c>.
                /// </summary>
                /// <param name="libs">An array of entries to register.</param>
                abstract setExtraLibs:
                    libs:
                        ResizeArray<{|
                            content: string
                            filePath: string option
                        |}> ->
                        unit

                /// Get current TypeScript compiler options for the language service.
                abstract getCompilerOptions: unit -> CompilerOptions
                /// Set TypeScript compiler options.
                abstract setCompilerOptions: options: CompilerOptions -> unit
                /// Get the current diagnostics options for the language service.
                abstract getDiagnosticsOptions: unit -> DiagnosticsOptions
                /// Configure whether syntactic and/or semantic validation should
                /// be performed
                abstract setDiagnosticsOptions: options: DiagnosticsOptions -> unit
                /// Configure webworker options
                abstract setWorkerOptions: options: WorkerOptions -> unit
                /// No-op.
                abstract setMaximumWorkerIdleTime: value: float -> unit
                /// Configure if all existing models should be eagerly sync'd
                /// to the worker on start or restart.
                abstract setEagerModelSync: value: bool -> unit
                /// Get the current setting for whether all existing models should be eagerly sync'd
                /// to the worker on start or restart.
                abstract getEagerModelSync: unit -> bool
                /// Configure inlay hints options.
                abstract setInlayHintsOptions: options: InlayHintsOptions -> unit

            [<AllowNullLiteral>]
            type TypeScriptWorker =
                /// Get diagnostic messages for any syntax issues in the given file.
                abstract getSyntacticDiagnostics: fileName: string -> Promise<ResizeArray<Diagnostic>>
                /// Get diagnostic messages for any semantic issues in the given file.
                abstract getSemanticDiagnostics: fileName: string -> Promise<ResizeArray<Diagnostic>>
                /// Get diagnostic messages for any suggestions related to the given file.
                abstract getSuggestionDiagnostics: fileName: string -> Promise<ResizeArray<Diagnostic>>
                /// Get the content of a given file.
                abstract getScriptText: fileName: string -> Promise<string option>
                /// <summary>Get diagnostic messages related to the current compiler options.</summary>
                /// <param name="fileName">Not used</param>
                abstract getCompilerOptionsDiagnostics: fileName: string -> Promise<ResizeArray<Diagnostic>>
                /// <summary>Get code completions for the given file and position.</summary>
                /// <returns><c>Promise&lt;typescript.CompletionInfo | undefined&gt;</c></returns>
                abstract getCompletionsAtPosition: fileName: string * position: float -> Promise<obj option option>

                /// <summary>Get code completion details for the given file, position, and entry.</summary>
                /// <returns><c>Promise&lt;typescript.CompletionEntryDetails | undefined&gt;</c></returns>
                abstract getCompletionEntryDetails:
                    fileName: string * position: float * entry: string -> Promise<obj option option>

                /// <summary>Get signature help items for the item at the given file and position.</summary>
                /// <returns><c>Promise&lt;typescript.SignatureHelpItems | undefined&gt;</c></returns>
                abstract getSignatureHelpItems:
                    fileName: string * position: float * options: obj option -> Promise<obj option option>

                /// <summary>Get quick info for the item at the given position in the file.</summary>
                /// <returns><c>Promise&lt;typescript.QuickInfo | undefined&gt;</c></returns>
                abstract getQuickInfoAtPosition: fileName: string * position: float -> Promise<obj option option>

                abstract getDocumentHighlights:
                    fileName: string * position: float * filesToSearch: ResizeArray<string> ->
                        Promise<ResizeArray<obj option> option>

                /// <summary>Get the definition of the item at the given position in the file.</summary>
                /// <returns><c>Promise&lt;ReadonlyArray&lt;typescript.DefinitionInfo&gt; | undefined&gt;</c></returns>
                abstract getDefinitionAtPosition:
                    fileName: string * position: float -> Promise<ResizeArray<obj option> option>

                /// <summary>Get references to the item at the given position in the file.</summary>
                /// <returns><c>Promise&lt;typescript.ReferenceEntry[] | undefined&gt;</c></returns>
                abstract getReferencesAtPosition:
                    fileName: string * position: float -> Promise<ResizeArray<obj option> option>

                /// <summary>Get outline entries for the item at the given position in the file.</summary>
                /// <returns><c>Promise&lt;typescript.NavigationTree | undefined&gt;</c></returns>
                abstract getNavigationTree: fileName: string -> Promise<obj option option>

                /// <summary>Get changes which should be applied to format the given file.</summary>
                /// <param name="options"><c>typescript.FormatCodeOptions</c></param>
                /// <returns><c>Promise&lt;typescript.TextChange[]&gt;</c></returns>
                abstract getFormattingEditsForDocument:
                    fileName: string * options: obj option -> Promise<ResizeArray<obj option>>

                /// <summary>Get changes which should be applied to format the given range in the file.</summary>
                /// <param name="options"><c>typescript.FormatCodeOptions</c></param>
                /// <returns><c>Promise&lt;typescript.TextChange[]&gt;</c></returns>
                abstract getFormattingEditsForRange:
                    fileName: string * start: float * ``end``: float * options: obj option ->
                        Promise<ResizeArray<obj option>>

                /// <summary>Get formatting changes which should be applied after the given keystroke.</summary>
                /// <param name="options"><c>typescript.FormatCodeOptions</c></param>
                /// <returns><c>Promise&lt;typescript.TextChange[]&gt;</c></returns>
                abstract getFormattingEditsAfterKeystroke:
                    fileName: string * postion: float * ch: string * options: obj option ->
                        Promise<ResizeArray<obj option>>

                /// <summary>Get other occurrences which should be updated when renaming the item at the given file and position.</summary>
                /// <returns><c>Promise&lt;readonly typescript.RenameLocation[] | undefined&gt;</c></returns>
                abstract findRenameLocations:
                    fileName: string *
                    positon: float *
                    findInStrings: bool *
                    findInComments: bool *
                    providePrefixAndSuffixTextForRename: bool ->
                        Promise<ResizeArray<obj option> option>

                /// <summary>Get edits which should be applied to rename the item at the given file and position (or a failure reason).</summary>
                /// <param name="options"><c>typescript.RenameInfoOptions</c></param>
                /// <returns><c>Promise&lt;typescript.RenameInfo&gt;</c></returns>
                abstract getRenameInfo: fileName: string * positon: float * options: obj option -> Promise<obj option>
                /// <summary>Get transpiled output for the given file.</summary>
                /// <returns><c>typescript.EmitOutput</c></returns>
                abstract getEmitOutput: fileName: string -> Promise<EmitOutput>

                /// <summary>Get possible code fixes at the given position in the file.</summary>
                /// <param name="formatOptions"><c>typescript.FormatCodeOptions</c></param>
                /// <returns><c>Promise&lt;ReadonlyArray&lt;typescript.CodeFixAction&gt;&gt;</c></returns>
                abstract getCodeFixesAtPosition:
                    fileName: string *
                    start: float *
                    ``end``: float *
                    errorCodes: ResizeArray<float> *
                    formatOptions: obj option ->
                        Promise<ResizeArray<obj option>>

                /// <summary>Get inlay hints in the range of the file.</summary>
                /// <param name="fileName" />
                /// <returns><c>Promise&lt;typescript.InlayHint[]&gt;</c></returns>
                abstract provideInlayHints:
                    fileName: string * start: float * ``end``: float -> Promise<ResizeArray<obj option>>

            [<StringEnum>]
            [<RequireQualifiedAccess>]
            type InlayHintsOptionsIncludeInlayParameterNameHints =
                | None
                | Literals
                | All

            [<RequireQualifiedAccess>]
            type DiagnosticMessageChainCategory =
                | N0 = 0
                | N1 = 1
                | N2 = 2
                | N3 = 3

            [<AllowNullLiteral>]
            type DiagnosticReportsUnnecessary =
                interface
                end

        [<AllowNullLiteral>]
        type IMonarchLanguageTokenizer =
            [<EmitIndexer>]
            abstract Item: name: string -> ResizeArray<IMonarchLanguageRule> with get, set

    module Worker =

        [<AllowNullLiteral>]
        type IMirrorTextModel =
            abstract version: float

        [<AllowNullLiteral>]
        type IMirrorModel =
            inherit IMirrorTextModel
            abstract uri: Uri
            abstract version: float
            abstract getValue: unit -> string

        type IWorkerContext = IWorkerContext<obj>

        [<AllowNullLiteral>]
        type IWorkerContext<'H> =
            /// A proxy to the main thread host object.
            abstract host: 'H with get, set
            /// Get all available mirror models in this worker.
            abstract getMirrorModels: unit -> ResizeArray<IMirrorModel>

    [<AllowNullLiteral>]
    type IMarkdownStringUris =
        [<EmitIndexer>]
        abstract Item: href: string -> UriComponents with get, set
