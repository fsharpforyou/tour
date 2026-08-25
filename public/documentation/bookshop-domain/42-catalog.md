# Modeling the bookshop catalog

## What you will learn

Combine records, unions, maps, sets, wrappers, and pure queries into one coherent catalog.

The earlier `Book` records were deliberately small. A shop now needs stable identity, an ISBN, authors, genres, and a price:

```fsharp
type BookId = BookId of int
type Isbn = Isbn of string

type Author =
    { Name: string }

type Genre =
    | Fiction
    | History
    | Science

type Book =
    {
        Id: BookId
        Isbn: Isbn
        Title: string
        Authors: Author list
        Genres: Set<Genre>
        PriceInCents: int
    }
```

Money is represented as integer cents in this teaching domain. That avoids introducing binary floating-point rounding into order totals. A production .NET system might instead choose `decimal` or a dedicated money type and would document its rounding and currency policy explicitly.

## Index by stable identity

```fsharp
type Catalog = Map<BookId, Book>

let add book catalog =
    catalog |> Map.add book.Id book

let find bookId catalog =
    catalog |> Map.tryFind bookId
```

`Catalog` is a type abbreviation because it names an existing map shape. `BookId` remains a wrapper because confusing it with another identifier would be a bug.

`add` returns a successor catalog; the original map remains available. `find` returns `Book option` because an ID may be absent.

## Search is a transformation, not a mutation

```fsharp
let all catalog =
    catalog
    |> Map.toList
    |> List.map (fun (_, book) -> book)

let normalize (text: string) =
    text.Trim().ToLowerInvariant()

let titleContains query book =
    let wanted = normalize query
    let title = normalize book.Title
    title.Contains(wanted)

let search query catalog =
    catalog
    |> all
    |> List.filter (titleContains query)
```

The query produces a list view and leaves the indexed catalog unchanged. Search can later grow to inspect authors or genres without changing how books are stored.

## Preserve useful distinctions

Authors stay as records, ISBN stays distinct from plain text, and genres stay in a set because duplicates have no meaning. The model grows by preserving guarantees we have already earned.

Stock belongs outside `Book` because catalog metadata and inventory change for different reasons. The next lesson gives inventory its own model.

## Experiment

- Add another author and another genre.
- Search with different capitalization and surrounding spaces.
- Look up a missing ID and handle `None`.
- Add a second book without changing the query functions.

## Summary

A catalog is an immutable index of explicit book values. Queries derive views; updates return a new index; separate concepts such as inventory remain separate.
