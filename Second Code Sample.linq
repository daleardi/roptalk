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
        | Failure f -> Failure f
		
// create an infix operator
let (>>=) twoTrackInput switchFunction = 
    bind switchFunction twoTrackInput 

// create a composition operator
let (>=>) switch1 switch2 x = 
    match switch1 x with
    | Success s -> switch2 s
    | Failure f -> Failure f 

// convert a one-track function into a switch
let lift f x= 
    f x |> succeed

// execute dead end function and pass along input
let tee f x = 
    f x |> ignore
    x

// call function and fail if caught
let tryCatch f x =
    try
        f x |> succeed
    with
    | ex -> fail ex.Message
		
///////////////////////////////////////////////////
// Model
///////////////////////////////////////////////////
type Request = {name:string; email:string}

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
   if input.name = "" then fail "Name must not be blank"
   else succeed input

let validate2 input =
   if input.name.Length > 50 then fail "Name must not be longer than 50 chars"
   else succeed input

let validate3 input =
   if input.email = "" then fail "Email must not be blank"
   else succeed input

let combinedValidation = 
    // connect the two-tracks together
    validate1 
    >=> validate2 
    >=> validate3
  
///////////////////////////////////////////////////
// Tests
///////////////////////////////////////////////////
let input1 = {name="Dominick"; email="Dominick@FSharpRocks.com"}
input1
|> combinedValidation  
>>= lift lowerEmail
|> printfn "Result1 = %A"

let input2 = {name=""; email="Dominick@FSharpRocks.com"}
input2
|> combinedValidation  
>>= lift lowerEmail
|> printfn "Result2 = %A"

let input3 = {name="Dominick"; email="Dominick@FSharpRocks.com"}
input3
|> combinedValidation  
>>= lift lowerEmail
>>= tryCatch (tee updateDatabase)
|> printfn "Result3 = %A"

let input4 = {name="Paul Blasucci"; email="Dominick@FSharpRocks.com"}
input4
|> combinedValidation  
>>= lift lowerEmail
>>= tryCatch (tee updateDatabase)
|> printfn "Result4 = %A"


