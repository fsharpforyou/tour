type Book = {
    Id: int
    Title: string
    Available: bool
}

let books = [
    {
        Id = 1
        Title = "Kindred"
        Available = true
    }
    {
        Id = 2
        Title = "Dune"
        Available = false
    }
    {
        Id = 3
        Title = "Earthsea"
        Available = true
    }
]

let findBook id =
    books |> List.tryFind (fun book -> book.Id = id)

printfn "Search: %A" (findBook 2)
printfn "Missing search: %A" (findBook 99)
printfn "Any available: %b" (books |> List.exists (fun book -> book.Available))
printfn "All available: %b" (books |> List.forall (fun book -> book.Available))

// Make Dune available, then predict both boolean answers.
