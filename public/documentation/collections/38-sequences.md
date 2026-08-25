# Sequences: values produced on demand

## What you will learn

Sequences describe values that are produced as a consumer requests them rather than stored eagerly up front.

## Why another collection abstraction?

Lists and arrays hold a concrete collection in memory. Sometimes a program wants to describe how values can be produced and transform them without immediately constructing the entire result. F# calls that abstraction a sequence, written `seq<'a>`.

```fsharp
let days = seq { 1 .. 5 }
// seq<int>
```

The range syntax `1 .. 5` represents the inclusive integers from one through five. The sequence expression wraps the rule for producing them.

## Transformation is delayed

```fsharp
let doubled =
    days
    |> Seq.map (fun day -> day * 2)
```

`Seq.map` returns another sequence description. Its transformation runs as a consumer asks for elements. This is called *lazy* or *deferred* evaluation.

Converting to a concrete collection forces enumeration:

```fsharp
let values = doubled |> Seq.toList
// [ 2; 4; 6; 8; 10 ]
```

Printing a sequence with `%A` may show its representation instead of its elements. Convert it to a concrete collection when you want to inspect every value.

## Enumeration may repeat work

Consuming a transformed sequence twice may run its production logic twice. The `seq<'a>` abstraction provides enumeration, not a general caching guarantee. Convert the values to a list or array when repeated traversal must reuse the same computed results.

```fsharp
let labels =
    books
    |> Seq.map (fun book -> book.Title.ToUpper())

let firstPass = labels |> Seq.toList
let secondPass = labels |> Seq.toList
```

For pure transformations this repeats computation without changing meaning. For effectful generation it may repeat effects, another reason to keep sequence transformations pure and materialize them when repeated stable data is required.

## Sequence expressions

`yield` emits one element from a sequence expression:

```fsharp
let dispatchDays =
    seq {
        yield 7
        yield 14
        yield 21
    }
```

A `for` inside a sequence expression describes produced values rather than imperatively updating an accumulator:

```fsharp
let squares =
    seq {
        for number in 1 .. 5 do
            yield number * number
    }
```

The expression as a whole produces `seq<int>`. The controlled-mutation lesson later contrasts this with imperative loops whose purpose is an effect.

## Choose the simplest suitable collection

- Use a list for a small, repeatedly traversed immutable domain collection.
- Use an array for direct indexed access or array-based interoperation.
- Use a sequence when delayed production or a shared enumeration abstraction materially helps.

Choose `seq` when delayed production helps. Laziness introduces an evaluation question that a concrete collection does not have.

## Experiment

- Build a sequence for days 1 through 10 and retain the even days.
- Map a formatting function and convert the result to a list.
- Enumerate the same sequence twice.
- Rewrite a `seq { for ... yield ... }` expression using `Seq.map`.

## Summary

A sequence is a recipe for producing values as they are requested. Sequence transformations compose recipes; conversion or another consumer performs the enumeration.
