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
    let doesNotFail ()= ()

    let sut = attempt { 
        doesNotFail()
    }
    match sut with 
    | Ok _-> ()
    | Error e ->Assert.Fail("Error")

[<Test>]
let ``attempt without exception and return value`` () =
    let doesNotFail ()= 1

    let sut = attempt<int> { 
        return doesNotFail()
    }
    match sut with 
    | Ok v-> Assert.AreEqual(1, v)
    | Error e ->Assert.Fail("Error")
