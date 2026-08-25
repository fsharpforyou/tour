let title = "A Wizard of Earthsea"
let author = "Ursula K. Le Guin"
let categoryCode = 'F'
let label = $"%s{title} by %s{author} — category %c{categoryCode}"

printfn "%s" label
printfn "Title length: %d" title.Length
printfn "Contains Earthsea: %b" (title.Contains("Earthsea"))

// Try changing the title and search text.
