open System

type String with

    member string.IsUpperCase() = string = string.ToUpper()

let uppercaseString = "HELLO WORLD"
let lowercaseStringValue = "hello world"

printfn "uppercaseString.IsUpperCase() = %b" (uppercaseString.IsUpperCase())
printfn "lowercaseStringValue.IsUpperCase() = %b" (lowercaseStringValue.IsUpperCase())
