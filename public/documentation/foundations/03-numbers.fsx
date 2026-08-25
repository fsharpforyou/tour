let unitPrice = 14.95
let quantity = 3
let lineTotal = float quantity * unitPrice

let discountRate = 0.10
let discount = lineTotal * discountRate
let finalTotal = lineTotal - discount

printfn "Line total: %.2f" lineTotal
printfn "After discount: %.2f" finalTotal

// Try changing quantity or discountRate.
