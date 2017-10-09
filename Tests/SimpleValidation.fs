module Tests.SimpleValidation

open System
open Xunit
open FSharp.Results
open Result
open Helpers

type Request = 
  { Name : string
    EMail : string }

let validateInput input = 
  if input.Name = "" then Error "Name must not be blank"
  elif input.EMail = "" then Error "Email must not be blank"
  else Ok input // happy path

let validate1 input = 
  if input.Name = "" then Error "Name must not be blank"
  else Ok input

let validate2 input = 
  if input.Name.Length > 50 then Error "Name must not be longer than 50 chars"
  else Ok input

let validate3 input = 
  if input.EMail = "" then Error "Email must not be blank"
  else Ok input

let combinedValidation = 
  // connect the two-tracks together
  validate1
  >> bind validate2
  >> bind validate3

[<Fact>]
let ``should find empty name``() = 
  { Name = ""
    EMail = "" }
  |> combinedValidation
  |> shouldBeErrorWithValue "Name must not be blank"

[<Fact>]
let ``should find empty mail``() = 
  { Name = "Scott"
    EMail = "" }
  |> combinedValidation
  |> shouldBeErrorWithValue "Email must not be blank"

[<Fact>]
let ``should find long name``() = 
  { Name = "ScottScottScottScottScottScottScottScottScottScottScottScottScottScottScottScottScottScottScott"
    EMail = "" }
  |> combinedValidation
  |> shouldBeErrorWithValue "Name must not be longer than 50 chars"

[<Fact>]
let ``should not complain on valid data``() = 
  let scott = 
    { Name = "Scott"
      EMail = "scott@chessie.com" }
  scott
  |> combinedValidation
  |> shouldBeOkWithValue scott

let canonicalizeEmail input = { input with EMail = input.EMail.Trim().ToLower() }
let usecase = combinedValidation >> (map canonicalizeEmail)

[<Fact>]
let ``should canonicalize valid data``() = 
  { Name = "Scott"
    EMail = "SCOTT@CHESSIE.com" }
  |> usecase
  |> shouldBeOkWithValue ({ Name = "Scott"
                            EMail = "scott@chessie.com" })

[<Fact>]
let ``should not canonicalize invalid data``() = 
  { Name = ""
    EMail = "SCOTT@CHESSIE.com" }
  |> usecase
  |> shouldBeErrorWithValue "Name must not be blank"
