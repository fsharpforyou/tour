# Tuples: values that travel together

## What you will learn

A tuple groups a fixed number of values without defining a new named type.

```fsharp
let book = ("Kindred", 264)
```

The comma constructs the pair; its type is `string * int`. The `*` in a tuple type means “and,” not multiplication. A triple has three positions:

```fsharp
let copy = ("Kindred", 264, true)
```

Use a tuple pattern to take a tuple apart:

```fsharp
let (title, pages) = book
```

Patterns describe the shape of data. Each name is bound to the corresponding part.

The whole tuple is one value. This matters when a function returns more than one result:

```fsharp
let readingProgress pageCount pagesRead =
    let remaining = pageCount - pagesRead
    let completed = float pagesRead / float pageCount * 100.0
    (remaining, completed)

let progress = readingProgress 264 66
// int * float
```

Destructure at the point where names become useful:

```fsharp
let (remaining, completed) = progress
```

The function returns one value: a pair that contains two values.

## Curried and tupled functions

These are different shapes:

```fsharp
let add x y = x + y       // int -> int -> int
let addPair (x, y) = x + y // int * int -> int
```

Call them with `add 2 3` and `addPair (2, 3)`. The curried form supports partial application; the tupled form receives one paired value.

Tuples are handy for temporary, obvious groupings and for returning two results. Once positions become hard to remember, a record gives the data names.

Compare these values:

```fsharp
let book = ("Kindred", 264)
let customer = ("Ada", 5)
```

Both have type `string * int`, even though their meanings differ. A function expecting one can accidentally receive the other. Tuples preserve types and positions, not domain names. That limitation motivates records.

## Ignoring one position

The wildcard pattern `_` recognizes a value without binding a name:

```fsharp
let (title, _) = book
```

Use it when the position is genuinely irrelevant. Naming a value and then never using it makes readers wonder whether something was forgotten.

## Follow the types

If `book` is `string * int`, then this function has type `string * int -> string`:

```fsharp
let progressLabel (title, pagesRead) =
    $"%s{title}: %d{pagesRead} pages read"
```

The parentheses belong to the tuple pattern, not to function-call syntax. `progressLabel ("Kindred", 40)` constructs a tuple and passes that single value. Writing `progressLabel "Kindred" 40` instead supplies two curried arguments to a function that expects one pair. That is what the compiler means when it says it expected a tuple.

## Try it

- Destructure a triple.
- Swap the fields and observe the changed type.
- Convert a curried function into a tupled one.
- Return two calculated values from one function and destructure them immediately.

## Summary

Tuples group values by position. Tuple patterns deconstruct them, and a tupled parameter is not the same as several curried parameters.
