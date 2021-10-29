using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.PricingRules
{
    public class SyncChargeCategory_ActionView
    {
        public System.Guid Guid { get; set; }
        public System.Guid ChargeCategory_Guid { get; set; }
        public decimal BaseRate { get; set; }
        public System.Guid BasisPricingVariable_Guid { get; set; }
        public string Value { get; set; }
        public int SeqNo { get; set; }
        public bool IsVoid { get; set; }
        public bool IsProgressive { get; set; }
        public Nullable<System.Guid> SystemPricingCriteria_Guid { get; set; }
        public Nullable<System.Guid> SystemPricingVariable_Guid { get; set; }
        public IEnumerable<SyncChargeCategory_Action_ChargeView> TblChargeCategory_Action_Charge { get; set; }
    }
}
