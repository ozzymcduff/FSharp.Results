module Tests.ResultTests

open System
open Xunit
open FSharp.Results
open Result
open Helpers

type EmailValidation = 
  | EmptyEmail
  | NoAt

let failIfEmpty email = 
  if String.IsNullOrEmpty(email) then Error EmptyEmail
  else Ok email

let failIfNotAt (email : string) = 
  if (email.Contains("@")) then Ok email
  else Error NoAt

let validateEmail = failIfEmpty >> bind failIfNotAt

let testValidateEmail email (expected : Result<string, EmailValidation>) = 
  let actual = validateEmail email
  Assert.Equal(expected, actual)

[<Fact>]
let ``Can chain together successive validations``() = 
  testValidateEmail "" (Error EmptyEmail)
  testValidateEmail "something_else" (Error NoAt)
  testValidateEmail "some@email.com" (Ok "some@email.com")

let addOneOk (v : int) = Ok(v + 1)

[<Fact>]
let ``bind should modify result Ok value``() = 
  Ok 42
  |> bind addOneOk
  |> shouldBeOkWithValue 43

[<Fact>]
let ``bind Error should not modify Error``() = 
  Error "Error"
  |> bind addOneOk
  |> shouldBeErrorWithValue "Error"

let toUpper (v : string) = v.ToUpper()

[<Fact>]
let MapWillTransformOkValues() = 
  Ok "some@email.com"
  |> map toUpper
  |> shouldBeOkWithValue "SOME@EMAIL.COM"

[<Fact>]
let MapWillNotTransformErrorValues() = 
  Error "my error"
  |> map toUpper
  |> shouldBeErrorWithValue "my error"

[<Fact>]
let MapErrorWillTransformErrorValues() = 
  Error "my error"
  |> mapError toUpper
  |> shouldBeErrorWithValue "MY ERROR"

[<Fact>]
let MapErrorWillNotTransformOkValues() = 
  Ok "some@email.com"
  |> mapError toUpper
  |> shouldBeOkWithValue "some@email.com"

//
[<Fact>]
let ``mapError if Ok should not modify result``() = 
  Ok 42
  |> mapError (fun _ -> [ "err1" ])
  |> shouldBeOkWithValue 42

[<Fact>]
let ``mapError if Error should map over error``() = 
  Error "error"
  |> mapError (fun _ -> [ 42 ])
  |> shouldBeErrorWithValue [ 42 ]
