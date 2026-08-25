# Domain-specific identifiers

## What you will learn

Use single-case unions to stop identifiers with the same primitive representation from being mixed accidentally.

Our catalog, customers, and orders will all need identifiers. Using `int` for every one makes this mistake legal:

```fsharp
let findBook (bookId: int) catalog =
    catalog |> Map.tryFind bookId

let customerId = 12
findBook customerId catalog
```

The compiler sees only two integers. The domain sees two different categories.

## Give each identifier its own type

```fsharp
type BookId = BookId of int
type CustomerId = CustomerId of int
type OrderId = OrderId of int
```

`BookId 12` and `CustomerId 12` carry the same primitive value but have different F# types. A function requiring `BookId` rejects `CustomerId` before the program runs.

```fsharp
let findBook (bookId: BookId) catalog =
    catalog |> Map.tryFind bookId
```

The union case constructs a wrapped value. A pattern unwraps it when primitive behavior is needed:

```fsharp
let displayBookId (BookId value) =
    $"B-%d{value}"
```

Most domain functions should pass and compare `BookId` values without unwrapping them.

## A type abbreviation is different

```fsharp
type BookNumber = int
```

This gives `int` another name without creating a distinct type. The vocabulary improves, but the compiler still permits mixing. Use an abbreviation when two names describe the same values and a wrapper when exchanging them would be a bug.

## Validate construction

```fsharp
type IdError = NonPositiveId

let createBookId value =
    if value > 0 then
        Ok (BookId value)
    else
        Error NonPositiveId
```

Because the union case is still public, a caller can bypass the function and write `BookId -1`. Modules will soon let us make the case private and turn the constructor into the only public route.

Be precise about the current guarantee. The wrapper proves “this is a book identifier.” A value returned by `createBookId` has also passed the positive-number rule.

## Experiment

- Pass a `CustomerId` to `findBook` and read the two domain type names in the error.
- Add a `CartId` wrapper.
- Write display functions for each identifier.
- Change `createBookId` to reject values above a chosen limit.

## Summary

A wrapper union adds domain identity to primitive data. It is worthwhile when accidentally exchanging two values would be plausible and harmful.
