module Helpers

open NUnit.Framework
open FSharp.Results

let shouldBeOk expected maybeOk = match maybeOk with | Error e-> failwith "error!" | Ok v->Assert.AreEqual(expected, v)

let shouldBeError expected maybeError = match maybeError with | Error e-> Assert.AreEqual(expected, e) | Ok v-> failwith "ok!"

//let returnOrFail = shouldBeOk
