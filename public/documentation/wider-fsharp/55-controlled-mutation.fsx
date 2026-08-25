let stock = [| 2; 0; 4 |]
let sameStock = stock

stock[1] <- 3

printfn "The shared array sees the update: %A" sameStock

let restockedImmutably =
    stock |> Array.mapi (fun index count -> if index = 0 then count + 2 else count)

let receiveCopies shelfIndex delivered (counts: int array) =
    if delivered > 0 then
        counts[shelfIndex] <- counts[shelfIndex] + delivered

let withReceivedCopies shelfIndex delivered counts =
    counts
    |> Array.mapi (fun index count -> if index = shelfIndex then count + delivered else count)

printfn "Original after mutation: %A" stock
printfn "New immutable result: %A" restockedImmutably

receiveCopies 2 1 stock
let anotherResult = withReceivedCopies 0 5 stock

printfn "After receiveCopies: %A" stock
printfn "Returned successor: %A" anotherResult

// Predict what sameStock sees after each mutation.
