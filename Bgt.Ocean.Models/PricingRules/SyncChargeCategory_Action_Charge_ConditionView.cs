using System;

namespace Bgt.Ocean.Models.PricingRules
{
    public class SyncChargeCategory_Action_Charge_ConditionView
    {
        public System.Guid Guid { get; set; }
        public System.Guid ChargeCategory_Action_Charge_Guid { get; set; }
        public int SeqNo { get; set; }
        public Nullable<System.Guid> SystemPricingVariable_Guid { get; set; }
        public Nullable<System.Guid> SystemOperator_Guid { get; set; }
        public string Value { get; set; }
    }
}
