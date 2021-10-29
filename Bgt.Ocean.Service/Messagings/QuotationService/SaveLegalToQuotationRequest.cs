using Bgt.Ocean.Service.ModelViews.PricingRules;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.QuotationService
{
    public class SaveLegalToQuotationRequest : RequestBase
    {
        public Guid Quotation_Guid { get; set; }
        public Guid Product_Guid { get; set; }
        public IEnumerable<ClauseCopyDataView> LegalUpdatedList { get; set; } = new List<ClauseCopyDataView>();
    }
}
