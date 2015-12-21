﻿module Chessie
//Code copied from github repository fsprojects/Chessie
open System

/// Represents the result of a computation.
type Result<'TSuccess, 'TMessage> = 
    /// Represents the result of a successful computation.
    | Ok of 'TSuccess * 'TMessage list
    /// Represents the result of a failed computation.
    | Bad of 'TMessage list

    /// Creates a Failure result with the given messages.
    static member FailWith(messages:'TMessage seq) : Result<'TSuccess, 'TMessage> = Result<'TSuccess, 'TMessage>.Bad(messages |> Seq.toList)

    /// Creates a Failure result with the given message.
    static member FailWith(message:'TMessage) : Result<'TSuccess, 'TMessage> = Result<'TSuccess, 'TMessage>.Bad([message])
    
    /// Creates a Success result with the given value.
    static member Succeed(value:'TSuccess) : Result<'TSuccess, 'TMessage> = Result<'TSuccess, 'TMessage>.Ok(value,[])

    /// Creates a Success result with the given value and the given message.
    static member Succeed(value:'TSuccess,message:'TMessage) : Result<'TSuccess, 'TMessage> = Result<'TSuccess, 'TMessage>.Ok(value,[message])

    /// Creates a Success result with the given value and the given message.
    static member Succeed(value:'TSuccess,messages:'TMessage seq) : Result<'TSuccess, 'TMessage> = Result<'TSuccess, 'TMessage>.Ok(value,messages |> Seq.toList)

    /// Executes the given function on a given success or captures the failure
    static member Try(func: Func<_>) : Result<'TSuccess,exn> =
        try
            Ok(func.Invoke(),[])
        with
        | exn -> Bad[exn]

    /// Converts the result into a string.
    override this.ToString() =
        match this with
        | Ok(v,msgs) -> sprintf "OK: %A - %s" v (String.Join(Environment.NewLine, msgs |> Seq.map (fun x -> x.ToString())))
        | Bad(msgs) -> sprintf "Error: %s" (String.Join(Environment.NewLine, msgs |> Seq.map (fun x -> x.ToString())))    

/// Basic combinators and operators for error handling.
[<AutoOpen>]
module Trial =
    /// Wraps a value in a Success
    let inline ok<'TSuccess,'TMessage> (x:'TSuccess) : Result<'TSuccess,'TMessage> = Ok(x, [])

    /// Wraps a value in a Success
    let inline pass<'TSuccess,'TMessage> (x:'TSuccess) : Result<'TSuccess,'TMessage> = Ok(x, [])

    /// Wraps a value in a Success and adds a message
    let inline warn<'TSuccess,'TMessage> (msg:'TMessage) (x:'TSuccess) : Result<'TSuccess,'TMessage> = Ok(x,[msg])

    /// Wraps a message in a Failure
    let inline fail<'TSuccess,'Message> (msg:'Message) : Result<'TSuccess,'Message> = Bad([ msg ])

    /// Returns true if the result was not successful.
    let inline failed result = 
        match result with
        | Bad _ -> true
        | _ -> false
    
    /// Converts an option into a Result.
    let inline failIfNone message result = 
        match result with
        | Some x -> ok x
        | None -> fail message
    ///Changes the value of the success
    let inline changeValue input result =
      match result with
      | Ok(_, msgs) -> Ok(input,msgs)
      | Bad(msgs) ->Bad(msgs)

    let inline changeToWarn input result =
        match result with
        | Ok(_, msgs) -> Ok(input,msgs)
        | Bad(msgs) -> Ok(input, msgs)
    
    /// Takes a Result and maps i with fSuccess if it is a Success otherwise it maps it with fFailure.
    let inline either fSuccess fFailure trialResult = 
        match trialResult with
        | Ok(x, msgs) -> fSuccess (x, msgs)
        | Bad(msgs) -> fFailure (msgs)

    /// If the given result is a Success the wrapped value will be returned. 
    ///Otherwise the function throws an exception with Failure message of the result.
    let inline returnOrFail result = 
        let inline raiseExn msgs = 
            msgs
            |> Seq.map (sprintf "%O")
            |> String.concat (Environment.NewLine + "\t")
            |> failwith
        either fst raiseExn result

    /// Appends the given messages with the messages in the given result.
    let inline mergeMessages msgs result = 
        let inline fSuccess (x, msgs2) = Ok(x, msgs @ msgs2)
        let inline fFailure errs = Bad(errs @ msgs)
        either fSuccess fFailure result

    /// If the result is a Success it executes the given function on the value.
    /// Otherwise the exisiting failure is propagated.
    let inline bind f result = 
        let inline fSuccess (x, msgs) = f x |> mergeMessages msgs
        let inline fFailure (msgs) = Bad msgs
        either fSuccess fFailure result

   /// Flattens a nested result given the Failure types are equal
    let inline flatten (result : Result<Result<_,_>,_>) =
        result |> bind (fun x -> x)

    /// If the result is a Success it executes the given function on the value. 
    /// Otherwise the exisiting failure is propagated.
    /// This is the infix operator version of ErrorHandling.bind
    let inline (>>=) result f = bind f result

    /// If the wrapped function is a success and the given result is a success the function is applied on the value. 
    /// Otherwise the exisiting error messages are propagated.
    let inline apply wrappedFunction result = 
        match wrappedFunction, result with
        | Ok(f, msgs1), Ok(x, msgs2) -> Ok(f x, msgs1 @ msgs2)
        | Bad errs, Ok(_, msgs) -> Bad(errs)
        | Ok(_, msgs), Bad errs -> Bad(errs)
        | Bad errs1, Bad errs2 -> Bad(errs1 @ errs2)

    /// If the wrapped function is a success and the given result is a success the function is applied on the value. 
    /// Otherwise the exisiting error messages are propagated.
    /// This is the infix operator version of ErrorHandling.apply
    let inline (<*>) wrappedFunction result = apply wrappedFunction result

    /// Lifts a function into a Result container and applies it on the given result.
    let inline lift f result = apply (ok f) result

    /// Lifts a function into a Result and applies it on the given result.
    /// This is the infix operator version of ErrorHandling.lift
    let inline (<!>) f result = lift f result

    /// Promote a function to a monad/applicative, scanning the monadic/applicative arguments from left to right.
    let inline lift2 f a b = f <!> a <*> b

    /// If the result is a Success it executes the given success function on the value and the messages.
    /// If the result is a Failure it executes the given failure function on the messages.
    /// Result is propagated unchanged.
    let inline eitherTee fSuccess fFailure result =
        let inline tee f x = f x; x;
        tee (either fSuccess fFailure) result

    /// If the result is a Success it executes the given function on the value and the messages.
    /// Result is propagated unchanged.
    let inline successTee f result = 
        eitherTee f ignore result

    /// If the result is a Failure it executes the given function on the messages.
    /// Result is propagated unchanged.
    let inline failureTee f result = 
        eitherTee ignore f result

    /// Collects a sequence of Results and accumulates their values.
    /// If the sequence contains an error the error will be propagated.
    let inline collect xs = 
        Seq.fold (fun result next -> 
            match result, next with
            | Ok(rs, m1), Ok(r, m2) -> Ok(r :: rs, m1 @ m2)
            | Ok(_, m1), Bad(m2) | Bad(m1), Ok(_, m2) -> Bad(m1 @ m2)
            | Bad(m1), Bad(m2) -> Bad(m1 @ m2)) (ok []) xs
        |> lift List.rev
        
    ///combine the results
    let inline combine xs = 
        List.fold (fun result next -> 
            match result, next with
            | Ok(_, m1), Ok(_, m2) -> Ok([], m1 @ m2)
            | Ok(_, m1), Bad(m2) | Bad(m1), Ok(_, m2) -> Bad(m1 @ m2)
            | Bad(m1), Bad(m2) -> Bad(m1 @ m2)) (ok []) xs
        |> lift List.rev

    let inline combineAndChange fs input = 
        fs |> List.map(fun f -> f input) |> combine |> changeValue input

    /// Categorizes a result based on its state and the presence of extra messages
    let inline (|Pass|Warn|Fail|) result =
      match result with
      | Ok  (value, []  ) -> Pass  value
      | Ok  (value, msgs) -> Warn (value,msgs)
      | Bad         msgs  -> Fail        msgs

    let inline failOnWarnings result =
      match result with
      | Warn (_,msgs) -> Bad msgs
      | _             -> result 

    /// Builder type for error handling computation expressions.
    type TrialBuilder() = 
        member __.Zero() = ok()
        member __.Bind(m, f) = bind f m
        member __.Return(x) = ok x
        member __.ReturnFrom(x) = x
        member __.Combine (a, b) = bind b a
        member __.Delay f = f
        member __.Run f = f ()
        member __.TryWith (body, handler) =
            try
                body()
            with
            | e -> handler e
        member __.TryFinally (body, compensation) =
            try
                body()
            finally
                compensation()
        member x.Using(d:#IDisposable, body) =
            let result = fun () -> body d
            x.TryFinally (result, fun () ->
                match d with
                | null -> ()
                | d -> d.Dispose())
        member x.While (guard, body) =
            if not <| guard () then
                x.Zero()
            else
                bind (fun () -> x.While(guard, body)) (body())
        member x.For(s:seq<_>, body) =
            x.Using(s.GetEnumerator(), fun enum ->
                x.While(enum.MoveNext,
                    x.Delay(fun () -> body enum.Current)))

    /// Wraps computations in an error handling computation expression.
    let trial = TrialBuilder()