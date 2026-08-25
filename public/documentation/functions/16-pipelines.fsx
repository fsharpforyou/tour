let tidyTitle (title: string) = title.Trim()

let addCatalogPrefix title = "Catalog: " + title

let surround left right text = left + text + right

let countCharacters (text: string) = text.Length

let rawTitle = "  A Wizard of Earthsea  "

let catalogLabel = rawTitle |> tidyTitle |> addCatalogPrefix |> surround "[" "]"

let characterCount = catalogLabel |> countCharacters

printfn "%s" catalogLabel
printfn "The final label has %d characters" characterCount

// Rewrite catalogLabel with nested function application.
