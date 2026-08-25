# Values and `let` bindings

## What you will learn

How `let` gives a stable name to a value.

## Naming a result

Repeating a literal makes its meaning unclear. F# uses `let` to introduce a binding:

```fsharp
let pageCount = 310
```

Read this as “bind the name `pageCount` to the value `310`.” It does not declare a mutable local variable. F# bindings are immutable by default: `pageCount` continues to mean the same value throughout its scope.

Names use camel case by convention:

```fsharp
let bookTitle = "The Hobbit"
let isAvailable = true
```

The `=` has two closely related jobs in F#. In a `let` binding it separates the pattern being bound from the expression being evaluated. Later, inside an expression, `=` tests equality.

Bindings can depend on earlier bindings:

```fsharp
let pagesRead = 125
let pagesRemaining = pageCount - pagesRead
```

`pagesRemaining` is a new value. Nothing changed `pageCount` or `pagesRead`.

Evaluation happens before the name is bound. In this example, F# calculates `310 - 125`, then binds `pagesRemaining` to `185`:

```fsharp
let pagesRemaining = 310 - 125
// 185
```

The expression is evaluated once, and the name refers to its result. It is not a formula that recalculates itself later.

## Shadowing is a new binding

F# permits a later binding to reuse a name:

```fsharp
let label = "Kindred"
let label = label + " — available"
```

This is *shadowing*, not mutation. The right side of the second line refers to the earlier `label`; afterward the newer binding is the one in scope. Shadowing is useful in short transformation sequences, but repeatedly reusing a name can obscure which value is meant. Prefer distinct names while learning.

## Why immutability helps

Code is easier to follow when a name keeps the same meaning. F# supports mutation, but you must request it explicitly. We will wait until immutable transformations feel familiar before using it.

## In the bookshop

We can now name one book's facts and derive its reading progress. The model is still primitive, but the intent is becoming visible.

## Try it

- Change `pagesRead` and predict `pagesRemaining`.
- Add a `dailyTarget` binding calculated from `pagesRemaining`.
- Try writing `pageCount = 400` on a later line. Notice that this is an equality expression, not assignment.
- Shadow `bookTitle` with `bookTitle + " — available"` and confirm the earlier string was used to create the new value.

## Summary

`let` binds a name to an evaluated value. Bindings are immutable unless you explicitly mark them otherwise.
