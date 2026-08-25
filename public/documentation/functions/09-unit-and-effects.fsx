let makeLabel title = "Featured book: " + title

let printDivider () =
    printfn "%s" "------------------------------"

let announce title =
    printDivider ()
    printfn "%s" (makeLabel title)
    printDivider ()

announce "Kindred"

// Predict the output order, then change makeLabel without changing announce.
