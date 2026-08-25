type ValidationError =
    | EmptyName
    | NameTooLong of maximum: int
    | InvalidQuantity

let validateName (name: string) =
    let cleaned = name.Trim()

    if cleaned = "" then Error EmptyName
    elif cleaned.Length > 40 then Error(NameTooLong 40)
    else Ok cleaned

let describe result =
    match result with
    | Ok name -> $"Valid customer: %s{name}"
    | Error EmptyName -> "Name cannot be empty"
    | Error(NameTooLong maximum) -> $"Name must be at most %d{maximum} characters"
    | Error InvalidQuantity -> "Quantity must be positive"

printfn "%s" (describe (validateName "  Grace Hopper  "))
printfn "%s" (describe (validateName "   "))

// Add a minimum-name-length error and update describe exhaustively.
