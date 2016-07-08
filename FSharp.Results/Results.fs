namespace FSharp.Results

module Results =

  let ifNone (error : 'TError) = function
    | Some (o) -> Ok o
    | None -> Error error

  let map f = function
    | Ok x -> f x
    | Error e -> Error e

  let bind f m=
    match m with
    | Ok x -> f x
    | Error e -> Error e

  let tryResult (body, err) =
    try
      Ok <| body()
    with e -> Error err

  type ResultBuilder () =
    member __.Zero () = Error
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