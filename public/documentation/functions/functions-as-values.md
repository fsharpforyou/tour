# Functions as Values

In F#, all functions are values and can be passed around as such. These are called _higher order functions_ or functions that accept other functions as parameters.

Let's define a `double` function which has a single parameter.

```fsharp
let double x = x * 2
```

This function will have a signature of `int -> int`. If we wanted to define a function which takes the `double` function as a parameter, that parameter would have a type of `int -> int`.

```fsharp
let apply (f: int -> int) = f 5
```

You don't need to pass named functions to `apply`. Anonymous functions, sometimes called lambda functions, allow you to pass an inline function as a parameter.

```fsharp
apply (fun value -> value * 5)
```
