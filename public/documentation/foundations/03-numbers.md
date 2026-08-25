# Numbers and arithmetic

## What you will learn

How F# distinguishes numeric types and evaluates arithmetic expressions.

## Whole numbers

`int` is the usual whole-number type:

```fsharp
let copiesInStock = 12
let copiesReserved = 5
let availableCopies = copiesInStock - copiesReserved
```

The familiar operators `+`, `-`, `*`, and `/` produce values. Multiplication and division have higher precedence than addition and subtraction. Parentheses make grouping explicit:

```fsharp
let result = (2 + 3) * 4
```

Integer division produces an integer and truncates toward zero: `7 / 2` is `3`, while `-7 / 2` is `-3`. Use `%` for the remainder:

```fsharp
let completeShelves = 17 / 5
let booksLeftOver = 17 % 5
// 3 complete shelves, 2 books left over
```

## Floating-point numbers

A literal with a decimal point is a `float`:

```fsharp
let replacementCost = 14.95
```

F# does not silently mix `int` and `float`. Convert deliberately:

```fsharp
let totalCost = float copiesInStock * replacementCost
```

That explicit conversion makes the numeric intent visible. `float` produces a new value and leaves the original integer alone.

```fsharp
let pages = 264
let half = float pages / 2.0
// 132.0
```

Other numeric types exist, including `int64` and `decimal`, but choosing among them involves range, precision, and runtime considerations beyond this first arithmetic lesson. This course initially uses `int` for counts and `float` for approximate rates and teaching-domain prices; later it models money more deliberately.

> .NET note: `int` is the F# name for `System.Int32`, and `float` is the name commonly used for `System.Double`. Floating-point arithmetic is approximate, which is why the later money model uses integer cents.

## In the bookshop

Inventory uses integers. Introductory prices and discount rates use floats. Conversions happen only where those two worlds meet.

## Try it

- Change `quantity` to `5` and predict the total.
- Remove `float` from a mixed calculation and read the compiler error.
- Compare `7 / 2` with `7.0 / 2.0`.
- Predict `-7 / 2` and `-7 % 2`, then run them.

## Summary

Numeric types are distinct. Arithmetic produces new values, and conversions are explicit.
