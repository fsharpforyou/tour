open System

let parseCount (text: string) =
    match Int32.TryParse(text) with
    | true, value -> Some value
    | false, _ -> None

let titles = [ "Kindred"; "Dune"; "Earthsea" ]
printfn "Catalog: %s" (String.concat ", " titles)
printfn "Parsed: %A" (parseCount "42")
printfn "Invalid: %A" (parseCount "many")

// Try parsing a negative count and decide whether parsing or domain validation should reject it.
