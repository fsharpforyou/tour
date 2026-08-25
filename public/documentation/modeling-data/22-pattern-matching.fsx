type BookFormat =
    | Hardcover
    | Paperback
    | Ebook
    | Audiobook of durationMinutes: int

let describe format =
    match format with
    | Hardcover -> "Hardcover"
    | Paperback -> "Paperback"
    | Ebook -> "Ebook"
    | Audiobook minutes when minutes > 600 -> $"Long audiobook: %d{minutes} minutes"
    | Audiobook minutes -> $"Audiobook: %d{minutes} minutes"

let shortLabel =
    function
    | Hardcover -> "hardback"
    | Paperback -> "paperback"
    | Ebook -> "digital"
    | Audiobook _ -> "audio"

printfn "%s" (describe Paperback)
printfn "%s" (describe (Audiobook 615))
printfn "%s" (shortLabel Ebook)

// Try adding a LargePrint case and let the warnings guide both functions.
