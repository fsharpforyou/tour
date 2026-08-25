type Shelf(label: string, capacity: int) =
    member _.Label = label
    member _.Capacity = capacity
    member _.HasSpace(bookCount: int) = bookCount < capacity

    member this.Describe() =
        $"%s{this.Label}: space for %d{this.Capacity} books"

let shelf = Shelf("Science fiction", 40)

printfn "%s" (shelf.Describe())
printfn "Has space after 37 books: %b" (shelf.HasSpace(37))

// Build a second shelf and compare its properties and method results.
