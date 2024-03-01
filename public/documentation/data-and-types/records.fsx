type Person = { FirstName: string; LastName: string }

let johnDoe = { FirstName = "John"; LastName = "Doe" }
printfn "%A" johnDoe

let janeDoe = { johnDoe with FirstName = "Jane" }
printfn "%A" janeDoe
