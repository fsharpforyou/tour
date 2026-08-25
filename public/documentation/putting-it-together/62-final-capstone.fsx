module Bookshop =
    type InputError = InputError of string

    let private required name (text: string) =
        let cleaned = text.Trim()

        if cleaned = "" then
            Error(InputError($"%s{name} must not be blank"))
        else
            Ok cleaned

    type BookId = private BookId of int

    module BookId =
        let create value =
            if value > 0 then
                Ok(BookId value)
            else
                Error(InputError "book ID must be positive")

    type CustomerId = private CustomerId of int

    module CustomerId =
        let create value =
            if value > 0 then
                Ok(CustomerId value)
            else
                Error(InputError "customer ID must be positive")

    type OrderId = private OrderId of int

    module OrderId =
        let create value =
            if value > 0 then
                Ok(OrderId value)
            else
                Error(InputError "order ID must be positive")

    type Isbn = private Isbn of string

    module Isbn =
        let create (text: string) =
            required "ISBN" text
            |> Result.bind (fun value ->
                let hasExpectedLength = value.Length = 10 || value.Length = 13

                let containsOnlyDigits =
                    value |> Seq.forall (fun character -> character >= '0' && character <= '9')

                if hasExpectedLength && containsOnlyDigits then
                    Ok(Isbn value)
                else
                    Error(InputError "ISBN must contain 10 or 13 digits"))

    type EmailAddress = private EmailAddress of string

    module EmailAddress =
        let create text =
            required "email address" text
            |> Result.bind (fun value ->
                if value.Contains("@") && not (value.Contains(" ")) then
                    Ok(EmailAddress value)
                else
                    Error(InputError "email address must contain @ and no spaces"))

    type Money = private Money of int

    module Money =
        let zero = Money 0

        let create cents =
            if cents >= 0 then
                Ok(Money cents)
            else
                Error(InputError "money must not be negative")

        let add (Money left) (Money right) = Money(left + right)
        let multiply quantity (Money amount) = Money(quantity * amount)

    type DiscountPercent = private DiscountPercent of int

    module DiscountPercent =
        let create value =
            if value >= 0 && value <= 100 then
                Ok(DiscountPercent value)
            else
                Error(InputError "discount must be between 0 and 100")

        let discountAmount (DiscountPercent percent) (Money amount) = Money(amount * percent / 100)

        let apply (DiscountPercent percent) (Money amount) = Money(amount - amount * percent / 100)

    type LastFourDigits = private LastFourDigits of string

    module LastFourDigits =
        let create (text: string) =
            let containsOnlyDigits =
                text |> Seq.forall (fun character -> character >= '0' && character <= '9')

            if text.Length = 4 && containsOnlyDigits then
                Ok(LastFourDigits text)
            else
                Error(InputError "card detail must contain exactly four digits")

    type TrackingNumber = private TrackingNumber of string

    module TrackingNumber =
        let create text =
            required "tracking number" text |> Result.map TrackingNumber

    type Genre =
        | Fiction
        | History
        | Science

    type Author = { Name: string }

    type Book = {
        Id: BookId
        Isbn: Isbn
        Title: string
        Authors: Author list
        Genres: Set<Genre>
        Price: Money
    }

    type Membership =
        | Standard
        | Member of DiscountPercent

    type Customer = {
        Id: CustomerId
        Name: string
        Email: EmailAddress option
        Membership: Membership
    }

    type Stock = private { OnHand: int; Reserved: int }

    module Stock =
        let create onHand =
            if onHand >= 0 then
                Ok { OnHand = onHand; Reserved = 0 }
            else
                Error(InputError "stock must not be negative")

        let available stock = stock.OnHand - stock.Reserved

        let reserve quantity stock =
            if quantity > 0 && quantity <= available stock then
                Some {
                    stock with
                        Reserved = stock.Reserved + quantity
                }
            else
                None

        let release quantity stock = {
            stock with
                Reserved = stock.Reserved - quantity
        }

    type CartLine = { BookId: BookId; Quantity: int }

    type Cart = {
        CustomerId: CustomerId
        Lines: CartLine list
    }

    type ShippingMethod =
        | Collection
        | StandardPost
        | ExpressPost

    type Payment = { Card: LastFourDigits }

    type OrderStatus =
        | AwaitingPayment
        | Paid of Payment
        | Shipped of payment: Payment * trackingNumber: TrackingNumber
        | Cancelled of reason: string

    type PricedLine = {
        BookId: BookId
        Title: string
        Quantity: int
        UnitPrice: Money
        LineTotal: Money
    }

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

    type ShopError =
        | CustomerNotFound
        | OrderNotFound
        | EmptyCart
        | BookNotFound of BookId
        | InvalidQuantity of BookId
        | NotEnoughStock of BookId * available: int
        | AlreadyPaid
        | PaymentRequired
        | OrderClosed
        | InvalidCancellationReason
        | InvalidState of InputError

    let searchCatalog (query: string) (state: State) =
        let normalized = query.Trim().ToLowerInvariant()

        state.Catalog
        |> Map.toList
        |> List.map (fun (_, book) -> book)
        |> List.filter (fun book -> book.Title.ToLowerInvariant().Contains(normalized))

    let booksInGenre genre state =
        state.Catalog
        |> Map.toList
        |> List.map (fun (_, book) -> book)
        |> List.filter (fun book -> book.Genres |> Set.contains genre)

    let lowStock threshold state =
        state.Inventory
        |> Map.toList
        |> List.choose (fun (bookId, stock) ->
            let quantity = Stock.available stock

            if quantity <= threshold then
                Some(bookId, quantity)
            else
                None)

    let addToCart bookId quantity (cart: Cart) =
        if quantity <= 0 then
            Error(InvalidQuantity bookId)
        else
            let previousQuantity =
                cart.Lines
                |> List.tryFind (fun line -> line.BookId = bookId)
                |> Option.map (fun line -> line.Quantity)
                |> Option.defaultValue 0

            let otherLines = cart.Lines |> List.filter (fun line -> line.BookId <> bookId)

            let line: CartLine = {
                BookId = bookId
                Quantity = previousQuantity + quantity
            }

            Ok { cart with Lines = line :: otherLines }

    let private priceLine (book: Book) (line: CartLine) : PricedLine = {
        BookId = book.Id
        Title = book.Title
        Quantity = line.Quantity
        UnitPrice = book.Price
        LineTotal = Money.multiply line.Quantity book.Price
    }

    let private prepareLine (catalog: Map<BookId, Book>) (inventory: Map<BookId, Stock>) (line: CartLine) =
        match Map.tryFind line.BookId catalog, Map.tryFind line.BookId inventory with
        | None, _ -> Error(BookNotFound line.BookId)
        | _, None -> Error(NotEnoughStock(line.BookId, 0))
        | Some _, Some _ when line.Quantity <= 0 -> Error(InvalidQuantity line.BookId)
        | Some book, Some stock ->
            match Stock.reserve line.Quantity stock with
            | None -> Error(NotEnoughStock(line.BookId, Stock.available stock))
            | Some reserved ->
                let inventory = inventory |> Map.add line.BookId reserved
                Ok(priceLine book line, inventory)

    let private prepareLines catalog inventory (lines: CartLine list) =
        lines
        |> List.fold
            (fun result line ->
                result
                |> Result.bind (fun (priced, currentInventory) ->
                    prepareLine catalog currentInventory line
                    |> Result.map (fun (nextLine, nextInventory) -> nextLine :: priced, nextInventory)))
            (Ok([], inventory))
        |> Result.map (fun (reversed, nextInventory) -> List.rev reversed, nextInventory)

    let private subtotal lines =
        lines |> List.fold (fun total line -> Money.add total line.LineTotal) Money.zero

    let private discountFor customer amount =
        match customer.Membership with
        | Standard -> Money.zero, amount
        | Member percent -> DiscountPercent.discountAmount percent amount, DiscountPercent.apply percent amount

    let private shippingCost method (Money subtotal) =
        match method with
        | Collection -> Money.zero
        | StandardPost -> Money 500
        | ExpressPost when subtotal >= 5000 -> Money.zero
        | ExpressPost -> Money 900

    let placeOrder shippingMethod (cart: Cart) (state: State) =
        match state.Customers |> Map.tryFind cart.CustomerId with
        | None -> Error CustomerNotFound
        | Some _ when cart.Lines = [] -> Error EmptyCart
        | Some customer ->
            prepareLines state.Catalog state.Inventory cart.Lines
            |> Result.bind (fun (lines, inventory) ->
                let beforeDiscount = subtotal lines
                let discount, afterDiscount = discountFor customer beforeDiscount
                let delivery = shippingCost shippingMethod beforeDiscount

                match OrderId.create state.NextOrderId with
                | Error inputError -> Error(InvalidState inputError)
                | Ok orderId ->
                    let order = {
                        Id = orderId
                        CustomerId = customer.Id
                        Lines = lines
                        Subtotal = beforeDiscount
                        Discount = discount
                        ShippingCost = delivery
                        Total = Money.add afterDiscount delivery
                        ShippingMethod = shippingMethod
                        Status = AwaitingPayment
                    }

                    Ok(
                        {
                            state with
                                Inventory = inventory
                                Orders = state.Orders |> Map.add orderId order
                                NextOrderId = state.NextOrderId + 1
                        },
                        order
                    ))

    let private updateOrder orderId transition state =
        match state.Orders |> Map.tryFind orderId with
        | None -> Error OrderNotFound
        | Some order ->
            transition order
            |> Result.map (fun updated -> {
                state with
                    Orders = state.Orders |> Map.add orderId updated
            })

    let pay orderId payment state =
        let transition order =
            match order.Status with
            | AwaitingPayment -> Ok { order with Status = Paid payment }
            | Paid _ -> Error AlreadyPaid
            | Shipped _
            | Cancelled _ -> Error OrderClosed

        updateOrder orderId transition state

    let ship orderId trackingNumber state =
        let transition order =
            match order.Status with
            | Paid payment ->
                Ok {
                    order with
                        Status = Shipped(payment, trackingNumber)
                }
            | AwaitingPayment -> Error PaymentRequired
            | Shipped _
            | Cancelled _ -> Error OrderClosed

        updateOrder orderId transition state

    let cancel orderId (reason: string) (state: State) =
        if reason.Trim() = "" then
            Error InvalidCancellationReason
        else
            match state.Orders |> Map.tryFind orderId with
            | None -> Error OrderNotFound
            | Some order ->
                match order.Status with
                | AwaitingPayment ->
                    let inventory =
                        order.Lines
                        |> List.fold
                            (fun current line ->
                                current |> Map.change line.BookId (Option.map (Stock.release line.Quantity)))
                            state.Inventory

                    let cancelled = {
                        order with
                            Status = Cancelled(reason.Trim())
                    }

                    Ok {
                        state with
                            Inventory = inventory
                            Orders = state.Orders |> Map.add orderId cancelled
                    }
                | Paid _
                | Shipped _
                | Cancelled _ -> Error OrderClosed

    type StatusCategory =
        | AwaitingCategory
        | PaidCategory
        | ShippedCategory
        | CancelledCategory

    let private statusCategory order =
        match order.Status with
        | AwaitingPayment -> AwaitingCategory
        | Paid _ -> PaidCategory
        | Shipped _ -> ShippedCategory
        | Cancelled _ -> CancelledCategory

    let private allOrders state =
        state.Orders |> Map.toList |> List.map (fun (_, order) -> order)

    let orderCounts state =
        state
        |> allOrders
        |> List.groupBy statusCategory
        |> List.map (fun (status, orders) -> status, List.length orders)

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

