# Optional enrichment: computation expressions for Result

## What you will learn

A computation expression can present already-understood Result binding as sequential syntax interpreted by a small builder.

## Why this lesson is optional

Matches, `Result.map`, and `Result.bind` are enough to write clear F#. Computation-expression syntax builds on those operations and can make a longer chain of dependent validations read sequentially.

Nothing later depends on this syntax. If explicit Result pipelines are still new, feel free to return to this chapter later.

## The workflow before new syntax

```fsharp
let prepareOrder request =
    findCustomer request.CustomerId
    |> Result.bind (fun customer ->
        validateCart request.Cart
        |> Result.bind (fun lines ->
            validateShipping request.Shipping
            |> Result.map (fun shipping -> createDraft customer lines shipping)))
```

Every operation is visible, but dependent names create nested lambdas.

## A minimal Result builder

FSharp.Core provides Result functions but no single universal Result computation-expression builder. The small builder below defines only the behavior we need:

```fsharp
type ResultBuilder() =
    member _.Bind(result, next) =
        Result.bind next result

    member _.Return(value) =
        Ok value

    member _.ReturnFrom(result) =
        result

let result = ResultBuilder()
```

These class members give meaning to the syntax inside `result { ... }`.

## `let!`, `do!`, `return`, and `return!`

```fsharp
let prepareOrder request =
    result {
        let! customer = findCustomer request.CustomerId
        let! lines = validateCart request.Cart
        do! validateShipping request.Shipping
        return createDraft customer lines request.Shipping
    }
```

- `let!` calls `Bind`; `Ok` supplies its inner value to the remaining block and `Error` skips it.
- `do!` binds a successful unit result when no name is needed.
- `return value` calls `Return`, wrapping a value in `Ok`.
- `return! existingResult` calls `ReturnFrom` for a Result already produced elsewhere.

Here, `return` belongs to computation-expression syntax. A regular F# function still produces its final expression without a return keyword.

## Desugar one step

```fsharp
result {
    let! customer = findCustomer customerId
    return customer.Name
}
```

corresponds conceptually to:

```fsharp
Result.bind
    (fun customer -> Ok customer.Name)
    (findCustomer customerId)
```

The builder adds syntax for its members' bind behavior. It adds no exceptions, mutation, background work, or second error type.

## Use the abstraction only when it helps

A Result computation expression is useful when several fallible steps depend on earlier successes and the team recognizes the builder. Prefer a normal pipeline when two transformations already read clearly. Prefer explicit matching when different errors cause different domain behavior instead of simple propagation.

A builder is an API, and its members determine exactly what the syntax means. Builders from libraries may support more operations, so read their contract instead of assuming that every `result {}` block behaves alike.

## Experiment

- Rewrite the playground with nested `Result.bind`.
- Make customer lookup and cart validation fail separately.
- Add `return!` around an already validated result.
- Remove `ReturnFrom` and observe which syntax stops compiling.

## Summary

A computation expression is syntax interpreted by builder members. For Result, it can present short-circuiting binds in sequence while keeping success and failure explicit in the type.
