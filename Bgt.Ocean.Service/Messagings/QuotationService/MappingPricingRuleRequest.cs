using Bgt.Ocean.Service.ModelViews.PricingRules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.QuotationService
{
    public class MappingPricingRuleRequest : RequestBase
    {
        [Required]
        public Guid Quotation_Guid { get; set; }
        public List<QuotationMappingPricingView> PricingRuleInQuotationList { get; set; } = new List<QuotationMappingPricingView>();
    }
}
