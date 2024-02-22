module Navigation

open Documentation

[<RequireQualifiedAccess>]
type Page =
    | Homepage
    | TableOfContents
    | DocEntry of Entry
    | NotFound

[<RequireQualifiedAccess>]
module Page =
    let fromUrl docEntries url =
        match url with
        | [] -> Page.Homepage
        | [ "table-of-contents" ] -> Page.TableOfContents
        | routeSegments ->
            docEntries
            |> List.tryFind (fun docEntry -> docEntry.Route = routeSegments)
            |> Option.map Page.DocEntry
            |> Option.defaultValue Page.NotFound

type CurrentEntry =
    | Entry of Entry
    | NotViewingEntry

type DocEntryNavigation = {
    PreviousEntry: Entry option
    NextEntry: Entry option
}

let getDocEntryNavigation (currentEntry: CurrentEntry) (allEntries: Entry list) =
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
