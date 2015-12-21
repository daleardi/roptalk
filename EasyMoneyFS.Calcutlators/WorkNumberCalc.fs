module WorkNumberCalc 

open WorkNumberExecution
open Dates
open Utility 

let monthlyFactor = function
    |Annual -> 1M
    |Monthly -> 12M
    |SemiMonthly -> 24M 
    |BiWeekly -> 26M
    |Weekly -> 52M    
    |Hourly -> 52M //this times avg hours
    |SemiAnnually -> 2M
    |Quarterly -> 4M
    |TenPerYear -> 10M
    |ElevenPerYear -> 11M
    |ThirteenPerYear -> 13M
    |Daily -> 52M //this times avg days  

let avgHoursPerWeekA = 40M
let avgDaysPerWeekA = 5M

let methodB {BaseYTD = ytd; RateOfPay = pay} {EffectiveDate = effDate;} =
    (ytd/WeeksYTD effDate)/pay

let methodC {PreviousBase = prevBase; RateOfPay = pay} {EffectiveDate = effDate; HireDate = hireDate;}= 
    let weeksLastYear = WeeksBetweenFirstOfYear effDate hireDate |> min 52M
    prevBase/weeksLastYear/pay

let pickAverage avgA wnBase emp =
    let {EffectiveDate = effDate} = emp
    match dateIsAfterMayFirst effDate with
    | true -> [avgA; methodB wnBase emp; methodC wnBase emp] |> List.min
    | false -> [avgA; methodB wnBase emp] |> List.min
       
let avgHoursPerWeek = pickAverage avgHoursPerWeekA
let avgDaysPerWeek = pickAverage avgDaysPerWeekA

let monthlyBase wnBase emp = 
    let {Frequency = freq; RateOfPay = pay} = wnBase
    match freq with 
    |Hourly -> (pay * monthlyFactor freq * avgHoursPerWeek wnBase emp) |> monthly
    |Daily -> (pay *  monthlyFactor freq * avgDaysPerWeek wnBase emp) |> monthly
    |_ -> pay * monthlyFactor freq |> monthly
    
let monthlyURBEs {Year1SchedALine21 = yr1; Year2SchedALine21 = yr2} =     
        decreasing100 yr2 yr1 |> monthly

let monthlyBonus {ThisYear = yr1; LastYear = yr2; TwoYearsAgo = yr3} = 
        match twoAreZero yr1 yr2 yr3 with 
        | true -> 0M
        | false -> match yr1 <> 0M with
                    |true  -> increasing90 yr2 yr1 |> monthly
                    |false -> avg yr2 yr3 |> monthly

let monthlyCommission {CommissionYTD = ytd; LastYearCommission = prev} {EffectiveDate = effDate;}= 
        (ytd / WeeksYTD effDate) * 52M |> noZerosInc90 prev |> monthly

let monthlyOvertime {OvertimeYTD =ytd; LastYearOvertime = prev} {EffectiveDate = effDate;} =
        (ytd / WeeksYTD effDate) * 52M |> noZerosInc90 prev |> monthly


let qualifiedIncome {Employment = emp; Base = ba; Urbes = u; Bonus = bo; Commission = co; Overtime = ot} = 
    let tempBase = monthlyBase ba emp |> round2
    let urbes = monthlyURBEs u |> round2
    let bonus = monthlyBonus bo |> round2
    let commission = monthlyCommission co emp |> round2
    let overtime = monthlyOvertime ot emp |> round2
    let adjBase = tempBase-urbes |> round2
    let total = adjBase+bonus+commission+overtime |> round2
    {Total = total; AdjustedBase = tempBase-urbes; Urbes = urbes; Bonus = bonus; Commission = commission; Overtime = overtime}