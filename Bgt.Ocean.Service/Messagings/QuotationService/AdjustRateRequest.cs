using System;

namespace Bgt.Ocean.Service.Messagings.QuotationService
{
    public class AdjustRateRequest : RequestBase
    {
        /// <summary>
        /// Optional for adjust all pricing in product need this
        /// </summary>
        public Guid? Product_Guid { get; set; }
        public Guid? PricingRule_Guid { get; set; }
        public int AdjustRateTypeID { get; set; }
        public decimal Percentage { get; set; }
        public Guid Quotation_Guid { get; set; }
        public decimal Amount { get; set; }

        /// <summary>
        /// Get sale user from user login
        /// </summary>
        public Guid MasterUser_Guid { get; set; }
    }
}
