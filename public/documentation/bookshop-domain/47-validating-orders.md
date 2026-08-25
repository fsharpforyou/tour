# Validating an order draft

## What you will learn

Combine customer, cart, shipping, catalog, and inventory facts into a validated order draft.

Checkout is the first workflow that crosses several parts of the shop. A request can fail for different reasons:

```fsharp
type CheckoutError =
    | EmptyCart
    | CustomerNotFound
    | BookNotFound of BookId
    | InvalidQuantity of BookId
    | InsufficientStock of BookId * available: int
```

The error cases carry only information that helps the caller understand or present the refusal.

## Validate one line

```fsharp
let validateLine catalog inventory line =
    match Map.tryFind line.BookId catalog, Map.tryFind line.BookId inventory with
    | None, _ -> Error (BookNotFound line.BookId)
    | _, None -> Error (InsufficientStock (line.BookId, 0))
    | Some book, Some stock when line.Quantity <= 0 ->
        Error (InvalidQuantity line.BookId)
    | Some _, Some stock when line.Quantity > available stock ->
        Error (InsufficientStock (line.BookId, available stock))
    | Some book, Some _ ->
        Ok (priceLine book line.Quantity)
```

Tuple matching considers two lookups together. Guards add rules that depend on the successful values.

## Validate every line with a fold

```fsharp
let validateLines catalog inventory lines =
    lines
    |> List.fold
        (fun result line ->
            result
            |> Result.bind (fun validated ->
                validateLine catalog inventory line
                |> Result.map (fun priced -> priced :: validated)))
        (Ok [])
    |> Result.map List.rev
```

The accumulator is `Result<PricedLine list, CheckoutError>`. Each successful line is prepended efficiently. `List.rev` restores cart order once at the end. After the first error, `Result.bind` no longer calls `validateLine` for later elements, although `List.fold` still steps through the remaining list.

Here, `fold`, `Result.bind`, and immutable lists come together naturally. Their types coordinate a larger rule from ideas we already understand separately.

## Build only after validation

```fsharp
type OrderDraft =
    {
        Customer: Customer
        Lines: PricedLine list
        Shipping: ShippingMethod
        Total: Money
    }
```

Construct the draft only after the customer and every line have been validated. Later functions can rely on its lines having positive quantities, known books, accepted prices, and sufficient stock for each checked line at validation time. This lesson assumes cart operations have merged duplicate book IDs; lesson 60 carries successor inventory through the fold so aggregate stock remains correct even if that assumption is broken.

## Experiment

- Validate an empty cart before validating its lines.
- Make the second cart line unavailable and confirm the first error is retained.
- Reverse the cart and observe which stock error is reported first.
- Explain why the final `List.rev` is required.

## Summary

A validation workflow turns several loose inputs into one trustworthy domain value. Carrying `Result` through the fold ensures that partial output never escapes after a failure.
