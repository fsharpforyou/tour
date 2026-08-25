let calculateSalePrice price = price * 0.9

let firstPrice = calculateSalePrice 12.0
let secondPrice = calculateSalePrice (10.0 + 15.0)

printfn "Sale price for 12.00: %.2f" firstPrice
printfn "Sale price for 25.00: %.2f" secondPrice

// Try changing the discount or adding another call.
