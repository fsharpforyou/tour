type BookId = BookId of int
type Money = Money of int

type Book = {
    Id: BookId
    Title: string
    Price: Money
}

type CartLine = { BookId: BookId; Quantity: int }

type Stock = { OnHand: int; Reserved: int }

type PricedLine = {
    BookId: BookId
    Title: string
    Quantity: int
    UnitPrice: Money
    LineTotal: Money
}

type CheckoutError =
    | BookNotFound of BookId
    | InvalidQuantity of BookId
    | InsufficientStock of BookId * available: int

let available stock = stock.OnHand - stock.Reserved

let priceLine (book: Book) quantity : PricedLine =
    let (Money unitPrice) = book.Price

    {
        BookId = book.Id
        Title = book.Title
        Quantity = quantity
        UnitPrice = book.Price
        LineTotal = Money(unitPrice * quantity)
    }

let validateLine (catalog: Map<BookId, Book>) (inventory: Map<BookId, Stock>) (line: CartLine) =
    match Map.tryFind line.BookId catalog, Map.tryFind line.BookId inventory with
    | None, _ -> Error(BookNotFound line.BookId)
    | _, None -> Error(InsufficientStock(line.BookId, 0))
    | Some _, Some _ when line.Quantity <= 0 -> Error(InvalidQuantity line.BookId)
    | Some _, Some stock when line.Quantity > available stock -> Error(InsufficientStock(line.BookId, available stock))
    | Some book, Some _ -> Ok(priceLine book line.Quantity)

let validateLines catalog inventory (lines: CartLine list) =
    lines
    |> List.fold
        (fun result line ->
            result
            |> Result.bind (fun validated ->
                validateLine catalog inventory line
                |> Result.map (fun priced -> priced :: validated)))
        (Ok [])
    |> Result.map List.rev

let book = {
    Id = BookId 1
    Title = "Kindred"
    Price = Money 1299
}

let catalog = Map.ofList [ (book.Id, book) ]
let inventory = Map.ofList [ (book.Id, { OnHand = 3; Reserved = 0 }) ]
let cart: CartLine list = [ { BookId = book.Id; Quantity = 2 } ]

printfn "Validated lines: %A" (validateLines catalog inventory cart)

// Change the quantity to four and predict the error before running.
