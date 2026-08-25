module Catalog =
    type BookId = BookId of int
    type Entry = { Id: BookId; Title: string }
    type Catalog = Map<BookId, Entry>

    let private normalize (text: string) = text.Trim().ToLowerInvariant()

    let create entries =
        entries |> List.map (fun entry -> entry.Id, entry) |> Map.ofList

    let search query catalog =
        let wanted = normalize query

        catalog
        |> Map.toList
        |> List.map (fun (_, entry) -> entry)
        |> List.filter (fun entry ->
            let title = normalize entry.Title
            title.Contains(wanted))

let catalog =
    Catalog.create [
        {
            Id = Catalog.BookId 1
            Title = "Kindred"
        }
        {
            Id = Catalog.BookId 2
            Title = "Dune"
        }
    ]

printfn "%A" (Catalog.search "kind" catalog)

// Make normalize public, call it, then make it private again.
