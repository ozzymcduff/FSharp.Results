namespace FSharp.Results

module Result =
  type Result<'T, 'TError> =
    | Ok of 'T
    | Error of 'TError

  let ifNone (error : 'TError) = function
    | Some (o) -> Ok o
    | None -> Error error

  let map f = function
    | Ok v -> f v
    | Error e -> Error e

  let bind m f =
    match m with
    | Ok v -> f v
    | Error e -> Error e

  let retn v = Ok v

  let retnFrom v = v

  let tryResult (body, err) =
    try
      retn <| body()
    with e -> Error err

  type ResultBuilder () =
    member __.Zero () = Error
    member __.Bind (m , f) = bind m f
    member __.Return (v) = retn v
    member __.ReturnFrom (v) = retnFrom v
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

  let result = ResultBuilder()