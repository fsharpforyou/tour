# Recursive domain data and deeper recursion

## What you will learn

Recursive functions follow recursive data toward a base case, including domain trees rather than only lists.

## From recursive lists to recursive domains

The list lesson introduced recursion over `[]` and `head :: tail`. Recursion becomes indispensable when the domain itself can contain more values of the same shape.

A bookshop category may contain child categories:

```fsharp
type Category =
    | Category of name: string * children: Category list
```

This is a recursive discriminated union: `Category` contains a list whose elements are also `Category` values.

```fsharp
let catalog =
    Category (
        "Bookshop",
        [
            Category ("Fiction", [ Category ("Fantasy", []) ])
            Category ("Non-fiction", [])
        ]
    )
```

The value forms a tree. The root has two children; Fiction has one child; the other nodes are leaves with empty child lists.

## Traverse one level, then recurse

```fsharp
let rec countCategories category =
    match category with
    | Category (_, children) ->
        let add total value = total + value

        let childCounts =
            children |> List.map countCategories

        1 + (childCounts |> List.fold add 0)
```

The current category contributes one. `List.map countCategories` recursively calculates each child's tree size, and fold sums those results.

It is idiomatic to mix explicit recursion with collection functions. Recursion handles the custom tree, while `map` and `fold` handle each list of children.

Sometimes each input produces a list and those lists should become one flat list. `List.collect` combines mapping and concatenation:

```fsharp
let rec categoryNames category =
    match category with
    | Category (name, children) ->
        name :: (children |> List.collect categoryNames)
```

For every child, `categoryNames` returns `string list`; `collect` concatenates those child lists into one. Its shape is:

```text
('a -> 'b list) -> 'a list -> 'b list
```

Use `map` when each item produces one result and `collect` when each item produces zero or more results that should be flattened.

## Base cases may be implicit in the data

There is only one union case, but a node with `children = []` is effectively a leaf. Mapping an empty list produces an empty list and folding it from zero produces zero, so the result is one for the current leaf.

A base case need not have its own union case. What matters is that some input finishes without another recursive call.

## Tail recursion and accumulators

The simple list count from lesson 30 writes:

```fsharp
1 + count rest
```

The addition must wait for the recursive result. An accumulator can carry completed work so the recursive call is the final operation:

```fsharp
let count values =
    let rec loop total remaining =
        match remaining with
        | [] -> total
        | _ :: rest -> loop (total + 1) rest

    loop 0 values
```

This is tail recursion: the recursive call is the branch's final operation, and the accumulator already contains the work completed so far. Small functions do not need to be rewritten mechanically: collection functions usually express list processing better, and rewriting a tree traversal in tail-recursive form can require a different algorithm.

## Mutually recursive definitions

Occasionally two functions call one another. F# joins their definitions with `and`:

```fsharp
let rec describeCategory category =
    match category with
    | Category (name, children) ->
        name + describeChildren children

and describeChildren children =
    match children with
    | [] -> ""
    | first :: rest ->
        " > " + describeCategory first + describeChildren rest
```

This syntax is useful when the problem genuinely has two recursive roles. A single local recursive helper is usually simpler when it fits.

## Compiler and reasoning clinic

`let rec` permits self-reference but does not prove termination. Before running, identify:

1. the input that stops;
2. the smaller input used by every recursive call;
3. how recursive results combine.

If one of those is unclear, the function deserves another design pass.

## Experiment

- Add several nested categories and predict the total count.
- Write `maximumDepth` for the category tree.
- Rewrite list counting with an accumulator and trace its state.
- On paper, trace what would happen if the function recurred on the same category. Do not run that non-terminating version in the browser; explain why the type checker cannot reject it.

## Summary

Recursion follows data that contains smaller values of its own shape. Collection combinators handle standard list recursion; explicit recursive functions remain essential for custom trees and other recursive domains.
