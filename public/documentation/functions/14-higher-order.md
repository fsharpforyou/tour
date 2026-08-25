# Higher-order functions

## What you will learn

A function can receive or return another function, allowing reusable code to accept changing behavior.

## The repetition hiding in plain sight

Suppose a price receives the same five-percent reduction twice:

```fsharp
let reduceFivePercent price = price * 0.95

let finalPrice =
    reduceFivePercent (reduceFivePercent 20.0)
```

The repeated idea is “apply the same transformation twice.” We can separate that repetition from the particular transformation:

```fsharp
let applyTwice transform value =
    transform (transform value)

let finalPrice = applyTwice reduceFivePercent 20.0
```

Try another function with the same input and output type:

```fsharp
let addPrefix text = "Bookshop: " + text
let label = applyTwice addPrefix "Kindred"
// "Bookshop: Bookshop: Kindred"
```

The result is not a useful label, but it demonstrates that `applyTwice` is about repeated transformation rather than prices.

## Give the idea a name

A function that receives or returns another function is called a *higher-order function*. It needs no special declaration syntax: `transform` is a parameter whose value happens to be callable.

```text
applyTwice : ('a -> 'a) -> 'a -> 'a
```

Read it one piece at a time:

1. `('a -> 'a)` is a function whose input and output types match.
2. The next input is a value of that same type `'a`.
3. The final result is also `'a`.

The apostrophe introduces a generic type variable. It stands for one type the compiler need not choose in advance. Within one use, all occurrences of `'a` must agree. `float -> float` and `string -> string` fit; `string -> int` does not, because its output cannot be fed back into itself.

## Functions can produce functions

```fsharp
let minimumLength minimum =
    fun (text: string) -> text.Length >= minimum

let validShortTitle = minimumLength 3
let validLongTitle = minimumLength 10
```

`minimumLength 3` returns a `string -> bool` function that remembers `minimum`. That returned function is a closure over an immutable value.

## A domain-shaped use

```fsharp
let atLeast minimum amount = amount >= minimum

let freeStandardDelivery = atLeast 30.0
let freeExpressDelivery = atLeast 60.0

let evaluateRule rule basketTotal =
    rule basketTotal
```

The threshold-specific predicates share the same evaluator. The caller supplies the rule instead of leaving it hidden inside `evaluateRule`.

## Compiler clinic

`applyTwice String.length "Kindred"` cannot compile: `String.length` produces an `int`, but a second call would require a `string`. The error exposes a real broken connection between output and input.

## Try it

- Define `double number = number * 2`, then evaluate `applyTwice double 3`.
- Define a transformation returning a different type and predict why it fails.
- Build `maximumLength maximum` as a returned predicate.
- Write `applyThreeTimes` without copying a particular pricing rule into it.

## Summary

Functions are values, so they can serve as inputs and outputs. Generic type variables describe the relationships that must remain true without fixing one concrete domain type.
