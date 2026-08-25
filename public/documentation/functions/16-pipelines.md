# The pipeline operator

## What you will learn

The forward-pipe operator lets a sequence of function calls read in the same direction as the data moves.

## Nested calls become difficult to read

Suppose a catalog title passes through three functions:

```fsharp
let tidyTitle (title: string) = title.Trim()
let addCatalogPrefix title = "Catalog: " + title
let countCharacters (text: string) = text.Length
```

`Trim()` is another string method. It returns text without whitespace at either end; it does not alter the original string.

Normal application is clearest for one step:

```fsharp
let cleaned = tidyTitle "  Kindred  "
```

Nested application works for several steps, but its execution order is visually inside-out:

```fsharp
let count = countCharacters (addCatalogPrefix (tidyTitle "  Kindred  "))
// 16
```

F# provides the forward pipeline operator for the same applications:

```fsharp
let count =
    "  Kindred  "
    |> tidyTitle
    |> addCatalogPrefix
    |> countCharacters
```

Read it top to bottom: start with this value, pass it to this function, then pass the result onward.

## `|>` is still function application

The rule is small:

```fsharp
value |> function
```

means:

```fsharp
function value
```

The pipeline adds no concurrency, mutation, collection behavior, or hidden error handling. It only rearranges a function application so the input appears first.

## Functions with earlier arguments

The piped value becomes the final unapplied argument:

```fsharp
let surround left right text =
    left + text + right

let label =
    "Kindred"
    |> surround "[" "]"
// "[Kindred]"
```

`surround "[" "]"` is partially applied first, producing `string -> string`; the pipeline supplies the title. This is why transformation-oriented APIs often place configuration before data.

## Pipelines are a readability choice

This is unnecessarily ceremonial:

```fsharp
let cleaned = title |> tidyTitle
```

`let cleaned = tidyTitle title` may be more direct. Pipelines earn their space when they expose a meaningful sequence or avoid deeply nested calls.

Named intermediate values are also valuable:

```fsharp
let cleaned = tidyTitle rawTitle
let labelled = addCatalogPrefix cleaned
let count = countCharacters labelled
```

This version is longer but makes intermediate values inspectable. Choose between them based on what a reader needs to understand.

## Debug a pipeline using types

If one stage fails to type-check, write it as normal application:

```fsharp
let cleaned = tidyTitle rawTitle
let labelled = addCatalogPrefix cleaned
```

Hover `cleaned`, then compare its type with the next function's input. A pipeline error usually means one stage produced a type the next stage cannot accept.

## Experiment

- Rewrite the nested catalog expression as a pipeline, then back again.
- Change the stage order and explain why the result changes.
- Partially apply a function so its remaining input fits the pipeline.
- Replace a long pipeline with named bindings and compare readability.

## Summary

A pipeline sends a value through a series of functions. It changes the reading direction, not what the calls mean.
