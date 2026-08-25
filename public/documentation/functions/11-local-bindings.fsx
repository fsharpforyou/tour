let calculateCharge price quantity memberDiscount =
    let subtotal = price * float quantity
    let discountAmount = subtotal * memberDiscount
    let discounted = subtotal - discountAmount
    discounted

let regularCharge = calculateCharge 12.0 2 0.0
let memberCharge = calculateCharge 12.0 2 0.10

printfn "Regular charge: %.2f" regularCharge
printfn "Member charge: %.2f" memberCharge

// Change the quantity and predict which local values are affected.
