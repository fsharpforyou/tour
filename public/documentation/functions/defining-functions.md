# Defining Functions

In F#, functions are defined using the `let` syntax followed by a list of parameters and an expression.

```fsharp
let add x y = x + y
```

As you can see, no type annotations are required. The compiler will infer the type of the parameters and the return value from the function definitions. Here, the type of `x`, `y`, and the return value are all `int`.

Sometimes, the compiler doesn't have enough information to infer the types of the parameters or the return type from the definition. In these cases, you can supply type annotations to any parameters and the return type if necessary.

```fsharp
let add (x: float) (y: float) : float = x + y
```