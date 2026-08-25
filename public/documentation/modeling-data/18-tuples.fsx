let describeBook (title, author, pages) =
    $"%s{title} by %s{author} has %d{pages} pages"

let book = ("Kindred", "Octavia E. Butler", 264)
let (title, _, pages) = book

printfn "%s" (describeBook book)
printfn "%s has %d pages" title pages

// Add a genre as a fourth position, then decide whether a record would read better.
