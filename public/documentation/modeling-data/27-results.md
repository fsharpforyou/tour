# Modeling success and failure with Result

## What you will learn

`Result` represents either a successful value or an expected, described failure.

```fsharp
type AddToCartError =
    | CustomerInactive
    | BookUnavailable

let addToCart mayOrder inStock =
    if not mayOrder then Error CustomerInactive
    elif not inStock then Error BookUnavailable
    else Ok "book added"
```

The type is `Result<string, AddToCartError>`: `Ok` carries the success type, while `Error` carries the error type. Match both:

```fsharp
match result with
| Ok message -> message
| Error CustomerInactive -> "Contact customer support"
| Error BookUnavailable -> "Choose another book"
```

Use `Option` when absence is enough information. Use `Result` when a caller needs to know why an expected operation failed.

Errors are values here, not thrown control flow. Exceptions still have a place for unexpected runtime failures and are covered later.

Trace the possible calls:

```fsharp
addToCart true true
// Ok "book added"

addToCart false true
// Error CustomerInactive

addToCart true false
// Error BookUnavailable
```

The first failing prerequisite determines the error, and later rules are skipped. That ordering is part of this cart decision.

## Option or Result?

Searching for a subtitle normally needs no reason, so `string option` is enough. Refusing to add an item must tell the caller whether the customer, stock, or quantity caused the refusal, so `Result<CartLine, AddToCartError>` fits.

The two generic positions may differ completely. Success might carry a `CartLine` record while failure carries a union case. A complete pattern match handles both broad outcomes and binds the appropriate payload in each branch.

## Give errors useful structure

```fsharp
type AddToCartError =
    | CustomerInactive
    | BookUnavailable
    | InvalidQuantity of attempted: int
```

The payload gives a caller useful information without forcing domain logic to choose display wording. Prefer structured cases when callers must branch; convert them to prose at the presentation boundary.

Returning `Error` does not unwind the call stack or require `try/with`. It is data, so the caller chooses whether to handle it or pass it along.

## Result does not mean an error was handled

Creating `Error BookUnavailable` records a failure. A caller must still decide
what happens next:

```fsharp
let message =
    match addToCart true 0 1 with
    | Ok quantity -> $"Added %d{quantity}"
    | Error BookUnavailable -> "That book is out of stock"
    | Error CustomerInactive -> "This account cannot order"
    | Error (InvalidQuantity attempted) ->
        $"Quantity %d{attempted} is invalid"
```

The compiler checks that the match accounts for every error case. Adding a new
case can therefore reveal every decision point that needs a policy.

## The two type arguments answer different questions

Read `Result<int, AddToCartError>` as:

- on success, what trustworthy value is now available? the accepted `int`;
- on failure, what information explains refusal? `AddToCartError`.

It is common for a beginner to return `Ok true`. That loses the validated value
the next function needs. Prefer returning the accepted quantity, cart line, or
order so success carries evidence that the checks passed.

If F# reports that it expected `int` but found
`Result<int, AddToCartError>`, the failure branch has not been handled yet.
Pattern matching exposes both paths; the next lessons introduce helpers for
transforming and chaining results.

## Try it

- Trigger every result case.
- Add `InvalidQuantity` carrying the attempted quantity.
- Change the success payload to the accepted quantity.
- Intentionally return a plain quantity from one branch and `Error` from
  another. Read the branch-type mismatch, then wrap success with `Ok`.

## Summary

`Result<'ok,'error>` makes expected success and failure part of a function's visible type. Typed error cases make recovery explicit.
