module ClubbedToDeath

open FSharp.Results
open Result
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
