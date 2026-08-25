type ResultBuilder() =
    member _.Bind(result, next) = Result.bind next result
    member _.Return(value) = Ok value
    member _.ReturnFrom(result) = result

let result = ResultBuilder()

type CheckoutError =
    | CustomerMissing
    | EmptyCart
    | InvalidShipping

let findCustomer exists =
    if exists then Ok "Ada" else Error CustomerMissing

let validateCart lineCount =
    if lineCount > 0 then Ok lineCount else Error EmptyCart

let validateShipping supplied =
    if supplied then Ok() else Error InvalidShipping

let prepareOrder customerExists lineCount shippingSupplied =
    result {
        let! customer = findCustomer customerExists
        let! validatedCount = validateCart lineCount
        do! validateShipping shippingSupplied
        return $"%s{customer}: %d{validatedCount} line(s) ready"
    }

printfn "%A" (prepareOrder true 2 true)
printfn "%A" (prepareOrder true 0 true)

// Make each input invalid separately and confirm later steps are skipped.
