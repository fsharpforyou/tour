# Interfaces and object abstraction

## What you will learn

An interface describes a set of object members without fixing the class that
implements them.

## Different objects can promise the same behavior

The previous lesson defined classes with members. A shelf and a customer label
are different classes, but both might know how to produce display text. An
interface names that shared object-oriented contract:

```fsharp
type IDisplayable =
    abstract member Display: unit -> string
```

`abstract member` declares the member without providing an implementation.
`unit -> string` means callers invoke `Display()` without meaningful input and
receive a string.

A class implements the contract in an interface block:

```fsharp
type Shelf(label: string, capacity: int) =
    member _.Label = label
    member _.Capacity = capacity

    interface IDisplayable with
        member _.Display() =
            $"%s{label}: space for %d{capacity} books"
```

Another class can make the same promise differently:

```fsharp
type CustomerLabel(name: string) =
    interface IDisplayable with
        member _.Display() = "Customer: " + name
```

## Depend on the contract

```fsharp
let display (item: IDisplayable) =
    item.Display()
```

The annotation says that `display` accepts any object implementing
`IDisplayable`. Inside the function, only members promised by that interface are
available. Constructor parameters and unrelated class members remain outside
the contract.

```fsharp
display (Shelf("Science fiction", 40))
display (CustomerLabel("Ada"))
```

The concrete objects differ, but the consuming function needs only their shared
behavior.

## Interface, union, or function parameter?

These tools express different kinds of variation:

- A discriminated union is closed: the type definition lists every case, and a
  match without a wildcard can be checked for exhaustiveness.
- An interface is open: new classes can implement the contract without changing
  its definition.
- A function parameter asks for one piece of behavior without requiring an
  object to implement a named contract.

Order status is a closed set of domain states, so a union fits. A .NET API may
ask for an interface implementation, so implement the interface. A pricing
operation that needs only `Money -> Money` can accept that function directly.

Object-oriented and functional styles are not opposing teams. Choose the
smallest representation that accurately states what callers and implementations
need from one another.

## Compiler clinic

If a class claims to implement `IDisplayable` but omits `Display`, the compiler
reports that the interface has not been completely implemented. If `display`
tries to access `.Capacity`, it fails because `IDisplayable` does not promise
that property—even though one concrete shelf happens to have it.

## Experiment

- Implement `IDisplayable` for an order label.
- Add a second member to the interface and follow the compiler errors.
- Replace the interface-consuming function with one accepting `unit -> string`.
- Decide which version states the requirement more directly for that one call.

## Summary

An interface defines an open object contract. Classes provide implementations,
and consumers can depend on the promised members without depending on one
concrete class.
