# Functions with multiple inputs

## What you will learn

F# functions commonly receive inputs one at a time, separated by spaces.

## Adding the missing context

A discount depends on both its rate and the original price:

```fsharp
let applyDiscount rate price =
    price * (1.0 - rate)
```

Apply it by supplying arguments in the same order:

```fsharp
let salePrice = applyDiscount 0.10 20.0
```

Trace the names rather than reading the numbers as an undifferentiated argument
list:

```text
rate   = 0.10
price  = 20.0
result = 20.0 * (1.0 - 0.10) = 18.0
```

Each call creates fresh parameter bindings for that evaluation. Calling
`applyDiscount 0.25 8.0` does not change what `rate` meant in the earlier call.

```text
float -> float -> float
```

Read the signature from left to right for now: the function receives a `float`
rate, receives a `float` price, and produces a `float` result. Lesson 15 will
look beneath that convenient reading and explain why the arrows are written as
a chain.

Parentheses control which expression becomes an argument:

```fsharp
let price = applyDiscount (0.05 + 0.05) (15.0 + 5.0)
```

They do not surround a whole argument list. Each argument remains a separate
expression following the function name.

## Parameter order is part of the contract

The first argument supplied becomes `rate`; the second becomes `price`.
Reversing the parameters changes every call even though the arithmetic can
remain the same:

```fsharp
let applyDiscountTo price rate =
    price * (1.0 - rate)

let salePrice = applyDiscountTo 20.0 0.10
```

Choose an order that makes ordinary calls readable. Later lessons show how
partial application and pipelines give the final parameter an additional role.

## Compiler clinic

If the rate is text, the diagnostic points toward the arithmetic that expected
a number. If rate and price are accidentally reversed, the program still
type-checks because both are `float`; types cannot distinguish two values with
the same representation. Clear parameter names, argument order, and realistic
examples still matter after code compiles.

## Try it

- Predict `rate`, `price`, and the result for two calls before running them.
- Reverse the parameters and update every call.
- Add `lineTotal quantity price = float quantity * price`.
- Deliberately pass text where a number is expected and locate the mismatched input.

## Summary

Multiple-input functions use space-separated parameters and arguments. Their arrow signatures describe a sequence of inputs leading to an output.
