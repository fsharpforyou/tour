let applyPricingPolicy policy price = policy price

let memberPrice = applyPricingPolicy (fun price -> price * 0.9) 20.0
let clearancePrice = applyPricingPolicy (fun price -> price * 0.6) 20.0

printfn "Member price: %.2f" memberPrice
printfn "Clearance price: %.2f" clearancePrice

// Predict both prices before running. Then rewrite one lambda as a named
// function. Finally remove its grouping parentheses, read the error, and repair it.
