# Optional values

## What you will learn

`Option` represents a value that may deliberately be absent.

A catalog search may find a book or find nothing. Returning an empty title would blur a real title with absence. F# provides two cases:

```fsharp
Some "Kindred"
None
```

Their type is `string option`, also written `Option<string>`. Match both possibilities before using the inner value:

```fsharp
let describe result =
    match result with
    | Some title -> $"Found %s{title}"
    | None -> "No matching book"
```

`Some` is not a decorative wrapper: a `string option` is a different type from `string`. That distinction records possible absence in the function's type.

`Option` is a generic union. Its type argument says what a present value contains: `int option`, `Book option`, and `string option` share the same present-or-absent structure but are different types.

```fsharp
let subtitle = None
```

This alone can be too ambiguous, so annotate when no `Some` value supplies evidence:

```fsharp
let subtitle: string option = None
```

Option differs from `null`: it is a union with two cases that pattern matching can cover exhaustively. Null still appears at some interop boundaries, and `Some null` is technically possible for a nullable reference type, so Option alone does not validate its inner value. Domain code is clearest when it uses `None` for absence and validates nullable input at the boundary.

## Absence can mean different things

An optional subtitle means a book legitimately may not have one. A catalog lookup returning `None` means no matching book was found. Both need only presence or absence, so both fit `Option`.

If a caller must distinguish malformed input from a missing catalog entry, `None` loses too much information. The next Result lessons attach an explicit reason.

## Follow the wrapper

Suppose `findBook` returns `Book option`. A successful match binds a plain `Book` inside the `Some` branch; the `None` branch has no book to bind. Code after the match receives only the match's common output type.

A frequent beginner error is accessing `result.Title`. The compiler complains because `result` is an option, not a book. This is helpful pressure: first decide what “not found” means for the current operation, then access the value only where its presence is established.

An empty string is not a substitute for `None`. It is still a present string, and its type cannot communicate whether it means “missing,” “invalid,” or genuinely empty.

## Try it

- Change `Some` to `None`.
- Add an optional middle name to a record.
- Try using an option as a plain string and read the mismatch.

## Summary

Options make absence explicit with `Some value` or `None`. Pattern matching handles both paths safely.
