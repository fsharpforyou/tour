# Selecting values with filter

## What you will learn

`List.filter` retains elements for which a predicate returns `true`.

Its recursive shape differs from map at one decision:

```fsharp
let rec filter predicate values =
    match values with
    | [] -> []
    | value :: rest ->
        let filteredRest = filter predicate rest
        if predicate value then
            value :: filteredRest
        else
            filteredRest
```

The element is either prepended unchanged or omitted. The library function packages this recurring traversal.

```fsharp
let isLong book = book.Pages >= 300
let longBooks = books |> List.filter isLong
```

A predicate is a function that returns `bool`. Filter keeps the original values in their original order, though the result may be shorter.

```text
List.filter : ('a -> bool) -> 'a list -> 'a list
```

Unlike `map`, input and output element types are the same. Combine named predicates when the rule matters:

```fsharp
let matches query book = book.Title.Contains(query)
let search query = List.filter (matches query)
```

Here partial application produces a predicate configured with `query`, and then a catalog transformation.

Trace `[ shortBook; longBook ] |> List.filter isLong` one item at a time:

1. `isLong shortBook` returns `false`, so no element is added.
2. `isLong longBook` returns `true`, so that original record is kept.
3. the result is `[ longBook ]`, not a list of boolean answers.

That last distinction separates `filter` from `map`:

```fsharp
books |> List.map isLong
// bool list: one answer for every book

books |> List.filter isLong
// Book list: only books whose answer was true
```

Filter answers “which existing values?” It should not change elements or invent fallback values.

## Predicates are reusable rules

`isLong` can be called with one book, passed to `List.filter`, or combined with another predicate. It knows nothing about lists; filter supplies one element at a time and keeps it when the answer is `true`.

An empty result is a valid list, not `None`: the search operation completed and found zero matching elements. Use `tryFind` in the next lesson when the program specifically wants at most one matching value.

## Filtering answers one kind of question

A report of in-stock books can quietly omit everything else. A failed attempt to add one particular book to a cart needs an explanation, so a typed error fits better. Choose the collection operation from the caller's question, not just from the desired final count.

Filtering an empty list returns an empty list. Filtering with a predicate that
always returns `false` does too. Neither situation is exceptional: the question
was answered and no values qualified.

## Try it

- Filter short books instead.
- Search using a different query.
- Combine two boolean conditions in one predicate.
- Replace `filter` with `map` without changing the predicate. Predict the new
  element type before reading the compiler's inferred type.

## Summary

Filter selects values using a boolean-returning function. Its signature reveals that element type stays unchanged.
