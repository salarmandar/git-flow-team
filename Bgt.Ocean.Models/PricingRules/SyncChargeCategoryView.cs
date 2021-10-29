using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.PricingRules
{
    public class SyncChargeCategoryView
    {
        public System.Guid Guid { get; set; }
        public System.Guid PricingRule_Guid { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Minimum { get; set; }
        public Nullable<decimal> Maximum { get; set; }
        public bool IsVoid { get; set; }
        public System.Guid MasterRevenueCategory_Guid { get; set; }
        public System.Guid SystemPricingChargeType_Guid { get; set; }
        public int SeqNo { get; set; }
        public System.Guid SystemPricingSatisfy_Guid { get; set; }
        public System.Guid SystemPricingCriteria_Guid { get; set; }
        public IEnumerable<SyncChargeCategory_ActionView> TblChargeCategory_Action { get; set; }
        public IEnumerable<SyncChargeCategory_RuleView> TblChargeCategory_Rule { get; set; }
    }
}
