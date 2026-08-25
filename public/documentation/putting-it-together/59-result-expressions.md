# Result computation expressions

## What you will learn

Use a computation expression to write a sequence of dependent `Result`
operations without nesting `Result.bind` and `Result.map` callbacks.

## Result pipelines eventually bend inward

A short Result pipeline is easy to read:

```fsharp
validateQuantity line
|> Result.map priceLine
```

One transformation follows one validation, so the pipeline already states the
flow clearly. Keep code like this as a pipeline.

The shape changes when several successful values are needed later:

```fsharp
let prepareOrder request =
    findCustomer request.CustomerId
    |> Result.bind (fun customer ->
        validateCart request.Cart
        |> Result.bind (fun lines ->
            validateShipping request.Shipping
            |> Result.map (fun shipping ->
                createDraft customer lines shipping)))
```

Each operation depends on an earlier success. The lambdas nest because
`customer`, `lines`, and `shipping` must remain in scope until the draft is
created. The mixture of several binds and a final map is correct, but its
indentation emphasizes plumbing instead of the workflow.

Use a Result computation expression when a pipeline contains multiple dependent
`bind` and `map` operations and begins to nest. Keep a direct `Result.map` or
`Result.bind` when it remains flatter and clearer.

## A computation expression gives the nesting a sequential form

The same workflow can be written as:

```fsharp
let prepareOrder request =
    result {
        let! customer = findCustomer request.CustomerId
        let! lines = validateCart request.Cart
        let! shipping = validateShipping request.Shipping
        return createDraft customer lines shipping
    }
```

Read it from top to bottom:

1. obtain a customer or stop with that error;
2. obtain validated lines or stop with that error;
3. obtain validated shipping or stop with that error;
4. construct the successful draft.

The computation expression has not made failure implicit. The function still
returns `Result<OrderDraft, CheckoutError>`, and the first `Error` still becomes
the result of the whole block.

## The builder defines what the syntax means

FSharp.Core provides the `Result` type and functions such as `Result.map` and
`Result.bind`, but it does not provide a built-in `result { ... }` builder. This
small builder supplies the operations needed by the playground:

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

The name before the braces is a value. Its members interpret special syntax
inside the block. Another builder may give the same syntax different behavior.

## `let!` is bind; `return` supplies the final map

This block:

```fsharp
result {
    let! customer = findCustomer customerId
    return customer.Name
}
```

corresponds to:

```fsharp
findCustomer customerId
|> Result.map (fun customer -> customer.Name)
```

Conceptually, `let!` uses `Bind`. For `Ok customer`, it binds the inner value
and continues; for `Error error`, it skips the remaining body and preserves the
error. When the remaining body only returns an ordinary value, `return` wraps
that value with `Ok`, giving the whole expression the effect of a final map.

The exact translation is expressed in terms of builder members, but this
bind/map correspondence is the useful way to read an ordinary Result workflow.

## `do!` and `return!`

Use `do!` when a step returns `Result<unit, 'error>` and success carries no value
that needs a name:

```fsharp
result {
    let! customer = findCustomer customerId
    do! ensureCustomerMayOrder customer
    return customer
}
```

Use `return!` when the final expression already is a `Result`:

```fsharp
result {
    let! customer = findCustomer customerId
    return! createOrder customer
}
```

- `let!` unwraps a successful value and continues.
- `do!` performs the same bind when the success value is `unit`.
- `return` wraps an ordinary successful value.
- `return!` returns an already wrapped Result.

Here, `return` and `return!` belong to computation-expression syntax. A normal
F# function still produces the value of its final expression without a return
keyword.

## Use established builders in real code

Defining a tiny builder exposes the mechanics, but production code does not
usually need to maintain its own. The widely used
[`FsToolkit.ErrorHandling`](https://github.com/demystifyfp/FsToolkit.ErrorHandling)
library provides builders and supporting functions for common error-handling
shapes. These include `result`, `option`, `validation`, and combinations with
asynchronous workflows such as `taskResult`.

The builders support more syntax and conversions than this teaching version.
Use the builder whose returned type matches the workflow, and consult its
documentation rather than assuming all computation expressions handle failure
or accumulation identically. In particular, a Result workflow normally stops
at the first error, while validation-oriented abstractions may accumulate
independent errors.

The playground defines its builder locally so it remains a complete standalone
program. It is demonstrating the same core `Bind`, `Return`, and `ReturnFrom`
relationship that library builders package more thoroughly.

## Choose between a pipeline, match, and computation expression

- Use a pipeline for one or two flat transformations where `Result.map` or
  `Result.bind` makes the operation obvious.
- Use a Result computation expression when several dependent binds and maps
  introduce nested lambdas or when several successful values must remain in
  scope.
- Use pattern matching when different errors lead to substantially different
  domain decisions rather than ordinary short-circuit propagation.

A computation expression is not automatically clearer. It earns its place when
it removes structural nesting and makes the workflow's dependent steps visible.

## Experiment

- Trace which steps run when customer lookup fails.
- Rewrite the playground with nested `Result.bind` and compare indentation.
- Add one final transformation first with `Result.map`, then with `return`.
- Add a `Result<unit, CheckoutError>` check using `do!`.
- Replace the final `return` with a function that already returns `Result`, read
  the nested type or error, and repair it with `return!`.

## Summary

A Result computation expression presents nested, dependent binds and maps as a
linear workflow. Use it once an ordinary Result pipeline bends into nested
callbacks; keep simple Result transformations as pipelines. In application
code, established libraries such as `FsToolkit.ErrorHandling` provide complete
builders for Result and related error-handling contexts.
