# Placing orders

## What you will learn

Catalog, customer, cart, inventory, pricing, and order values can cooperate in one immutable order-placement transformation that returns either an error or a complete successor state.

## Familiar pieces now cooperate

The shop has grown one concern at a time: catalog books, validated customers, inventory, carts, money, discounts, shipping, and order states. We will connect them through one workflow—placing an order—before adding payment, shipping, and reports in the next lessons.

```fsharp
type Shop =
    {
        Catalog: Map<BookId, Book>
        Customers: Map<CustomerId, Customer>
        Inventory: Map<BookId, Stock>
        Orders: Map<OrderId, Order>
        NextOrderId: int
    }
```

The state stores authoritative facts. Search results, available quantities, discount amounts, and report groups are calculated when needed.

## Place an order in explicit stages

```text
customer ID + cart + shipping + shop
→ find customer
→ reject an empty cart
→ validate and price every line
→ reserve inventory
→ calculate subtotal, discount, and shipping
→ construct an awaiting-payment order
→ return the successor shop and order
```

Each stage either returns a value needed by the next stage or an explicit error.

## Validate one line and reserve its stock

```fsharp
let prepareLine catalog inventory line =
    match Map.tryFind line.BookId catalog, Map.tryFind line.BookId inventory with
    | None, _ -> Error (BookNotFound line.BookId)
    | _, None -> Error (NotEnoughStock (line.BookId, 0))
    | Some _, Some _ when line.Quantity <= 0 ->
        Error (InvalidQuantity line.BookId)
    | Some book, Some stock when line.Quantity <= available stock ->
        let priced = priceLine book line
        let updatedStock = { stock with Reserved = stock.Reserved + line.Quantity }
        let updatedInventory = inventory |> Map.add line.BookId updatedStock
        Ok (priced, updatedInventory)
    | Some _, Some stock ->
        Error (NotEnoughStock (line.BookId, available stock))
```

The result contains both consequences that must remain together: the accepted price snapshot and successor inventory.

## Fold while carrying Result and inventory

```fsharp
let prepareLines catalog initialInventory lines =
    lines
    |> List.fold
        (fun state line ->
            state
            |> Result.bind (fun (pricedLines, inventory) ->
                prepareLine catalog inventory line
                |> Result.map (fun (priced, nextInventory) ->
                    priced :: pricedLines, nextInventory)))
        (Ok ([], initialInventory))
    |> Result.map (fun (reversed, inventory) ->
        List.rev reversed, inventory)
```

Each line sees inventory already reserved by earlier lines. This matters if a manually constructed cart contains the same book twice. After the first failure, the fold still visits the remaining list cells, but `Result.bind` skips their preparation. No partially updated state escapes because every update exists only inside the Result accumulator.

## Calculate policy after validation

```fsharp
let subtotal = pricedLines |> List.fold addLineTotal (Money 0)
let discount = calculateDiscount customer subtotal
let shipping = shippingCost shippingMethod subtotal
let total = subtotal |> subtract discount |> add shipping
```

These are pure calculations over validated values. The order snapshots all three amounts so later catalog or membership changes cannot rewrite history.

## Return the successor and the created value

```text
placeOrder : CustomerId -> Cart -> ShippingMethod -> Shop
          -> Result<Shop * Order, CheckoutError>
```

The caller needs the successor shop for later commands and the created order for display or payment. Returning a tuple keeps both related outcomes together.

On `Error`, no successor `Shop` is returned and the original value remains unchanged. On `Ok`, the returned shop contains both the inventory reservation and the new order. This is an all-or-error property of the pure value transformation, not a database transaction or concurrency guarantee.

## Experiment

- Place the sample order and inspect reserved inventory.
- Request more copies than are available.
- Use a missing customer or book ID.
- Add a second cart line and trace the fold accumulator.
- Change membership and compare discount and total.

## Summary

A substantial workflow can still be built from small transformations. Here, immutable state and Result ensure that callers receive either a complete successor value or an error, without hidden mutation.
