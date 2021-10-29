using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.PricingRules
{
    public class SyncPricingRuleView
    {
        public System.Guid Guid { get; set; }
        public System.Guid Product_Guid { get; set; }
        public string Name { get; set; }
        public System.Guid MasterCurrency_Guid { get; set; }
        public string WordShownInTariff { get; set; }
        public int SeqNo { get; set; }
        public string UserCreated { get; set; }
        public System.DateTimeOffset DatetimeCreated { get; set; }
        public string UserModified { get; set; }
        public System.DateTimeOffset DatetimeModified { get; set; }
        public Nullable<System.Guid> MasterPricingRule_Guid { get; set; }
        public IEnumerable<SyncChargeCategoryView> TblChargeCategory { get; set; }
    }
}
