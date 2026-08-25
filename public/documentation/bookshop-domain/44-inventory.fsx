type BookId = BookId of int

type Stock = {
    BookId: BookId
    OnHand: int
    Reserved: int
}

type InventoryError =
    | BookNotStocked
    | InvalidQuantity
    | InsufficientStock of available: int

let available stock = stock.OnHand - stock.Reserved

let reserve quantity stock =
    if quantity <= 0 then
        Error InvalidQuantity
    elif quantity > available stock then
        Error(InsufficientStock(available stock))
    else
        Ok {
            stock with
                Reserved = stock.Reserved + quantity
        }

let reserveBook bookId quantity inventory =
    match inventory |> Map.tryFind bookId with
    | None -> Error BookNotStocked
    | Some stock ->
        reserve quantity stock
        |> Result.map (fun updated -> inventory |> Map.add bookId updated)

let bookId = BookId 1

let inventory =
    Map.ofList [
        (bookId,
         {
             BookId = bookId
             OnHand = 5
             Reserved = 1
         })
    ]

printfn "Reservation: %A" (reserveBook bookId 2 inventory)

// Try reserving five copies and inspect the explicit error.
