using System;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.PricingRuleService
{
    public class SavePricingRuleRequest : RequestBase
    {
        public Guid? Guid { get; set; }

        [Required]
        public Guid Product_Guid { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Guid MasterCurrency_Guid { get; set; }

        public string WordShownInTariff { get; set; }

        public Guid Quotation_Guid { get; set; }
    }
}
