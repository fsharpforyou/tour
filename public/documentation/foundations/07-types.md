# Types and type inference

## What you will learn

How F# infers types, how to read simple annotations, and how type errors help.

## The compiler has been tracking types all along

F# is statically typed. Every well-typed expression has a type established before it runs. You usually do not write those types because the compiler infers them:

```fsharp
let copyCount = 3       // int
let lateFee = 1.5       // float
let title = "Kindred"  // string
let available = true   // bool
```

Comments after `//` show the inferred types; they are not required code.

The compiler follows how a value is used. In this expression:

```fsharp
let nextCount = copyCount + 1
```

`+` and the integer literal constrain `nextCount` to `int`.

Inference also flows through a chain of bindings:

```fsharp
let maximum = 5
let requested = 2
let remaining = maximum - requested
```

The integer literals and subtraction constrain all three values to `int`. The
next lesson introduces function parameters; after that, the course develops how
inference follows values through function calls as well.

## Explicit annotations

Add an annotation after a name when it clarifies a boundary:

```fsharp
let maximumQuantity: int = 5
let memberDiscount: float = 0.1
```

An annotation supplies an expected type. Modern F# supports a limited set of
type-directed implicit conversions when both the source and destination types
are known. In particular, an `int` can be widened implicitly to `int64`,
`nativeint`, or `float`:

```fsharp
let fee: float = 2
let largeCount: int64 = 2
```

An unsuffixed whole-number literal such as `2` normally has type `int`. Here the
annotations provide known destination types, so the compiler inserts safe
widening conversions. Every `int` value can be represented by `int64`, and every
32-bit integer can be represented exactly by `float`, which is F#'s name for
the double-precision floating-point type.

This is not restricted to literals. An existing `int` value can be widened when
the destination is also known:

```fsharp
let copyCount = 2
let wideCount: int64 = copyCount
let averageCopies: float = copyCount
```

F# does not apply implicit numeric conversion generally. Narrowing conversions,
conversions that may lose information, and many other numeric combinations
remain explicit:

```fsharp
let average = 2.75
let wholeCopies: int = int average
```

The call to `int` makes the potentially lossy conversion visible. Even some
widenings outside F#'s supported implicit set require an explicit conversion.
Treat implicit conversion as a small, type-directed convenience rather than a
rule that all numeric types mix automatically.

Annotate the smallest useful boundary. Writing types on every local value fights inference and adds noise; refusing all annotations can leave overloaded members or public domain boundaries ambiguous.

## Read a diagnostic from the outside in

For this mistake:

```fsharp
let available = 4
let label = if available then "yes" else "no"
```

The compiler expected the `if` condition to be `bool` but found `int`. Start at the construct imposing the expectation (`if`), then inspect the supplied expression (`available`). This technique scales better than reading a long diagnostic as one sentence.

## Reading errors

Many F# errors reduce to “expected one type, received another.” Look for the two type names and then inspect the expression joining them. A message mentioning `string` and `int` often means a string was supplied where arithmetic expected a number.

## In the bookshop

Annotations can document boundaries such as a maximum order quantity. Inference keeps the calculations inside those boundaries uncluttered.

## Try it

- Annotate `title` as `string`.
- Widen the same `int` binding to both `int64` and `float`.
- Try converting a `float` to `int` without calling `int`, read the error, and
  then add the explicit conversion.
- Give `maximumQuantity` a string value and read the error.
- Remove all annotations and verify that the program still works.

## Summary

F# infers types from expressions. Annotations communicate intent; they are not mandatory ceremony.
