type ShelfCopy = { Title: string; Copies: int }

let shelf = [|
    { Title = "Kindred"; Copies = 2 }
    { Title = "Dune"; Copies = 0 }
    { Title = "Earthsea"; Copies = 3 }
|]

let labels =
    shelf
    |> Array.mapi (fun index book -> $"%d{index + 1}. %s{book.Title} — %d{book.Copies} copies")

let available = shelf |> Array.filter (fun book -> book.Copies > 0)

printfn "First shelf title: %s" shelf[0].Title
printfn "All labels: %A" labels
printfn "Available titles: %A" (available |> Array.map (fun book -> book.Title))

// Try adding a fourth book and changing the filter rule.
