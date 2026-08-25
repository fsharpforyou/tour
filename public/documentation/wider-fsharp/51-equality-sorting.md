# Equality, sorting, and grouping

## What you will learn

Learn what structural equality compares, then sort and group with keys that state the domain question clearly.

By default, records and unions whose contents support equality receive structural equality:

```fsharp
let same = firstOrder = secondOrder
```

Tuples and lists likewise compare their contents when their element types support equality. Function values do not support structural equality. Floating-point equality follows floating-point rules, including the fact that `nan = nan` is false.

Equality is not domain identity. Two order values may contain equal fields but still represent different real orders if their `OrderId` values differ. Compare IDs when asking about identity; compare complete records only when complete value equality is the intended question.

## Sort with a visible key

```fsharp
let byTotal =
    orders |> List.sortBy (fun order -> order.Total)

let newestFirst =
    orders |> List.sortByDescending (fun order -> order.PlacedOnDay)
```

For several keys, return a tuple:

```fsharp
orders |> List.sortBy (fun order -> order.CustomerName, order.Id)
```

Tuple comparison uses the first part and then later parts to break ties. Prefer a named function such as `sortForDispatch` when ordering expresses business policy.

When equal keys require deterministic output, include an explicit tie-breaker such as an ID rather than relying on incidental input order.

## Group values for a report

`List.groupBy` applies a key function and returns one pair per distinct key:

```fsharp
let byStatus =
    orders |> List.groupBy (fun order -> order.Status)
```

Its result has the conceptual shape:

```text
(OrderStatus * Order list) list
```

Each tuple contains a status and all orders that produced that key. Grouping does not change the orders; it creates a report-oriented view.

```fsharp
let statusCounts =
    orders
    |> List.groupBy (fun order -> order.Status)
    |> List.map (fun (status, matching) -> status, List.length matching)
```

The grouping key must support equality. Sorting, maps, and sets require comparison instead, which is a stronger constraint. Unions whose payloads support the required operation work naturally unless equality or comparison generation has been disabled explicitly.

## Representation can leak into comparison

A wrapper union normally compares through its payload, so `OrderId 2 < OrderId 10`. That provides deterministic map and set behavior, but it does not automatically mean one order is more important than another. Use representation ordering only where the domain question supports it.

## Experiment

- Sort orders by total and then by ID.
- Group them by status and calculate counts.
- Add another order to an existing group.
- Compare two records differing only in ID.

## Summary

Structural equality compares complete data shapes. Sorting and grouping should use explicit keys that state the domain question being asked.
