namespace FSharp.Results

type Result<'T, 'TError> =
    | Ok of 'T
    | Error of 'TError
