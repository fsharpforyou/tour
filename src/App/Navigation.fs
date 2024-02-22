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
