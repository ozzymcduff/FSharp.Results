#I "packages/FAKE/tools/"
#r "FakeLib.dll"

open System
open System.IO
open System.Text.RegularExpressions
open Fake
let libDir  = "./lib/bin/Debug/"
let testDir   = "./tests/bin/Debug/"

let appReferences  = 
    !! "lib/*.fsproj" 

let testReferences = 
    !! "tests/*.fsproj"

let solutionFile  = "FSharp.Results.sln"

Target "build" (fun _ ->
    !! solutionFile
    |> MSBuildDebug "" "Rebuild"
    |> ignore
)

Target "build_release" (fun _ ->
    !! solutionFile
    |> MSBuildRelease "" "Rebuild"
    |> ignore
)

Target "test" (fun _ ->  
    !! (testDir + "/Tests*.dll")
        |> NUnit (fun p -> 
            {p with
                DisableShadowCopy = true; 
                OutputFile = testDir + "TestResults.xml"})
)

Target "pack" (fun _ ->
    Paket.Pack(fun p ->
        { p with
            OutputPath = libDir})
)

Target "clean" (fun _ ->
    CleanDirs [libDir; testDir]
)

Target "push" (fun _ ->
    Paket.Push(fun p ->
        { p with
            WorkingDir = libDir })
)


Target "install" DoNothing

"build_release"
    ==> "pack"

"build"
    ==> "test"


RunTargetOrDefault "build"