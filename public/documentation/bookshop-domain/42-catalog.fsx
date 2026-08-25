type BookId = BookId of int
type Isbn = Isbn of string

type Author = { Name: string }

type Genre =
    | Fiction
    | History
    | Science

type Book = {
    Id: BookId
    Isbn: Isbn
    Title: string
    Authors: Author list
    Genres: Set<Genre>
    PriceInCents: int
}

type Catalog = Map<BookId, Book>

let all catalog =
    catalog |> Map.toList |> List.map (fun (_, book) -> book)

let normalize (text: string) = text.Trim().ToLowerInvariant()

let titleContains query book =
    let wanted = normalize query
    let title = normalize book.Title
    title.Contains(wanted)

let search query catalog =
    catalog |> all |> List.filter (titleContains query)

let books = [
    {
        Id = BookId 1
        Isbn = Isbn "9780807083697"
        Title = "Kindred"
        Authors = [ { Name = "Octavia E. Butler" } ]
        Genres = Set.ofList [ Fiction ]
        PriceInCents = 1299
    }
    {
        Id = BookId 2
        Isbn = Isbn "9780441478125"
        Title = "A Wizard of Earthsea"
        Authors = [ { Name = "Ursula K. Le Guin" } ]
        Genres = Set.ofList [ Fiction ]
        PriceInCents = 1099
    }
]

let catalog = books |> List.map (fun book -> book.Id, book) |> Map.ofList

printfn "Search results: %A" (search " earth " catalog)

// Try adding a History book and searching for part of its title.
