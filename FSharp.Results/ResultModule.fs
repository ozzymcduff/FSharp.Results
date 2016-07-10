namespace FSharp.Results

open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Result =

  [<CompiledName("Map")>]
  let map f inp = match inp with Error e -> Error e | Ok x -> Ok (f x)

  [<CompiledName("MapError")>]
  let mapError f inp = match inp with Error e -> Error (f e) | Ok x -> Ok x

  [<CompiledName("Bind")>]
  let bind f inp = match inp with Error e -> Error e | Ok x -> f x

  [<CompiledName("Apply")>]
  let apply f x =
      match f,x with
      | Ok f, Ok x -> Ok (f x)
      | Error e, _            -> Error e
      | _           , Error e -> Error e

  [<CompiledName("Attempt")>]
  let attempt f =
      try
        Ok (f())
      with e -> Error e

  type ResultBuilder () =
    // in https://github.com/jack-pappas/ExtCore/blob/master/ExtCore/Control.fs#L872 a Ok result is used
    // see also: https://fsharpforfunandprofit.com/posts/computation-expressions-builder-part1/
    member __.Zero () : Result<unit,unit> = Ok()
    member __.Bind (m , f) = bind f m
    member __.Return (v) = Ok v
    // see https://github.com/jack-pappas/ExtCore/blob/master/ExtCore/Control.fs#L861
    /// Error operation. Similar to the Return method ('return'), but used for returning an error value.
    [<CustomOperation("error")>]
    member __.Error value : Result<'T, 'TError> = Error value
    member __.ReturnFrom (v) = v
    member __.Delay (f) = fun () -> f ()
    member __.Run (f) = f()
    member __.TryWith (body, handler) =
      try
        body ()
      with ex -> handler ex

    member __.TryFinally (body, handler) =
      try
        body()
      finally
        handler ()

    // https://github.com/jack-pappas/ExtCore/blob/master/ExtCore/Control.fs#L917
    member __.Using (resource : ('T :> System.IDisposable), body : _ -> Result<_,_>)
        : Result<'U, 'TError> =
        try body resource
        finally
            if not <| isNull (box resource) then
                resource.Dispose ()

    // https://github.com/jack-pappas/ExtCore/blob/master/ExtCore/Control.fs#L925
    // (unit -> bool) * M<'T> -> M<'T>
    member this.While (guard, body : unit -> Result<unit, 'TError>) : Result<_,_> =
        if guard () then
            match body () with
            | Ok () ->
                this.While (guard, body)
            | err -> err
        else
            // Return Ok () to indicate success when the loop
            // finishes normally (because the guard returned false).
            Ok ()

    // https://github.com/jack-pappas/ExtCore/blob/master/ExtCore/Control.fs#L936
    // seq<'T> * ('T -> M<'U>) -> M<'U>
    // or
    // seq<'T> * ('T -> M<'U>) -> seq<M<'U>>
    member __.For (sequence : seq<_>, body : 'T -> Result<unit, 'TError>) =
        use enumerator = sequence.GetEnumerator ()

        let mutable errorResult = None
        while enumerator.MoveNext () && Option.isNone errorResult do
            match body enumerator.Current with
            | Ok () -> ()
            | error ->
                errorResult <- Some error

        // If we broke out of the loop early because the 'body' function
        // returned an error for some element, return the error.
        // Otherwise, return the 'zero' value (representing a 'success' which carries no value).
        match errorResult with
        | Some errorResult -> errorResult
        | None -> Ok()

  let trial = ResultBuilder()
