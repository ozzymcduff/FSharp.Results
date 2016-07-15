module Tests.ResultTests

open System
open NUnit.Framework
open FSharp.Results
open Result
open Helpers

type EmailValidation=
    | EmptyEmail
    | NoAt

let failIfEmpty email=
    if String.IsNullOrEmpty(email) then Error EmptyEmail else Ok email

let failIfNotAt (email:string)=
    if (email.Contains("@")) then Ok email else Error NoAt

let validateEmail =
    failIfEmpty
    >> bind failIfNotAt

let testValidateEmail email (expected:Result<string,EmailValidation>) =
    let actual = validateEmail email
    Assert.AreEqual(expected,actual)

[<Test>]
let ``Can chain together successive validations``() =
    testValidateEmail "" (Error EmptyEmail)
    testValidateEmail "something_else" (Error NoAt)
    testValidateEmail "some@email.com" (Ok "some@email.com")

let addOneOk (v:int) = Ok (v+1)

[<Test>]
let ``bind should modify result Ok value`` () =
    Ok 42
    |> bind addOneOk
    |> shouldBeOkWithValue 43

[<Test>]
let ``bind Error should not modify Error`` () =
    Error "Error"
    |> bind addOneOk
    |> shouldBeErrorWithValue "Error"

let toUpper (v:string) = v.ToUpper()

[<Test>]
let MapWillTransformOkValues() =
    Ok "some@email.com" 
    |> map toUpper
    |> shouldBeOkWithValue "SOME@EMAIL.COM"

[<Test>]
let MapWillNotTransformErrorValues() =
    Error "my error" 
    |> map toUpper
    |> shouldBeErrorWithValue "my error"

[<Test>]
let MapErrorWillTransformErrorValues() =
    Error "my error" 
    |> mapError toUpper
    |> shouldBeErrorWithValue "MY ERROR"

[<Test>]
let MapErrorWillNotTransformOkValues() =
    Ok "some@email.com" 
    |> mapError toUpper
    |> shouldBeOkWithValue "some@email.com"

//

[<Test>]
let ``mapError if Ok should not modify result`` () =
    Ok 42
    |> mapError (fun _ -> ["err1"])
    |> shouldBeOkWithValue 42

[<Test>]
let ``mapError if Error should map over error`` () =
    Error "error"
    |> mapError (fun _ -> [42])
    |> shouldBeErrorWithValue [42]
