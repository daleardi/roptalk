<Query Kind="FSharpProgram" />

///////////////////////////////////////////////////
// ROP
///////////////////////////////////////////////////

// the two-track type
type Result<'TSuccess,'TFailure> = 
    | Success of 'TSuccess
    | Failure of 'TFailure

// convert a single value into a two-track result
let succeed x = 
    Success x

let fail x = 
    Failure x

// convert a switch function into a two-track function
let bind switchFunction = 
    fun twoTrackInput -> 
        match twoTrackInput with
        | Success s -> switchFunction s
        | Failure f -> fail f

// create an infix operator
let (>>=) twoTrackInput switchFunction = 
    bind switchFunction twoTrackInput 

// create a composition operator
let (>=>) switch1 switch2 x = 
    match switch1 x with
    | Success s -> switch2 s
    | Failure f -> fail f 

// convert a one-track function into a switch
let lift f x= 
    f x |> succeed

// execute dead end function and pass along input
let tee f x = 
    f x |> ignore
    x

// call function and fail if caught
let tryCatch exType f x  =
    try
        f x |> succeed
    with
    | ex -> fail [exType ex.Message]

// combine multiple results together
let combine addSuccess addFailure switch1 switch2 x = 
    match (switch1 x),(switch2 x) with
    | Success s1,Success s2 -> succeed (addSuccess s1 s2)
    | Failure f1,Success _  -> fail f1
    | Success _ ,Failure f2 -> fail f2
    | Failure f1,Failure f2 -> fail (addFailure f1 f2)

// create a "combine" function for validation functions
let (&&&) v1 v2 = 
    let addSuccess r1 r2 = r1 // return first
    let addFailure s1 s2 = s1@s2 // concat
    combine addSuccess addFailure v1 v2 


///////////////////////////////////////////////////
// Model
///////////////////////////////////////////////////
type Request = {name:string; email:string}

type Error = 
    |BlankName of string
    |LongName of string
    |BlankEmail of string
    |DbError of string

///////////////////////////////////////////////////
// Other Functions
///////////////////////////////////////////////////
let lowerEmail input =
   { input with email = input.email.Trim().ToLower() }

let updateDatabase input =
    match input.name with
    |"Paul Blasucci"-> failwith "Paul Blasucci is not cool enough for this database"
    |_ -> ()
  
///////////////////////////////////////////////////
// Validation
///////////////////////////////////////////////////
let validate1 input =
   if input.name = "" then fail [BlankName "Name must not be blank"]
   else succeed input

let validate2 input =
   if input.name.Length > 50 then fail [LongName "Name must not be longer than 50 chars"]
   else succeed input

let validate3 input =
   if input.email = "" then fail [BlankEmail "Email must not be blank"]
   else succeed input

let combinedValidation = 
    // connect the two-tracks together
    validate1 
    &&& validate2 
    &&& validate3

///////////////////////////////////////////////////
// Tests
///////////////////////////////////////////////////
let input1 = {name=""; email=""}
input1
|> combinedValidation  
>>= lift lowerEmail
|> printfn "Result1 = %A"

let input2 = {name="Paul Blasucci"; email=""}
input2
|> combinedValidation  
>>= lift lowerEmail
|> printfn "Result2 = %A"

let usecase = 
    combinedValidation
    &&& tryCatch DbError (tee updateDatabase) 

let input3 = {name="Paul Blasucci"; email=""}
input3
|> usecase
|> printfn "Result3 = %A"

let input4 = {name="Dominick"; email="Hello@sample.com"}
input4
|> usecase
|> printfn "Result4 = %A"

