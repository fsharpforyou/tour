# Lists and their recursive shape

## What you will learn

An F# list is an immutable recursive collection built from an empty case and a head joined to a tail.

## More than one book

The bookshop now needs to hold several books of the same type. An F# list is an ordered, immutable collection:

```fsharp
let titles = [ "Kindred"; "Dune"; "Earthsea" ]
// string list
```

Semicolons separate elements; commas would construct tuple values. Every element has the same element type, so a string cannot be inserted into `int list`.

The empty list needs type context because it contains no element from which to infer a type:

```fsharp
let noTitles: string list = []
```

## Constructing without changing

The `::` operator prepends one element:

```fsharp
let expanded = "Beloved" :: titles
```

`expanded` has four values; `titles` still has three. Prepending is efficient because the new list can refer to the unchanged original list as its tail.

Use `@` to concatenate two complete lists:

```fsharp
let combined = titles @ [ "Beloved"; "Parable of the Sower" ]
```

Concatenation traverses its left list, so repeated appending is not the natural way to build a list one item at a time. Prefer prepending while accumulating, then reverse when order matters—or use a more suitable representation.

## Lists have two possible shapes

Every F# list is either:

```text
[]
```

or:

```text
firstElement :: remainingElements
```

Pattern matching follows those exact shapes:

```fsharp
let describeFirst values =
    match values with
    | [] -> "The catalog is empty"
    | first :: rest ->
        $"First: %s{first}; remaining: %d{List.length rest}"
```

In the non-empty branch, `first` is one string and `rest` is another `string list`. The wildcard can ignore a part:

```fsharp
| first :: _ -> first
```

## The shape suggests recursion

Suppose the standard library did not provide `List.length`. A function can process the head and call itself with the smaller tail:

```fsharp
let rec count values =
    match values with
    | [] -> 0
    | _ :: rest -> 1 + count rest
```

`rec` permits the function to refer to itself. The empty-list branch is the base case. The non-empty branch makes progress by passing `rest`, which is one element shorter.

Trace `[ "Kindred"; "Dune" ]`:

```text
count [ "Kindred"; "Dune" ]
= 1 + count [ "Dune" ]
= 1 + (1 + count [])
= 1 + (1 + 0)
= 2
```

This small recursive function matters because the next lessons reveal that `map`, `filter`, and `fold` capture recurring traversals over the same empty-or-head-and-tail structure.

## Compiler and runtime clinic

If the match omits `[]`, the compiler warns that the function does not handle every possible list. If the recursive call receives `values` instead of `rest`, the type still checks but the function never moves toward its base case. Types verify shapes, not termination.

Avoid unsafe head access when a list may be empty. Pattern matching makes both possibilities visible, and later `try` functions represent missing values with `Option`.

## Experiment

- Prepend a title and verify the original list remains unchanged.
- Match lists containing zero, one, and several values.
- Trace `count` by hand with three titles.
- Write `totalPages` recursively for a list of integers.
- On paper, replace `rest` with the unchanged list and explain why evaluation would never reach the base case. Do not run that non-terminating version in the browser.

## Summary

A list is an immutable recursive data structure: empty or one head plus another list. That shape explains both pattern matching and the collection transformations that follow.
