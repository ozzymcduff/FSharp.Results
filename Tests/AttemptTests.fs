module Tests.AttemptTests


open System
open NUnit.Framework
open FSharp.Results
open Result
open Helpers

[<Test>]
let ``attempt with exception`` () =
    let sut = attempt (fun ()->failwith "bang")
    match sut with 
    | Ok _->Assert.Fail("Ok")
    | Error e ->Assert.AreEqual("bang", e.Message)

[<Test>]
let ``attempt without exception`` () =
    let sut = attempt (fun ()->1)
    sut |> shouldBeOk 1