# Discriminated Unions

Discriminated unions represent a single choice between several named cases. Each of these cases has an identifier and optionally, data associated with it of varying types. The identifier and the optional data serve as a _case constructor_ or function that will construct an instance of the specified union type.

```fsharp
type Color =
    | Red
    | Green
    | Blue
    | RGB of int * int * int
```

The discriminated union defined above has four cases: `Red`, `Green`, `Blue`, and `Rgb`.
Only the `RGB` case has data associated with it. You can construct instances of these cases using the identifier and any data.

```fsharp
let red = Red
let black = RGB (0, 0, 0)
```

The case constructor for the `RGB` case is a function with the signature of `int * int * int -> Color`.

```fsharp
let rgb: int * int * int -> Color = RGB
```

You can match against the cases of a discriminated union by using the _identifier_ pattern. The _identifier_ pattern allows you to match against the case by its identifier and additionally, supply a pattern for any data associated with it.

```fsharp
let rgb color =
    match color with
    | Red -> 255, 0, 0
    | Green -> 0, 255, 0
    | Blue -> 0, 0, 255
    | RGB (r, g, b) -> r, g, b

let color = Red
let (r, g, b) = rgb color
```

When dealing with a case that has data in the form of a tuple, it can be difficult to discern which tuple value corresponds to which piece of the data. In these cases, it is good practice to include labels on tuple elements like so:

```fsharp
type Color =
    | Red
    | Green
    | Blue
    | Rgb of r: int * g: int * b: int

let color = Rgb (r = 255, g = 255, b = 255)
```