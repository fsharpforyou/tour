# Currying and Partial Application

All `let` bound functions in F# are automatically curried. This means that every function has a single input and a single output. 

Let's take a look at the `add` function introduced earlier to see what is going on.

```fsharp
let add x y = x + y
```

This function has a signature of `int -> int -> int`. What does that mean? The arrows in a function signature indicate the inputs and outputs of a function: `input -> output`. The add function has a single `int` input and returns another function with a signature of `int -> int`.

We can see this clearly by redefining the `add` function to explicitly return another function.

```fsharp
let add x = fun y -> x + y
```

This new `add` function has a signature of `int -> int -> int`, just like the original.

An advantage of curried functions is the ability to partially apply parameters. This is often used to compose two functions or build functions from existing ones. You can supply a single parameter to the `add` function to get back a new function with the `x` parameter filled in. The implicit parameter to this new function refers to the `y` parameter in the original function as it hasn't been filled in yet.

```fsharp
let add x y = x + y
let addFive = add 5
```