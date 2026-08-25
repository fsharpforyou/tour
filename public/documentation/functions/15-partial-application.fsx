let applyDiscount rate price = price * (1.0 - rate)

let isAtLeast minimum amount = amount >= minimum

let addHandling amount total = total + amount

let regularPrice = applyDiscount 0.0
let memberPrice = applyDiscount 0.10
let clearancePrice = applyDiscount 0.40
let qualifiesForFreeDelivery = isAtLeast 30.0
let addGiftWrap = addHandling 2.0

printfn "Regular price: %.2f" (regularPrice 20.0)
printfn "Member price: %.2f" (memberPrice 20.0)
printfn "Clearance price: %.2f" (clearancePrice 20.0)
printfn "35.00 qualifies for free delivery: %b" (qualifiesForFreeDelivery 35.0)
printfn "20.00 with gift wrap: %.2f" (addGiftWrap 20.0)

// Try hovering each partially applied binding and predict its remaining input.
