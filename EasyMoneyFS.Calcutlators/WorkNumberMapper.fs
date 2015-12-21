module WorkNumberMapper
open System
open WorkNumberExecution
open WorkNumberRequest
open Utility 
open Chessie
open Errors


let private map request = 
    let emp:Employment = {
        EffectiveDate = request.EffectiveDate.Value;
        HireDate = request.HireDate.Value
    }
    let frequency = fromString<Frequency> request.BaseFrequency.Value
    let wnBase = {
        RateOfPay = request.BaseRateOfPay.Value;
        Frequency = frequency.Value;
        BaseYTD = request.BaseYtd.Value;
        PreviousBase = request.BaseYear1.Value;
    }
    let urbes = {
        Year1SchedALine21 = request.SchedALine21Year1.Value;
        Year2SchedALine21 = request.SchedALine21Year2.Value;
    }
    let bonus = {
        ThisYear = request.BonusYtd.Value;
        LastYear = request.BonusYear1.Value;
        TwoYearsAgo = request.BonusYear2.Value
    }
    let comm = {
        CommissionYTD = request.CommissionYtd.Value;
        LastYearCommission = request.CommissionYear1.Value;
    }
    let ot = {
        OvertimeYTD = request.OvertimeYtd.Value;
        LastYearOvertime = request.OvertimeYear1.Value;
    }
    {Employment = emp;
    Base = wnBase;
    Urbes = urbes;
    Bonus = bonus;
    Commission = comm;
    Overtime = ot; 
    }

let toWorkNumberExecution x = 
    match x with
    |Valid y -> (map y)
    |InValid -> failwith "Inproper Validation"