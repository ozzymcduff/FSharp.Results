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
      // why this patter? In order not to throw away information.
      // having the same error type for f and x is also to restrictive
      | Error e1, Error e2 -> Error (Some e1, Some e2)
      | _       , Error e2 -> Error (None,Some e2)
      | Error e1, _        -> Error (Some e1, None)

  type AttemptBuilder () =
    member __.Zero () : Result<unit,unit> = Ok()
    member __.Bind (m , f) = bind f m
    member __.Delay (f : unit->Result<'T, 'TError>) = f
    member __.Run (f) = 
        try
            Ok( f())
        with ex ->
            Error ex

  [<CompiledName("Attempt")>]
  let attempt = AttemptBuilder()

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
    member __.ReturnFrom (v) : Result<'T, 'TError> = v
    member __.Delay (f : unit->Result<'T, 'TError>) = f
    member __.Run (f) = f()
    member __.TryWith (body:(unit->Result<'T,_>), handler) =
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


  let trial = ResultBuilder()
