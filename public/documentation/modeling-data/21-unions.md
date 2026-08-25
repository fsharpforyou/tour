# Discriminated unions: naming the possibilities

## What you will learn

A discriminated union defines the exact alternatives a value may have.

## Strings do not define a vocabulary

An early model might store a format as text:

```fsharp
let format = "paperbak"
```

The misspelling is still a valid string. Every function must remember the same
spellings, and unrelated strings can be passed wherever a format is expected. A
union gives this small vocabulary its own type:

```fsharp
type BookFormat =
    | Hardcover
    | Paperback
    | Ebook
```

Each case is a value of type `BookFormat`. Cases are not strings or numeric enum labels; the compiler knows they belong to this type.

Construction now rejects `"paperbak"`, and a function accepting `BookFormat`
cannot accidentally receive a customer name. The next lesson uses pattern
matching to consume every case.

Cases can carry different data:

```fsharp
type PaymentMethod =
    | Cash
    | Card of lastFourDigits: string
    | GiftCard of code: string
```

Construct a payload case with normal function application: `Card "4242"`. The label `lastFourDigits` documents the payload; it does not create a field accessed with a dot.

A union replaces loosely coordinated strings and fields. A gift-card code matters only for `GiftCard`, so that case carries the code itself.

The next lesson shows how to inspect cases. Even before that, construction prevents misspelled case names and mismatched payloads.

## Cases are constructors

`Cash` needs no data, so it is already a complete `PaymentMethod` value. `Card` needs text, so `Card` by itself behaves like a function from `string` to `PaymentMethod`; `Card "4242"` is the completed value. This connects unions to the function model you already know.

Compare their inferred shapes:

```text
Cash         : PaymentMethod
Card         : string -> PaymentMethod
Card "4242" : PaymentMethod
```

Trying to store `Card` where a completed payment method is required produces a
function-type mismatch because its payload is still missing.

Case names conventionally begin with capitals. Type context usually identifies their union; qualification such as `BookFormat.Hardcover` can make intent explicit.

## Payloads can have several parts

```fsharp
type DeliveryMethod =
    | Collection
    | Posted of street: string * postalCode: string
```

`Posted ("12 Elm Road", "AB12 3CD")` constructs one case carrying a tuple payload. When several positions become hard to distinguish, use a record as the payload. Unions answer “which possibility?”; records name facts that coexist.

```fsharp
type PostalDetails =
    { Street: string; PostalCode: string }

type DeliveryMethod =
    | Collection
    | Posted of PostalDetails
```

The record payload makes construction slightly longer but gives both facts
names. Choose the representation that makes accidental exchanges least likely.

The playground uses `%A` to inspect these new values. `%A` asks F# for a general structural representation. It is useful diagnostic output while learning, but it is not carefully designed customer-facing text.

## Try it

- Add an `Audiobook of durationMinutes: int` format.
- Construct every payment case.
- Try giving `Card` an integer and read the type error.
- Bind `Card` without its payload and inspect the inferred function type.
- Replace the two-part postal payload with a record.

## Summary

Discriminated unions model alternatives. Payload cases attach exactly the information relevant to one alternative.
