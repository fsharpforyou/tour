type OrderId = OrderId of int
type CustomerId = CustomerId of int
type BookId = BookId of int
type Money = Money of int

module Money =
    let zero = Money 0
    let add (Money left) (Money right) = Money(left + right)

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
    | Shipped of payment: Payment * trackingNumber: string
    | Cancelled of reason: string

// The pricing and customer fields from placement remain available.
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

type ShippingError =
    | PaymentRequired
    | OrderClosed

let ship trackingNumber order =
    match order.Status with
    | Paid payment ->
        Ok {
            order with
                Status = Shipped(payment, trackingNumber)
        }
    | AwaitingPayment -> Error PaymentRequired
    | Shipped _
    | Cancelled _ -> Error OrderClosed

type StatusCategory =
    | AwaitingPaymentCategory
    | PaidCategory
    | ShippedCategory
    | CancelledCategory

let statusCategory status =
    match status with
    | AwaitingPayment -> AwaitingPaymentCategory
    | Paid _ -> PaidCategory
    | Shipped _ -> ShippedCategory
    | Cancelled _ -> CancelledCategory

let allOrders state =
    state.Orders |> Map.toList |> List.map (fun (_, order) -> order)

let orderCountsByStatus state =
    state
    |> allOrders
    |> List.groupBy (fun order -> statusCategory order.Status)
    |> List.map (fun (category, orders) -> category, List.length orders)

let revenue state =
    state
    |> allOrders
    |> List.filter (fun order ->
        match order.Status with
        | Paid _
        | Shipped _ -> true
        | AwaitingPayment
        | Cancelled _ -> false)
    |> List.fold (fun total order -> Money.add total order.Total) Money.zero

let makeOrder id status = {
    Id = OrderId id
    CustomerId = CustomerId 1
    Lines = []
    Subtotal = Money 3200
    Discount = Money 0
    ShippingCost = Money 500
    Total = Money 3700
    ShippingMethod = StandardPost
    Status = status
}

let payment = { Method = Card "4242"; PaidOnDay = 20 }
let awaiting = makeOrder 1 AwaitingPayment
let paid = makeOrder 2 (Paid payment)

match ship "TRACK-100" paid with
| Error error -> printfn "Unexpected shipping failure: %A" error
| Ok shipped ->
    let state = {
        Orders = Map.ofList [ (awaiting.Id, awaiting); (shipped.Id, shipped) ]
    }

    printfn "Counts: %A" (orderCountsByStatus state)
    printfn "Revenue: %A" (revenue state)

printfn "Ship unpaid: %A" (ship "TRACK-101" awaiting)

// Add a cancelled order, predict the report, then run it.
