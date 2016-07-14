module Tests.AttemptTests


open System
open NUnit.Framework
open FSharp.Results
open Result
open Helpers

[<Test>]
let ``attempt with exception`` () =
    let sut = attempt { 
        failwith "bang" 
    }
    match sut with 
    | Ok _->Assert.Fail("Ok")
    | Error e ->Assert.AreEqual("bang", e.Message)

[<Test>]
let ``attempt without exception`` () =
    let doesNotFail ()= if 1<>1 then () else ()

    let sut = attempt { 
        doesNotFail()
    }
    match sut with 
    | Ok _-> ()
    | Error e ->Assert.Fail("Error")
