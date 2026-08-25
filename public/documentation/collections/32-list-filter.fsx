type Book = {
    Title: string
    Available: bool
    Pages: int
}

let books = [
    {
        Title = "Kindred"
        Available = true
        Pages = 264
    }
    {
        Title = "Dune"
        Available = false
        Pages = 412
    }
    {
        Title = "Earthsea"
        Available = true
        Pages = 205
    }
]

let availableShortBooks =
    books |> List.filter (fun book -> book.Available && book.Pages < 300)

printfn "%A" (availableShortBooks |> List.map (fun book -> book.Title))

// Predict the titles before running. Change the threshold, then replace filter
// with map and inspect how the result's element type changes.
