using System;

namespace Bgt.Ocean.Service.ModelViews.PricingRules
{
    public class ClauseCopyDataView
    {
        public Guid MasterClause_Guid { get; set; }
        public Guid? MasterClauseCat_Guid { get; set; }
        public string ProductId { get; set; }
        public string ClauseID { get; set; }
        public string ClauseName { get; set; }
        public string ClauseCategoryName { get; set; }
        public string ClauseDescription { get; set; }
        public string ClauseDetail { get; set; }
        public string ClauseDetailFieldID { get; set; }
        public bool FlagMandatory { get; set; }
        public bool FlagInherit { get; set; }
        public bool FlagEditableInContract { get; set; }
        public bool FlagVisibleInContract { get; set; }
        public bool FlagVisibleInQuotation { get; set; }
        public int ClauseSeqNo { get; set; }
    }
}
