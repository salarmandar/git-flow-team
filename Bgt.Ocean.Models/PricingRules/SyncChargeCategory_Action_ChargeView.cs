using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.PricingRules
{
    public class SyncChargeCategory_Action_ChargeView
    {
        public System.Guid Guid { get; set; }
        public System.Guid ChargeCategory_Action_Guid { get; set; }
        public int SeqNo { get; set; }
        public System.Guid BasisPricingVariable_Guid { get; set; }
        public bool FlagForEvery { get; set; }
        public decimal QuantityOfBasis { get; set; }
        public Nullable<System.Guid> SystemLogicalOperator_Guid { get; set; }
        public System.Guid SystemOperator_Guid { get; set; }
        public decimal Value { get; set; }
        public Nullable<decimal> Minimum { get; set; }
        public Nullable<decimal> Maximum { get; set; }
        public bool IsVoid { get; set; }
        public Nullable<System.Guid> SystemDivisionRounding_Guid { get; set; }
        public Nullable<System.Guid> SystemPricingVariable_Guid { get; set; }
        public IEnumerable<SyncChargeCategory_Action_Charge_ConditionView> TblChargeCategory_Action_Charge_Condition { get; set; }
    }
}
