# Chaining fallible operations

## What you will learn

`Result.map` and `Result.bind` continue a workflow only while it remains successful.

`Result.map` transforms the value inside `Ok` and leaves `Error` untouched:

```fsharp
let displayName = validateName input |> Result.map (fun name -> name.ToUpper())
```

Its behavior is the same shape as:

```fsharp
let mapResult transform result =
    match result with
    | Ok value -> Ok (transform value)
    | Error error -> Error error
```

Use `Result.bind` when the next function can itself fail:

```fsharp
let createCustomer name =
    validateName name
    |> Result.bind checkNotReserved
```

If validation fails, `checkNotReserved` is not called. If it succeeds, `bind` passes the inner value onward without nesting results.

```fsharp
let bindResult next result =
    match result with
    | Ok value -> next value
    | Error error -> Error error
```

```text
Result.map  : ('a -> 'b) -> Result<'a,'e> -> Result<'b,'e>
Result.bind : ('a -> Result<'b,'e>) -> Result<'a,'e> -> Result<'b,'e>
```

Both steps must agree on the error type. That constraint is often helpful: it encourages a workflow-level error union.

Choose from the next function's signature:

```text
string -> Customer                         // use map
string -> Result<Customer, CustomerError> // use bind
```

Pattern matching is still preferable when different failures require different branches of domain behavior. Helpers are best for linear transformations.

## Evaluate the chain

With `"Ada"`, `validateName` produces `Ok "Ada"`; bind extracts the name for `rejectReservedName`; map then formats the final success. With an empty string, the first function returns `Error EmptyName`. Neither later function runs, and the exact error reaches the end unchanged.

`Result.bind` provides the short-circuiting without exceptions or a hidden error type. If two steps use different error unions, convert them to one workflow error type explicitly before chaining them.

## Branch when the domain branches

A pipeline of binds suits a linear workflow. If one error triggers a retry, compensation, or different operation, use an explicit match. Idiomatic functional code is not code with the fewest `match` expressions; it is code whose control flow is visible in values and types.

## Seeing structured values in the playground

Several later playgrounds use `%A`:

```fsharp
printfn "%A" (register "Ada")
```

`%A` prints a general structural representation such as `Ok "Ada"` or `Error EmptyName`. It is handy for inspecting records, unions, options, results, and collections while learning. For customer-facing output, format the message yourself.

## Try it

- Change a successful input into an empty one.
- Replace `bind` with `map` and inspect the nested type.
- Add a third validation step.

## Summary

`map` transforms successful values; `bind` sequences operations that may fail. Errors pass through unchanged until handled.
