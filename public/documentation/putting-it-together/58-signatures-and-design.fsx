type BookId = BookId of int
type Money = Money of int

type Book = {
    Id: BookId
    Title: string
    Price: Money
}

type CartLine = { BookId: BookId; Quantity: int }

type PricedLine = {
    BookId: BookId
    Quantity: int
    UnitPrice: Money
    Total: Money
}

type CheckoutError = InvalidQuantity of BookId

let validateQuantity (line: CartLine) =
    if line.Quantity > 0 then
        Ok line
    else
        Error(InvalidQuantity line.BookId)

let calculateLineTotal (Money price) quantity = Money(price * quantity)

let createPricedLine (book: Book) (line: CartLine) : PricedLine = {
    BookId = book.Id
    Quantity = line.Quantity
    UnitPrice = book.Price
    Total = calculateLineTotal book.Price line.Quantity
}

let priceRequestedLine (book: Book) (line: CartLine) =
    line |> validateQuantity |> Result.map (createPricedLine book)

let book = {
    Id = BookId 1
    Title = "Kindred"
    Price = Money 1299
}

let validLine: CartLine = { BookId = book.Id; Quantity = 2 }
let invalidLine: CartLine = { BookId = book.Id; Quantity = 0 }

printfn "Valid: %A" (priceRequestedLine book validLine)
printfn "Invalid: %A" (priceRequestedLine book invalidLine)

// Add a maximum-quantity rule without changing createPricedLine.
