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
  let apply fOpt xOpt = 
    match fOpt,xOpt with
    | Ok f, Ok x -> Ok (f x)
    | Error e, _-> Error e
    | _, Error e -> Error e

  type ResultBuilder () =
    member __.Zero () = Error() // is this correct?
    member __.Bind (m , f) = bind f m
    member __.Return (v) = Ok v
    member __.ReturnFrom (v) = v
    member __.Delay (f) = fun () -> f ()
    member __.Run (f) = f()
    member __.TryWith (body, handler) =
      try
        __.Run body
      with e -> handler e

    member __.TryFinally (body, handler) =
      try
        __.Run body
      finally
        handler ()

  let attempt = ResultBuilder()
