# Final capstone: the functional bookshop

## What you will learn

Read, trace, and change one program that connects the domain types, validation,
collections, immutable state, and order transitions developed throughout the
course.

## This program grew rather than appeared

The playground is longer than earlier examples, but its ideas are familiar. It
combines the same pieces you have already used:

1. constrained values at input boundaries;
2. records for books, customers, carts, stock, and orders;
3. unions for alternatives and lifecycle states;
4. maps and sets for the catalog, inventory, orders, and genres;
5. options for contact information and lookups;
6. results for validation and business refusal;
7. folds for pricing, reservation, release, and reporting;
8. pure state transitions for placement, payment, cancellation, and shipping.

Read one section at a time. The function signatures are the seams between them.

## Validate primitive input once

An `int` can represent a quantity, a price, or three different kinds of ID. A
`string` can represent an ISBN, email address, card detail, or tracking number.
The capstone gives those values different types and puts construction rules next
to them:

```fsharp
type DiscountPercent = private DiscountPercent of int

module DiscountPercent =
    let create value =
        if value >= 0 && value <= 100 then
            Ok (DiscountPercent value)
        else
            Error (InputError "discount must be between 0 and 100")
```

Code outside the defining module cannot construct the private case directly. It
must handle the `Result` from `create`. Once creation succeeds, pricing code can
rely on the range without checking it again.

The validators are deliberately modest. The email rule checks a useful local
shape; it does not claim to decide whether an address exists. The ISBN rule
checks length and digits, not the ISBN checksum. A type should promise exactly
what its constructor establishes.

The ISBN constructor uses `Seq.forall` to check every character. It has the same
all-elements meaning as `List.forall`, but accepts any sequence, including the
characters produced by a string.

## Keep invalid stock out of the model

`Stock.create` rejects a negative on-hand quantity. The record case is private,
so callers cannot bypass that entry point. Reservation returns `Some` only when
the requested quantity is positive and available:

```text
Stock.reserve : int -> Stock -> Stock option
```

The stock operation does not know about checkout errors. `prepareLine` gives a
failed reservation its domain meaning by returning `NotEnoughStock`. This keeps
the small stock type reusable while the workflow still reports a precise error.

## Query the catalog without changing it

`searchCatalog`, `booksInGenre`, and `lowStock` derive answers from the current
state. They demonstrate three different collection questions:

- filter books whose normalized titles contain a query;
- keep books whose genre set contains a value;
- choose only inventory entries whose available quantity is low.

None of these answers is stored alongside the source maps. Storing both would
create two facts that could disagree.

## Turn cart intent into an accepted order

The cart contains requests. The order contains accepted facts. Placement crosses
that boundary:

```text
cart + shipping method + current state
→ find the customer
→ reject an empty cart
→ find, validate, price, and reserve every line
→ calculate discount and shipping
→ construct an awaiting-payment order
→ return the successor state and new order
```

`prepareLines` carries a `Result` containing both the priced lines and the
successor inventory. Each reservation therefore sees reservations made for
earlier lines. If any line fails, no successor `State` escapes.

The order snapshots title, unit price, discount, shipping cost, and total. A
later catalog price change should not rewrite an order already accepted.

## Model money operations at the level of the rule

The earlier version exposed a subtraction function that silently clamped a
negative answer to zero. That made an invalid calculation look valid. The final
model instead exposes the operations the policy needs:

```fsharp
DiscountPercent.discountAmount percent subtotal
DiscountPercent.apply percent subtotal
```

Because a `DiscountPercent` is between 0 and 100, applying it cannot make a
non-negative `Money` value negative. The useful invariant follows from the input
types rather than a hidden correction inside arithmetic.

This course models money as whole minor units, such as cents, in one implicit
currency. Multi-currency arithmetic and rounding policies would require more
domain information.

## Preserve errors through the whole scenario

The sample does not replace errors with an empty cart or an earlier order. Each
step feeds success into the next step with `Result.bind`:

```fsharp
addToCart bookId 2 emptyCart
|> Result.bind (fun cart -> placeOrder StandardPost cart initial)
|> Result.bind (fun (placed, order) ->
    pay order.Id payment placed
    |> Result.bind (ship order.Id tracking))
```

The final match prints either the first error or reports over the shipped state.
No branch pretends that failure was success.

The `expect` helper used while creating fixed sample data is different. Invalid
hard-coded seed data is a programmer mistake in this playground, so the helper
raises an exception with context. Cart, placement, and lifecycle failures are
expected domain outcomes and remain `Result` values.

## Cancellation restores reserved stock

Cancellation is allowed only while payment is pending. It updates the order and
releases every reserved line in the same returned state. Returning only the
cancelled order would leave inventory inconsistent.

This is still a teaching policy. A larger shop might allow cancellation after
payment through a refund workflow. The union and transition would then grow to
represent those additional states explicitly.

## Trace before running

Predict these values before pressing Run:

1. the cart contains one line with quantity two;
2. placement changes available stock from five to three;
3. payment changes only the stored status;
4. shipping preserves payment and adds a tracking number;
5. low-stock reporting includes the book at threshold three;
6. revenue includes the shipped order;
7. status counts contain one shipped order.

Then compare your prediction with the printed values. If one differs, locate the
smallest function responsible before changing the code.

## Experiment

- Predict the `ShopError`, then request zero copies and run the program.
- Request six copies. Confirm that no successor state is returned.
- Stop after placement, cancel the order, and inspect released inventory.
- Try a blank tracking number at the construction boundary.
- Add a second science book and check search, genre, and low-stock queries.
- Change the customer to `Standard` and calculate the expected total by hand.
- Deliberately pass a `CustomerId` where a `BookId` is required. Read the two
  types named by the compiler, then repair the call.

## Summary

The finished bookshop is a composition of ordinary F# ideas. Constrained values
establish trustworthy inputs; records and unions describe the domain; options
and results keep uncertainty visible; collection functions derive answers; and
pure transitions return complete successor states. The program is substantial
because these small pieces cooperate, not because the language changes at the
end.
