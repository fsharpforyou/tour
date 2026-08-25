type ListingState =
    | ForSale
    | SoldOut
    | Discontinued of reason: string

type BookListing = {
    Title: string
    Price: float
    State: ListingState
}

let markSoldOut listing = { listing with State = SoldOut }

let describe listing =
    match listing.State with
    | ForSale -> $"%s{listing.Title} costs %f{listing.Price}"
    | SoldOut -> $"%s{listing.Title} is sold out"
    | Discontinued reason -> $"%s{listing.Title} was discontinued: %s{reason}"

let listing = {
    Title = "Kindred"
    Price = 9.99
    State = ForSale
}

let soldOut = listing |> markSoldOut
printfn "%s" (describe listing)
printfn "%s" (describe soldOut)

// Predict both descriptions. Add ComingSoon of releaseDay: int, run once to see
// the incomplete-match warning, then update describe and construct the new case.
