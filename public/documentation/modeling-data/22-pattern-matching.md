# Pattern matching: reasoning about shapes

## What you will learn

`match` chooses an expression by deconstructing a value according to its shape.

```fsharp
let describe format =
    match format with
    | Hardcover -> "hardcover"
    | Paperback -> "paperback"
    | Ebook -> "ebook"
    | Audiobook minutes -> $"audiobook, %d{minutes} minutes"
```

Each line begins with a pattern. `Audiobook minutes` both recognizes the case and binds its payload. The expression after `->` becomes the match result.

For `describe (Audiobook 615)`, the first three patterns fail and the fourth succeeds. Within that branch, `minutes` is bound to `615`. Matching reads and unpacks the value; it never changes it.

Matches are expressions, so every branch must produce a compatible type. The compiler warns when a case is missing—valuable feedback when a domain grows.

## Constants, wildcards, and guards

Patterns can also recognize constants:

```fsharp
let stockLabel quantity =
    match quantity with
    | 0 -> "sold out"
    | 1 -> "last copy"
    | _ -> "in stock"
```

The wildcard `_` accepts any remaining value without naming it. A guard adds a condition:

```fsharp
let stockLabel quantity =
    match quantity with
    | quantity when quantity < 0 -> "invalid stock"
    | 0 -> "sold out"
    | 1 -> "last copy"
    | _ -> "in stock"
```

Guards run only after their pattern matches. Order matters because the first matching branch wins. Keep a wildcard last because it matches anything.

## Patterns are not assignments

In `Audiobook minutes`, the name receives data because the existing value has the `Audiobook` shape. Constant patterns such as `0` test equality; case patterns test and unpack a union; tuple patterns unpack positions.

Prefer listing meaningful union cases over a wildcard. Add a new format and the compiler can then point to every decision that needs reconsideration.

## `function` matches its argument immediately

When a function immediately matches its only input, F# offers a shorthand:

```fsharp
let formatLabel =
    function
    | Hardcover -> "hardcover"
    | Paperback -> "paperback"
    | Ebook -> "digital"
    | Audiobook _ -> "audio"
```

This means the same as `let formatLabel format = match format with ...`. Use it when the matched input is obvious and the shorter form remains readable.

## Try it

- Delete a format branch and inspect the warning.
- Add a guarded description for audiobooks longer than 600 minutes.
- Match a tuple using `(title, pages)`.
- Rewrite a one-argument match with `function`, then rewrite it back.

## Summary

Pattern matching recognizes and deconstructs data. Exhaustiveness checking turns domain changes into useful compiler guidance.
