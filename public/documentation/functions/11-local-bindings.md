# Local bindings and multi-step functions

## What you will learn

A function can name intermediate results without turning its calculation into mutable steps.

## Making a calculation readable

This works, but asks the reader to untangle everything at once:

```fsharp
let checkoutTotal price quantity discount =
    price * float quantity * (1.0 - discount)
```

Bindings indented inside the function are local to that call:

```fsharp
let checkoutTotal price quantity discount =
    let subtotal = price * float quantity
    let discountAmount = subtotal * discount
    subtotal - discountAmount
```

Each `let` names a value. Nothing is reassigned. The last expression is the result, so this function still has type:

```text
float -> int -> float -> float
```

`subtotal` and `discountAmount` cannot be used outside the function. Their small scope is useful: those names explain the calculation exactly where they matter.

## Branches can be local values too

Because `if` is an expression, it fits naturally on the right side of a binding:

```fsharp
let loanDays isChildrensBook =
    let standardDays = 21
    let allowedDays = if isChildrensBook then 14 else standardDays
    allowedDays
```

The final `allowedDays` could be replaced by the `if`, but the name may make the business rule clearer.

## Scope follows indentation

```fsharp
let calculateFine daysLate =
    let dailyRate = 0.25
    float daysLate * dailyRate
```

`dailyRate` is available only inside `calculateFine`. A later function cannot accidentally depend on it. Narrow scope reduces the number of meanings a reader must keep in mind.

Bindings are evaluated in order, so a local value can use an earlier local value but not one declared below it. This is a flow of dependencies, not a sequence of assignments.

## Give names to ideas, not punctuation

Names should explain something. `let convertedDays = float daysLate` may help when conversion matters; `let one = 1` usually adds ceremony. Prefer the version whose intermediate values reveal domain steps such as subtotal, discount, and final charge.

## Try it

- Add a local `tax` value to the total.
- Return `subtotal` temporarily and observe the result.
- Move a local binding outside and notice how its scope changes.

## Summary

Local bindings split a function into named expressions. They improve clarity while preserving immutability and a simple input-to-output shape.
