let nonBlank (text: string) =
    let cleaned = text.Trim()
    if cleaned = "" then None else Some cleaned

let displaySubtitle subtitle =
    subtitle
    |> Option.bind nonBlank
    |> Option.map (fun text -> "Subtitle: " + text)
    |> Option.defaultValue "No subtitle"

printfn "%s" (displaySubtitle (Some "  A Novel  "))
printfn "%s" (displaySubtitle (Some "   "))
printfn "%s" (displaySubtitle None)

// Predict all three lines. Replace bind with map and inspect the nested option,
// then restore bind and change only the display default.
