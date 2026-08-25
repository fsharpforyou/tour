# Unit, effects, and functions that do things

## What you will learn

Some functions calculate a useful value. Others perform an observable action
and return `unit` to say that there is no useful result to pass onward.

## A function result is not always interesting data

The sale-price function from the previous lesson returns a number:

```fsharp
let calculateSalePrice price =
    price * 0.9
// float -> float
```

`printfn` behaves differently. Its important outcome is that text appears in
the output pane:

```fsharp
printfn "%s" "Kindred"
```

After printing, the expression produces `()`. This value has type `unit`.
There is exactly one ordinary `unit` value, so it carries no choice or domain
information. It lets an expression fit into F#'s type system even when its
purpose is an effect such as displaying text.

```fsharp
let printed = printfn "%s" "Kindred"
// printed : unit
// printed = ()
```

An *effect* is an observable interaction beyond returning a value. Printing is
an effect. Later examples include mutation and exceptions. F# permits effects;
functional style asks us to keep them visible instead of pretending they are
ordinary calculations.

## Functions can return unit

```fsharp
let announce title =
    printfn "Featured book: %s" title
// string -> unit
```

The arrow still means “input to output.” `announce` accepts a string and
returns `unit`. Calling it performs the print:

```fsharp
announce "Kindred"
```

Compare that with a function that only calculates:

```fsharp
let makeLabel title =
    "Featured book: " + title
// string -> string
```

`makeLabel` is easier to reuse because it does not decide where the string goes.
The caller can print it, store it, or combine it with other text. A common
design is to calculate first and perform the effect at the edge:

```fsharp
let label = makeLabel "Kindred"
printfn "%s" label
```

## A unit input means “no information required”

A function may require no meaningful input but still need an explicit call:

```fsharp
let printDivider () =
    printfn "%s" "----------------"
// unit -> unit
```

Call it with the unit value:

```fsharp
printDivider ()
```

Without `()`, `printDivider` refers to the function value; it does not call the
function. This is the same distinction as referring to `calculateSalePrice`
without supplying a price.

## Several effects run in order

Indented expressions in a function body are evaluated from top to bottom:

```fsharp
let showBook title =
    printDivider ()
    printfn "%s" title
    printDivider ()
```

Each of the first two expressions returns `unit`, and evaluation continues. The
final expression is also unit-producing, so `showBook` has type
`string -> unit`.

A non-final expression in such a sequence is normally expected to return
`unit`. Accidentally discarding a useful value often produces a warning,
because it may mean a calculation was forgotten.

## Experiment

- Predict the order of three printed lines before running them.
- Remove `()` from a `printDivider` call and inspect the type error.
- Change `makeLabel` without changing the printing code.
- Bind the result of `announce "Dune"` and inspect its type.

## Summary

`unit` represents the absence of a useful result. A unit-returning function can
perform an effect, while a unit input makes a no-information call explicit.
Separating calculations from effects usually leaves both easier to understand.
