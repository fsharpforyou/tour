# Validation as domain logic

## What you will learn

Validation functions convert untrusted primitive values into explicit success or failure.

```fsharp
type ValidationError = EmptyName | NameTooLong of maximum: int

let validateName (name: string) =
    let cleaned = name.Trim()
    if cleaned = "" then Error EmptyName
    elif cleaned.Length > 40 then Error (NameTooLong 40)
    else Ok cleaned
```

The successful value is cleaned and ready for the next step in this workflow. Because it is still a plain string, its type alone does not prove that every string came through this function; a later lesson introduces wrapper types and private construction when that guarantee matters. The error is structured data, so a caller can choose its own wording.

Trace the boundary with several inputs:

```fsharp
validateName "  Ada  "
// Ok "Ada"

validateName "   "
// Error EmptyName
```

Normalization happens first, so whitespace-only input becomes empty and successful names never retain accidental surrounding spaces.

Validation belongs at the boundary where loose values become trusted domain values. A boolean such as `isValidName` loses both the cleaned result and the reason for failure.

Keep one rule per small function when that improves clarity. We will soon combine fallible steps without deeply nested matches.

Validation order is observable because this function returns its first error. Check fundamental rules first. If an interface eventually needs every independent error at once, it will need a collection-based design after collections have been introduced.

## Validate at a useful boundary

Trimming a customer name every time it is displayed scatters the same rule throughout the program. Clean it once during construction and return the useful normalized value, not just `Ok true`.

A blank name is expected input that the program can reject and explain; it is not an exceptional runtime failure. Representing it as data leaves the caller free to show a message, retry, or stop.

## A boolean loses the successful value

```fsharp
let isValidName name = name.Trim() <> ""
```

This answers yes or no but discards both the cleaned name and the reason for rejection. Good validation should make the next function easier to write, not just block bad input.

## Reading errors

If one branch returns `Error EmptyName` and another returns a plain string, the compiler reports incompatible branch types. Every branch must return the same `Result` shape.

## Try it

- Add a minimum length rule.
- Return the cleaned value and prove spaces were removed.
- Write validation for a positive cart quantity.

## Summary

Validation can produce a trustworthy value or a precise domain error. This gives later functions simpler assumptions.
