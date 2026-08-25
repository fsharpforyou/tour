# Partial application and currying

## What you will learn

Curried functions accept inputs one at a time, so supplying only some inputs creates a useful specialized function.

## One function, one input at a time

Consider the familiar discount function:

```fsharp
let applyDiscount rate price =
    price * (1.0 - rate)
```

Its inferred type is `float -> float -> float`, which means `float -> (float -> float)` because arrows associate to the right. The function receives a rate and returns a function awaiting a price.

When both arguments are written together, application associates to the left:

```fsharp
applyDiscount 0.10 20.0
// equivalent to
(applyDiscount 0.10) 20.0
```

This one-input-at-a-time representation is called *currying*. It is how F# normally represents functions with several parameters.

## Stop after the first application

```fsharp
let memberPrice = applyDiscount 0.10
// float -> float

let finalPrice = memberPrice 20.0
// 18.0
```

Creating `memberPrice` is *partial application*. The resulting closure remembers `0.10`.

```fsharp
let regularPrice = applyDiscount 0.0
let memberPrice = applyDiscount 0.10
let clearancePrice = applyDiscount 0.40
```

Each binding has type `float -> float`, but remembers a different rate.

## Argument order becomes design

```fsharp
let isAtLeast minimum amount = amount >= minimum
let qualifiesForStandard = isAtLeast 30.0
let qualifiesForExpress = isAtLeast 60.0
```

Stable configuration often belongs first because it makes specialization convenient, while changing data often belongs last because it flows well through pipelines. If that order reads awkwardly in the domain, choose clarity instead.

## Pause and predict

```fsharp
let addHandling amount total = total + amount
let addGiftWrap = addHandling 2.0
let result = addGiftWrap 20.0
```

```text
addHandling : float -> float -> float
addGiftWrap : float -> float
result      : float, with value 22.0
```

Trying to print `addGiftWrap` with a numeric formatter fails because it is still a function, not a number. Supply its remaining argument first.

## Try it

- Create regular, member, and clearance pricing functions.
- Reverse the parameters of `applyDiscount` and inspect the partial application.
- Fully parenthesize a three-input function one application at a time.
- Find one function where configuration-first ordering helps and one where it harms readability.

## Summary

A curried function is a chain of one-input functions. Partial application follows part of that chain and keeps the remaining function for later.
