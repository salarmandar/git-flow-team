using System;

namespace Bgt.Ocean.Service.ModelViews.PricingRules
{
    public class QuotationMappingPricingView
    {
        /// <summary>
        /// PK mapping quotation -> pricing rule
        /// </summary>
        public Guid? Guid { get; set; }
        public PricingRuleView PricingRuleView { get; set; }
        public bool FlagFromContract { get; set; }
        public bool FlagEditable { get { return Guid.HasValue; } }
        public bool FlagApproved { get; set; }
        public decimal? AdjustedAmount { get; set; }
        public bool FlagExceed { get; set; }
        public bool FlagRateChanged { get; set; }
        public bool FlagRemoved { get; set; }
    }
}
