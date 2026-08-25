# Everything starts with expressions

## What you will learn

F# programs are built from expressions: pieces of code that produce values.

## Values appear when expressions are evaluated

The smallest useful F# examples are literals. A literal is a textual representation of a value in source code:

```fsharp
42
"The Hobbit"
true
```

These expressions produce an integer, a string, and a boolean value. An expression can also combine smaller expressions:

```fsharp
1 + 2
```

The result is `3`. F# is expression-oriented: calculations, decisions, and eventually whole workflows produce values that other expressions can consume.

Try reading a larger expression from the inside out:

```fsharp
(10 - 4) * 3
// 18
```

The parenthesized expression produces `6`; multiplication consumes that value and produces `18`. Nothing in that description is a statement that changes stored state. It is one expression assembled from smaller expressions.

Text following `//` is a comment. The compiler ignores it:

```fsharp
42 // this explanation is for the reader
```

The playgrounds use comments for suggested experiments. A comment ends at the
end of its line; it does not affect the value produced by the code before it.

To see those values in the playground, we use F#'s `printfn` function. For now, read it as “print a line.” `%d` marks a place for an integer, `%s` for a string, and `%b` for a boolean.

```fsharp
printfn "%d" 42
printfn "%s" "The Hobbit"
printfn "%b" true
```

Function calls and formatting get careful treatment later. They are introduced here only as the playground's display controls.

After printing, `printfn` produces `()`, pronounced “unit.” Unit means that there is no useful calculated value to pass along. We will return to it when we study functions.

## In the bookshop

Our application begins as a few facts: a title, a page count, and whether a book is in stock. They are not connected yet. That simplicity is useful; each later lesson will give the facts more structure.

```fsharp
printfn "%s" "Kindred"
printfn "%d" (264 - 40)
printfn "%b" true
```

Before running, predict the three lines. The second argument to the middle call is itself an arithmetic expression, so it is evaluated before `printfn` displays its value.

## A useful first error

```fsharp
printfn "%d" "Kindred"
```

`%d` indicates that an integer will be supplied, but the argument is a string. The compiler can reject this before the program runs. For now, look for the two disagreeing types in a diagnostic: expected `int`, received `string`.

## Try it

- Change each literal and predict the output.
- Replace `1 + 2` with `10 - 3`.
- Try printing a string with `%d`. Read the type error as “an integer was expected, but a string was supplied.”

## Summary

Expressions evaluate to values. Values have types, even before we have named or formally discussed those types.
