let tidyTitle (title: string) = title.Trim()

let normalizeCase (title: string) = title.ToUpperInvariant()

let addCatalogPrefix title = "Catalog: " + title

let countCharacters (text: string) = text.Length

let prepareCatalogTitle = tidyTitle >> normalizeCase >> addCatalogPrefix

let prepareAndCount = prepareCatalogTitle >> countCharacters

let first = prepareCatalogTitle "  the dispossessed  "
let second = prepareCatalogTitle "  kindred  "

printfn "%s" first
printfn "%s" second
printfn "First label length: %d" (prepareAndCount "  the dispossessed  ")

// Rewrite prepareCatalogTitle with << and verify the output is unchanged.
