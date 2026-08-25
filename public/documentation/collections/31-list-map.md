# Transforming lists with map

## What you will learn

`List.map` applies one function to every element and returns the transformed list.

The recursive traversal from the previous lesson can express a specific transformation:

```fsharp
let rec titleLengths titles =
    match titles with
    | [] -> []
    | title :: rest ->
        String.length title :: titleLengths rest
```

If we next need uppercase titles, then catalog labels, the traversal repeats while only the element operation changes. `List.map` names that common pattern.

```fsharp
let titleLength title = title.Length
let lengths = titles |> List.map titleLength
```

The original list remains unchanged. Map preserves element count and order while its output element type may differ.

Its conceptual recursive shape is:

```fsharp
let rec map transform values =
    match values with
    | [] -> []
    | value :: rest ->
        transform value :: map transform rest
```

Use the library's `List.map`; this definition exists to make its behavior visible. The empty input produces empty output. Each non-empty step transforms exactly one head and recursively maps the tail.

```text
List.map : ('a -> 'b) -> 'a list -> 'b list
```

Read it from left to right: give `map` an element transformation, then a list of input elements, and receive a list of outputs. Partial application lets `List.map titleLength` become a whole-list transformation.

Records make the domain example richer:

```fsharp
let label book = $"%s{book.Title} by %s{book.Author}"
let labels = books |> List.map label
```

Use `map` when every input has exactly one output. It is not a loop with a hidden mutable accumulator; it describes a transformation.

## Walk one element through

Given three `Book` records, map calls `label` three times and builds three strings in the same order. The original records are untouched. An empty input produces an empty output list of the inferred result type.

A common error is passing `label book` as map's first argument. That expression calls the function immediately and produces a string. Map needs the function itself: `List.map label books`.

## Read the signature together with the function's contract

When traversal completes normally, `List.map` has applied the supplied function once to each input element, in list order, and produced one output element for each input element. Its generic signature describes the allowed type relationship; the documented behavior supplies the traversal guarantee. The signature alone cannot prove purity or prevent the supplied function from reading external state, performing an effect, or raising an exception.

## Try it

- Map prices to discounted prices.
- Use an anonymous function to uppercase titles.
- Predict the output type before running.

## Summary

Map changes each element through a function, preserving collection structure and making element-to-element intent explicit.
