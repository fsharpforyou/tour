# Modeling relationships with records

## What you will learn

Several focused record types communicate more than one oversized bundle of primitive fields.

```fsharp
type Author = { Name: string; Country: string }
type Book = { Title: string; Author: Author; PageCount: int }
```

The `Author` field contains another record. Access nested fields with `book.Author.Name`.

The relationship is part of the type: `Book.Author` must be an `Author`, not an arbitrary string. If the shop later needs an author's country, the model already says where that fact belongs.

Functions can preserve the model's immutability:

```fsharp
let retitle newTitle book =
    { book with Title = newTitle }
```

Putting the record last permits `book |> retitle "New title"`. A record is data, not a class that must own every operation. Plain functions make transformations explicit.

Use records when field names matter and several values belong together. There is no need to wrap every string immediately; add precision when it solves a real modeling problem.

## Domain step

A catalog book and an inventory item describe related but distinct facts:

```fsharp
type InventoryItem =
    {
        Book: Book
        QuantityOnHand: int
    }
```

`Book` holds descriptive information. `InventoryItem` holds the stock fact that changes as the shop receives and sells copies. One book value can safely appear in several immutable calculations.

```fsharp
let stocked =
    { Book = book; QuantityOnHand = 3 }

let afterSale =
    { stocked with QuantityOnHand = stocked.QuantityOnHand - 1 }
```

The update creates a new value. `stocked.QuantityOnHand` remains `3`; `afterSale.QuantityOnHand` is `2`.

## Ask what one value represents

`QuantityOnHand` does not belong on `Book`. A title and its current stock have different lifecycles: changing stock should not mean rewriting bibliographic information. The useful question is “what does this value represent?”

Likewise, one giant record containing book metadata, customer contact, stock, and order facts would make every function depend on unrelated fields. Split records when the domain describes distinct things. Keep simple data simple; introduce a separate type when it clarifies a real boundary.

## Try it

- Add a nested publisher record.
- Write `sellOne` with copy-and-update.
- Confirm the original inventory item retains its quantity.

## Summary

Nested records describe relationships without merging distinct concepts. Functions over records keep data and transformations clear while the model evolves.
