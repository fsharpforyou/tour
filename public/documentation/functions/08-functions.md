# Your first functions

## What you will learn

A function gives a name to a calculation that can be repeated with different input.

## From one calculation to a reusable rule

We can calculate one sale price with values and expressions:

```fsharp
let price = 12.0
let salePrice = price * 0.9
```

A shop needs the same ten-percent rule for many prices. Put the varying value after the function's name:

```fsharp
let calculateSalePrice price =
    price * 0.9
```

`price` is a *parameter*. The indented expression is the function body. A function produces the value of its final expression; there is normally no `return` keyword.

Call, or *apply*, the function by putting an argument after it:

```fsharp
let salePrice = calculateSalePrice 12.0
```

This is how F# applies a function. Unlike several other languages, F# does not use parentheses as call punctuation. Parentheses group an expression when grouping is needed:

```fsharp
let result = calculateSalePrice (10.0 + 2.0)
```

The compiler infers `calculateSalePrice : float -> float`: it accepts a `float` and produces a `float`. Read the arrow as “to.”

```fsharp
let first = calculateSalePrice 10.0
let second = calculateSalePrice 25.0
// 9.0 and 22.5
```

Each call evaluates the function with the supplied argument. This function keeps no memory of earlier calls.

## Functions are values, and effects are functions too

A function definition is still a `let` binding: the name is bound to a function value. Applying it does not print automatically; `printfn` is a separate function with the observable effect of displaying text.

`calculateSalePrice` is *pure*: for the same price it always returns the same result and changes nothing elsewhere. F# is not a purely functional language, but pure calculations are especially easy to reason about.

## Parentheses group an argument

```fsharp
calculateSalePrice 10.0 + 2.0
```

means “calculate the sale price, then add two.” By contrast, `calculateSalePrice (10.0 + 2.0)` adds first and passes twelve. Function application binds more tightly than arithmetic.

## The next missing piece

A reusable discount really needs both a rate and a price:

```fsharp
let discountAmount rate price = price * rate
```

That function has two inputs, which the next lesson examines carefully.

## Try it

- Predict `calculateSalePrice 0.0` before running it.
- Change the fixed discount from ten to twenty percent.
- Define `addTax price = price * 1.2` and call it.

## Summary

A function maps input to output. Define it with `let`, apply it with whitespace, and look to its final expression for its result.
