# Controlled mutation and mutable arrays

## What you will learn

F# makes mutable bindings and mutable array elements explicit, so shared change
can be recognized and contained.

## Change is available when you need it

The course has emphasized immutable values because they make domain transformations easy to follow. F# is a multi-paradigm language, not a purely functional one. It also supports mutable bindings and array updates.

The useful skill is knowing when mutation is local and helpful, then keeping it contained so the surrounding domain model remains easy to follow.

## Mutable bindings

A plain `let` creates an immutable binding:

```fsharp
let count = 0
```

Request mutation explicitly with `mutable` and assign with `<-`:

```fsharp
let mutable count = 0
count <- count + 1
```

`<-` is assignment. Its distinct spelling separates mutation from `=`, which appears in bindings and equality expressions.

Every reader can now see that `count` may change. That visibility is useful, but it also means the value at a line depends on the path taken through earlier assignments.

## Mutating array elements

Arrays permit indexed assignment:

```fsharp
let stock = [| 2; 0; 4 |]
stock[1] <- 3
// stock is now [| 2; 3; 4 |]
```

Unlike `Array.map`, this changes the existing array value observed by every reference to it. Keep such updates local when possible.

If the domain operation is naturally expressed as a returned successor, prefer an immutable transformation:

```fsharp
let restocked =
    stock
    |> Array.mapi (fun index value ->
        if index = 1 then 3 else value)
```

Neither form is universally superior. The second makes old and new arrays distinct; the first can be appropriate inside a contained low-level algorithm or when using an imperative API.

## References and shared change

F# also has reference cells, but mutable bindings and arrays are enough for this tour. What matters is shared observable change: once several parts of a program can modify the same location, behavior depends on ordering and ownership.

Keep mutation:

- inside one small function;
- away from core domain values when immutable successors suffice;
- explicit in names and types where callers can observe it;
- out of higher-order callbacks unless the effect is the purpose.

## Contain mutation behind a function

```fsharp
let receiveCopies shelfIndex delivered (stock: int array) =
    if delivered > 0 then
        stock[shelfIndex] <- stock[shelfIndex] + delivered
```

The function's unit result signals that its purpose is the update. The type
`int array` does not reveal whether a function mutates the array, so its name
and documentation must make that effect clear. Every alias of the supplied
array observes the change.

Compare it with an immutable alternative:

```fsharp
let withReceivedCopies shelfIndex delivered stock =
    stock
    |> Array.mapi (fun index count ->
        if index = shelfIndex then count + delivered else count)
```

This version returns another array and preserves the supplied one. The next
lesson introduces loops and shows how local mutable state can implement a
calculation without exposing that state to callers.

## Experiment

- Update one array element and observe another binding referring to the same array.
- Rewrite the update with `Array.mapi` to return a new value.
- Call both receiving functions and compare which arrays changed.
- Bind the result of `receiveCopies` and inspect its `unit` type.

## Summary

F# makes mutation opt-in at the binding or assignment site. Immutable
transformations remain the default for domain values; when shared mutation is
needed, keep its ownership and observable effects narrow and explicit.
