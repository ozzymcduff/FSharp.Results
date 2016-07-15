module Tests.AttemptTests


open System
open NUnit.Framework
open FSharp.Results
open Result
open Helpers
open Attempt
let getExnMessage e=
    (e :> exn).Message

[<Test>]
let ``attempt with exception`` () =
    let sut = attempt { 
        failwith "bang" 
    }
    match runAttempt sut with 
    | Ok _->Assert.Fail("Ok")
    | Error e ->Assert.AreEqual("bang", getExnMessage e)

[<Test>]
let ``attempt without exception`` () =
    let doesNotFail ()= ()

    let sut = attempt { 
        doesNotFail()
    }
    match runAttempt sut with 
    | Ok _-> ()
    | Error e ->Assert.Fail("Error")

[<Test>]
let ``attempt without exception and return value`` () =
    let doesNotFail ()= 1

    let sut = attempt { 
        return doesNotFail()
    }
    match runAttempt sut with 
    | Ok v-> Assert.AreEqual(1, v)
    | Error e ->Assert.Fail("Error")
