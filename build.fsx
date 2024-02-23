#r "nuget: Fun.Build"
#r "nuget: Fake.IO.FileSystem"

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

    let verify =
        stage "Verify" {
            stage "Check Formatting" { run "dotnet fantomas --check ." }
            stage "Build" { run "dotnet build" }
        }

    let bundle = stage "Bundle" { run "npm run bundle" }

pipeline "setup" {
    Stages.dotnetRestore
    Stages.npmInstall
    Stages.clean
    Stages.copyModules

    runIfOnlySpecified
}

pipeline "verify" {
    Stages.dotnetRestore
    Stages.npmInstall
    Stages.clean
    Stages.copyModules
    Stages.verify

    runIfOnlySpecified
}

pipeline "bundle" {
    Stages.dotnetRestore
    Stages.npmInstall
    Stages.clean
    Stages.copyModules
    Stages.verify
    Stages.bundle

    runIfOnlySpecified
}
