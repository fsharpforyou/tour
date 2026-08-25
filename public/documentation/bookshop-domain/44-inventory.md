# Inventory and stock levels

## What you will learn

Represent inventory separately from catalog metadata and express stock changes as checked transformations.

A book's title and ISBN rarely change. Its stock changes whenever copies arrive or orders reserve them. One record should not pretend those facts have the same lifecycle.

```fsharp
type BookId = BookId of int

type Stock =
    {
        BookId: BookId
        OnHand: int
        Reserved: int
    }

type Inventory = Map<BookId, Stock>
```

Available quantity is derived:

```fsharp
let available stock =
    stock.OnHand - stock.Reserved
```

Storing `Available` as another field would allow it to disagree with the two authoritative counts.

This public record can still be constructed with negative counts or with `Reserved` greater than `OnHand`. The transition functions below preserve sensible values when they start from sensible stock, but the type itself does not yet enforce that starting invariant. Private validated construction would be needed for that stronger guarantee.

## Checked stock changes

```fsharp
type StockError =
    | InvalidQuantity
    | InsufficientStock of available: int

let reserve quantity stock =
    if quantity <= 0 then
        Error InvalidQuantity
    elif quantity > available stock then
        Error (InsufficientStock (available stock))
    else
        Ok { stock with Reserved = stock.Reserved + quantity }
```

The success case contains a complete successor value. On failure, no successor is returned; the original stock value is unchanged in either case.

Releasing stock is another transition:

```fsharp
let release quantity stock =
    if quantity <= 0 || quantity > stock.Reserved then
        Error InvalidQuantity
    else
        Ok { stock with Reserved = stock.Reserved - quantity }
```

The same `InvalidQuantity` case covers several invalid inputs in this small model. A richer domain could distinguish negative quantities from releasing more than was reserved.

## Update the inventory map

```fsharp
let reserveBook bookId quantity inventory =
    match inventory |> Map.tryFind bookId with
    | None -> Error BookNotStocked
    | Some stock ->
        reserve quantity stock
        |> Result.map (fun updated -> inventory |> Map.add bookId updated)
```

The map handles lookup and replacement. The `reserve` function owns the arithmetic rule. Keeping those concerns separate lets the stock rule be understood without an inventory map.

## Experiment

- Reserve one available copy and inspect old and new stock.
- Request more than is available.
- Add a `restock` function for a positive delivery quantity.
- Explain why `Available` should remain a function rather than a stored field.

## Summary

Inventory is an index of stock values. Each operation checks the requested change and returns an updated stock or inventory value, leaving no hidden mutation behind.
