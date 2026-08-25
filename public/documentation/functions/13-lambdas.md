# Anonymous functions

## What you will learn

Use `fun` to write a small function directly where a function value is needed.

## Behavior without another name

Named functions remain the clearest default:

```fsharp
let takeTenPercent price = price * 0.9
```

F# can express the same value anonymously:

```fsharp
let takeTenPercent = fun price -> price * 0.9
```

Read `fun price -> ...` as “a function that receives `price` and produces …”. The arrow separates parameters from the body; it is not the type-signature arrow, though the ideas are related.

Anonymous functions are useful when behavior is tiny and local:

```fsharp
let applyPolicy policy price = policy price
let salePrice = applyPolicy (fun price -> price * 0.8) 25.0
```

The same call with a named function is:

```fsharp
let takeTwentyPercent price = price * 0.8
let salePrice = applyPolicy takeTwentyPercent 25.0
```

Both forms describe the same function. Choose based on whether a name would help the reader.

Parentheses group the anonymous function so it becomes the first argument. Multiple parameters appear before the arrow:

```fsharp
let total = (fun quantity price -> float quantity * price) 2 12.0
```

The lambda is curried in exactly the same way as a named function. Its inferred
type is `int -> float -> float`: receive an `int`, then receive a `float`, then
produce a `float`. `fun (quantity, price) -> ...` would instead receive one
tuple, just as a tupled named function does.

## Closures remember surrounding values

```fsharp
let discount = 0.15
let discounted = fun price -> price * (1.0 - discount)
```

The function *closes over* the immutable `discount` binding. The resulting value is called a closure. Capturing immutable values is predictable; capturing mutable state is possible later, but changes the reasoning model.

## A common parsing mistake

`applyPolicy fun price -> price * 0.8 25.0` is not grouped as intended. Parenthesize a lambda when it appears as one argument among others:

```fsharp
applyPolicy (fun price -> price * 0.8) 25.0
```

The parentheses do not call the lambda. They mark where that function value
ends so F# can pass it as one argument. The final `25.0` is then applied by
`applyPolicy`.

## When a name earns its keep

Compare these two pricing calls:

```fsharp
applyPolicy (fun price -> price * 0.8) 25.0
```

```fsharp
let clearancePrice price =
    price * 0.8

applyPolicy clearancePrice 25.0
```

The lambda keeps a one-use mechanical condition close to the call. The named
version gives a domain rule a reusable vocabulary. Anonymous does not mean
better or more functional; it means the function has no binding of its own.

## Try it

- Change the anonymous discount from twenty to thirty percent.
- Rewrite a named one-line function using `fun`.
- Rewrite it back and decide which reads better.
- Remove the parentheses around the lambda passed to `applyPolicy`. Read where
  the compiler says the expression became incomplete, then restore them.

## Summary

`fun parameter -> expression` constructs a function value. Lambdas are most readable for short, local behavior.
