namespace FSharp.Results
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Result =
  [<Class>]
  type ResultBuilder =
    new : unit -> ResultBuilder
    member Bind : m:Result<'h,'i> * f:('h -> Result<'j,'i>) -> Result<'j,'i>
    member
      Combine : a:Result<'a,'e> * b:Result<'b,'e> -> Result<('a * 'b),'e>
    member Combine : a:Result<unit,'e> * b:Result<'b,'e> -> Result<'b,'e>
    member Combine : a:Result<'a,'e> * b:Result<unit,'e> -> Result<'a,'e>
    member
      Delay : f:(unit -> Result<'T,'TError>) -> (unit -> Result<'T,'TError>)
    [<CustomOperationAttribute ("error")>]
    member Error : value:'TError -> Result<'T,'TError>
    member Return : v:'f -> Result<'f,'g>
    member ReturnFrom : v:Result<'T,'TError> -> Result<'T,'TError>
    member Run : f:(unit -> 'e) -> 'e
    member TryFinally : body:(unit -> 'c) * handler:(unit -> unit) -> 'c
    member
      TryWith : body:(unit -> Result<'T,'d>) *
                handler:(exn -> Result<'T,'d>) -> Result<'T,'d>
    member
      Using : resource:'T * body:('T -> Result<'U,'TError>) ->
                Result<'U,'TError> when 'T :> System.IDisposable
    member Yield : x:'a -> Result<'a,'b>
    member YieldFrom : x:Result<'a,'b> -> Result<'a,'b>
    member Zero : unit -> Result<unit,unit>
  val trial : ResultBuilder
  