# Exceptions and explicit errors

## What you will learn

Exceptions handle exceptional control flow, while Option and Result keep expected domain outcomes visible in types.

## Two different kinds of failure

The bookshop already represents expected outcomes with `Option` and `Result`:

- a title may have no subtitle;
- a search may find nothing;
- checkout may be refused because stock is unavailable.

Programs also encounter failures that are exceptional at the current boundary: a supposedly trusted imported record is corrupt, a supported external API throws, or an internal invariant is violated. F# supports exceptions for those cases.

Choose the mechanism from the kind of failure. Ask whether callers are expected to branch on the outcome as part of normal domain behavior.

## Raising an exception

```fsharp
let requireImportedTitle (title: string) =
    let cleaned = title.Trim()

    if cleaned = "" then
        failwith "Imported title was empty"
    else
        cleaned
```

`failwith` raises an exception carrying the message. Unlike `Error`, it returns no union case. Evaluation stops and control searches outward for a matching handler.

This function assumes imported data has already satisfied a contract. If blank titles are routine user input, `Result` validation is the better design.

## Catching with `try/with`

```fsharp
type ImportError =
    | InvalidImportedRecord of message: string

let importTitle title =
    try
        let validTitle = requireImportedTitle title
        Ok validTitle
    with
    | ex -> Error (InvalidImportedRecord ex.Message)
```

`try ... with` is an expression. The successful body and every handler must produce compatible types; here both produce `Result<string, ImportError>`.

The `ex` pattern binds the exception object, and `.Message` reads one of its properties. This boundary translates an exception-based API into the explicit error model used by the rest of the domain.

## Specific exception patterns

F# also supports type-test patterns:

```fsharp
try
    operation ()
with
| :? System.ArgumentException as ex ->
    Error ex.Message
| ex ->
    Error ex.Message
```

Specific handlers must appear before the general `ex` pattern because the first matching pattern wins. Catch exception types documented by the .NET API at the boundary you are calling; avoid guessing from implementation details.

## Keep the protected region narrow

This is risky:

```fsharp
let importSafely title =
    try
        importTitle title
    with
    | ex -> Error (InvalidImportedRecord ex.Message)
```

It can disguise a programming defect anywhere in the workflow as a friendly operational error. Wrap the smallest external or invariant-sensitive operation that is expected to throw, then translate or handle it deliberately.

Avoid empty catch-all handlers. Silently continuing after an exception destroys information and can leave callers believing work succeeded.

## Exception or Result?

Use `Result` when:

- failure is a normal modeled outcome;
- callers should inspect a typed reason;
- the operation naturally composes with other fallible domain functions.

Exceptions are reasonable when:

- a called API uses exceptions;
- a trusted invariant is unexpectedly broken;
- the current function cannot produce a meaningful local recovery value.

The boundary is not absolute. What matters is whether failure should remain visible in normal control flow or interrupt it through exception handling.

## Exceptions in .NET

.NET exception types derive from `System.Exception`. A handler can match a specific exception type, bind the exception object, and inspect members such as `Message`.

Catch the narrowest documented exception that the current boundary can handle meaningfully. Reserve a broad `ex` handler for an outer boundary that translates errors, and preserve useful diagnostic information there. Resource-owning .NET APIs introduce deterministic cleanup concerns, but filesystem and process infrastructure remain outside this tour.

## Experiment

- Pass a valid and blank imported title through `importTitle`.
- Move the `try` boundary outward and identify which unrelated mistakes it could hide.
- Rewrite routine blank-name validation with `Result` and compare the call sites.
- Add a specific handler before the general one, using only a supported operation.

## Summary

Results make expected failure explicit in a function's type. Exceptions provide non-local control flow for exceptional boundaries and invariant failures. Translate between them deliberately rather than treating either as a universal answer.
