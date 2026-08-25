type Category = Category of name: string * children: Category list

let rec countCategories category =
    match category with
    | Category(_, children) ->
        let add total value = total + value

        let childCounts = children |> List.map countCategories

        1 + (childCounts |> List.fold add 0)

let rec categoryNames category =
    match category with
    | Category(name, children) -> name :: (children |> List.collect categoryNames)

let categoryTree =
    Category(
        "Bookshop",
        [
            Category("Fiction", [ Category("Fantasy", []); Category("Science fiction", []) ])
            Category("Non-fiction", [])
        ]
    )

printfn "Category count: %d" (countCategories categoryTree)
printfn "Category names: %A" (categoryNames categoryTree)

// Try adding a nested Biography category and predict both outputs.
