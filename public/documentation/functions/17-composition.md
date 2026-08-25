# Function composition

## What you will learn

Composition connects compatible functions now to create a new function for values supplied later.

## A pipeline needs a value now

This pipeline performs work on one title:

```fsharp
let label =
    title
    |> tidyTitle
    |> addCatalogPrefix
```

Sometimes the title will arrive later, but we still want to prepare the transformation now. The forward composition operator `>>` connects the functions:

```fsharp
let prepareCatalogTitle =
    tidyTitle >> addCatalogPrefix

let first = prepareCatalogTitle "  Kindred  "
let second = prepareCatalogTitle "  Dune  "
```

Composition produces a new function without running either input function yet. When the new function is called, it runs `tidyTitle` and feeds that result into `addCatalogPrefix`.

## Types are the joints

Suppose:

```text
tidyTitle       : string -> string
countCharacters : string -> int
```

Then:

```fsharp
let tidyAndCount = tidyTitle >> countCharacters
// string -> int
```

The output type of the first function must be compatible with the input type of the second. Composition cannot connect `string -> int` to a function expecting `bool`.

The general shape is:

```text
('a -> 'b) -> ('b -> 'c) -> ('a -> 'c)
```

Start with an `'a`, let the first function produce `'b`, let the second consume that `'b` and produce `'c`, and the composed function connects `'a` directly to `'c`.

## Backward composition

`<<` writes the outer function first:

```fsharp
let prepareCatalogTitle =
    addCatalogPrefix << tidyTitle
```

It still runs `tidyTitle` first. Forward composition usually matches F#'s left-to-right pipeline style; backward composition can be helpful when reading a definition as conventional nested application. Prefer one direction consistently within a short expression.

## Composition versus piping

```fsharp
let output = input |> first |> second
```

passes a value now.

```fsharp
let combined = first >> second
```

constructs behavior to call later.

That distinction matters more than the operators' visual similarity.

## Prefer clarity over point-free style

This is concise:

```fsharp
let prepare = tidyTitle >> normalizeCase >> addCatalogPrefix
```

It is readable only when each named step tells a clear story. When business decisions, branching, or several inputs become involved, explicit parameters and local bindings usually communicate more:

```fsharp
let prepare title =
    let cleaned = tidyTitle title
    let normalized = normalizeCase cleaned
    addCatalogPrefix normalized
```

F# expertise is not measured by how many parameters can be hidden.

## Experiment

- Compose `string -> string` with `string -> int` and predict the final signature.
- Attempt to compose incompatible functions and identify the mismatched boundary.
- Rewrite a composition as a pipeline and as nested application.
- Decide which of three versions best explains the catalog operation.

## Summary

Piping supplies a value; composition builds a function. In both cases, the output type of one step must fit the input type of the next.
