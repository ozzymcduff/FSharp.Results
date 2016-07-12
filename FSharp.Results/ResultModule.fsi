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
  val apply: (Result<'T->'U, 'TFuncError>) -> Result<'T, 'TValueError> -> Result<'U, 'TFuncError option *'TValueError option>

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
    //member TryFinally : M<'T> * (unit -> unit) -> M<'T>   
    member TryFinally : (unit->Result<'T,'TError>)*(unit->unit)-> Result<'T,'TError>
    //member Using: 'a * ('a -> M<'b>) -> M<'b> when 'a :> IDisposable
    member Using : ('T :> System.IDisposable) * ('T -> Result<'U,'TError>) -> Result<'U,'TError>
    //
    member Yield :  'T -> Result<'T,'TError>
    member YieldFrom : Result<'T,'TError> -> Result<'T,'TError>
    member Combine : (Result<'T,'TError>)* (Result<'T,'TError>) -> Result<('T list),('TError list)>

    member Combine : (Result<'T,'TError>)* (Result<'T list,'TError list>) -> Result<('T list),('TError list)>
    member Combine : (Result<'T list,'TError list>)* (Result<'T,'TError>) -> Result<('T list),('TError list)>

    member Combine : (Result<'T list,'TError list>)* (Result<'T list,'TError list>) -> Result<('T list),('TError list)>

  val trial : ResultBuilder
