# Sets and unique values

## What you will learn

A set represents unique values when membership matters but duplicates and
positions do not.

## A list can represent the wrong promise

A book may have several genre tags:

```fsharp
let genres = [ "fiction"; "classic"; "fiction" ]
```

The list preserves the duplicate and gives every value a position. Neither fact
has useful meaning for tags. A set states the intended rules directly:

```fsharp
let genres = Set.ofList [ "fiction"; "classic"; "fiction" ]
// set [ "classic"; "fiction" ]
```

The repeated value contributes only one member. Set display order follows the
type's comparison order; insertion order is not part of the abstraction.

## Ask about membership

```fsharp
let isFiction = genres |> Set.contains "fiction"
let tagCount = genres |> Set.count
```

`Set.contains` returns a boolean. Unlike searching a list for a matching
record, there is no separate associated value to return—the member itself is
the fact.

Updates return new sets:

```fsharp
let expanded = genres |> Set.add "award winner"
let reduced = expanded |> Set.remove "classic"
```

`genres` is unchanged. Adding an existing member returns an equivalent set.

## Compare groups

Set operations express relationships that would otherwise require several list
queries:

```fsharp
let customerInterests = Set.ofList [ "fiction"; "history" ]
let bookGenres = Set.ofList [ "classic"; "fiction" ]

let shared = Set.intersect customerInterests bookGenres
let combined = Set.union customerInterests bookGenres
let onlyCustomer = Set.difference customerInterests bookGenres
```

- `intersect` keeps members present in both sets;
- `union` keeps members present in either set;
- `difference` keeps members from the first set that are absent from the second.

These operations return new sets and preserve uniqueness.

## Set, list, or map?

- Use a list when order and repeated values are meaningful.
- Use a set for unique membership and set relationships.
- Use a map when each unique key is associated with another value.

Set elements must support comparison. Strings, numbers, and the domain unions
used later in the course commonly do. Functions do not provide the required
comparison behavior.

## Domain step

The catalog will store a book's genres as `Set<Genre>`. That means one book
cannot carry the same genre twice, and recommendation code can ask whether a
customer's interests intersect the book's genres.

## Experiment

- Add the same genre twice and inspect the count.
- Compute the union and intersection of two genre sets.
- Remove a missing member and compare the result with the original.
- Rewrite a tag list as a set and state which information was deliberately lost.

## Summary

A set models unique membership. Its operations express adding, removing, and
comparing groups without introducing meaningless duplicates or positions.
