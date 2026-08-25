type AddToCartError =
    | CustomerInactive
    | BookUnavailable
    | InvalidQuantity of attempted: int

let addToCart isActive available quantity =
    if not isActive then Error CustomerInactive
    elif quantity <= 0 then Error(InvalidQuantity quantity)
    elif quantity > available then Error BookUnavailable
    else Ok quantity

let describe result =
    match result with
    | Ok quantity -> $"Added %d{quantity} book(s)"
    | Error CustomerInactive -> "Customer is inactive"
    | Error BookUnavailable -> "Not enough stock"
    | Error(InvalidQuantity attempted) -> $"Invalid quantity: %d{attempted}"

printfn "%s" (describe (addToCart true 4 2))
printfn "%s" (describe (addToCart true 4 5))

// Predict which validation wins when several inputs are bad. Run it, then add a
// new error case and follow the compiler warning to every match that needs it.
