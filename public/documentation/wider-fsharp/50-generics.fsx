type Page<'item> = { Items: 'item list; Number: int }

let transformPage transform page = {
    Items = page.Items |> List.map transform
    Number = page.Number
}

let titles = {
    Items = [ "Kindred"; "Dune" ]
    Number = 1
}

let lengths = titles |> transformPage (fun title -> title.Length)
printfn "%A" lengths

// Try transforming the same Page structure into uppercase titles.
