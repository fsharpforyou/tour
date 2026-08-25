let titles = [| "Kindred"; "Dune"; "Earthsea" |]

printfn "Catalog with a for loop:"

for index in 0 .. titles.Length - 1 do
    printfn "%d. %s" (index + 1) titles[index]

let totalCharactersImperative (values: string array) =
    let mutable total = 0
    let mutable index = 0

    while index < values.Length do
        total <- total + values[index].Length
        index <- index + 1

    total

let totalCharactersFunctional (values: string array) =
    values |> Array.fold (fun total title -> total + title.Length) 0

printfn "Imperative total: %d" (totalCharactersImperative titles)
printfn "Functional total: %d" (totalCharactersFunctional titles)

// Add another title and predict both totals before running.
