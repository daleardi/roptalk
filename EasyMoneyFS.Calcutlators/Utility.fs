module Utility

open System
open Dates
open Microsoft.FSharp.Reflection

let avg x y = (x+y)/2M
let round (y:int) (x:decimal) = Math.Round(x,y)
let round2 = round 2

let increasingBy fac yr2 yr1 = 
    yr1 >= yr2*fac

let private increasingRule fac yr2 yr1 = 
    match increasingBy fac yr2 yr1 with
    |true -> avg yr1 yr2
    |false -> yr1

let private decreasingRule fac yr2 yr1 = 
    match increasingBy fac yr2 yr1 with
    |true -> yr1
    |false -> avg yr1 yr2
    
let increasing100 = increasingRule 1M
let increasing90 = increasingRule 0.9M
let decreasing100 = decreasingRule 1M
    
let monthly x = x/12M    

let twoAreZero x y z = 
    match (x,y,z) with 
    |(0M,0M,_) -> true
    |(0M,_,0M) -> true
    |(_,0M,0M) -> true
    |_ -> false

let oneIsZero x y = 
    twoAreZero 0M x y

let noZerosInc90 x y =
    match oneIsZero x y with
        |true -> 0M
        |false -> increasing90 x y      
         
let fromString<'a> (s:string) =
    match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun case -> case.Name = s) with
    |[|case|] -> Some(FSharpValue.MakeUnion(case,[||]) :?> 'a)
    |_ -> None 