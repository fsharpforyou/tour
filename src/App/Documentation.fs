[<RequireQualifiedAccess>]
module Documentation

open Feliz.Router
open Thoth.Json

type Page = {
    Title: string
    Route: string list
    MarkdownDocumentation: string
}

type Category = { Title: string; Pages: Page list }

type TableOfContents = {
    RootMarkdown: string
    Categories: Category list
}

[<RequireQualifiedAccess>]
module TableOfContents =
    let allPages tableOfContents =
        tableOfContents.Categories |> List.collect _.Pages

    let toMarkdownString tableOfContents =
        let bulletPoint value = $"* {value}"
        let createMarkdownLink value url = $"[{value}]({url})"

        let createHeading level value =
            let hashtags = String.replicate level "#"
            $"{hashtags} {value}"

        let createUrlForPage page = Router.format page.Route

        let categoryToPageUrls category =
            category.Pages
            |> List.map (fun page -> page |> createUrlForPage |> createMarkdownLink page.Title |> bulletPoint)

        let createMarkdownForCategory category =
            [ createHeading 2 category.Title ] @ categoryToPageUrls category

        [ createHeading 1 "Table of Contents" ]
        @ List.collect createMarkdownForCategory tableOfContents.Categories
        |> String.concat "\n"

module private Json =
    type PageJson = {
        Title: string
        RouteSegment: string
        FSharpFile: string
        MarkdownFile: string
    }

    type CategoryJson = {
        Title: string
        RouteSegment: string
        Pages: PageJson list
    }

    type TableOfContentsJson = {
        RootMarkdownFile: string
        Categories: CategoryJson list
    }

    let pageDecoder =
        Decode.object (fun get -> {
            Title = get.Required.Field "title" Decode.string
            RouteSegment = get.Required.Field "route_segment" Decode.string
            FSharpFile = get.Required.Field "fsharp_file" Decode.string
            MarkdownFile = get.Required.Field "markdown_file" Decode.string
        })

    let categoryDecoder =
        Decode.object (fun get -> {
            Title = get.Required.Field "title" Decode.string
            RouteSegment = get.Required.Field "route_segment" Decode.string
            Pages = get.Required.Field "pages" (Decode.list pageDecoder)
        })

    let tableOfContentsDecoder =
        Decode.object (fun get -> {
            RootMarkdownFile = get.Required.Field "root" Decode.string
            Categories = get.Required.Field "categories" (Decode.list categoryDecoder)
        })

let emptyTableOfContents = { RootMarkdown = ""; Categories = [] }
let private documentationPath path = Constants.documentation + path

let private fetchAsString path =
    Fetch.fetch path [] |> Promise.bind (fun res -> res.text ())

let private loadPageFromJson (categoryJson: Json.CategoryJson) (pageJson: Json.PageJson) =
    promise {
        let! markdownDoc = fetchAsString (documentationPath pageJson.MarkdownFile)

        return {
            Title = pageJson.Title
            Route = [ categoryJson.RouteSegment; pageJson.RouteSegment ]
            MarkdownDocumentation = markdownDoc
        }
    }

let private loadCategoryFromJson (categoryJson: Json.CategoryJson) =
    promise {
        let! pages =
            categoryJson.Pages
            |> List.map (fun page -> loadPageFromJson categoryJson page)
            |> Promise.all

        return {
            Title = categoryJson.Title
            Pages = Seq.toList pages
        }
    }

let private loadTableOfContentsFromJson (tableOfContentsJson: Json.TableOfContentsJson) =
    promise {
        let! rootMarkdown = fetchAsString (documentationPath tableOfContentsJson.RootMarkdownFile)
        let! categories = tableOfContentsJson.Categories |> Seq.map loadCategoryFromJson |> Promise.all

        return {
            RootMarkdown = rootMarkdown
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
