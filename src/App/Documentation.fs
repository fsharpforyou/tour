module Documentation

open Feliz.Router
open Thoth.Json

type Entry = {
    Title: string
    Route: string list
    FSharpCode: string
    MarkdownDocumentation: string
}

type Category = { Title: string; Pages: Entry list }

type TableOfContents = {
    RootMarkdown: string
    Categories: Category list
}

[<RequireQualifiedAccess>]
module TableOfContents =
    let allEntries tableOfContents =
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

let decodePage =
    Decode.object (fun get -> {|
        title = get.Required.Field "title" Decode.string
        routeSegment = get.Required.Field "route_segment" Decode.string
        fsharpFile = get.Required.Field "fsharp_file" Decode.string
        markdownFile = get.Required.Field "markdown_file" Decode.string
    |})

let decodeCategory =
    Decode.object (fun get -> {|
        title = get.Required.Field "title" Decode.string
        routeSegment = get.Required.Field "route_segment" Decode.string
        pages = get.Required.Field "pages" (Decode.list decodePage)
    |})

let decoder =
    Decode.object (fun get -> {|
        rootMarkdownFile = get.Required.Field "root" Decode.string
        categories = get.Required.Field "categories" (Decode.list decodeCategory)
    |})

let documentationPath path = Constants.documentation + path
let emptyTableOfContents = { RootMarkdown = ""; Categories = [] }

let fetchAsString path =
    Fetch.fetch path [] |> Promise.bind (fun res -> res.text ())

let loadTableOfContents () =
    Fetch.fetch Constants.tableOfContents []
    |> Promise.bind (fun response -> response.json ())
    |> Promise.map (fun json -> Decode.fromValue "$" decoder json)
    |> Promise.bind (fun result ->
        match result with
        | Error _ -> Promise.lift emptyTableOfContents
        | Ok contents ->
            promise {
                let! rootMarkdown = fetchAsString (documentationPath contents.rootMarkdownFile)

                let! categories =
                    contents.categories
                    |> Seq.map (fun category ->
                        promise {
                            let! pages =
                                category.pages
                                |> List.map (fun page ->
                                    promise {
                                        let! fsharpCode = fetchAsString (documentationPath page.fsharpFile)
                                        let! markdownDoc = fetchAsString (documentationPath page.markdownFile)

                                        return {
                                            Title = page.title
                                            Route = [ category.routeSegment; page.routeSegment ]
                                            FSharpCode = fsharpCode
                                            MarkdownDocumentation = markdownDoc
                                        }
                                    })
                                |> Promise.all

                            return {
                                Title = category.title
                                Pages = Seq.toList pages
                            }
                        })
                    |> Promise.all

                return {
                    RootMarkdown = rootMarkdown
                    Categories = Seq.toList categories
                }
            })
