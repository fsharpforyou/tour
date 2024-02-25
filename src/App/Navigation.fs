module Navigation

[<RequireQualifiedAccess>]
type Page =
    | Homepage
    | TableOfContents
    | DocPage of Documentation.Page
    | NotFound

[<RequireQualifiedAccess>]
module Page =
    let fromUrl docPages url =
        match url with
        | [] -> Page.Homepage
        | [ "table-of-contents" ] -> Page.TableOfContents
        | routeSegments ->
            docPages
            |> List.tryFind (fun (docPage: Documentation.Page) -> docPage.Route = routeSegments)
            |> Option.map Page.DocPage
            |> Option.defaultValue Page.NotFound

type CurrentEntry =
    | Entry of Documentation.Page
    | NotViewingEntry

type DocEntryNavigation = {
    PreviousEntry: Documentation.Page option
    NextEntry: Documentation.Page option
}

let getDocEntryNavigation (currentEntry: CurrentEntry) (allEntries: Documentation.Page list) =
    let previous, next =
        match currentEntry with
        | NotViewingEntry ->
            let previous = None
            let next = List.tryItem 0 allEntries
            previous, next
        | Entry entry ->
            match List.tryFindIndex (fun elem -> elem = entry) allEntries with
            | None -> None, None
            | Some currentIndex ->
                let previous = List.tryItem (currentIndex - 1) allEntries
                let next = List.tryItem (currentIndex + 1) allEntries
                previous, next

    {
        PreviousEntry = previous
        NextEntry = next
    }
