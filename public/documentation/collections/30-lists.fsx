let catalog = [ "Kindred"; "Dune"; "A Wizard of Earthsea" ]

let expanded = "Beloved" :: catalog

let rec count titles =
    match titles with
    | [] -> 0
    | _ :: rest -> 1 + count rest

let describeFirst titles =
    match titles with
    | [] -> "The catalog is empty"
    | first :: rest -> $"First: %s{first}; more titles: %d{count rest}"

printfn "%s" (describeFirst expanded)
printfn "Original count: %d" (count catalog)
printfn "Expanded count: %d" (count expanded)

// Try tracing count with an empty list and a one-item list.
