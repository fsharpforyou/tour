# Tuples

Tuples allow us to define a set of unnamed values of various types.

```fsharp
let person = ("John", "Doe")
```

The length of a tuple and the types of each element are known at compile time and are present in its signature. The type of the tuple value `(1, 5)` is `int * int`.

Tuples can be deconstructed in let bindings and function arguments using pattern matching. The tuple pattern allows you to define a pattern for each element in a tuple. You can use the variable pattern to bind each tuple value to a name like so:

Two useful built-in functions for extracting tuple values are `fst` and `snd`. These functions will extract the first and second element of a two-value tuple respectively. 

```fsharp
let person = ("John", "Doe")
let firstName = fst person
let lastName = snd person
```