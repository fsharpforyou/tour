# Records and unions together

## What you will learn

Records describe information that exists together; unions describe which alternative exists.

```fsharp
type ListingState =
    | ForSale
    | SoldOut
    | Discontinued of reason: string

type BookListing =
    { Title: string; Price: float; State: ListingState }
```

The record always has a title, price, and state. Its `State` is exactly one possibility. Update the outer record while constructing a new union value:

```fsharp
let markSoldOut listing =
    { listing with State = SoldOut }
```

This first version is still permissive: it can mark an already discontinued listing as sold out. The type removes contradictory *states*; later, functions returning `Result` will enforce valid *transitions*.

Pattern matching expresses queries:

```fsharp
let mayOrder listing =
    match listing.State with
    | ForSale -> true
    | SoldOut -> false
    | Discontinued _ -> false
```

The underscore shows that this function does not need the discontinuation reason. That reads more clearly than coordinating several boolean flags and unrelated string fields.

Choose records for “and”: a listing has a title *and* price *and* state. Choose unions for “or”: its state is for sale *or* sold out *or* discontinued.

## Trace an update

Given a for-sale listing, `markSoldOut listing` constructs `SoldOut` and places it in a new record. The original listing remains for sale.

That matters when several functions share the old value: each sees the same immutable facts. The caller deliberately chooses whether to keep the returned successor.

## Put facts where they are meaningful

`Title` and `Price` remain record fields because every listing has them. A discontinuation reason belongs inside `Discontinued` because it is meaningful only in that state.

Use this practical rule while modeling:

- facts that always coexist belong in a record;
- facts relevant to one alternative belong in that union case;
- operations that transform values belong in functions.

## Read the types from the outside inward

`BookListing` is one record type. Its `State` field contains one
`ListingState` value. A value such as this therefore has two construction
layers:

```fsharp
let unavailable =
    {
        Title = "Kindred"
        Price = 9.99
        State = Discontinued "publisher request"
    }
```

`Discontinued "publisher request"` constructs the inner union value; the
braces construct the outer record. Pattern matching reverses that process: the
match first reads the `State` field and then deconstructs the selected case.

A separate `Reason: string` field would be present for every state. That would
force meaningless values such as an empty reason on a for-sale listing. Putting
the reason in `Discontinued` makes it exist only in the alternative that needs
it.

## Structure and transition rules are different guarantees

The union prevents simultaneous states. It does not decide whether a move from
one state to another is allowed. `markSoldOut` currently accepts every listing,
including a discontinued one. Lesson 27 introduces a type for returning either
the successor or a reason for refusal.

## Try it

- Add a `ComingSoon of releaseDay: int` case and follow the warnings.
- Write `discontinue reason listing`.
- Confirm updates do not alter the original.
- Temporarily add `Reason: string` to the record. List the meaningless
  combinations it permits, then remove the field again.

## Summary

Records and unions complement each other. Together they give domain values stable structure and explicit alternatives.
