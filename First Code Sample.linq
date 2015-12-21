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
		
///////////////////////////////////////////////////
// Model
///////////////////////////////////////////////////
type Request = {name:string; email:string}

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
    >> bind validate2 
    >> bind validate3
	
///////////////////////////////////////////////////
// Tests
///////////////////////////////////////////////////
let input1 = {name=""; email=""}
combinedValidation input1 
|> printfn "Result1 = %A"

let input2 = {name="Alice"; email=""}
combinedValidation input2
|> printfn "Result2 = %A"

let input3 = {name="Wolfeschlegelsteinhausenbergerdorffvoralternwarengewissenhaftschaferswesenchafewarenwholgepflegeundsorgfaltigkeitbeschutzenvonangereifenduchihrraubgiriigfeindewelchevorralternzwolftausendjahresvorandieerscheinenbanderersteerdeemmeshedrraumschiffgebrauchlichtalsseinursprungvonkraftgestartseinlangefahrthinzwischensternartigraumaufdersuchenachdiesternwelshegehabtbewohnbarplanetenkreisedrehensichundwohinderneurassevanverstandigmenshlichkeittkonntevortpflanzenundsicherfreunanlebenslamdlichfreudeundruhemitnichteinfurchtvorangreifenvonandererintlligentgeschopfsvonhinzwischensternartigraum"; email="hello@sample.com"}
combinedValidation input3
|> printfn "Result3 = %A"

let input4 = {name="Alice"; email="good"}
combinedValidation input4
|> printfn "Resul4 = %A"
