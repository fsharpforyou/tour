# Choosing and partitioning values

## What you will learn

Use `List.choose` when one input may produce one output or no output, and use
`List.partition` when both sides of a predicate matter.

## A filter and a transformation often travel together

Suppose the catalog contains optional staff recommendations:

```fsharp
type Book =
    {
        Title: string
        StaffNote: string option
    }
```

We want labels only for books that have notes. `List.map` would produce one
result for every book, including `None` values. `List.filter` could select the
books first, but we would then have to inspect the option again to obtain the
text.

Instead, write one function that returns an optional output:

```fsharp
let recommendedLabel book =
    match book.StaffNote with
    | Some note -> Some (book.Title + ": " + note)
    | None -> None
```

`List.choose` applies that function to every element, discards every `None`,
and unwraps every `Some`:

```fsharp
let recommendations =
    books |> List.choose recommendedLabel
```

Its signature explains the relationship:

```text
List.choose : ('a -> 'b option) -> 'a list -> 'b list
```

Each source `'a` gets a chance to produce one `'b`. Absence means that source
contributes nothing to the result.

## Trace one input at a time

For three functions results:

```text
Some "Kindred: Staff pick"
None
Some "Dune: Epic science fiction"
```

`List.choose` produces:

```text
[ "Kindred: Staff pick"; "Dune: Epic science fiction" ]
```

It preserves the relative order of the retained outputs. It does not retain an
explanation for discarded values. If rejection reasons matter, keep explicit
`Result` values or accumulate errors instead.

## Keep both sides with partition

`List.filter predicate` keeps values for which the predicate returns `true`.
Sometimes the rejected values are equally important. `List.partition` returns
both groups as a tuple:

```fsharp
let inStock, soldOut =
    books |> List.partition (fun book -> book.Copies > 0)
```

Its result has type:

```text
Book list * Book list
```

The first list contains the `true` side and the second contains the `false`
side. Relative order is preserved inside each list.

Choose the operation from the question:

- `filter`: which values satisfy this rule?
- `choose`: which optional outputs were produced?
- `partition`: what are both sides of this rule?

## Compiler clinic

The chooser must return an option. Returning a plain string in one branch and
`None` in the other fails because the branches have different types. Wrap the
present string in `Some` so both branches produce `string option`.

## Experiment

- Predict which recommendation labels remain before running.
- Add a book with no note and confirm the output is unchanged.
- Partition the books by stock and inspect both lists.
- Rewrite the chooser as `Option.map` over `book.StaffNote`.

## Summary

`choose` combines optional transformation with collection processing.
`partition` preserves both sides of a predicate. Their return types state what
information the caller keeps and what it discards.
