# Records: data with names

## What you will learn

Records define related data with names for every field.

```fsharp
type Book =
    {
        Title: string
        Author: string
        PageCount: int
    }
```

`type` introduces the type. Construct a value with matching field names:

```fsharp
let book =
    {
        Title = "Kindred"
        Author = "Octavia E. Butler"
        PageCount = 264
    }
```

Access fields with a dot: `book.Title`. The dot means “the named member or field on this value,” as it did for string members.

Unlike the earlier tuple `("Kindred", 264)`, this value carries field names in its type. Construction is all-or-nothing: the compiler rejects a missing field, an unknown field, or a field with the wrong type. That catches incomplete data at its boundary rather than when some later calculation happens to use it.

Record fields are immutable by default. Copy-and-update creates a new value:

```fsharp
let revised = { book with PageCount = 266 }
```

Plain functions make record transformations explicit:

```fsharp
let addPages additionalPages book =
    { book with PageCount = book.PageCount + additionalPages }

let corrected = book |> addPages 2
```

`book` is unchanged. The compiler normally infers a function parameter from distinctive fields; an annotation resolves ambiguity:

```fsharp
let describe (book: Book) = book.Title
```

By default, records support structural equality when all their field types support equality: two separate `Book` values with equal fields compare equal. F# attributes can explicitly disable generated equality for a record type, a detail you may encounter in wider code.

## Why the type definition matters

The compiler checks construction against the complete record shape. Omitting `PageCount`, misspelling `Author`, or using a string where an integer is expected produces an error at the value's boundary rather than much later. Field names also remove the positional uncertainty of `("Kindred", "Butler", 264)`.

Copy-and-update is shallow: updating `PageCount` copies every other field value unchanged. If a field refers to another object, the old and new records still refer to that same object. To change a nested record, update the nested value explicitly and place it in the outer copy.

## Formatting and compiler feedback

Prefer one field per line once a record grows. Semicolons are permitted in compact values, but vertical formatting makes additions and comparisons easier to scan.

If two record types share field names, inference may need an annotation such as `(book: Book)`. Annotate the parameter whose intended record type is ambiguous rather than annotating every local binding.

## Try it

- Add a `Year: int` field and fix construction.
- Create an updated title.
- Compare two equal book values.
- Omit one field and read which complete shape the compiler expected.

## Summary

Records replace positional data with explicit field names. With the default immutable fields, copy-and-update creates a revised value without changing the original.
