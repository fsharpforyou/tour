let applyTwice transform value = transform (transform value)

let reduceFivePercent price = price * 0.95

let addPrefix text = "Bookshop: " + text

let minimumLength minimum =
    fun (text: string) -> text.Length >= minimum

let atLeast minimum = fun amount -> amount >= minimum

let freeStandardDelivery = atLeast 30.0
let freeExpressDelivery = atLeast 60.0

printfn "Two reductions: %.2f" (applyTwice reduceFivePercent 20.0)
printfn "%s" (applyTwice addPrefix "Kindred")
printfn "Title is at least three characters: %b" (minimumLength 3 "Dune")
printfn "Free standard delivery at 35.00: %b" (freeStandardDelivery 35.0)
printfn "Free express delivery at 35.00: %b" (freeExpressDelivery 35.0)

// Try applyTwice with a function whose input and output types differ.
