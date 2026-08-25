# Shipping orders and deriving reports

## What you will learn

Complete the order lifecycle, then derive useful summaries from immutable shop
state without storing duplicate facts.

## Shipping must preserve payment

A shipped order still needs its payment history. Carry that value into the next
status instead of replacing it with a bare `Shipped` flag:

The `PaymentMethod`, `Payment`, and pricing fields remain exactly as they were
in the previous lesson. The model grows by adding one status case; shipping
does not discard or redefine facts the order already contains.

```fsharp
type OrderStatus =
    | AwaitingPayment
    | Paid of Payment
    | Shipped of payment: Payment * trackingNumber: string
    | Cancelled of reason: string
```

The shipping transition accepts only a paid order:

```fsharp
type ShippingError =
    | PaymentRequired
    | OrderClosed

let ship trackingNumber order =
    match order.Status with
    | Paid payment ->
        Ok { order with Status = Shipped (payment, trackingNumber) }
    | AwaitingPayment ->
        Error PaymentRequired
    | Shipped _
    | Cancelled _ ->
        Error OrderClosed
```

Two patterns can share one result when their handling is identical. The line
beginning with `| Cancelled _` continues the pattern alternative; it is not a
new result branch.

This version accepts blank tracking text because that validation is not yet
encoded. A validated `TrackingNumber` wrapper would move the rule to the input
boundary exactly as earlier wrappers did for IDs and email addresses.

## Reports are queries, not stored state

Suppose the shop stores orders in `Map<OrderId, Order>`. A report begins by
deriving the current values:

```fsharp
let allOrders state =
    state.Orders
    |> Map.toList
    |> List.map (fun (_, order) -> order)
```

Do not also store `PaidOrderCount`, `CancelledOrderCount`, and
`ShippedOrderCount` in the state. Those fields could drift away from the order
map. Derive them when needed.

## Group by a reporting category

Payload values make complete statuses unsuitable as report keys: two `Paid`
values containing different payment days are different values. First classify
each status into a payload-free category:

```fsharp
type StatusCategory =
    | AwaitingPaymentCategory
    | PaidCategory
    | ShippedCategory
    | CancelledCategory

let statusCategory status =
    match status with
    | AwaitingPayment -> AwaitingPaymentCategory
    | Paid _ -> PaidCategory
    | Shipped _ -> ShippedCategory
    | Cancelled _ -> CancelledCategory
```

Then group and count:

```fsharp
let orderCountsByStatus state =
    state
    |> allOrders
    |> List.groupBy (fun order -> statusCategory order.Status)
    |> List.map (fun (category, orders) -> category, List.length orders)
```

The report is a derived list. Running it twice does not modify the state.

## Fold a monetary summary

```fsharp
let revenue orders =
    orders
    |> List.filter (fun order ->
        match order.Status with
        | Paid _
        | Shipped _ -> true
        | AwaitingPayment
        | Cancelled _ -> false)
    |> List.fold (fun total order -> Money.add total order.Total) Money.zero
```

The business question determines which states count as revenue. If payment can
later be refunded, the model and report must change together.

## Experiment

- Try shipping an awaiting-payment order.
- Ship a paid order and confirm its payment remains available.
- Add orders in all four states and predict the grouped counts.
- Change the revenue rule to count only shipped orders.
- Introduce a validated tracking-number wrapper.

## Summary

Shipping is another explicit state transition. Reports classify, filter, group,
and fold authoritative order values; they do not create competing stored facts.
