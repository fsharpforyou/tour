type BookFormat =
    | Hardcover
    | Paperback
    | Ebook
    | Audiobook of durationMinutes: int

type PaymentMethod =
    | Cash
    | Card of lastFourDigits: string
    | GiftCard of code: string

let format = Paperback
let audioFormat = Audiobook 615
let payment = Card "4242"

printfn "Format: %A" format
printfn "Audio format: %A" audioFormat
printfn "Payment: %A" payment

// Try constructing GiftCard, then give Audiobook a different duration.
