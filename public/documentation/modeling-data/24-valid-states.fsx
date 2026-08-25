type CustomerStatus =
    | Active
    | Suspended of reason: string
    | Closed

type Customer = { Name: string; Status: CustomerStatus }

let mayOrder customer =
    match customer.Status with
    | Active -> true
    | Suspended _ -> false
    | Closed -> false

let ada = { Name = "Ada"; Status = Active }

let ben = {
    Name = "Ben"
    Status = Suspended "payment review"
}

printfn "%s may order: %b" ada.Name (mayOrder ada)
printfn "%s may order: %b" ben.Name (mayOrder ben)

// Try adding a Guest case and decide whether that customer may order.
