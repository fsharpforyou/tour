let applyDiscount rate price = price * (1.0 - rate)

let describeLine title quantity = $"%d{quantity} × %s{title}"

let price = applyDiscount 0.10 20.0
let description = describeLine "The Left Hand of Darkness" 2

printfn "%s" description
printfn "Discounted unit price: %.2f" price

// Try swapping the argument order of applyDiscount and update its call.
