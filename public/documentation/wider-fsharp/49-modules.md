# Modules: organizing a growing program

## What you will learn

Modules group related types and functions under a qualified name.

```fsharp
module Catalog =
    type BookId = BookId of int
    let find id catalog = Map.tryFind id catalog
```

Use `Catalog.find` from outside. `open Catalog` brings names into scope, but qualification often makes domain vocabulary clearer and avoids collisions.

```fsharp
let found = Catalog.find (Catalog.BookId 1) catalog
```

Qualification tells the reader where vocabulary comes from. Once `Customers.find` and `Orders.find` exist, an unqualified `find` would be ambiguous even if the compiler could resolve it.

Definitions inside a module are indented beneath it. Module-level bindings are initialized when the module loads. A module is not a class and needs no construction; its main job is to organize names.

Accessibility can hide helpers:

```fsharp
let private normalize (text: string) = text.Trim().ToLowerInvariant()
```

Public functions can use the private implementation while callers see a smaller surface. In a single script, modules also prevent the many `Id` and `Error` names of a larger domain from becoming ambiguous.

## Hide construction to establish a guarantee

The earlier `BookId` wrapper still allowed `BookId -1`. A module can make the union representation private:

```fsharp
module BookId =
    type BookId = private BookId of int

    let create value =
        if value > 0 then
            Ok (BookId value)
        else
            Error "Book ID must be positive"

    let value (BookId value) =
        value
```

Outside the module, callers cannot write `BookId -1`; the public route is `create`. Values exposed by this module can therefore be guaranteed to have passed its constructor, provided the module's own implementation does not create invalid cases. Privacy turns a caller convention into an enforced public boundary.

A namespace organizes names across a .NET codebase; it cannot directly contain values. This playground mostly needs modules, while larger F# codebases commonly combine namespaces with modules and types.

## Shape the public vocabulary

Callers should need `Catalog.create`, `Catalog.search`, and the catalog types, not the details of lowercase normalization. Making `normalize` private communicates that it may change without affecting callers.

Avoid opening every module globally. `Catalog.find` is often clearer than an unqualified `find`, especially once `Customers.find` and `Orders.find` exist. `open` is most useful for a focused scope or a module whose vocabulary is unmistakable.

## Modules versus namespaces

A module can contain types, values, and functions. It is a named group of definitions, not an object that callers construct. A namespace organizes types and modules across source files but cannot directly contain `let` bindings. Scripts naturally use modules; multi-file libraries often place modules and types inside a namespace.

Modules help when related names need qualification, privacy, or a clear public vocabulary. File length alone is a poor reason to add one.

## Try it

- Move catalog search into a module.
- Call it qualified, then with `open`.
- Mark a helper private and try accessing it outside.

## Summary

Modules give domain vocabulary a home and control which details callers can see.
