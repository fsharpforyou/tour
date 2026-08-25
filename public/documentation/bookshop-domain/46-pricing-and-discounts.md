# Pricing and discounts

## What you will learn

Turn cart lines into priced lines and compose subtotal, discount, and shipping calculations without floating-point money.

```fsharp
type Money = Money of cents: int

let add (Money left) (Money right) =
    Money (left + right)

let multiply quantity (Money unitPrice) =
    Money (quantity * unitPrice)
```

The wrapper prevents a money value from being passed where a count is expected. Integer cents keep the addition and integer multiplication shown here exact in this single-currency model. The public case still permits negative amounts, and the type carries no currency; those constraints would need validated construction and a richer model if the domain required them.

## Snapshot the price used by an order

```fsharp
type PricedLine =
    {
        BookId: BookId
        Title: string
        Quantity: int
        UnitPrice: Money
        LineTotal: Money
    }
```

An order should not recalculate old totals from today's catalog. A priced line records the title and unit price accepted at checkout.

```fsharp
let priceLine book quantity =
    {
        BookId = book.Id
        Title = book.Title
        Quantity = quantity
        UnitPrice = book.Price
        LineTotal = multiply quantity book.Price
    }
```

## Fold lines into a subtotal

```fsharp
let subtotal lines =
    lines
    |> List.fold (fun total line -> add total line.LineTotal) (Money 0)
```

The accumulator and every line total have the same `Money` type. The compiler prevents accidentally adding a quantity directly.

## Model discount policy explicitly

```fsharp
type Discount =
    | NoDiscount
    | PercentOff of int

let applyDiscount discount (Money subtotal) =
    match discount with
    | NoDiscount -> Money subtotal
    | PercentOff percent ->
        Money (subtotal * (100 - percent) / 100)
```

For non-negative values, integer division here discards fractions of a cent. That is the function's rounding policy, and a real system should name it, define behavior for negative values, and validate the allowed percentage range.

Shipping can remain another function from an order subtotal and method to `Money`. A final total is then a composition of named calculations, not one opaque arithmetic expression.

## Experiment

- Price two quantities of one book.
- Fold several priced lines into a subtotal.
- Apply a 10-percent discount and calculate the exact cents.
- Add a `FixedAmountOff of Money` case and update the match.

## Summary

Pricing functions transform typed money values. Order lines snapshot accepted facts, while explicit discount cases make policy and rounding visible.
