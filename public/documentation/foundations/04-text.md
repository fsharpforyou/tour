# Strings, characters, and text

## What you will learn

How to represent and combine text without treating strings as mysterious objects.

## Strings and characters

A string is text between double quotes:

```fsharp
let title = "A Wizard of Earthsea"
```

A `char` literal is one UTF-16 code unit between single quotes:

```fsharp
let shelfLetter = 'F'
```

A `char` and a one-character `string` are different F# types. A single visible Unicode symbol can sometimes require more than one UTF-16 code unit, so `char` is best for constrained values such as an ASCII category code. Use `string` for general text.

Escape sequences represent otherwise awkward characters:

```fsharp
let quoted = "She said, \"Read this.\""
let twoLines = "First line\nSecond line"
```

Strings concatenate with `+`:

```fsharp
let description = title + " by " + author
```

String interpolation is often clearer:

```fsharp
let description = $"%s{title} by %s{author}"
```

The `$` enables interpolation. Each inserted expression has a format specifier
that states the type of value expected: `%s` for a string, `%d` for an integer,
`%f` for a floating-point value, `%b` for a boolean, and `%c` for a character.
The expression to insert follows in braces. Typed interpolation lets the
compiler check the format and avoids leaving the inserted value's type
unconstrained.

```fsharp
let copies = 3
let price = 8.50
let stockLabel = $"%d{copies} copies at %f{price} each"
```

## A few common operations

Strings have members, accessed with a dot:

```fsharp
title.Length
title.ToUpper()
title.Contains("Earthsea")
```

`.Length` is a property: it is read like data and has no call parentheses. It counts UTF-16 code units, not necessarily user-perceived characters. `.ToUpper()` and `.Contains(...)` are methods and use parentheses for their arguments. Case conversion can depend on culture; use `.ToUpperInvariant()` when a culture-independent normalization rule is intended.

F# also provides functions in the `String` module:

```fsharp
let characterCount = String.length title
let divider = String.replicate 3 "-"
```

F# works comfortably with both object-oriented members and function-oriented modules. Compare their call styles:

```fsharp
title.Contains("Earthsea")  // receiver.Member(argument)
String.length title          // function argument
```

Choose the form that makes the operation clearest. There is no benefit in mechanically converting every member call into a module call.

## Indexing needs care

```fsharp
let firstLetter = title[0]
```

Indexes start at zero. Indexing an empty string or using an out-of-range index fails at runtime because the type `string` does not encode its length. Prefer operations such as `Contains` when you do not actually need a position.

## In the bookshop

We can produce a readable catalogue label and check a simple title search.

## Try it

- Add the page count to the interpolated description.
- Search for a different word with `.Contains`.
- Print the first character with `title[0]`.
- Write the same character count once with `.Length` and once with `String.length`.

## Summary

Strings and characters are typed values. Interpolation combines values into readable text; members provide common operations.
