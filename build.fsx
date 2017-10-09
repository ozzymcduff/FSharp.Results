#I "packages/FAKE/tools/"
#r "FakeLib.dll"

open System
open System.IO
open System.Text.RegularExpressions
open Fake
open Fake.Testing.XUnit2
let libDir  = "./FSharp.Results/bin/Debug/"
let testDir   = "./Tests/bin/Debug/"

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
    !! (Path.Combine(testDir, "*Tests*.dll"))
        |> xUnit2 (fun p -> 
            p)
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