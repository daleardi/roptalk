module Dates 

open System
      
let FirstOfAYear (date:DateTime) = 
    new DateTime(date.Year,1,1)
      
let WeeksInBetween (startDate:DateTime) (endDate:DateTime) = 
    let span = endDate - startDate
    decimal span.Days / 7M |> abs

let WeeksBetweenFirstOfYear firstOfYearDate date = 
    firstOfYearDate |> FirstOfAYear |> WeeksInBetween date      

let WeeksYTD effDate = WeeksBetweenFirstOfYear effDate effDate

let dateIsAfterMayFirst (date:DateTime) = date.Day >= 1 && date.Month >= 5