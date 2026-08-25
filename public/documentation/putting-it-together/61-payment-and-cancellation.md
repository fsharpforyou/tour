# Payment and cancellation

## What you will learn

Represent order commands as checked state transitions that preserve the facts
needed by later transitions.

## Placement is only the beginning

The previous lesson created an order in `AwaitingPayment`. A boolean such as
`IsPaid` cannot explain when payment happened or prevent contradictory flag
combinations. The order status should describe its current lifecycle state:

```fsharp
type PaymentMethod =
    | Card of lastFourDigits: string
    | GiftCard of code: string

type Payment =
    {
        Method: PaymentMethod
        PaidOnDay: int
    }

type OrderStatus =
    | AwaitingPayment
    | Paid of Payment
    | Cancelled of reason: string
```

The `Paid` case carries the payment information because that information exists
only after payment. `Cancelled` carries the reason for the same reason.

## A transition receives the current value

```fsharp
type TransitionError =
    | AlreadyPaid
    | OrderAlreadyCancelled

let pay payment order =
    match order.Status with
    | AwaitingPayment ->
        Ok { order with Status = Paid payment }
    | Paid _ ->
        Error AlreadyPaid
    | Cancelled _ ->
        Error OrderAlreadyCancelled
```

Every branch explains one current state. Success returns a complete successor
order. Failure returns no successor and leaves the original value untouched.

Notice what the type does and does not guarantee. `OrderStatus` prevents one
status value from being both paid and cancelled. It does not prevent arbitrary
code from constructing `Paid` directly while the union cases remain public.
Keeping transitions in a module and exposing a smaller public surface can make
the intended route clearer.

## Cancellation has different rules

```fsharp
let cancel reason order =
    match order.Status with
    | AwaitingPayment ->
        Ok { order with Status = Cancelled reason }
    | Paid _ ->
        Error AlreadyPaid
    | Cancelled _ ->
        Error OrderAlreadyCancelled
```

This teaching domain refuses cancellation after payment because refunds have
not been modeled. A real domain might introduce `RefundPending`, `Refunded`, or
a separate refund workflow. The union should reflect the policy the program
actually implements, not an imagined universal order lifecycle.

## Update an order inside shop state

Orders are stored in a map, so the coordinating function first finds the order,
then applies the small transition, then stores the successor:

```fsharp
let updateOrder orderId transition state =
    match state.Orders |> Map.tryFind orderId with
    | None -> Error OrderNotFound
    | Some order ->
        transition order
        |> Result.map (fun updatedOrder ->
            {
                state with
                    Orders = state.Orders |> Map.add orderId updatedOrder
            })
```

`updateOrder` is higher-order: the caller supplies the transition. Payment can
partially apply `pay payment`; cancellation can partially apply
`cancel reason`.

```fsharp
let payOrder orderId payment state =
    updateOrder orderId (pay payment) state
```

The lookup concern and lifecycle rule remain separate, and both failures stay
explicit in `Result`.

## Trace before running

Starting from `AwaitingPayment`:

1. `payOrder` returns a state containing `Paid payment`.
2. Paying that returned state again produces `AlreadyPaid`.
3. Cancelling the original state succeeds because immutable values let both
   possible histories be explored independently.

That third point does not imply that a deployed application should accept two
concurrent histories. It shows only that pure transition functions do not
destroy their inputs.

## Experiment

- Pay the awaiting order and inspect the stored payment.
- Attempt to pay the returned state twice.
- Cancel the original unpaid state.
- Add a `PaymentDeclined` error and decide which function should produce it.
- Introduce a refund state before allowing cancellation of a paid order.

## Summary

Lifecycle unions record meaningful states and their associated facts. Transition
functions accept a current value and return either a complete successor or a
domain error, while a coordinator updates the surrounding map.
