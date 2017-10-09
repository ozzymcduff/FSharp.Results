module Helpers

open Xunit
open FSharp.Results

let shouldBeOkWithValue (expected:'a) (maybeOk:Result<'a,'e>) =
   match maybeOk with 
   | Error e-> failwith "error!" 
   | Ok v-> Assert.Equal<'a>(expected, v)

let shouldBeErrorWithValue (expected:'e) (maybeError:Result<'a,'e>) =
   match maybeError with 
   | Error e-> Assert.Equal<'e>(expected, e) 
   | Ok v-> failwith "ok!"

//let returnOrFail = shouldBeOk
