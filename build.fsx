#r "nuget: Fun.Build"
#r "nuget: Fake.IO.FileSystem"

open System.IO
open Fun.Build
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators

module Directories =
    let sourceDir = __SOURCE_DIRECTORY__
    let fableStandaloneNpmSrc = sourceDir </> "node_modules/fable-standalone/src"
    let fableStandaloneNpmDist = sourceDir </> "node_modules/fable-standalone/dist"
    let fableMetadataLib = sourceDir </> "node_modules/fable-metadata/lib/"
    let publicReplOutput = sourceDir </> "public/js/repl"
    let fableStandaloneFSharp = sourceDir </> "src/Standalone"
    let publicMetadataOutput = sourceDir </> "public/metadata"

module Stages =
    let clean =
        stage "Clean" { run (fun _ -> !! "public/js" ++ "dist/" |> Shell.cleanDirs) }

    let dotnetRestore =
        stage "Restore .NET dependencies" {
            run "dotnet tool restore"
            run "dotnet restore"
        }

    let npmInstall = stage "NPM install" { run "npm install" }

    let copyModules =
        stage "Copy modules" {
            run (fun _ ->
                Shell.cleanDir Directories.publicMetadataOutput
                Shell.copyDir Directories.publicMetadataOutput Directories.fableMetadataLib (fun _ -> true)

                // Change extension to .txt so Github pages compress the files when being served
                !!(Directories.publicMetadataOutput </> "*.dll")
                |> Seq.iter (fun filename -> Shell.rename (filename + ".txt") filename)

                Shell.copyDir Directories.publicReplOutput Directories.fableStandaloneNpmDist (fun _ -> true)

                Shell.copyDir Directories.fableStandaloneFSharp Directories.fableStandaloneNpmSrc (fun f ->
                    f.EndsWith(".fs")))
        }

pipeline "setup" {
    Stages.dotnetRestore
    Stages.npmInstall
    Stages.clean
    Stages.copyModules

    // stage "Copy *.js files into js/repl" {
    //     run (fun ctx ->
    //         for path in Directory.EnumerateFiles(Directories.fableStandaloneNpm </> "dist", "*.js") do
    //             let destination = Directories.publicReplOutput </> Path.GetFileName(path)
    //             File.Copy(path, destination)

    //         Ok())
    // }

    // // TODO: Need to copy into subdirectory??? "Standalone/*.fs & Standalone/Worker/*.fs"
    // // TODO: I can probably just stick this in the src/App/Standalone directory instead of src/Standalone
    // stage "Copy *.fs files into Standalone project" {
    //     run (fun ctx ->
    //         for path in
    //             Directory.EnumerateFiles(
    //                 Directories.fableStandaloneNpm </> "src",
    //                 "*.fs",
    //                 EnumerationOptions(RecurseSubdirectories = true)
    //             ) do
    //             let destination = Directories.fableStandaloneFSharp </> Path.GetFileName(path)
    //             File.Copy(path, destination)

    //         Ok())
    // }

    // stage "Copy metadata folders" {
    //     run (fun ctx ->
    //         for path in Directory.EnumerateFiles(Directories.fableMetadataLib) do
    //             let destination = Directories.publicMetadataOutput </> Path.GetFileName(path)
    //             File.Copy(path, destination)

    //         Ok())
    // }

    runIfOnlySpecified
}
