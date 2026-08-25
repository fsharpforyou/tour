# Arrays: indexed collections

## What you will learn

Arrays provide ordered, homogeneous data with direct indexed access and array-specific transformations.

## A different collection shape

Lists are excellent for immutable head-to-tail processing. Some problems instead need efficient indexed access or must interoperate with APIs that naturally use contiguous indexed collections. F# arrays provide that shape.

An array literal uses `[|` and `|]`:

```fsharp
let shelf = [| "Kindred"; "Dune"; "Earthsea" |]
```

The inferred type is `string array`. As with lists, every element has one element type.

## Indexes and length

```fsharp
let first = shelf[0]
let second = shelf[1]
let count = shelf.Length
```

Indexes begin at zero. A three-element array has valid indexes `0`, `1`, and `2`. Accessing an invalid index is a runtime error because `string array` records the element type, not a statically known length.

Use indexing when position is part of the problem. A book ID deserves an explicit lookup structure instead of a fragile array position.

## Array transformations

Arrays have their own module functions:

```fsharp
let titleLengths =
    shelf
    |> Array.map String.length
// [| 7; 4; 8 |]
```

`Array.map` returns a new array and leaves `shelf` unchanged. Its type has the same shape as `List.map`, with `array` replacing `list`:

```text
('a -> 'b) -> 'a array -> 'b array
```

`Array.filter`, `Array.choose`, and folds likewise mirror concepts already learned. The module name tells you which collection representation is returned.

`Array.mapi` additionally supplies each zero-based index:

```fsharp
let numbered =
    shelf
    |> Array.mapi (fun index title -> $"%d{index + 1}. %s{title}")
```

It still returns a new array. Receiving an index does not imply mutation.

## Arrays can be mutated—but not yet

Arrays permit element assignment, but mutation changes the reasoning model and deserves its own focused lesson after objects and interfaces. For now, use transformations that return new arrays. This lets us compare collection representations without mixing in state changes.

## List or array?

Choose from the job the collection needs to do:

- Prefer lists for immutable domain collections and recursive head-tail processing.
- Prefer arrays for indexed access, array-oriented APIs, or a later deliberately local mutation boundary.
- Convert explicitly with `List.toArray` and `Array.toList` when the representation genuinely needs to change.

Conversions allocate another collection. Rather than switching representations just to call a familiar function, learn the corresponding operation for the collection you already have.

## Compiler and runtime clinic

Passing an array to `List.map` is a compile-time type mismatch: `Book array` is not `Book list`. Accessing `shelf[99]` is different—the type is valid, but the position does not exist at runtime. Static types prevent element-type confusion, not every invalid numeric index.

## Experiment

- Create an array of three copy counts and map each to a doubled count.
- Use `Array.mapi` to produce human-friendly shelf numbers beginning at one.
- Convert the array to a list and inspect the resulting type.
- Deliberately use an invalid index, then remove it after observing the runtime failure.

## Summary

Arrays are ordered, homogeneous collections whose positions can be accessed directly. Their functional transformations return arrays just as list transformations return lists.
