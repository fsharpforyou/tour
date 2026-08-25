type Author = { Name: string; Country: string }

type Book = {
    Title: string
    Author: Author
    PageCount: int
}

type InventoryItem = { Book: Book; QuantityOnHand: int }

let sellOne item = {
    item with
        QuantityOnHand = item.QuantityOnHand - 1
}

let author = {
    Name = "Ursula K. Le Guin"
    Country = "United States"
}

let book = {
    Title = "A Wizard of Earthsea"
    Author = author
    PageCount = 205
}

let stocked = { Book = book; QuantityOnHand = 3 }

let afterSale = stocked |> sellOne

printfn "%s by %s" afterSale.Book.Title afterSale.Book.Author.Name
printfn "Before: %d; after: %d" stocked.QuantityOnHand afterSale.QuantityOnHand

// Try adding a second InventoryItem that shares the same Book value.
