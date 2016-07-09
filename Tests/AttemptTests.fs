module Tests.AttemptTests


open System
open NUnit.Framework
open FSharp.Results
open Result
open Helpers
open ClubbedToDeath


let Ken = { Person.Gender = Male; Age = 28; Clothes = set ["Tie"; "Shirt"]; Sobriety = Tipsy }
let Dave = { Person.Gender = Male; Age = 41; Clothes = set ["Tie"; "Jeans"]; Sobriety = Sober }
let Ruby = { Person.Gender = Female; Age = 25; Clothes = set ["High heels"]; Sobriety = Tipsy }

[<Test>]
let part1() =
    ClubbedToDeath.costToEnter Dave |> shouldBeError "Too old!"
    ClubbedToDeath.costToEnter Ken |> shouldBeOk 5m
    ClubbedToDeath.costToEnter Ruby |> shouldBeOk 0m
    ClubbedToDeath.costToEnter { Ruby with Age = 17 } |> shouldBeError "Too young!"
    ClubbedToDeath.costToEnter { Ken with Sobriety = Unconscious } |> shouldBeError "Sober up!"


[<Test>]
let ``Using CE syntax should be equivilent to bind`` () =
    let sut =
        attempt {
            let! bob = Ok "bob"
            let greeting = sprintf "Hello %s" bob
            return greeting
        }
    sut |> shouldBeOk (sprintf "Hello %s" "bob")

[<Test>]
let ``Try .. with works in CE syntax`` () =
    let sut =
        attempt {
            return
                try
                    failwith "bang"
                    "not bang"
                with
                | e -> e.Message
        }
    sut |> shouldBeOk "bang"

[<Test>]
let ``Try .. finally works in CE syntax`` () =
    let i = ref 0
    try
        attempt {
            try
                failwith "bang"
            finally
                i := 1
        }
    with
    | e -> Ok 
    |> ignore

    Assert.AreEqual(1, !i )
