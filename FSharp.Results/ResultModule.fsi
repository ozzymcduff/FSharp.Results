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

  [<Class>]
  type AttemptBuilder =
    member Zero : unit->Result<unit,unit>
    member Bind : (Result<'T,'TError>) * ('T->Result<'U,'TError>) -> Result<'U,'TError>
    member Delay : (unit->Result<'T,'TError>) -> (unit->Result<'T,'TError>)
    member Run : (unit->'T) -> Result<'T,exn>

  [<CompiledName("Attempt")>]
  val attempt : AttemptBuilder

  [<Class>]
  type ResultBuilder =
    member Zero : unit->Result<unit,unit>
    // member Bind : M<'a> * ('a -> M<'b>) -> M<'b>
    member Bind : (Result<'T,'TError>) * ('T->Result<'U,'TError>) -> Result<'U,'TError>
    // member Return : 'a -> M<'a>
    member Return : 'T -> Result<'T,_>
    [<CustomOperation("error")>]
    member Error : 'T -> Result<_,'T>

    member ReturnFrom : Result<'T,'TError> -> Result<'T,'TError>
    //member Delay : (unit -> M<'a>) -> (unit -> M<'a>)
    member Delay : (unit->Result<'T,'TError>) -> (unit->Result<'T,'TError>)
    member Run : (unit->'T) -> 'T
    //member TryWith : M<'T> * (exn -> M<'T>) -> M<'T>
    member TryWith : (unit->Result<'T,'TError>)*(exn->Result<'T,'TError>)-> Result<'T,'TError>
    //member TryFinally : M<'a> -> M<'a> -> M<'a>
    member TryFinally : (unit->'T)*(unit->unit)-> 'T
    //member Using: 'a * ('a -> M<'b>) -> M<'b> when 'a :> IDisposable
    member Using : ('T :> System.IDisposable) * ('T -> Result<'U,'TError>) -> Result<'U,'TError>

  val trial : ResultBuilder
