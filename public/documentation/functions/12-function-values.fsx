let regularPrice price = price
let memberPrice price = price * 0.9

let choosePolicy isMember =
    if isMember then memberPrice else regularPrice

let calculatePrice policy price = policy price

let policy = choosePolicy true
let finalPrice = calculatePrice policy 20.0

printfn "Final price: %.2f" finalPrice

// Try choosing the regular-price policy and predict the new result.
