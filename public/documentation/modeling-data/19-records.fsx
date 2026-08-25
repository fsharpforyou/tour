type Book = {
    Title: string
    Author: string
    PageCount: int
}

let book = {
    Title = "Kindred"
    Author = "Octavia E. Butler"
    PageCount = 264
}

let corrected = { book with PageCount = 266 }

printfn "%s by %s" corrected.Title corrected.Author
printfn "Original pages: %d; corrected: %d" book.PageCount corrected.PageCount

// Add a Year field and let the compiler identify every construction to update.
