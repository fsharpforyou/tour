# Conditional expressions

## What you will learn

How an `if` expression chooses which expression to evaluate.

## A decision produces a result

In F#, an `if` expression selects one branch and evaluates that branch’s expression to produce a value:

```fsharp
let availabilityLabel =
    if availableCopies > 0 then
        "Available"
    else
        "Unavailable"
```

The condition after `if` must be a `bool`. If it is true, the expression after `then` becomes the result. Otherwise the expression after `else` does.

F# has no general “truthy” conversion for conditions. An integer or string is
not silently treated as a boolean:

```fsharp
if availableCopies then "yes" else "no"
```

This is rejected because `availableCopies` is `int`, not `bool`. State the
actual question instead: `availableCopies > 0`.

Both branches must produce compatible types. This is invalid:

```fsharp
if availableCopies > 0 then
    "Available"
else
    0
```

The surrounding program needs to know the type of the whole `if`, so one branch cannot produce a `string` while the other produces an `int`.

The condition is evaluated first, but only the selected branch is evaluated.
This matters when one branch contains an operation that is valid only for that
case:

```fsharp
let firstLetter =
    if title = "" then
        '?'
    else
        title[0]
```

For an empty title, the indexing expression is never evaluated. This does not
make indexing generally safe; the protective check and the guarded operation
must remain connected.

Because the conditional itself is a value-producing expression, it can appear wherever an expression is expected:

```fsharp
let loanDays = if isReferenceBook then 7 else 21
let dueDay = checkoutDay + (if isHolidayWeek then 7 else 0)
```

The parentheses in the second example group the conditional as the right operand of `+`. Usually a named binding like `extensionDays` would be easier to read.

## Indentation is syntax

F# uses indentation to group code. The two branch expressions are indented beneath `if` and `else`. Consistent four-space indentation makes the structure visible without braces.

## Several conditions

Use `elif` for another condition:

```fsharp
let stockLabel =
    if copies = 0 then
        "Out of stock"
    elif copies = 1 then
        "Last copy"
    else
        "In stock"
```

Later, pattern matching will express decisions based on the *shape* of richer data. `if` remains excellent for boolean conditions.

An `elif` chain chooses the first true condition. Put narrower cases before
broader ones:

```fsharp
if copies = 0 then
    "Out of stock"
elif copies < 5 then
    "Low stock"
else
    "In stock"
```

If the `copies < 5` branch came first, zero would be classified merely as low
stock and the exact zero branch would never be reached.

## An `if` without `else`

F# permits an omitted `else` only when the `then` branch returns `unit`, commonly for an effect:

```fsharp
if availableCopies = 0 then
    printfn "%s" "No copies available"
```

The missing branch produces `()`. When an `if` calculates a value, write both branches so its result is clear.

## In the bookshop

We turn the stock calculation into a message a customer can understand, without creating a mutable temporary variable.

## Try it

- Test zero, one, and several copies.
- Add a branch for five or more copies.
- Deliberately return an integer from one branch and inspect the error.
- Reorder overlapping conditions and explain the changed result.
- Guard a string index with an empty-string check.
- Try using an integer directly as a condition and find the two types in the error.

## Summary

`if` is an expression that produces a value. Its branches agree on a result type.
