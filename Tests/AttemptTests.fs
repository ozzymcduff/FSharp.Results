module Tests.AttemptTests


open System
open NUnit.Framework
open FSharp.Results
open Result
open FsUnit
open Helpers


type Sobriety = 
    | Sober
    | Tipsy
    | Drunk
    | Paralytic
    | Unconscious

type Gender = 
    | Male
    | Female

type Person = 
    { Gender : Gender
      Age : int
      Clothes : string Set
      Sobriety : Sobriety }

// Let's define the checks that *all* nightclubs make!
module Club = 
    let checkAge (p : Person) = 
        if p.Age < 18 then Error "Too young!"
        elif p.Age > 40 then Error "Too old!"
        else Ok p
    
    let checkClothes (p : Person) = 
        if p.Gender = Male && not (p.Clothes.Contains "Tie") then Error "Smarten up!"
        elif p.Gender = Female && p.Clothes.Contains "Trainers" then Error "Wear high heels"
        else Ok p
    
    let checkSobriety (p : Person) = 
        match p.Sobriety with
        | Drunk | Paralytic | Unconscious -> Error "Sober up!"
        | _ -> Ok p

module ClubbedToDeath =
    open Club
    
    let costToEnter p =
        attempt {
            let! a = checkAge p
            let! b = checkClothes a
            let! c = checkSobriety b
            return 
                match c.Gender with
                | Female -> 0m
                | Male -> 5m
        }

let Ken = { Person.Gender = Male; Age = 28; Clothes = set ["Tie"; "Shirt"]; Sobriety = Tipsy }
let Dave = { Person.Gender = Male; Age = 41; Clothes = set ["Tie"; "Jeans"]; Sobriety = Sober }
let Ruby = { Person.Gender = Female; Age = 25; Clothes = set ["High heels"]; Sobriety = Tipsy }

[<Test>]
let part1() =
    ClubbedToDeath.costToEnter Dave |> shouldBeError |> should equal "Too old!"
    ClubbedToDeath.costToEnter Ken |> shouldBeOk |> should equal 5m
    ClubbedToDeath.costToEnter Ruby |> shouldBeOk |> should equal 0m
    ClubbedToDeath.costToEnter { Ruby with Age = 17 } |> shouldBeError|> should equal "Too young!"
    ClubbedToDeath.costToEnter { Ken with Sobriety = Unconscious } |> shouldBeError|> should equal "Sober up!"

