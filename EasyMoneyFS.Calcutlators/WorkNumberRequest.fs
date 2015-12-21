module WorkNumberRequest

open System

[<CLIMutable>]
type WorkNumberRequest = {
    // Employment
    EffectiveDate : Option<DateTime>;
    HireDate : Option<DateTime>;
    // Base
    BaseRateOfPay : Option<decimal>;
    BaseFrequency : Option<string>; 
    BaseYtd : Option<decimal>;
    BaseYear1 : Option<decimal>
    // Urbes
    SchedALine21Year1 : Option<decimal>;
    SchedALine21Year2 : Option<decimal>
    // Bonus
    BonusYtd : Option<decimal>;
    BonusYear1 : Option<decimal>;
    BonusYear2 : Option<decimal>
    // Commission
    CommissionYtd : Option<decimal>;
    CommissionYear1 : Option<decimal>;
    // Overtime
    OvertimeYtd : Option<decimal>;
    OvertimeYear1 : Option<decimal>;
}

type ValidatedWorkNumberRequest = 
    |Valid of WorkNumberRequest
    |InValid