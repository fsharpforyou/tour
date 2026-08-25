type OrderId = OrderId of int

type OrderStatus =
    | AwaitingPayment
    | Paid
    | Shipped

type Order = {
    Id: OrderId
    CustomerName: string
    PlacedOnDay: int
    TotalInCents: int
    Status: OrderStatus
}

let orders = [
    {
        Id = OrderId 1
        CustomerName = "Ada"
        PlacedOnDay = 12
        TotalInCents = 2400
        Status = Paid
    }
    {
        Id = OrderId 2
        CustomerName = "Ben"
        PlacedOnDay = 14
        TotalInCents = 1800
        Status = AwaitingPayment
    }
    {
        Id = OrderId 3
        CustomerName = "Ada"
        PlacedOnDay = 13
        TotalInCents = 3200
        Status = Paid
    }
]

let byTotal = orders |> List.sortBy (fun order -> order.TotalInCents, order.Id)

let statusCounts =
    orders
    |> List.groupBy (fun order -> order.Status)
    |> List.map (fun (status, matching) -> status, List.length matching)

printfn "By total: %A" byTotal
printfn "Status counts: %A" statusCounts

// Add a shipped order and predict the new groups.
