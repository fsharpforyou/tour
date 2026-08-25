# Function signatures and small pure functions

## What you will learn

Function signatures expose design choices, and small pure functions make a larger order workflow easier to assemble and reason about.

## Types describe a design before an implementation

A signature now tells us far more than syntax. It names the information a function requires and the outcomes it can produce:

```text
findBook     : BookId -> Catalog -> Book option
priceLine    : Book -> CartLine -> PricedLine
validateCart : Catalog -> Inventory -> Cart -> Result<PricedLine list, CheckoutError>
```

The algorithms remain hidden, but callers already know what to supply and what to handle. The first lookup may return nothing; the last operation can explain why checkout failed.

Write annotations at meaningful boundaries:

```fsharp
let findBook (bookId: BookId) (catalog: Catalog) : Book option =
    catalog |> Map.tryFind bookId
```

Inside small helpers, inference keeps implementations uncluttered. At a domain boundary, annotations document the contract and resolve ambiguous record fields or overloaded members.

## Narrow inputs expose real dependencies

This rule receives an entire shop state:

```fsharp
let hasEnoughStock shop line =
    match shop.Inventory |> Map.tryFind line.BookId with
    | None -> false
    | Some stock -> line.Quantity <= available stock
```

The arithmetic itself needs only two numbers:

```fsharp
let hasEnough availableQuantity requestedQuantity =
    requestedQuantity > 0 && requestedQuantity <= availableQuantity
```

The second function is easier to understand and reuse because unrelated catalog, customer, and order values cannot influence it. The lookup belongs in a coordinating function; the stock rule does not.

Narrow dependencies should not erase meaning. A validated `PricedLine` is more cohesive than four unrelated primitive arguments. Accept a record when the whole record is the concept the function needs.

## Let outputs tell the necessary truth

```text
CartLine -> bool
CartLine -> Result<unit, CheckoutError>
CartLine -> Result<PricedLine, CheckoutError>
```

The boolean answers only yes or no. The unit result can explain refusal. The final signature also constructs the successful value needed by the next step. Choose the smallest output that preserves information the caller genuinely needs.

## Extract the real steps

A large checkout block may contain these smaller operations:

```fsharp
let validateQuantity line =
    if line.Quantity > 0 then Ok line
    else Error (InvalidQuantity line.BookId)

let calculateLineTotal (Money price) quantity =
    Money (price * quantity)

let createPricedLine book line =
    {
        BookId = book.Id
        Quantity = line.Quantity
        UnitPrice = book.Price
        Total = calculateLineTotal book.Price line.Quantity
    }
```

Extract a binding when it names a meaningful rule, has a coherent signature, or appears in more than one workflow. A wrapper that only renames obvious syntax adds little.

## Pure functions as the default domain tool

A pure function depends only on its inputs, returns a value, and produces no observable side effect. F# is not purely functional: it also supports printing, mutation, exceptions, and objects.

```fsharp
let shippingCost method subtotal =
    match method with
    | Collection -> Money 0
    | StandardPost -> Money 500
    | ExpressPost when subtotal >= Money 5000 -> Money 0
    | ExpressPost -> Money 900
```

The function receives policy facts and produces a value. It reads no clock, updates no order, and prints nothing.

Keep effects at the edge:

```fsharp
let report = buildOrderReport orders
printfn "%s" report
```

## Composition should clarify, not conceal

Composition can make a reusable transformation:

```fsharp
let normalizeQuery = trim >> lowercase
```

But checkout depends on several named values and branching errors. Explicit arguments, matches, and local bindings are clearer there. Point-free code is an option, not an achievement level.

## Review signatures before bodies

Ask:

1. Does every input affect the result?
2. Is required context explicit rather than global?
3. Does absence need `Option`, or refusal need `Result`?
4. Does success carry the trustworthy value needed next?
5. Is one record a cohesive concept or an oversized bag of context?

## Experiment

- Write signatures for catalog lookup, pricing one line, and placing an order before implementing them.
- Narrow a stock rule from `Shop -> CartLine -> bool` to its actual facts.
- Move `printfn` out of a calculation.
- Compare a short composition with a version using named intermediate values.

## Summary

Signatures are executable design constraints. Small pure functions work best when each type describes one meaningful responsibility and larger workflows coordinate their returned values explicitly.
