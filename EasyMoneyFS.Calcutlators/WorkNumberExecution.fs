module WorkNumberExecution

open System

type Frequency = 
    |Annual
    |Monthly
    |SemiMonthly
    |BiWeekly
    |Weekly
    |Hourly
    |SemiAnnually
    |Quarterly
    |TenPerYear
    |ElevenPerYear
    |ThirteenPerYear
    |Daily
    
type Employment = {
    EffectiveDate : DateTime;
    HireDate : DateTime;
}

type Base = {
    RateOfPay : decimal;
    Frequency : Frequency;
    BaseYTD : decimal;
    PreviousBase : decimal
}

type Urbes = {
    Year1SchedALine21 : decimal;
    Year2SchedALine21 : decimal
}

type Bonus = {
    ThisYear : decimal;
    LastYear : decimal;
    TwoYearsAgo : decimal
}

type Commission = {
    CommissionYTD : decimal;
    LastYearCommission : decimal;
}

type Overtime = {
    OvertimeYTD : decimal;
    LastYearOvertime : decimal;
}

type WorkNumberExecution = {
    Employment : Employment;
    Base : Base;
    Urbes : Urbes;
    Bonus : Bonus;
    Commission : Commission;
    Overtime : Overtime; 
}

type WorkNumberResponse = {
    Total : decimal;
    AdjustedBase : decimal;
    Urbes : decimal;
    Bonus : decimal;
    Commission : decimal;
    Overtime : decimal;
}