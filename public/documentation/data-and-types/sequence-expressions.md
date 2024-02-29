# Sequence Expressions

Sequence expressions allow us to dynamically build lists and sequences using ranges, loops, and conditional expressions.

We can produce a list from `a` to `b` using the range operator. If we wanted to produce a sequence of values from `1` to `10` we could use the range `1..10`.

```fsharp
let oneThroughTen = [ 1..10 ]
let oneThroughTenSeq = seq { 1..10 }
```

Ranges can also contain a step amount inserted between the start and end index,
which indicates the number of steps per element. The default step is `1` which indicates a single step from one element to the next, ex: `0` to `1`. If we used a step of `2` it would be `0` to `2` instead.

```fsharp
let evenNumbers = [ 0..2..10 ]
let evenNumbersSeq = seq { 0..2..10 }
```

We can make use of `for..in` expressions to iterate over a series of elements and produce N number of elements into the resulting list or sequence.

```fsharp
let doubled = [ for number in 0..10 -> number * 2 ]
let doubledSeq = seq { for number in 0..10 -> number * 2 }
```

The `->` operator will _yield_ the result of the expression into the resulting list. The `->` operator can only be used if every part of the expression block on the right returns a value. Sometimes, you may want an iteration to produce multiple values into a list. For this, you would substitute the `->` operator with a `do` and an optional `yield` for each value.

```fsharp
let numbers = [
    for x in 0..10 do
        for y in 0..10 do
            x + y
            x * y
//          ^^^^^
// produce multiple values per iteration.
]
```

Sequence expressions can also contain conditional expressions that produce zero or many values conditionally.

```fsharp
let numbers = [
    for number in 0..10 do
        if number % 2 = 0
        then $"{number} is even"
        else $"{number} is odd"
]

let onlyEvenNumbers = [
    for number in 0..10 do
        if number % 2 = 0 then number
//      ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
// only produce even numbers into the resulting list
]
```