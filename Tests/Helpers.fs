module Helpers

open NUnit.Framework
open FSharp.Results
open Result
open FsUnit

let shouldBeOk maybeOk= match maybeOk with | Error e-> failwith "error!" | Ok v->v

let shouldBeError maybeError= match maybeError with | Error e-> e | Ok v-> failwith "ok!"

let returnOrFail = shouldBeOk
