# Functions as Values

In F#, all functions are values and can be passed around as such. These are called _higher order functions_ or functions that accept other functions as parameters.

Let's define a `double` and `half` function which has a single argument.

```fsharp
let double x = x * 2
let half x = x / 2

let fifty = double 25
let twentyFive = half 50

printfn "fifty = %d" fifty
printfn "twentyFive = %d" twentyFive
```

These function have a signature of `int -> int`. If we wanted to define a function which takes the `double` and `half` functions as parameter, that parameter would have a type of `int -> int`.

```fsharp
let double x = x * 2
let half x = x / 2

let apply (f: int -> int) (x: int) = f x
let fifty = apply double 25
let twentyFive = apply half 50

printfn "fifty = %d" fifty
printfn "twentyFive = %d" twentyFive
```

You don't need to pass named functions to `apply`. Anonymous functions, sometimes called lambda functions, allow you to pass an inline function as a parameter.

```fsharp
let apply (f: int -> int) (x: int) = f x
let fifty = apply (fun x -> x * 10) 5
printfn "fifty = %d" fifty 
```