module Shop =
    type ResultBuilder() =
        member _.Bind(result, next) = Result.bind next result
        member _.Return(value) = Ok value
        member _.ReturnFrom(result) = result

    let result = ResultBuilder()

    type BookId = BookId of int
    type CustomerId = CustomerId of int
    type OrderId = OrderId of int
    type Isbn = Isbn of string
    type EmailAddress = EmailAddress of string
    type Money = Money of int

    type Author = { Name: string }

    type Book = {
        Id: BookId
        Isbn: Isbn
        Title: string
        Authors: Author list
        Price: Money
    }

    type Membership =
        | Standard
        | Member of discountPercent: int

    type Customer = {
        Id: CustomerId
        Name: string
        Email: EmailAddress option
        Membership: Membership
    }

    type Stock = { OnHand: int; Reserved: int }

    type CartLine = { BookId: BookId; Quantity: int }

    type Cart = { Lines: CartLine list }

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

    type OrderStatus = | AwaitingPayment

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

    type State = {
        Catalog: Map<BookId, Book>
        Customers: Map<CustomerId, Customer>
        Inventory: Map<BookId, Stock>
        Orders: Map<OrderId, Order>
        NextOrderId: int
    }

    type CheckoutError =
        | CustomerNotFound
        | EmptyCart
        | BookNotFound of BookId
        | InvalidQuantity of BookId
        | NotEnoughStock of BookId * available: int

    let add (Money left) (Money right) = Money(left + right)
    let subtract (Money reduction) (Money amount) = Money(amount - reduction)
    let multiply quantity (Money amount) = Money(quantity * amount)
    let available stock = stock.OnHand - stock.Reserved

    let priceLine (book: Book) (line: CartLine) : PricedLine = {
        BookId = book.Id
        Title = book.Title
        Quantity = line.Quantity
        UnitPrice = book.Price
        LineTotal = multiply line.Quantity book.Price
    }

    let prepareLine catalog inventory (line: CartLine) =
        match Map.tryFind line.BookId catalog, Map.tryFind line.BookId inventory with
        | None, _ -> Error(BookNotFound line.BookId)
        | _, None -> Error(NotEnoughStock(line.BookId, 0))
        | Some _, Some _ when line.Quantity <= 0 -> Error(InvalidQuantity line.BookId)
        | Some book, Some stock when line.Quantity <= available stock ->
            let priced = priceLine book line

            let updatedStock = {
                stock with
                    Reserved = stock.Reserved + line.Quantity
            }

            let updatedInventory = inventory |> Map.add line.BookId updatedStock
            Ok(priced, updatedInventory)
        | Some _, Some stock -> Error(NotEnoughStock(line.BookId, available stock))

    let prepareLines catalog initialInventory (lines: CartLine list) =
        lines
        |> List.fold
            (fun state line ->
                result {
                    let! pricedLines, inventory = state
                    let! priced, nextInventory = prepareLine catalog inventory line
                    return priced :: pricedLines, nextInventory
                })
            (Ok([], initialInventory))
        |> Result.map (fun (reversed, inventory) -> List.rev reversed, inventory)

    let subtotal lines =
        lines |> List.fold (fun total line -> add total line.LineTotal) (Money 0)

    let discountFor customer (Money amount) =
        match customer.Membership with
        | Standard -> Money 0
        | Member percent -> Money(amount * percent / 100)

    let shippingCost method (Money amount) =
        match method with
        | Collection -> Money 0
        | StandardPost -> Money 500
        | ExpressPost when amount >= 5000 -> Money 0
        | ExpressPost -> Money 900

    let placeOrder customerId (cart: Cart) shippingMethod (state: State) =
        match state.Customers |> Map.tryFind customerId with
        | None -> Error CustomerNotFound
        | Some _ when cart.Lines = [] -> Error EmptyCart
        | Some customer ->
            prepareLines state.Catalog state.Inventory cart.Lines
            |> Result.map (fun (lines, inventory) ->
                let beforeDiscount = subtotal lines
                let discount = discountFor customer beforeDiscount
                let delivery = shippingCost shippingMethod beforeDiscount
                let total = beforeDiscount |> subtract discount |> add delivery
                let orderId = OrderId state.NextOrderId

                let order = {
                    Id = orderId
                    CustomerId = customer.Id
                    Lines = lines
                    Subtotal = beforeDiscount
                    Discount = discount
                    ShippingCost = delivery
                    Total = total
                    ShippingMethod = shippingMethod
                    Status = AwaitingPayment
                }

                let nextState = {
                    state with
                        Inventory = inventory
                        Orders = state.Orders |> Map.add orderId order
                        NextOrderId = state.NextOrderId + 1
                }

                nextState, order)

open Shop

let book = {
    Id = BookId 1
    Isbn = Isbn "9780807083697"
    Title = "Kindred"
    Authors = [ { Name = "Octavia E. Butler" } ]
    Price = Money 1299
}

let customer = {
    Id = CustomerId 1
    Name = "Ada"
    Email = Some(EmailAddress "ada@example.org")
    Membership = Member 10
}

let initial = {
    Catalog = Map.ofList [ (book.Id, book) ]
    Customers = Map.ofList [ (customer.Id, customer) ]
    Inventory = Map.ofList [ (book.Id, { OnHand = 5; Reserved = 0 }) ]
    Orders = Map.empty
    NextOrderId = 1
}

let cart = {
    Lines = [ { BookId = book.Id; Quantity = 2 } ]
}

match placeOrder customer.Id cart StandardPost initial with
| Error error -> printfn "Order failed: %A" error
| Ok(updated, order) ->
    printfn "Order: %A" order
    printfn "Inventory: %A" updated.Inventory

// Try an unavailable quantity, a standard customer, and a second cart line.
