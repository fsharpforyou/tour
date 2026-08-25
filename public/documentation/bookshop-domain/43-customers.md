# Customers and validated contact information

## What you will learn

Model stable customer identity, optional validated contact information, and membership without nullable fields or contradictory flags.

```fsharp
type CustomerId = CustomerId of int
type EmailAddress = EmailAddress of string

type Membership =
    | Standard
    | Member of discountPercent: int

type Customer =
    {
        Id: CustomerId
        Name: string
        Email: EmailAddress option
        Membership: Membership
    }
```

An email may be absent, but any present email has the `EmailAddress` type. At this stage the union case is public, so the wrapper distinguishes an email string but does not yet prove that construction used `createEmail`. Lesson 49 will use module privacy to enforce that route. Membership is exactly one case; a member case carries the discount that belongs to that status.

## Convert loose input into a domain value

```fsharp
type EmailError = InvalidEmail

let createEmail (text: string) =
    let cleaned = text.Trim()

    if cleaned.Contains("@") then
        Ok (EmailAddress cleaned)
    else
        Error InvalidEmail
```

This small check is not a complete implementation of the email standard. Its purpose is to show how successful construction can return a more meaningful type.

```fsharp
createEmail "ada@example.org"
// Ok (EmailAddress "ada@example.org")

createEmail "not-an-address"
// Error InvalidEmail
```

Code that receives the successful result from `createEmail` does not need to repeat this boundary check. Until construction is made private, other `EmailAddress` values may still bypass it.

## Derive policy rather than storing another flag

```fsharp
let discountPercent customer =
    match customer.Membership with
    | Standard -> 0
    | Member percent -> percent
```

Adding an `IsMember` boolean next to `Membership` would create two facts that could disagree. Keep the union authoritative and derive the numeric policy with a function. The current `Member of int` case still permits negative or excessive percentages; a validated constructor can enforce a range once construction is private.

Customer identity remains stable when membership changes:

```fsharp
let joinMembership percent customer =
    { customer with Membership = Member percent }
```

The original customer remains unchanged.

## Optional and validated are separate ideas

When values are created through `createEmail`, `EmailAddress option` expresses two facts:

- contact information may legitimately be absent;
- present contact information passed the construction rule.

A plain `string option` expresses only the first fact. The capstone keeps the wrapper so that validated contact information remains distinct from unchecked text.

## Experiment

- Create a customer without an email.
- Give a standard customer a 10-percent membership.
- Pattern match to produce a membership label.
- Pass an invalid string through `createEmail` and handle the error.

## Summary

Validated wrappers strengthen primitive values; options model legitimate absence; unions keep mutually exclusive customer states explicit.
