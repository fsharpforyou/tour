let title = "The Left Hand of Darkness"
let availableCopies = 2
let isForSale = true

let hasCopies = availableCopies > 0
let canOrder = hasCopies && isForSale
let needsAttention = availableCopies = 0 || not isForSale

printfn "%s can be ordered: %b" title canOrder
printfn "Needs staff attention: %b" needsAttention

// Try zero copies, then try making the book unavailable for sale.
