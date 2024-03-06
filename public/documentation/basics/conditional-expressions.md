# Conditional Expressions

Conditional expressions are a control flow mechanism to evaluate one of many expressions based on boolean values. If a boolean expressions evaluates to `true`, the associated expression will be evaluated.

```fsharp
let age = 20
let ageDescription = if age >= 18 then "Adult" else "Juvenile"
printfn "%s" ageDescription
```

The above conditional expression will evaluate to `"Adult"` if `age >= 18`, otherwise it will evaluate to `"Juvenile"`. Conditional expressions can include multiple conditions by including `elif` branches, which is an `else` branch with `if` condition.

```fsharp
let number = 20

let fizzBuzz =
    if number % 15 = 0 then "FizzBuzz"
    elif number % 5 = 0 then "Buzz"
    elif number % 3 = 0 then "Fizz"
    else string number
```
