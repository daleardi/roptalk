module WorkNumberValidation
open System
open WorkNumberRequest
open Utility 
open Chessie
open Errors

// Validation Functions
let validateNonNegativeDecimal (warning:NegativeError) (x:Decimal) = 
    match x >= 0M with 
    |true  -> ok x
    |false -> warn (Negative(warning)) x

let validateOptionalNonNegativeDecimal nullError negWarning = 
    failIfNone nullError 
    >> bind (validateNonNegativeDecimal negWarning)

let validateDatetime (warning:DefaultDateError) x = 
    match x <> Unchecked.defaultof<DateTime>  with
    |true -> ok x
    |false -> warn (DefaultDate(warning)) x

let validateOptionalDatetime nullError defaultWarning =
    failIfNone nullError 
    >> bind (validateDatetime defaultWarning)

let ParseDU<'T> (error:InvalidDuError) x = 
    match fromString<'T> x with
    |Some y -> ok y
    |None -> fail (Union(error))

let ParseOptionalDU<'T> nullError duError = 
    failIfNone nullError 
    >> bind (ParseDU<'T> duError)


//Validate API Requst
let validateWorkNumberRequest (request:WorkNumberRequest) =
    let toValid input = Valid(input)
    //Employment
    let validateEffectiveDate input = 
        input.EffectiveDate 
        |> validateOptionalDatetime (Null(EffectiveDateIsNull)) EffectiveDateIsDefault
        |> changeToWarn request
    let validateHireDate input = 
        input.HireDate
        |> validateOptionalDatetime (Null(HireDateIsNull)) HireDateIsDefault
        |> changeToWarn request 
    //Base
    let validateRateOfPay input =
        input.BaseRateOfPay
        |> validateOptionalNonNegativeDecimal (Null(BaseRateOfPayIsNull)) RateOfPayIsNegative
        |> changeToWarn request
    let validateBaseFrequency input =
        input.BaseFrequency
        |> ParseOptionalDU (Null(BaseFrequencyIsNull)) BaseFrequencyIsNotValid
        |>changeToWarn request
    let validateBaseYear1 input =
        input.BaseYtd
        |> validateOptionalNonNegativeDecimal (Null(BaseYtdIsNull)) BaseYtdIsNegative
        |> changeToWarn request
    let validateBaseYear2 input =
        input.BaseYear1
        |> validateOptionalNonNegativeDecimal (Null(BaseYear1IsNull)) BaseYear1IsNegative
        |> changeToWarn request
    //Urbes
    let validateUrbesYear1 input =
        input.SchedALine21Year1
        |> validateOptionalNonNegativeDecimal (Null(SchedALine21Year1IsNull)) SchedALine21Year1IsNegative
        |> changeToWarn request
    let validateUrbesYear2 input =
        input.SchedALine21Year2
        |> validateOptionalNonNegativeDecimal (Null(SchedALine21Year2IsNull)) SchedALine21Year2IsNegative
        |> changeToWarn request
    //Bonus
    let validateBonusYear1 input =
        input.BonusYtd
        |> validateOptionalNonNegativeDecimal (Null(BonusYtdIsNull)) BonusYtdIsNegative
        |> changeToWarn request
    let validateBonusYear2 input =
        input.BonusYear1
        |> validateOptionalNonNegativeDecimal (Null(BonusYear1IsNull)) BonusYear1IsNegative
        |> changeToWarn request
    let validateBonusYear3 input =
        input.BonusYear2
        |> validateOptionalNonNegativeDecimal (Null(BonusYear2IsNull)) BonusYear2IsNegative
        |> changeToWarn request
    //Commission
    let validateCommissionYear1 input =
        input.CommissionYtd
        |> validateOptionalNonNegativeDecimal (Null(CommissionYtdIsNull)) CommissionYtdIsNegative
        |> changeToWarn request
    let validateCommissionYear2 input =
        input.CommissionYear1
        |> validateOptionalNonNegativeDecimal (Null(CommissionYear1IsNull)) CommissionYear1IsNegative
        |> changeToWarn request
    //Overtime
    let validateOvertimeYear1 input =
        input.OvertimeYtd
        |> validateOptionalNonNegativeDecimal (Null(OvertimeYtdIsNull)) OvertimeYtdIsNegative
        |> changeToWarn request
    let validateOvertimeYear2 input =
        input.OvertimeYear1
        |> validateOptionalNonNegativeDecimal (Null(OvertimeYear1IsNull)) OvertimeYear1IsNegative
        |> changeToWarn request

    let validateWorkNumber = 
        validateEffectiveDate 
        >> bind validateHireDate 
        >> bind validateRateOfPay 
        >> bind validateBaseFrequency 
        >> bind validateBaseYear1 
        >> bind validateBaseYear2
        >> bind validateUrbesYear1 
        >> bind validateUrbesYear2 
        >> bind validateBonusYear1 
        >> bind validateBonusYear2 
        >> bind validateBonusYear3
        >> bind validateCommissionYear1 
        >> bind validateCommissionYear2 
        >> bind validateOvertimeYear1 
        >> bind validateOvertimeYear2

    validateWorkNumber >> (lift toValid) <| request