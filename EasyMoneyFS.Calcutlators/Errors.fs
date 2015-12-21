module Errors

type NullError = 
    //Employment
    |EffectiveDateIsNull
    |HireDateIsNull
    //Base
    |BaseRateOfPayIsNull
    |BaseFrequencyIsNull
    |BaseYtdIsNull
    |BaseYear1IsNull
    //Urbes
    |SchedALine21Year1IsNull
    |SchedALine21Year2IsNull
    //Bonus
    |BonusYtdIsNull
    |BonusYear1IsNull
    |BonusYear2IsNull
    //Commission
    |CommissionYtdIsNull
    |CommissionYear1IsNull
    ///Overtime
    |OvertimeYtdIsNull
    |OvertimeYear1IsNull

type InvalidDuError = 
    //Base
    |BaseFrequencyIsNotValid

type NegativeError = 
    //Base
    |RateOfPayIsNegative
    |BaseYtdIsNegative
    |BaseYear1IsNegative
    //Urbes
    |SchedALine21Year1IsNegative
    |SchedALine21Year2IsNegative
    //Bonus
    |BonusYtdIsNegative
    |BonusYear1IsNegative
    |BonusYear2IsNegative
    //Commission
    |CommissionYtdIsNegative
    |CommissionYear1IsNegative
    //Overtime
    |OvertimeYtdIsNegative
    |OvertimeYear1IsNegative
type DefaultDateError = 
    //Employment
    |EffectiveDateIsDefault
    |HireDateIsDefault

type WorkNumberError = 
    |Null of error:NullError
    |Union of error:InvalidDuError
    |Negative of error:NegativeError
    |DefaultDate of error:DefaultDateError