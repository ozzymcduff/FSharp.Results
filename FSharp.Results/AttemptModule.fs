namespace FSharp.Results

open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Attempt =
    type Attempt<'S> = (unit -> Result<'S,exn>)
  
    let private succeed x = (fun () -> Ok x)
    let private failed err = (fun () -> Error err)
    let private runAttempt (a: Attempt<_>) = 
        try
            a ()
        with exn->Error exn

    let private either successTrack failTrack (input : Attempt<_>) : Attempt<_> =
      match runAttempt input with
      | Ok s -> successTrack s
      | Error f -> failTrack f
    let private bind successTrack = either successTrack failed
    let private delay f = (fun () -> f() |> runAttempt)
  
    type AttemptBuilder() =
          member this.Bind(m : Attempt<_>, success) = bind success m
          member this.Bind(m : Result<_, _>, success) = bind success (fun () -> m)
          member this.Bind(m : Result<_, _> option, success) = 
              match m with
              | None -> this.Combine(this.Zero(), success)
              | Some x -> this.Bind(x, success)
          member this.Return(x) : Attempt<_> = succeed x
          member this.ReturnFrom(x : Attempt<_>) = x
          member this.Combine(v, f) : Attempt<_> = bind f v
          member this.Yield(x) = Ok x
          member this.YieldFrom(x) = x
          member this.Delay(f) : Attempt<_> = delay f
          member this.Zero() : Attempt<_> = succeed ()
          member this.While(guard, body: Attempt<_>) =
              if not (guard()) 
              then this.Zero() 
              else this.Bind(body, fun () -> 
                  this.While(guard, body))  
  
          member this.TryWith(body, handler) =
              try this.ReturnFrom(body())
              with e -> handler e
  
          member this.TryFinally(body, compensation) =
              try this.ReturnFrom(body())
              finally compensation() 
  
          member this.Using(disposable:#System.IDisposable, body) =
              let body' = fun () -> body disposable
              this.TryFinally(body', fun () -> 
                  match disposable with 
                      | null -> () 
                      | disp -> disp.Dispose())
  
          member this.For(sequence:seq<'a>, body: 'a -> Attempt<_>) =
              this.Using(sequence.GetEnumerator(),fun enum -> 
                  this.While(enum.MoveNext, 
                      this.Delay(fun () -> body enum.Current)))
  
  
    [<CompiledName("Attempt")>]
    let attempt = AttemptBuilder()

    type Attempt =
        [<CompiledName("Run")>]
        static member run x = runAttempt x
