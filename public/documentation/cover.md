# F# Language Tour

Hello there! Curious about this thing called F# and maybe even interested in learning it? You've come to the right place!

F# is a versatile, multi-paradigm programming language with a functional-first approach, known for its strong and static typing. It seamlessly runs on Microsoft's `.NET` runtime, offering a powerful integration with the .NET ecosystem. Additionally, F# extends its reach beyond the .NET platform, supporting compilation to JavaScript, TypeScript, Python, and [more](https://fable.io/docs/#available-targets), making it a truly cross-platform language with broad applicability. It's a language that values being *succinct*, *correct*, and *performant*. Now, what does any of that mean?
- Succinct in this case means the ability to convey ideas using
  relatively little code. If you've used Python before, F#'s syntax
  should seem somewhat familiar
- Correct meaning that with the use of F#'s strong type system, one
  can make entire classes of bugs non-applicable
  - For instance, using `Option` types to denote optional values
    reduces the presence of nulls
  - And, using `Result` types reduces the presence of exceptions by
    requiring you to handle errors explicitly
- Multi-paradigm means that it supports imperative, object-oriented,
  and functional coding styles
- Functional-first means that it defaults to functional programming,
  using simple, immutable pieces of data being passed around by
  functions that take inputs and return output, without side effects
- Being a `.NET` language, F# has access to a rich standard library
  and extensive ecosystem on top of a battle-tested, high-performance
  runtime

F# is particularly good for web applications, machine learning, and data science. It's also great for interactive development, both via
REPL and notebooks.

This tour covers all aspects of the F# language, and assuming you have some prior programming experience should teach you everything you need to write real programs in F#. If at any point you get stuck or have a question do not hesitate to ask in the
[F# Discord server](https://discord.gg/fsharp-196693847965696000).

If this sounds appealing to you, then go on ahead and let's get started!