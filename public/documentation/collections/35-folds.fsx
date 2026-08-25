type Book = {
    Title: string
    Pages: int
    Available: bool
}

type Summary = {
    BookCount: int
    PageCount: int
    AvailableCount: int
}

let books = [
    {
        Title = "Kindred"
        Pages = 264
        Available = true
    }
    {
        Title = "Dune"
        Pages = 412
        Available = false
    }
    {
        Title = "Earthsea"
        Pages = 205
        Available = true
    }
]

let summarize summary book = {
    BookCount = summary.BookCount + 1
    PageCount = summary.PageCount + book.Pages
    AvailableCount = summary.AvailableCount + (if book.Available then 1 else 0)
}

let empty = {
    BookCount = 0
    PageCount = 0
    AvailableCount = 0
}

printfn "%A" (books |> List.fold summarize empty)

let labels = [ "A"; "B"; "C" ]

let fromRight =
    List.foldBack (fun label text -> $"(%s{label}+%s{text})") labels "end"

printfn "Right-associated: %s" fromRight

// Change the label order and trace the foldBack result on paper first.
