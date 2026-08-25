# Objects and classes

## What you will learn

F# can define and consume classes with constructors, properties, and methods
while records and functions remain available where they fit better.

## Why F# includes object-oriented features

Records, discriminated unions, and functions suit most of our bookshop domain. F# is also a multi-paradigm .NET language: it consumes object-oriented APIs and can define classes when they fit the boundary.

The goal is to read and use object-oriented F# without abandoning the functional model you already know.

## Classes and primary constructors

```fsharp
type Shelf(label: string, capacity: int) =
    member _.Label = label
    member _.Capacity = capacity
    member _.HasSpace(bookCount: int) =
        bookCount < capacity
```

The parameters after `Shelf` form its primary constructor. Construct an object with:

```fsharp
let shelf = Shelf("Science fiction", 40)
```

Class constructors use the parenthesized member-call style. Plain F# functions still use whitespace for application.

`Label` and `Capacity` are read-only properties:

```fsharp
shelf.Label
shelf.Capacity
```

`HasSpace` was defined with a parenthesized, .NET-style argument list, so its call uses parentheses:

```fsharp
shelf.HasSpace(37)
// true
```

The underscore in `member _.Label` means the current object is not needed. Name it `this` when one member calls another:

```fsharp
member this.Describe() =
    $"%s{this.Label}: %d{this.Capacity} spaces"
```

Constructor parameters are in scope throughout the class body but are not automatically public properties. Expose only the members callers should use.

## Properties are not guaranteed to be pure

A property reads like data and a method reads like an operation, but that convention says nothing about purity. Either may hide mutation or other effects, so check the contract of an unfamiliar API.

## Class or record?

A record is usually the simpler representation for immutable domain facts. It
provides named fields, structural equality, pattern matching, and copy-and-update
without writing members.

A class is useful when construction and an object-shaped API belong together,
when identity or encapsulated state matters, or when an API expects ordinary
.NET members. A class does not automatically make a model more realistic.

```fsharp
type ShelfFacts = { Label: string; Capacity: int }

let hasSpace bookCount shelf =
    bookCount < shelf.Capacity
```

This record and function may express the shelf rule more directly than a class.
The class version may fit better when callers already work through member calls.
Compare actual use sites rather than choosing by habit.

## Working with the wider .NET ecosystem

Classes, methods, and properties belong to the .NET type system. F# can use types written in other .NET languages, and those languages can use suitably exposed F# types. Reflection, inheritance-heavy design, and application infrastructure are beyond this tour. The next lesson introduces interfaces as a separate idea.

## Experiment

- Add a `Describe()` method that uses two properties through `this`.
- Express the same immutable shelf facts as a record and compare construction and use.
- Try accessing a constructor parameter that was not exposed as a property.

## Summary

F# domain logic can remain function-and-data oriented while interoperating
comfortably with objects. Classes combine construction with an object-shaped
member API; records and functions often remain simpler for immutable facts.
