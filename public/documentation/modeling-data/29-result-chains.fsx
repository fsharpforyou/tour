type CustomerError =
    | EmptyName
    | ReservedName

let validateName (name: string) =
    let cleaned = name.Trim()
    if cleaned = "" then Error EmptyName else Ok cleaned

let rejectReservedName name =
    if name = "Bookshop" then Error ReservedName else Ok name

let register name =
    validateName name
    |> Result.bind rejectReservedName
    |> Result.map (fun validName -> $"Registered: %s{validName}")

printfn "%A" (register "  Ada  ")
printfn "%A" (register "Bookshop")

// Add a third validation step and make each step fail separately.
