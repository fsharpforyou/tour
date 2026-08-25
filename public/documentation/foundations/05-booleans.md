# Booleans and comparisons

## What you will learn

How expressions answer yes-or-no questions.

## Boolean values

`bool` has two values: `true` and `false`.

```fsharp
let isAvailable = true
let isReferenceOnly = false
```

Comparisons produce boolean values:

```fsharp
let hasCopies = availableCopies > 0
let exactlyOne = availableCopies = 1
let notEmpty = title <> ""
```

In expressions, `=` tests equality and `<>` tests inequality. This differs from languages that use `==` or `!=`.

Other comparison operators are `<`, `>`, `<=`, and `>=`.

## Combining questions

`&&` means both expressions must be true. `||` means at least one must be true. `not` reverses a boolean:

```fsharp
let canBorrow = hasCopies && not isReferenceOnly
let needsAttention = isReferenceOnly || availableCopies = 0
```

`not` is a function. Function application has higher precedence than infix operators, so `not isReferenceOnly` is evaluated before `&&` combines the results.

Boolean operators short-circuit. In `left && right`, F# evaluates `right` only when `left` is true; in `left || right`, it evaluates `right` only when `left` is false. This matters when the second expression performs work or could fail, although domain predicates are usually easiest to reason about when they are pure.

Parentheses can make a mixed rule unambiguous:

```fsharp
let canUseCopy =
    isAvailable && (isMember || not isRestricted)
```

When grouping makes the business rule clearer, prefer it to relying on remembered precedence.

## Equality is an expression

In `let exactlyOne = availableCopies = 1`, the first `=` belongs to the binding
syntax and the second compares two integers. Read the right-hand side as a
complete expression:

```text
availableCopies = 1
→ true or false
```

This is why writing `availableCopies = 4` by itself does not assign four. It
asks whether the current value equals four.

## Structural equality

F# can compare many values structurally. Strings and numbers compare by value. Records, tuples, and lists will later compare their contents when their contained values support equality.

## In the bookshop

Our first business rule asks whether a book can be ordered. It is still only a boolean; later a discriminated union will explain *why* ordering may be impossible.

The limitation matters: `false` cannot distinguish “sold out” from “not currently for sale.” booleans answer yes-or-no questions well, but richer outcomes need richer types.

## Try it

- Set `availableCopies` to zero.
- Make the book unavailable for sale.
- Predict each intermediate boolean before running.
- Expand `canOrder` into its smaller comparisons and evaluate them one at a time.

## Summary

Comparisons produce booleans. boolean operators combine small questions into larger rules.