open Bookshop

let expect label result =
    match result with
    | Ok value -> value
    | Error error -> failwith $"%s{label}: %A{error}"

let bookId = BookId.create 1 |> expect "sample book ID"
let customerId = CustomerId.create 1 |> expect "sample customer ID"
let isbn = Isbn.create "9780807083697" |> expect "sample ISBN"
let email = EmailAddress.create "ada@example.org" |> expect "sample email"
let price = Money.create 1299 |> expect "sample price"
let discount = DiscountPercent.create 10 |> expect "sample discount"
let stock = Stock.create 5 |> expect "sample stock"
let card = LastFourDigits.create "4242" |> expect "sample card detail"
let tracking = TrackingNumber.create "TRACK-100" |> expect "sample tracking number"

let book = {
    Id = bookId
    Isbn = isbn
    Title = "Kindred"
    Authors = [ { Name = "Octavia E. Butler" } ]
    Genres = Set.ofList [ Fiction; History ]
    Price = price
}

let customer = {
    Id = customerId
    Name = "Ada"
    Email = Some email
    Membership = Member discount
}

let initial = {
    Catalog = Map.ofList [ (book.Id, book) ]
    Customers = Map.ofList [ (customer.Id, customer) ]
    Inventory = Map.ofList [ (book.Id, stock) ]
    Orders = Map.empty
    NextOrderId = 1
}

let emptyCart = { CustomerId = customerId; Lines = [] }
let payment = { Card = card }

let completed =
    addToCart bookId 2 emptyCart
    |> Result.bind (fun cart -> placeOrder StandardPost cart initial)
    |> Result.bind (fun (placed, order) -> pay order.Id payment placed |> Result.bind (ship order.Id tracking))

match completed with
| Error error -> printfn "Workflow failed: %A" error
| Ok finalState ->
    printfn "Search: %A" (searchCatalog "kind" finalState)
    printfn "History books: %A" (booksInGenre History finalState)
    printfn "Low stock: %A" (lowStock 3 finalState)
    printfn "Order counts: %A" (orderCounts finalState)
    printfn "Revenue: %A" (revenue finalState)

// Predict the result, run it, then try an invalid quantity and a cancelled unpaid order.
