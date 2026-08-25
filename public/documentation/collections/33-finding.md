# Finding values in lists

## What you will learn

Collection searches use options so “not found” stays explicit.

```fsharp
let found = books |> List.tryFind (fun book -> book.Title = query)
```

`List.tryFind` stops at the first match and returns `Some book`, or `None`. Its type connects predicates, lists, and options:

```text
('a -> bool) -> 'a list -> 'a option
```

Follow the returned option instead of inventing a sentinel:

```fsharp
books |> List.tryFind (fun book -> book.Id = 2)
// Some matchingBook

books |> List.tryFind (fun book -> book.Id = 99)
// None
```

The first match wins. If several matches are meaningful, use `List.filter`; if duplicate IDs should be impossible, use a map keyed by ID later.

`List.exists predicate` answers only whether any value matches. `List.forall predicate` checks whether every value matches.

```fsharp
let anyAvailable = books |> List.exists (fun book -> book.Available)
let allAvailable = books |> List.forall (fun book -> book.Available)
```

For an empty list, `exists` is false and `forall` is true: no element disproves the claim that every element satisfies the predicate. Check that this convention matches the question your domain is asking.

## Choose the question first

Use `exists` for “does any available copy exist?” because the matching copy itself is irrelevant. Use `tryFind` for “give me the first available copy.” Use `filter` for “give me all available copies.” These similar-looking functions encode different questions in their return types.

Avoid a made-up fallback record when search fails. `None` preserves the truth that no catalog value was found and forces the caller to decide whether to display a message, try another source, or stop.

`forall` asks a different question from “find every matching value.” It can
finish as soon as one value disproves the predicate, and it returns only a
Boolean. If the caller needs the offending books, use `filter` with the inverse
predicate instead.

## Trace the empty-list cases

`List.exists predicate []` is `false` because there is no matching element.
`List.forall predicate []` is `true` because there is no element that disproves
the predicate. This is sometimes called vacuous truth, but the practical point
is simpler: check whether that behavior answers the domain question you meant
to ask. “Are all cart lines valid?” may need a separate non-empty-cart rule.

## Try it

- Search for a missing title.
- Use `exists` to ask whether any copy is available.
- Use `forall` to ask whether every book is available.
- Compare all three operations on an empty catalog.
- Replace `tryFind` with `filter` and compare their result types.

## Summary

`exists` answers whether any value matches, `forall` checks every value, and
`tryFind` returns the first matching value as an option. Choose the return type
that preserves exactly what the caller needs.
