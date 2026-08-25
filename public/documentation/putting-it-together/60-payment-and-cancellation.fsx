type OrderId = OrderId of int
type CustomerId = CustomerId of int
type BookId = BookId of int
type Money = Money of int

type ShippingMethod =
    | Collection
    | StandardPost
    | ExpressPost

type PricedLine = {
    BookId: BookId
    Title: string
    Quantity: int
    UnitPrice: Money
    LineTotal: Money
}

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
    | Cancelled of reason: string

// This is the same order shape produced by the placement lesson.
// Only Status has grown to represent later lifecycle facts.
type Order = {
    Id: OrderId
    CustomerId: CustomerId
    Lines: PricedLine list
    Subtotal: Money
    Discount: Money
    ShippingCost: Money
    Total: Money
    ShippingMethod: ShippingMethod
    Status: OrderStatus
}

type State = { Orders: Map<OrderId, Order> }

type TransitionError =
    | OrderNotFound
    | AlreadyPaid
    | OrderAlreadyCancelled

let pay payment order =
    match order.Status with
    | AwaitingPayment -> Ok { order with Status = Paid payment }
    | Paid _ -> Error AlreadyPaid
    | Cancelled _ -> Error OrderAlreadyCancelled

let cancel reason order =
    match order.Status with
    | AwaitingPayment -> Ok { order with Status = Cancelled reason }
    | Paid _ -> Error AlreadyPaid
    | Cancelled _ -> Error OrderAlreadyCancelled

let updateOrder orderId transition state =
    match state.Orders |> Map.tryFind orderId with
    | None -> Error OrderNotFound
    | Some order ->
        transition order
        |> Result.map (fun updatedOrder -> {
            state with
                Orders = state.Orders |> Map.add orderId updatedOrder
        })

let orderId = OrderId 1

let order = {
    Id = orderId
    CustomerId = CustomerId 1
    Lines = [
        {
            BookId = BookId 1
            Title = "Kindred"
            Quantity = 2
            UnitPrice = Money 1299
            LineTotal = Money 2598
        }
    ]
    Subtotal = Money 2598
    Discount = Money 259
    ShippingCost = Money 500
    Total = Money 2839
    ShippingMethod = StandardPost
    Status = AwaitingPayment
}

let initial = {
    Orders = Map.ofList [ (orderId, order) ]
}

let payment = { Method = Card "4242"; PaidOnDay = 20 }

let paid = initial |> updateOrder orderId (pay payment)
let cancelled = initial |> updateOrder orderId (cancel "customer request")

printfn "Paid history: %A" paid
printfn "Cancelled history: %A" cancelled
printfn "Pay twice: %A" (paid |> Result.bind (updateOrder orderId (pay payment)))

// Predict each Result, run it, then try cancelling the paid history.
