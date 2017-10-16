module Tests.TrialTests

open System
open Xunit
open FSharp.Results
open Result
open Helpers
open ClubbedToDeath

let Ken = 
  { Person.Gender = Male
    Age = 28
    Clothes = set [ "Tie"; "Shirt" ]
    Sobriety = Tipsy }

let Dave = 
  { Person.Gender = Male
    Age = 41
    Clothes = set [ "Tie"; "Jeans" ]
    Sobriety = Sober }

let Ruby = 
  { Person.Gender = Female
    Age = 25
    Clothes = set [ "High heels" ]
    Sobriety = Tipsy }

[<Fact>]
let part1() = 
  ClubbedToDeath.costToEnter Dave |> shouldBeErrorWithValue "Too old!"
  ClubbedToDeath.costToEnter Ken |> shouldBeOkWithValue 5m
  ClubbedToDeath.costToEnter Ruby |> shouldBeOkWithValue 0m
  ClubbedToDeath.costToEnter { Ruby with Age = 17 } |> shouldBeErrorWithValue "Too young!"
  ClubbedToDeath.costToEnter { Ken with Sobriety = Unconscious } |> shouldBeErrorWithValue "Sober up!"

[<Fact>]
let ``Using CE syntax should be equivilent to bind``() = 
  let sut = 
    trial { 
      let! bob = Ok "bob"
      let greeting = sprintf "Hello %s" bob
      return greeting
    }
  sut |> shouldBeOkWithValue (sprintf "Hello %s" "bob")

[<Fact>]
let ``Try .. with works in CE syntax``() = 
  let sut = 
    trial { 
      return! try 
                failwith "bang"
                Error("not bang")
              with e -> Ok(e.Message)
    }
  sut |> shouldBeOkWithValue "bang"

[<Fact>]
let ``Try .. finally works in CE syntax``() = 
  let i = ref 0
  try 
    trial { 
      try 
        failwith "bang"
      finally
        i := 1
    }
  with e -> Ok()
  |> ignore
  Assert.Equal(1, !i)

let errorIfFalse v = 
  if v then Ok()
  else Error()

let func param sideEffect = 
  trial { 
    do! errorIfFalse param
    sideEffect()
    return param
  }

[<Fact>]
let ``SideEffects 1: Should return correct value of happy path``() = 
  let mutable count = 0
  let sideEffect() = count <- count + 1
  let res = func true sideEffect
  Assert.Equal(Ok true, res)
  Assert.Equal(1, count)

[<Fact>]
let ``SideEffects 1: Should return correct value of failing path``() = 
  let mutable count = 0
  let sideEffect() = count <- count + 1
  let res = func false sideEffect
  Assert.Equal(Error(), res)
  Assert.Equal(0, count)

let funcDo param sideEffect = 
  trial { 
    do! errorIfFalse param
    do! sideEffect()
    return param
  }

[<Fact>]
let ``SideEffects 2 do: Should return correct value of happy path``() = 
  let mutable count = 0
  
  let sideEffect() = 
    count <- count + 1
    Ok()
  
  let res = funcDo true sideEffect
  Assert.Equal(Ok true, res)
  Assert.Equal(1, count)

[<Fact>]
let ``SideEffects 2 do: Should return correct value of failing path``() = 
  let mutable count = 0
  
  let sideEffect() = 
    count <- count + 1
    Ok()
  
  let res = funcDo false sideEffect
  Assert.Equal(Error(), res)
  Assert.Equal(0, count)

type ApiError = 
  | FailedToConnect
  | CouldNotFindProduct

type Product = 
  { Cost : float
    Name : string }

let getProductById pid : Result<Product, ApiError> = failwith "!"
let minByCost a b = failwith "!"

let calcDiscountTotal1 prod1Id prod2Id discount : Result<float, ApiError> = 
  trial { 
    let! product1 = getProductById prod1Id
    let! product2 = getProductById prod2Id
    let (c1, c2) = minByCost product1 product2
    return (c1 - (discount / 100.0 * c1) + c2)
  }