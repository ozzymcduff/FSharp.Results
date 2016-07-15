module Tests.AttemptTests


open System
open NUnit.Framework
open FSharp.Results
open Result
open Helpers
open Attempt

[<Test>]
let ``attempt with exception`` () =
    let sut = attempt { 
        failwith "bang" 
    }
    match Attempt.run sut with 
    | Ok _->Assert.Fail("Ok")
    | Error e ->Assert.AreEqual("bang", e.Message)

[<Test>]
let ``attempt without exception`` () =
    let doesNotFail ()= ()

    let sut = attempt { 
        doesNotFail()
    }
    match Attempt.run sut with 
    | Ok _-> ()
    | Error e ->Assert.Fail("Error")

[<Test>]
let ``attempt without exception and return value`` () =
    let doesNotFail ()= 1

    let sut = attempt { 
        return doesNotFail()
    }
    match Attempt.run sut with 
    | Ok v-> Assert.AreEqual(1, v)
    | Error e ->Assert.Fail("Error")

[<Test>]
let ``attempt with exception and return value`` () =
    let doesFail () :int= failwith "bang"

    let sut = attempt { 
        return doesFail()
    }
    match Attempt.run sut with 
    | Ok _->Assert.Fail("Ok")
    | Error e ->Assert.AreEqual("bang", e.Message)
