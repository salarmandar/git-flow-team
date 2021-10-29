using System;

namespace Bgt.Ocean.Service.Messagings.PricingRuleService
{
    public class CopyPricingRuleRequest
    {
        /// <summary>
        /// new Guid of duplicated product
        /// </summary>
        public Guid Product_Guid { get; set; }

        /// <summary>
        /// original product (for copy in feature duplicate product)
        /// </summary>
        public Guid SourceProduct_Guid { get; set; }
    }
}
