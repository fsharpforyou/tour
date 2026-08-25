type Genre =
    | Fiction
    | History
    | Science
    | Fantasy

let customerInterests = Set.ofList [ Fiction; History; Fiction ]
let bookGenres = Set.ofList [ Fiction; Science ]

let shared = Set.intersect customerInterests bookGenres
let allRelevant = Set.union customerInterests bookGenres
let unexplored = Set.difference customerInterests bookGenres

printfn "Unique interests: %d" (Set.count customerInterests)
printfn "Shared genres: %A" shared
printfn "All relevant genres: %A" allRelevant
printfn "Interests not covered by this book: %A" unexplored

// Add Fantasy to both sets and predict how each result changes.
