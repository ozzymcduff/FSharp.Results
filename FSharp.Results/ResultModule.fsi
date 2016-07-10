namespace FSharp.Results
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Result =

  [<CompiledName("Map")>]
  val map : ('T -> 'U) -> Result<'T, 'TError> -> Result<'U, 'TError>

  [<CompiledName("MapError")>]
  val mapError: ('TError -> 'U) -> Result<'T, 'TError> -> Result<'T, 'U>

  [<CompiledName("Bind")>]
  val bind: ('T -> Result<'U, 'TError>) -> Result<'T, 'TError> -> Result<'U, 'TError>

  [<CompiledName("Attempt")>]
  val attempt : (unit -> 'a) -> Result<'a,exn>

  //E<(a->b)> -> E<a> -> E<b>
  [<CompiledName("Apply")>]
  val apply: (Result<'T->'U, 'TError>) -> Result<'T, 'TError> -> Result<'U, 'TError>

  [<Class>]
  type ResultBuilder =
    member Zero : unit->Result<unit,unit>
    member Bind : (Result<'T,'TError>) * ('T->Result<'U,'TError>) -> Result<'U,'TError>
    member Return : 'T -> Result<'T,_>
    member ReturnFrom : 'T -> 'T
    member Delay : (unit->'T) -> (unit->'T)
    member Run : (unit->'T) -> 'T
    member TryWith : (unit->'T)*(exn->'T)-> 'T
    member TryFinally : (unit->'T)*(unit->unit)-> 'T
    member Using : ('T :> System.IDisposable) * ('T -> Result<'U,'TError>) -> Result<'U,'TError>
    member While : (unit->bool) * (unit->Result<unit,'TError>) -> Result<unit, 'TError>

  val trial : ResultBuilder
