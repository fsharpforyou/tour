type Book = { Id: int; Title: string; Copies: int }

let books = [
    {
        Id = 1
        Title = "Kindred"
        Copies = 2
    }
    { Id = 2; Title = "Dune"; Copies = 0 }
]

let catalog = books |> List.map (fun book -> book.Id, book) |> Map.ofList

let revised =
    catalog |> Map.change 2 (Option.map (fun book -> { book with Copies = 3 }))

printfn "Book 1: %A" (catalog |> Map.tryFind 1)
printfn "Missing book: %A" (catalog |> Map.tryFind 99)
printfn "Original Dune: %A" (catalog |> Map.tryFind 2)
printfn "Revised Dune: %A" (revised |> Map.tryFind 2)

// Add a third book, then replace an existing key and compare both maps.
