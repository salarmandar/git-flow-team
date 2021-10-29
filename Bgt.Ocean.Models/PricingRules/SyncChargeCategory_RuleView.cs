using System.Collections.Generic;

namespace Bgt.Ocean.Models.PricingRules
{
    public class SyncChargeCategory_RuleView
    {
        public System.Guid Guid { get; set; }
        public System.Guid ChargeCategory_Guid { get; set; }
        public int SeqNo { get; set; }
        public System.Guid SystemPricingVariable_Guid { get; set; }
        public System.Guid SystemOperator_Guid { get; set; }
        public IEnumerable<SyncChargeCategory_Rule_ValueView> TblChargeCategory_Rule_Value { get; set; }
    }
}
