type Book = {
    Title: string
    Copies: int
    StaffNote: string option
}

let books = [
    {
        Title = "Kindred"
        Copies = 3
        StaffNote = Some "Staff pick"
    }
    {
        Title = "Dune"
        Copies = 0
        StaffNote = Some "Epic science fiction"
    }
    {
        Title = "Earthsea"
        Copies = 2
        StaffNote = None
    }
]

let recommendedLabel book =
    book.StaffNote |> Option.map (fun note -> book.Title + ": " + note)

let recommendations = books |> List.choose recommendedLabel
let inStock, soldOut = books |> List.partition (fun book -> book.Copies > 0)

printfn "Recommendations: %A" recommendations
printfn "In stock: %A" (inStock |> List.map (fun book -> book.Title))
printfn "Sold out: %A" (soldOut |> List.map (fun book -> book.Title))

// Add a note to Earthsea and restock Dune; predict both kinds of output.
