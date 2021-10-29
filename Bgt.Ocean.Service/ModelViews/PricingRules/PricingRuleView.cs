using System;

namespace Bgt.Ocean.Service.ModelViews.PricingRules
{
    public class PricingRuleView
    {
        public Guid Guid { get; set; }
        public Guid Product_Guid { get; set; }
        public string Name { get; set; }
        public Guid MasterCurrency_Guid { get; set; }
        public string MasterCurrencyAbbreviation { get; set; }
        public string DisplayName { get { return $"{Name} - {MasterCurrencyAbbreviation}"; } }       
        public string WordShownInTariff { get; set; }
        public int SeqNo { get; set; }

        /// <summary>
        /// Reference pricing rule after clone
        /// </summary>
        public Guid? MasterPricingRule_Guid { get; set; }

        #region Product info
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public Guid? SystemLineOfBusiness_Guid { get; set; }
        #endregion
    }
}
