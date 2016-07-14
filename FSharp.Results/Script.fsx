
#load "Result.fs"
#load "ResultModule.fs"
open FSharp.Results
type ResultException (v: obj)=
  inherit System.Exception()


type AttemptzBuilder<'T>() =
    member __.Zero () : Result<unit,unit> = printfn "Zero"; Ok()
    member __.Bind (m , f) = printfn "Bind %A %A" m f; Result.bind f m
    member __.Delay (f : unit->Result<'T, 'TError>) = printfn "Delay %A" f; f
    member __.Run (f:unit->Result<'T,'TError>) : Result<'T,exn>=
        printfn "Run %A" f
        try
            match f() with
            | Ok v->Ok v
            | Error e->
                failwith "Unexpected error"
        with ex ->
            Error ex

    member __.Run (f:unit->Result<'T,exn>) : Result<'T,exn>=
        printfn "Run %A" f
        try
            match f() with
            | Ok v->Ok v
            | Error ex-> Error ex
        with ex ->
            Error ex

    member __.Run (f:unit->'T) =
        printfn "Run %A" f
        try
            Ok( f())
        with ex ->
            Error ex
    member __.Return (v:Result<'T,'TError>) =
        printfn "Return %A" v
        v
    member __.Return (v: 'T) =
        printfn "Return %A" v
        Ok v

let attemptz<'a> = AttemptzBuilder<'a>()

let failing () :int = failwith "bla"

attemptz<int>{
   return failing()
}
attemptz<int>{
   return 1
}
// Delay
// Run

//printfn "%A" v
