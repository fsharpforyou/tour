# Recursive Functions

In F#, recursive functions must be explicitly marked with the `rec` keyword.

```fsharp
let rec fib n =
    if n <= 1
    then n
    else fib (n - 1) + fib (n - 2)
//       ^^^           ^^^
// function is allowed to be recursive
// as the `rec` keyword is present.
```

Mutually recursive functions can be defined using the `and` keyword.

```fsharp
let rec first x = if x > 0 then second (x - 1) else x
and second x = if x > 0 then first (x - 1) else x
```