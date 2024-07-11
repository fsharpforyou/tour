[<RequireQualifiedAccess>]
module Documentation

open Feliz.Router
open Thoth.Json

type Entry = {
    Title: string
    Route: string list
    MarkdownDocumentation: string
    GitHubUrl: string
}

type Category = { Title: string; Entries: Entry list }

type TableOfContents = {
    RootMarkdown: string
    RootGitHubUrl: string
    Categories: Category list
}

let makeGitHubUrl relativePath =
    $"https://github.com/fsharpforyou/tour/tree/main/public/documentation/{relativePath}"

[<RequireQualifiedAccess>]
module TableOfContents =
    let allEntries tableOfContents =
        tableOfContents.Categories |> List.collect _.Entries

module private Json =
    type EntryJson = {
        Title: string
        RouteSegment: string
        MarkdownFile: string
    }

    type CategoryJson = {
        Title: string
        RouteSegment: string
        Entries: EntryJson list
    }

    type TableOfContentsJson = {
        RootMarkdownFile: string
        Categories: CategoryJson list
    }

    let entryDecoder =
        Decode.object (fun get -> {
            Title = get.Required.Field "title" Decode.string
            RouteSegment = get.Required.Field "route_segment" Decode.string
            MarkdownFile = get.Required.Field "markdown_file" Decode.string
        })

    let categoryDecoder =
        Decode.object (fun get -> {
            Title = get.Required.Field "title" Decode.string
            RouteSegment = get.Required.Field "route_segment" Decode.string
            Entries = get.Required.Field "entries" (Decode.list entryDecoder)
        })

    let tableOfContentsDecoder =
        Decode.object (fun get -> {
            RootMarkdownFile = get.Required.Field "root" Decode.string
            Categories = get.Required.Field "categories" (Decode.list categoryDecoder)
        })

let emptyTableOfContents = {
    RootMarkdown = ""
    RootGitHubUrl = ""
    Categories = []
}

let private documentationPath path = Constants.documentation + path

let private fetchAsString path =
    Fetch.fetch path [] |> Promise.bind (fun res -> res.text ())

let private loadEntryFromJson (categoryJson: Json.CategoryJson) (entryJson: Json.EntryJson) =
    promise {
        let! markdownDoc = fetchAsString (documentationPath entryJson.MarkdownFile)

        return {
            Title = entryJson.Title
            Route = [ categoryJson.RouteSegment; entryJson.RouteSegment ]
            MarkdownDocumentation = markdownDoc
            GitHubUrl = makeGitHubUrl entryJson.MarkdownFile
        }
    }

let private loadCategoryFromJson (categoryJson: Json.CategoryJson) =
    promise {
        let! entries =
            categoryJson.Entries
            |> List.map (fun entry -> loadEntryFromJson categoryJson entry)
            |> Promise.all

        return {
            Title = categoryJson.Title
            Entries = Seq.toList entries
        }
    }

let private loadTableOfContentsFromJson (tableOfContentsJson: Json.TableOfContentsJson) =
    promise {
        let! rootMarkdown = fetchAsString (documentationPath tableOfContentsJson.RootMarkdownFile)
        let! categories = tableOfContentsJson.Categories |> Seq.map loadCategoryFromJson |> Promise.all

        return {
            RootMarkdown = rootMarkdown
            RootGitHubUrl = makeGitHubUrl tableOfContentsJson.RootMarkdownFile
            Categories = Seq.toList categories
        }
    }

let loadTableOfContents () =
    Fetch.fetch Constants.tableOfContents []
    |> Promise.bind (fun response -> response.json ())
    |> Promise.map (fun json -> Decode.fromValue "$" Json.tableOfContentsDecoder json)
    |> Promise.bind (
        Result.map loadTableOfContentsFromJson
        >> Result.defaultValue (Promise.lift emptyTableOfContents)
    )
