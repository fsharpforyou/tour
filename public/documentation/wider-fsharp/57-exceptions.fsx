type ImportError = InvalidImportedRecord of message: string

let requireImportedTitle (title: string) =
    let cleaned = title.Trim()

    if cleaned = "" then
        failwith "Imported title was empty"
    else
        cleaned

let importTitle title =
    try
        let validTitle = requireImportedTitle title
        Ok validTitle
    with ex ->
        Error(InvalidImportedRecord ex.Message)

let display result =
    match result with
    | Ok title -> $"Imported: %s{title}"
    | Error(InvalidImportedRecord message) -> $"Import failed: %s{message}"

printfn "%s" (importTitle "  Kindred  " |> display)
printfn "%s" (importTitle "   " |> display)

// Routine user validation should return Result directly rather than throw.
// Try both inputs, then rewrite blank-title handling as a direct Result.
