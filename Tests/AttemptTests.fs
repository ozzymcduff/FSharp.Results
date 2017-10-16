module Tests.AttemptTests

open System
open Xunit
open FSharp.Results
open Result
open Helpers
open Attempt

[<Fact>]
let ``attempt with exception``() = 
  let sut = attempt { failwith "bang" }
  match Attempt.run sut with
  | Ok _ -> failwith "Ok"
  | Error e -> Assert.Equal("bang", e.Message)

[<Fact>]
let ``attempt without exception``() = 
  let doesNotFail() = ()
  let sut = attempt { doesNotFail() }
  match Attempt.run sut with
  | Ok _ -> ()
  | Error e -> failwith "Error"

[<Fact>]
let ``attempt without exception and return value``() = 
  let doesNotFail() = 1
  let sut = attempt { return doesNotFail() }
  match Attempt.run sut with
  | Ok v -> Assert.Equal(1, v)
  | Error e -> failwith "Error"

[<Fact>]
let ``attempt with exception and return value``() = 
  let doesFail() : int = failwith "bang"
  let sut = attempt { return doesFail() }
  match Attempt.run sut with
  | Ok _ -> failwith "Ok"
  | Error e -> Assert.Equal("bang", e.Message)
