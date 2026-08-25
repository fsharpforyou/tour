type OrderId = OrderId of int

type PaymentMethod =
    | Card of lastFourDigits: string
    | GiftCard of code: string

type Payment = {
    Method: PaymentMethod
    PaidOnDay: int
}

type OrderStatus =
    | AwaitingPayment
    | Paid of Payment
    | Shipped of trackingNumber: string
    | Cancelled of reason: string

type Order = { Id: OrderId; Status: OrderStatus }

type TransitionError =
    | AlreadyPaid
    | PaymentRequired
    | OrderClosed

let pay payment order =
    match order.Status with
    | AwaitingPayment -> Ok { order with Status = Paid payment }
    | Paid _ -> Error AlreadyPaid
    | Shipped _
    | Cancelled _ -> Error OrderClosed

let ship trackingNumber order =
    match order.Status with
    | Paid _ ->
        Ok {
            order with
                Status = Shipped trackingNumber
        }
    | AwaitingPayment -> Error PaymentRequired
    | Shipped _
    | Cancelled _ -> Error OrderClosed

let payment = {
    Method = Card "4242"
    PaidOnDay = 120
}

let order = {
    Id = OrderId 1
    Status = AwaitingPayment
}

let result = order |> pay payment |> Result.bind (ship "TRACK-001")

printfn "Final order: %A" result

// Predict the final status. Remove payment and inspect the refusal, then change
// Shipped so it preserves Payment and follow the compiler feedback to repair it.
