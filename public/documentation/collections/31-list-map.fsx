type Book = {
    Title: string
    Author: string
    Pages: int
}

let books = [
    {
        Title = "Kindred"
        Author = "Octavia E. Butler"
        Pages = 264
    }
    {
        Title = "Dune"
        Author = "Frank Herbert"
        Pages = 412
    }
    {
        Title = "Earthsea"
        Author = "Ursula K. Le Guin"
        Pages = 205
    }
]

let labels =
    books |> List.map (fun book -> $"%s{book.Title} (%d{book.Pages} pages)")

printfn "%A" labels

// Change the mapping function to return page counts instead of labels.
