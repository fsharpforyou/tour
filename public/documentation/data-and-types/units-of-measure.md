# Units of Measure

When working with numerical values representing units of measurement like miles, kilometers, pounds, kilograms, you may be tempted to use primitive values such as:

```fsharp
let miles = 10
let kilometers = 10
```


Both of these values are simple `int`'s. When passing these values around, how do you make the distinction between the use of kilometers and miles in your code? The answer: Units of Measure

Units of Measure are marker values for numerical primitives like `int`, `float`, `decimal`, and more. These are often used for units of measurement and to create distinct and specific primitive values for domain modeling. Let's define a simple Unit of Measure for `mile` and `kilometer`.

```fsharp
[<Measure>] type mile
[<Measure>] type kilometer
```

Now, we can specifically tag numerical values with these Units of Measure.

```fsharp
let miles = 10<mile>
let kilometers = 10<kilometer>

let calculateDistanceBetween
    (startMile: int<mile>)
    (endMile: int<mile>)
    =
    ...

let startMile = 10<mile>
let endMile = 20<mile>
let distance = calculateDistanceBetween startMile endMile
```