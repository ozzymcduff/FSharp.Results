namespace FSharp.Results
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Attempt =
    type Attempt<'S> = {fn: unit -> Result<'S,exn>;}

    [<Class>]
    type AttemptBuilder =
        new : unit -> AttemptBuilder
        member
          Bind : m:Attempt<'n> * success:('n -> Attempt<'o>) ->
                   Attempt<'o>
        member
          Bind : m:Result<'l,exn> * success:('l -> Attempt<'m>) ->
                   Attempt<'m>
        member
          Bind : m:Result<unit,exn> option *
                 success:(unit -> Attempt<'e>) -> Attempt<'e>
        member
          Combine : v:Attempt<unit> * f:(unit -> Attempt<'e>) ->
                      Attempt<'e>
        member Delay : f:(unit -> Attempt<'f>) -> Attempt<'f>
        member
          For : sequence:seq<'a> * body:('a -> Attempt<unit>) ->
                  Attempt<unit>
        member Return : x:'k -> Attempt<'k>
        member ReturnFrom : x:Attempt<'j> -> Attempt<'j>
        member
          TryFinally : body:(unit -> Attempt<'c>) *
                       compensation:(unit -> unit) -> Attempt<'c>
        member
          TryWith : body:(unit -> Attempt<'d>) *
                    handler:(exn -> Attempt<'d>) -> Attempt<'d>
        member
          Using : disposable:'a * body:('a -> Attempt<'b>) ->
                    Attempt<'b>
                    when 'a :> System.IDisposable and 'a : null
        member
          While : guard:(unit -> bool) * body:Attempt<unit> ->
                    Attempt<unit>
        member Yield : x:'h -> Result<'h,'i>
        member YieldFrom : x:'g -> 'g
        member Zero : unit -> Attempt<unit>

    [<CompiledNameAttribute ("Attempt")>]
    val attempt : AttemptBuilder
    type Attempt =
      class
        [<CompiledNameAttribute ("Run")>]
        static member run : x:Attempt<'a> -> Result<'a,exn>
      end
