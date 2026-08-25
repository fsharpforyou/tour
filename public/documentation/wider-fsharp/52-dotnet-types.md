# F# in the .NET type world

## What you will learn

F# uses the .NET type system directly and works naturally with .NET types, members, namespaces, and generic APIs.

## F# names are .NET types

Several familiar F# type names are aliases for types from the .NET Base Class Library:

```text
int    = System.Int32
float  = System.Double
bool   = System.Boolean
char   = System.Char
string = System.String
```

This is not a conversion table between two unrelated type systems. An F# value of type `int` is a `System.Int32` value. F# supplies concise names and its own syntax while using the same underlying .NET type system.

The F# language and FSharp.Core make types such as lists, options, results, tuples, and F# function values central to everyday code. They sit alongside Base Class Library types instead of replacing them.

The shared type system is what matters in practice. F# can call a class written in another .NET language, and those languages can call suitably exposed F# types. Their source syntax differs, but they work with the same familiar categories of types and members.

## Instance members

A value can expose properties and methods defined by its .NET type:

```fsharp
let title = "  Kindred  "
let cleaned = title.Trim()
let characterCount = cleaned.Length
let beginsWithK = cleaned.StartsWith("K")
```

`Length` is a property and is read without call parentheses. `Trim()` and `StartsWith(...)` are methods. The value before the dot is the object on which the member operates.

Strings are immutable. `Trim()` returns another string; it does not alter `title`.

## Static members

A static member belongs to a type rather than one existing instance:

```fsharp
let parsed = System.Int32.TryParse("42")
```

`TryParse` starts with a string and attempts to produce an integer, so it is exposed by `System.Int32` itself. In its convenient F# call form, the boolean success flag and parsed value are returned as a tuple.

```fsharp
let parseCount (text: string) =
    match System.Int32.TryParse(text) with
    | true, value -> Some value
    | false, _ -> None
```

The wrapper translates a conventional .NET “try” API into the `Option` vocabulary used by the surrounding F# code. Parsing and domain validation remain different: `"-3"` is a valid integer representation even if negative inventory is invalid business data.

## Namespaces and `open`

The fully qualified name `System.Int32` identifies the `Int32` type inside the `System` namespace. Opening that namespace shortens later references:

```fsharp
open System

let parsed = Int32.TryParse("42")
```

`open` changes how names are resolved in the current scope. It does not construct an object, load a package, or copy definitions.

A namespace and an F# module have different jobs:

- a namespace organizes types and modules across a .NET codebase;
- a module can contain types, values, and functions;
- `open` can bring names from either into scope;
- qualification remains useful when it makes ownership clearer.

## Members and module functions side by side

F# code commonly mixes .NET members with FSharp.Core module functions:

```fsharp
text.Trim()                 // instance method
System.Int32.TryParse(text) // static method
String.length text          // F# module function
String.concat ", " titles   // F# module function
```

All four are normal F# expressions. Their call shapes reflect where the operation is defined, not a hierarchy of quality. Choose the form that states the operation clearly.

This answers a common question about strings. `System.String` exposes members such as `.Trim()`, `.Contains(...)`, and `.Length`; the F# `String` module supplies helpers such as `String.length`, `String.concat`, and `String.replicate`. You will meet both styles throughout .NET and F# code.

## Overloads sometimes need help

.NET methods may be overloaded: one member name can have several parameter lists. F# usually infers the intended overload, but a small annotation can settle an ambiguous boundary:

```fsharp
let parseCount (text: string) =
    System.Int32.TryParse(text)
```

The annotation is not a retreat from type inference. It documents the external boundary and tells overload resolution which input type is intended.

## Try it

- Use both `title.Length` and `String.length title`.
- Parse valid, invalid, and negative integer text.
- Open `System` and shorten `System.Int32` to `Int32`.
- Identify the property, instance methods, static method, and module functions in the examples.

## Summary

F# is a .NET language with direct access to .NET types and members. F# aliases, FSharp.Core types, Base Class Library APIs, namespaces, modules, and object-oriented members form one practical programming model.
