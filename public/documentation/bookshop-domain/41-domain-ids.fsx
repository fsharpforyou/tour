type BookId = BookId of int
type CustomerId = CustomerId of int
type OrderId = OrderId of int

type IdError = NonPositiveId

let createBookId value =
    if value > 0 then Ok(BookId value) else Error NonPositiveId

let displayBookId (BookId value) = $"B-%d{value}"

match createBookId 42 with
| Ok id -> printfn "Created %s" (displayBookId id)
| Error NonPositiveId -> printfn "An ID must be positive"

// Try passing CustomerId 42 to displayBookId and read the type error.
