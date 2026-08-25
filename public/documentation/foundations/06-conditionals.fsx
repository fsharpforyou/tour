let title = "Kindred"
let availableCopies = 1

let stockLabel =
    if availableCopies = 0 then "Unavailable"
    elif availableCopies = 1 then "Last available copy"
    else "Available"

printfn "%s — %s" title stockLabel

// Try availableCopies values of 0, 1, and 5.
