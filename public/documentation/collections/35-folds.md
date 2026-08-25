# Folding collections

## What you will learn

`List.fold` combines a list into one accumulated result.

You have now seen several recursive functions that carry a changing result through a list. Fold makes that state explicit and lets the caller provide the update rule.

```fsharp
let addPages total book = total + book.Pages
let totalPages = books |> List.fold addPages 0
```

Fold receives an accumulator function, an initial state, and a list:

```text
('state -> 'item -> 'state) -> 'state -> 'item list -> 'state
```

Starting with `0`, it passes state and the first book to `addPages`, then uses that result with the next book, until the list is exhausted.

For page counts `[ 100; 250; 80 ]`, the states are:

| Step | Accumulator | Item | New accumulator |
|---|---:|---:|---:|
| Start | `0` | — | `0` |
| 1 | `0` | `100` | `100` |
| 2 | `100` | `250` | `350` |
| 3 | `350` | `80` | `430` |

The final accumulator, `430`, is the fold's result.

The state need not be numeric. It can be a record summary:

```fsharp
type Summary = { Books: int; Pages: int }
```

## You rarely need to write `fold` directly

Many common folds already have names in the collection API. Prefer those names
when they describe the question:

```fsharp
let pageCounts = books |> List.map (fun book -> book.Pages)
let totalPages = pageCounts |> List.sum
let longest = books |> List.maxBy (fun book -> book.Pages)
```

Other specific operations include `map`, `filter`, `choose`, `exists`,
`forall`, `length`, `sumBy`, `minBy`, and `maxBy`. They reveal the shape of the
operation immediately and reduce the amount of accumulator code a reader must
verify. For example, this:

```fsharp
let totalPages = books |> List.sumBy (fun book -> book.Pages)
```

states the calculation more directly than:

```fsharp
let totalPages =
    books |> List.fold (fun total book -> total + book.Pages) 0
```

Use `fold` directly when no more specific operation captures the job—especially
when one pass must build a custom state such as `Summary`, coordinate several
related totals, or carry domain state from one item to the next.

## `reduce` is a narrower kind of fold

`List.reduce` also combines a list into one value, but it uses the first element
as the initial accumulator:

```fsharp
let add left right = left + right
let total = [ 100; 250; 80 ] |> List.reduce add
```

Consequently, the accumulator and element must have the same type, and reducing
an empty list raises an exception because there is no first element. `fold`
receives an explicit initial state, can handle an empty list, and may accumulate
a type different from the element type.

Prefer `sum` for a sum. Use `reduce` only when combining a known non-empty
collection with no natural separate initial value. Use `fold` for genuinely
custom accumulation.

`List.foldBack` processes from the right and places the list before the initial state. This matters when the combining operation is order-sensitive:

```fsharp
let labels = [ "A"; "B"; "C" ]

let fromLeft =
    labels |> List.fold (fun text label -> $"(%s{text}+%s{label})") "start"
// "(((start+A)+B)+C)"

let fromRight =
    List.foldBack (fun label text -> $"(%s{label}+%s{text})") labels "end"
// "(A+(B+(C+end)))"
```

Pay attention to both direction and argument order. Use `foldBack` when right-associated construction matches the problem; `fold` remains clearer for totals and state that moves forward.

The conceptual recursive implementation of left fold is:

```fsharp
let rec fold folder state values =
    match values with
    | [] -> state
    | value :: rest ->
        let nextState = folder state value
        fold folder nextState rest
```

Each recursive call receives the updated state. Unlike map, fold does not preserve the input's list structure.

## Trace the state

For books with 100 and 250 pages, a page-count fold starts at zero. The first call receives `0` and the first book, returning `100`; the second receives `100` and the second book, returning `350`. That last state is the result.

Choosing the initial state is part of the design. Addition starts at zero, multiplication at one, and a record summary starts with zeroed fields. An incorrect initial value systematically biases every result.

## Try it

- Multiply a list of numbers starting at one.
- Accumulate both book count and page count.
- Rewrite a page-total fold with `List.sumBy`.
- Compare `List.reduce add []` with `List.fold add 0 []`. Predict what happens
  before running either expression.
- Change the initial state and predict the effect.
- Trace the left and right folds above before running them.

## Summary

Fold threads explicit state through every element. Most common folds have more
specific names; use `fold` directly when custom accumulated state is the point.
