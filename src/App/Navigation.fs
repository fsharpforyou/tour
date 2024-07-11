module Navigation

open Feliz.Router

[<RequireQualifiedAccess>]
type Page =
    | Homepage
    | Docs
    | DocsEntry of Documentation.Entry
    | Playground of data: string option
    | NotFound

[<RequireQualifiedAccess>]
module Page =
    let fromUrl docEntries url =
        match url with
        | [] -> Page.Homepage
        | [ "playground" ] -> Page.Playground None
        | [ "playground"; Route.Query [ "data", dataUrl ] ] -> Page.Playground(Some dataUrl)
        | "docs" :: routeSegments ->
            match routeSegments with
            | [] -> Page.Docs
            | routeSegments ->
                printfn "route segments %A" routeSegments

                docEntries
                |> List.tryFind (fun (entry: Documentation.Entry) -> entry.Route = routeSegments)
                |> Option.map Page.DocsEntry
                |> Option.defaultValue Page.NotFound
        | _ -> Page.NotFound

type NavigationEntry = { Title: string; Route: string list }

[<RequireQualifiedAccess>]
module NavigationEntry =
    let fromDocEntry (docEntry: Documentation.Entry) = {
        Title = docEntry.Title
        Route = docEntry.Route
    }


type DocEntryNavigation = {
    PreviousEntry: NavigationEntry option
    NextEntry: NavigationEntry option
}

let getDocEntryNavigation (docPage: Documentation.Entry) (allEntries: Documentation.Entry list) =
    let previousDocEntry, nextDocEntry =
        match List.tryFindIndex (fun elem -> elem = docPage) allEntries with
        | None -> None, None
        | Some currentIndex ->
            let previous = List.tryItem (currentIndex - 1) allEntries
            let next = List.tryItem (currentIndex + 1) allEntries
            previous, next

    {
        PreviousEntry = previousDocEntry |> Option.map NavigationEntry.fromDocEntry
        NextEntry = nextDocEntry |> Option.map NavigationEntry.fromDocEntry
    }
