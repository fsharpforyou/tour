# Loops and imperative iteration

## What you will learn

Use `for`, `while`, ranges, and iteration functions when repeated effects or a
contained imperative algorithm are the clearest tools.

## Repeating an effect

`List.map` and `Array.map` calculate new collections. When the purpose is an
effect such as printing every label, a `for` loop says so directly:

```fsharp
for title in titles do
    printfn "%s" title
```

The loop variable is a new immutable binding for each iteration. The loop as a
whole returns `unit` because its result is the repeated effect, not a collection
of transformed values.

A `for` loop can traverse an inclusive range:

```fsharp
for shelfNumber in 1 .. 5 do
    printfn "Shelf %d" shelfNumber
```

`1 .. 5` produces the values one through five. A range can also use a step:

```fsharp
for evenNumber in 2 .. 2 .. 10 do
    printfn "%d" evenNumber
```

## Iteration functions

Collections also provide higher-order functions for repeated effects:

```fsharp
titles |> List.iter (fun title -> printfn "%s" title)
```

`List.iter` accepts a function returning `unit` and itself returns `unit`:

```text
('a -> unit) -> 'a list -> unit
```

Use `map` when you need transformed values. Use `iter` or a loop when the effect
is the purpose. Calling `map` merely to print and then ignoring the returned
list communicates the wrong intention.

## While a condition remains true

A `while` loop checks a boolean condition before every iteration:

```fsharp
let mutable index = 0

while index < titles.Length do
    printfn "%s" titles[index]
    index <- index + 1
```

The programmer must update state so the condition eventually becomes false.
The type checker cannot prove termination or prevent an invalid index caused by
incorrect arithmetic.

Use `while` when the stopping condition changes dynamically and does not fit a
simple collection traversal. For ordinary traversal, `for`, `iter`, or a
collection transformation is usually clearer.

## A locally imperative calculation

```fsharp
let totalStock counts =
    let mutable total = 0

    for count in counts do
        total <- total + count

    total
```

The mutation is contained inside the function. Callers receive an ordinary
integer and cannot observe the accumulator. The same calculation is naturally
a fold:

```fsharp
let totalStock counts =
    counts |> Array.fold (fun total count -> total + count) 0
```

The fold states the accumulation more directly. The loop may still be suitable
for an algorithm with several local indexes or when working with an imperative
API. Prefer the version whose state transitions are easiest to verify.

## Experiment

- Print the same titles with `for` and `List.iter`.
- Change an inclusive range and predict its final value.
- Trace the index in a `while` loop before running it.
- Remove the index update, reason about the result, and restore it without
  running the non-terminating version.
- Rewrite the stock total using `Array.fold`.

## Summary

Loops and iteration functions repeat effects. Keep manual state and termination
conditions local, and prefer value-producing collection functions when a new
collection or summary is the actual result.
