using System;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.QuotationService
{
    public class SavePricingRuleToBidRequest : RequestBase
    {
        public Guid? Guid { get; set; }

        [Required]
        public Guid BidProduct_Guid { get; set; }

        [Required]
        public Guid BidQuotation_Guid { get; set; }

        public string PricingRuleName { get; set; }

        [Required]
        public Guid MasterCurrency_Guid { get; set; }

        public string WordShownInTariff { get; set; }
    }
}
