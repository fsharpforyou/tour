let maximumQuantity: int = 5
let memberDiscount: float = 0.10
let customerName: string = "Amina"

let requestedQuantity = 3
let remainingAllowance = maximumQuantity - requestedQuantity
let discountOnTwenty = memberDiscount * 20.0
let wideMaximum: int64 = maximumQuantity
let floatingMaximum: float = maximumQuantity

printfn "%s may add %d more book(s)." customerName remainingAllowance
printfn "The discount on 20.00 is %.2f." discountOnTwenty
printfn "Implicit widenings: %A and %A" wideMaximum floatingMaximum

// Remove the annotations F# can infer, then try narrowing a float to int
// without calling int. Read the error before adding the explicit conversion.
