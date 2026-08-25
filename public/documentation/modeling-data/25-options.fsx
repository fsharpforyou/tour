type Book = {
    Title: string
    Subtitle: string option
}

let fullTitle book =
    match book.Subtitle with
    | Some subtitle -> $"%s{book.Title}: %s{subtitle}"
    | None -> book.Title

let first = { Title = "Kindred"; Subtitle = None }

let second = {
    Title = "Earthsea"
    Subtitle = Some "The First Three Books"
}

printfn "%s" (fullTitle first)
printfn "%s" (fullTitle second)

// Try giving the first book a subtitle, then predict the full title.
