type BookId = BookId of int
type Money = Money of cents: int

type Book = {
    Id: BookId
    Title: string
    Price: Money
}

type PricedLine = {
    BookId: BookId
    Title: string
    Quantity: int
    UnitPrice: Money
    LineTotal: Money
}

type Discount =
    | NoDiscount
    | PercentOff of int

let add (Money left) (Money right) = Money(left + right)

let multiply quantity (Money unitPrice) = Money(quantity * unitPrice)

let priceLine book quantity = {
    BookId = book.Id
    Title = book.Title
    Quantity = quantity
    UnitPrice = book.Price
    LineTotal = multiply quantity book.Price
}

let subtotal lines =
    lines |> List.fold (fun total line -> add total line.LineTotal) (Money 0)

let applyDiscount discount (Money amount) =
    match discount with
    | NoDiscount -> Money amount
    | PercentOff percent -> Money(amount * (100 - percent) / 100)

let book = {
    Id = BookId 1
    Title = "Kindred"
    Price = Money 1299
}

let lines = [ priceLine book 2 ]
let total = lines |> subtotal |> applyDiscount (PercentOff 10)

printfn "Discounted total: %A" total

// Predict the cents for three copies before changing the quantity.
