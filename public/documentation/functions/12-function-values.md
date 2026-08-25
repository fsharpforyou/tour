# Functions are values

## What you will learn

Function names can be bound, selected, and passed around just like number or string values.

## A name for behavior

```fsharp
let regularPrice price = price
let memberPrice price = price * 0.9

let pricingPolicy = memberPrice
let finalPrice = pricingPolicy 20.0
```

There is no argument after `memberPrice`, so `pricingPolicy` refers to the function itself, not to the result of calling it.

Compare the types:

```fsharp
let policy = memberPrice       // float -> float
let price = memberPrice 20.0  // float
```

The presence of an argument changes the expression from a function value to the result of applying it.

That distinction becomes especially useful when reading unfamiliar code. Ask
whether the function is named by itself or whether an argument follows it. In
`let chosen = memberPrice`, the right side has type `float -> float`. In
`let chosen = memberPrice 20.0`, it has type `float`.

An `if` expression can choose behavior:

```fsharp
let selectedPolicy =
    if isMember then memberPrice else regularPrice
```

Both branches must have the same function type. An `if` cannot choose between incompatible behaviors.

For example, this cannot compile:

```fsharp
let selectedPolicy =
    if isMember then memberPrice else "regular"
```

One branch produces a function and the other produces a string. The surrounding
binding needs one type regardless of which branch runs.

## Passing behavior into a function

```fsharp
let calculatePrice policy price =
    policy price
```

The `policy` parameter is applied inside the function. Its signature is:

```text
(float -> float) -> float -> float
```

The parentheses tell us that the first input is itself a function. Here are two behaviors with that type:

```fsharp
let takeTenPercent price = price * 0.9
let addGiftWrap price = price + 2.0

calculatePrice takeTenPercent 20.0 // 18.0
calculatePrice addGiftWrap 20.0    // 22.0
```

Calling functions “first-class” means we can bind them to names, pass them as arguments, select them with expressions, and return them from other functions.

## Follow one call

```fsharp
calculatePrice takeTenPercent 20.0
```

`policy` is bound to the `takeTenPercent` function. `price` is bound to `20.0`.
The body evaluates `policy price`, which is the same calculation as
`takeTenPercent 20.0`.

Nothing reflective happens. The function value has an ordinary statically
checked type, and the compiler verifies that its input and output fit the place
where it is used.

Selecting a function is often clearer than selecting a string such as
`"member"` and interpreting that label elsewhere. Later, discriminated unions
will fit cases where the named choice itself must remain available as data.

## Try it

- Bind `chosen = memberPrice` and apply it.
- Choose a policy with a boolean and predict its type before calling it.
- Add a pricing policy that subtracts `5.0`.
- Try choosing between functions with different input types and read the error.
- Trace the parameter bindings inside one `calculatePrice` call.

## Summary

Omitting an argument refers to a function value. That lets programs select and pass behavior instead of hard-coding every rule.
