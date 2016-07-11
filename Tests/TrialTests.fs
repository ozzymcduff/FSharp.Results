module Tests.TrialTests


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
        trial {
            let! bob = Ok "bob"
            let greeting = sprintf "Hello %s" bob
            return greeting
        }
    sut |> shouldBeOk (sprintf "Hello %s" "bob")

[<Test>]
let ``Try .. with works in CE syntax`` () =
    let sut =
        trial {
            return!
                try
                    failwith "bang"
                    Error("not bang")
                with
                | e -> Ok( e.Message)
        }
    sut |> shouldBeOk "bang"

[<Test>]
let ``Try .. finally works in CE syntax`` () =
    let i = ref 0
    try
        trial {
            try
                failwith "bang"
            finally
                i := 1
        }
    with
    | e -> Ok () 
    |> ignore

    Assert.AreEqual(1, !i )


type ApiError=
    | FailedToConnect
    | CouldNotFindProduct
type Product = { Cost:float; Name:string }
let getProductById pid : Result<Product,ApiError> = failwith "!"
let minByCost a b = failwith "!"

let calcDiscountTotal1 prod1Id prod2Id discount : Result<float, ApiError> =
  trial {
    let! product1 = getProductById prod1Id
    let! product2 = getProductById prod2Id 
    let (c1, c2) = minByCost product1 product2
    return (c1 - (discount / 100.0 * c1) + c2)
  }