type BookId = BookId of int

type CartLine = { BookId: BookId; Quantity: int }

type Cart = { Lines: CartLine list }

type CartError = InvalidQuantity

let addLine bookId quantity cart =
    if quantity <= 0 then
        Error InvalidQuantity
    else
        match cart.Lines |> List.tryFind (fun line -> line.BookId = bookId) with
        | None ->
            Ok {
                cart with
                    Lines = { BookId = bookId; Quantity = quantity } :: cart.Lines
            }
        | Some existing ->
            let updatedLines =
                cart.Lines
                |> List.map (fun line ->
                    if line.BookId = bookId then
                        {
                            line with
                                Quantity = existing.Quantity + quantity
                        }
                    else
                        line)

            Ok { cart with Lines = updatedLines }

let removeLine bookId cart = {
    cart with
        Lines = cart.Lines |> List.filter (fun line -> line.BookId <> bookId)
}

let emptyCart = { Lines = [] }

let result =
    emptyCart
    |> addLine (BookId 1) 1
    |> Result.bind (addLine (BookId 2) 2)
    |> Result.bind (addLine (BookId 1) 3)

printfn "Cart: %A" result

// Try adding a zero quantity, then remove BookId 2 from a successful cart.
