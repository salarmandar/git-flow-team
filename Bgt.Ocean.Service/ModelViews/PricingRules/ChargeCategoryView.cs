using System;

namespace Bgt.Ocean.Service.ModelViews.PricingRules
{
    public class ChargeCategoryView
    {
        public Guid Guid { get; set; }
        public Guid PricingRule_Guid { get; set; }
        public string Name { get; set; }
        public decimal? Minimum { get; set; }
        public decimal? Maximum { get; set; }
        public bool IsVoid { get; set; }
        public Guid MasterRevenueCategory_Guid { get; set; }
        public Guid SystemPricingChargeType_Guid { get; set; }
        public int SeqNo { get; set; }
        public Guid SystemPricingSatisfy_Guid { get; set; }
        public Guid SystemPricingCriteria_Guid { get; set; }
    }
}
