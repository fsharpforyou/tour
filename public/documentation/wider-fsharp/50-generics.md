# Generic functions and types

## What you will learn

Generic code preserves relationships between types without fixing those types in advance.

Higher-order functions introduced generic type variables. We can now move from reading generic signatures to designing generic functions and data types.

The simplest generic function is inferred without an annotation:

```fsharp
let keep value = value
// 'a -> 'a
```

The same input type must be the same output type, whether one call uses a book and another uses a number.

```fsharp
keep 42       // int
keep "Dune" // string
```

These are separate uses of one generalized definition. Within each call, input and output still agree exactly.

Generic domain containers name reusable structure:

```fsharp
type Page<'item> = { Items: 'item list; Number: int }
type Attempt<'value, 'error> = Succeeded of 'value | Failed of 'error
```

`Page<Book>` and `Page<Customer>` are distinct concrete types built from one definition.

The definition says nothing about what an item means. It only says that a page contains a list of one item type and a page number—the part that every paged result genuinely shares.

```fsharp
let transformPage transform page =
    { Items = page.Items |> List.map transform; Number = page.Number }
```

Its type relates input item, output item, and the function between them. Let inference generalize naturally; explicit generic annotations are most helpful in public designs or explanations.

Constraints appear when an operation needs a capability such as comparison. `Set<'a>` and `Map<'key,'value>` require comparable elements or keys.

```fsharp
let contains value values =
    values |> Set.contains value
```

The inferred type includes a comparison constraint because a set must order its values internally. Start with the operations the function needs and let inference reveal any required constraints.

## Generality is inferred, not declared for sport

`keep` uses no type-specific operation, so the compiler safely generalizes it. If the body used `value.Length`, the input would need a type with that member and would no longer be freely generic. The implementation determines the signature.

Generic code should capture something genuinely shared. `Page<'item>` is useful because pagination works the same way for books and customers. Replacing meaningful domain types with `'a` only for the sake of abstraction would make the program harder to read.

## Generic code rarely needs reflection

The compiler checks each concrete use of a generic definition. Functions such as `map` rely on supplied functions and statically checked relationships; they do not need to inspect what `'a` happens to be at runtime.

## Read a larger signature

```text
transformPage : ('a -> 'b) -> Page<'a> -> Page<'b>
```

The first input converts one item. The second input is a page of source items. The result keeps the page structure but contains transformed items. Naming `'a` and `'b` differently communicates that the element type may change.

## Try it

- Use `keep` with two unrelated types.
- Create `Page<string>` and map lengths.
- Inspect how the compiler relates the type variables.

## Summary

Generic parameters stand for types while preserving exact relationships. This is why functional building blocks remain reusable and type-safe.
