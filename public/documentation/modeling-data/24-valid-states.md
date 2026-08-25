# Making invalid states hard to represent

## What you will learn

Types can encode business rules so contradictory values cannot be constructed casually.

Consider flags:

```fsharp
type CustomerFlags = { IsActive: bool; IsSuspended: bool }
```

What does `{ IsActive = true; IsSuspended = true }` mean? Replace the contradictory combination with explicit states:

```fsharp
type CustomerStatus =
    | Active
    | Suspended of reason: string
    | Closed
```

Now a suspended customer must carry a reason, while an active customer cannot carry an irrelevant one.

The same improvement applies to an order:

```fsharp
type OrderState =
    | AwaitingPayment
    | Paid of paidOnDay: int
    | Cancelled of reason: string
```

An order has exactly one lifecycle state, so it cannot be both paid and cancelled.

Two independent booleans create four combinations whether the business recognizes them or not. More flags multiply that accidental state space. A union lists the intended alternatives directly, so adding a new possibility is an explicit domain change.

Three independent flags create eight boolean combinations. If the domain has
only four meaningful states, half of that representable space consists of
contradictions or undefined combinations. The problem grows faster than the
number of flags.

A union does not merely improve naming. It changes which values can be
constructed: callers choose one case rather than coordinating several fields.

The type cannot enforce every rule: a day may still be negative and a reason may still be blank. Validated constructors will tighten those primitive boundaries later. Good modeling develops in steps—first remove contradictory structures, then validate the remaining values that matter.

## Preserve the facts later questions need

Suppose payment reports need both the method and day. Put both facts in the relevant case:

```fsharp
type PaymentDetails =
    {
        Method: string
        PaidOnDay: int
    }

type OrderState =
    | AwaitingPayment
    | Paid of PaymentDetails
    | Cancelled of reason: string
```

The best model is not the one with the fewest fields. It is the one in which representable values are coherent and necessary questions are straightforward to answer.

## State shape and transition rules are different

The union determines which individual state values are coherent. It does not by
itself determine which changes are permitted. A later transition function might
allow `AwaitingPayment` to become `Cancelled`, while refusing to cancel a paid
order until a refund workflow exists.

This distinction prevents a common overclaim. A precise union can make
contradictory values unrepresentable, but business rules involving history or a
change from one valid value to another still belong in functions. The Result
lessons will give those transition functions an explicit error vocabulary.

## Try it

- Try representing “active and suspended” with `CustomerStatus`.
- Add `Shipped of trackingCode: string` to `OrderState`.
- Compare the information required by each case.
- List the boolean combinations needed to represent the same order states.
- Identify one rule encoded by the state shape and one requiring a transition function.

## Summary

Precise unions move assumptions from comments into checked data shapes. Good models make valid values natural and invalid combinations difficult.
