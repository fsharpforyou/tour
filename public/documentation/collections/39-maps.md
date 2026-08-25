# Maps and keyed lookup

## What you will learn

Maps associate each unique key with one value.

```fsharp
let catalog = Map.ofList [ (1, "Kindred"); (2, "Dune") ]
let found = catalog |> Map.tryFind 2
```

The type is `Map<int, string>`. `Map.tryFind` makes both lookup outcomes explicit:

```fsharp
catalog |> Map.tryFind 1 // Some "Kindred"
catalog |> Map.tryFind 9 // None
```

`Map.tryFind` returns an option because a key may be absent. `Map.add key value` returns a new map. Keys must support comparison.

Adding an existing key replaces its associated value in the returned map while the old map remains unchanged:

```fsharp
let revised = catalog |> Map.add 2 "Dune Messiah"
```

`Map.remove key` returns a map without that association. `Map.empty` constructs an empty map when surrounding type context identifies its key and value types.

The type has two parameters: `Map<int, string>` associates integer keys with
string values. Every key appears at most once. Constructing a map from repeated
keys keeps the value associated with the last occurrence, so duplicated keys at
an input boundary may deserve validation rather than silent replacement.

## Updates return new collections

`catalog |> Map.add id book` returns a map containing the new association. If the key already exists, the returned map associates it with the new value; the old map remains unchanged.

`Map.tryFind` represents a missing key with `None`, unlike `Map.find`, which raises when the key is absent. A domain workflow can translate `None` into a meaningful `BookNotFound` result. The collection provides lookup behavior; the domain layer supplies the error vocabulary.

`Map.containsKey` answers only whether a key exists. `Map.change` can calculate
an updated optional association from the current one:

```fsharp
let removeIfSoldOut id catalog =
    catalog
    |> Map.change id (fun current ->
        match current with
        | Some book when book.Copies = 0 -> None
        | other -> other)
```

Returning `None` removes the key; returning `Some value` stores that value.
Use `change` when the update depends on whether an association already exists.

## Choose by meaning

- A reservation list preserves arrival order.
- A catalog map expresses unique keyed lookup.
- A list of search results preserves ordering and may contain several matches.
- A record has a fixed collection of named fields known when its type is defined.

Map keys must support comparison. Strings, numbers, tuples, records, and unions
commonly qualify when their contents do. Functions do not. A later lesson
examines equality and comparison directly.

Convert a map to a list only when a list-shaped operation is genuinely needed:

```fsharp
let books =
    catalog
    |> Map.toList
    |> List.map (fun (_, book) -> book)
```

The tuple contains each key and its value. Discarding the key is deliberate
here because the `Book` value already contains its identifier.

## Try it

- Add and remove a catalog entry.
- Search for a missing key.
- Add an existing key and inspect old and new maps.
- Use `Map.change` to remove only a sold-out book.

## Summary

Use a map when unique keys identify associated values. Lookup keeps absence
explicit, and updates return a new map rather than changing the existing one.
