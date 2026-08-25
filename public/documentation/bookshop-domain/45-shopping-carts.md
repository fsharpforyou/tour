# Shopping carts as immutable transformations

## What you will learn

Build a cart from records and lists, with functions that add, change, and remove lines without hidden mutation.

```fsharp
type BookId = BookId of int

type CartLine =
    {
        BookId: BookId
        Quantity: int
    }

type Cart =
    { Lines: CartLine list }
```

The cart contains intent: which books the customer wants and in what quantity. Current price and stock remain outside because they can change independently and must be checked at checkout.

## Add one book

```fsharp
type CartError = InvalidQuantity

let addLine bookId quantity cart =
    if quantity <= 0 then
        Error InvalidQuantity
    else
        match cart.Lines |> List.tryFind (fun line -> line.BookId = bookId) with
        | None ->
            Ok { cart with Lines = { BookId = bookId; Quantity = quantity } :: cart.Lines }
        | Some existing ->
            let updatedLines =
                cart.Lines
                |> List.map (fun line ->
                    if line.BookId = bookId then
                        { line with Quantity = existing.Quantity + quantity }
                    else
                        line)

            Ok { cart with Lines = updatedLines }
```

There are two successful shapes: construct a new line, or replace the matching line with an updated copy. Neither branch changes the original list.

## Remove and change quantities

```fsharp
let removeLine bookId cart =
    {
        cart with
            Lines = cart.Lines |> List.filter (fun line -> line.BookId <> bookId)
    }
```

Removing a missing line currently returns an equivalent cart. If callers need to distinguish “removed” from “not present,” return `Result` or a tuple carrying that information.

Quantity changes can reuse removal for zero:

```fsharp
let changeQuantity bookId quantity cart =
    if quantity < 0 then
        Error InvalidQuantity
    elif quantity = 0 then
        Ok (removeLine bookId cart)
    else
        let updated =
            cart.Lines
            |> List.map (fun line ->
                if line.BookId = bookId then { line with Quantity = quantity }
                else line)

        Ok { cart with Lines = updated }
```

This simple version leaves a missing book unchanged. An alternative API could return `LineNotFound`; the right behavior depends on what callers need to know.

## Experiment

- Add the same book twice and predict its quantity.
- Add a second book and remove the first.
- Change a quantity to zero.
- Decide whether changing a missing line should be silent or explicit, then model that choice.

## Summary

A cart is immutable data. Its operations receive the current value and return either a complete successor or an explicit refusal.
