# Order states and valid transitions

## What you will learn

Use a discriminated union and Result-returning functions to permit only meaningful order transitions.

Several boolean flags make contradictory orders possible:

```text
IsPaid = false
IsShipped = true
IsCancelled = true
```

An order status should instead be exactly one possibility:

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
    | Shipped of trackingNumber: string
    | Cancelled of reason: string
```

The payload for each case exists only when it is meaningful.

## Transitions are functions

```fsharp
type TransitionError =
    | AlreadyPaid
    | PaymentRequired
    | OrderClosed

let pay payment order =
    match order.Status with
    | AwaitingPayment -> Ok { order with Status = Paid payment }
    | Paid _ -> Error AlreadyPaid
    | Shipped _
    | Cancelled _ -> Error OrderClosed
```

The function receives the current order and returns either a complete successor or a typed refusal. The original order stays unchanged.

Shipping requires payment:

```fsharp
let ship trackingNumber order =
    match order.Status with
    | Paid _ -> Ok { order with Status = Shipped trackingNumber }
    | AwaitingPayment -> Error PaymentRequired
    | Shipped _
    | Cancelled _ -> Error OrderClosed
```

Pattern matching makes the transition table visible. When these matches list cases explicitly instead of using a wildcard, adding a status produces an incomplete-match warning wherever the new case has not been considered.

Write the policy as a table before changing the functions:

| Current status | Pay | Ship |
| --- | --- | --- |
| Awaiting payment | become paid | `PaymentRequired` |
| Paid | `AlreadyPaid` | become shipped |
| Shipped | `OrderClosed` | `OrderClosed` |
| Cancelled | `OrderClosed` | `OrderClosed` |

Each cell becomes one pattern-match branch. The table is not stored separately
in the program; it is a way to check that the implementation covers the policy
without vague fall-through behavior.

## Preserve facts needed later

The first shipping model stores only a tracking number:

```fsharp
| Shipped of trackingNumber: string
```

That is sufficient for the current operations, but it discards payment details
when an order ships. If reports or refunds later need those details, preserve
them in the shipped case:

```fsharp
| Shipped of payment: Payment * trackingNumber: string
```

Choosing a union payload is a domain decision: keep the facts later behavior
must know, without copying unrelated data into every case.

## Trace the composed transition

```fsharp
order |> pay payment |> Result.bind (ship "TRACK-001")
```

`pay payment order` produces `Result<Order, TransitionError>`. `ship` needs a
plain `Order`, so `Result.bind` calls it only for `Ok paidOrder`. If payment
fails, the same error continues and shipping is not attempted.

## Commands and events are optional modeling tools

A larger system may distinguish requests such as `PayOrder` from accepted facts such as `OrderPaid`. That can be valuable, but it is not required to write functional workflows. Direct functions of shape:

```text
Order -> Result<Order, TransitionError>
```

give us a simpler starting point and keep the focus on F# instead of an architectural pattern.

## Experiment

- Pay an awaiting order and then try to pay it again.
- Attempt to ship before payment.
- Add cancellation that is allowed only before shipping.
- Add a `Refunded` state and follow the compiler errors through every transition.
- Change `Shipped` to retain its `Payment`, then update construction and matches
  until the warnings disappear.

## Summary

A union enumerates valid lifecycle states. Transition functions make allowed movement explicit and return new domain values without hidden mutation.
