using Bgt.Ocean.Models;
using System;

namespace Bgt.Ocean.Service.ModelViews.PricingRules
{
    public class ProductClauseView
    {
        public string ProductID { get; set; }
        public TblLeedToCashProduct_Clause Product_Clause { get; set; }
        public bool FlagInherit { get; set; }
        public string ClauseName { get; set; }
        public string ClauseID { get; set; }
        public string ClauseCategoryName { get; set; }
        public string ClauseDescription { get; set; }
        public Nullable<bool> FlagMandatory { get; set; }
        public Nullable<bool> FlagEditableInContract { get; set; }
        public Nullable<bool> FlagVisibleInContract { get; set; }
        public Nullable<bool> FlagVisibleInQuotation { get; set; }
        public Nullable<int> ClauseSeqNo { get; set; }
        public Nullable<System.Guid> MasterClause_Guid { get; set; }
        public Nullable<System.Guid> MasterClauseCat_Guid { get; set; }
        public string ClauseDetail { get; set; }
        public string ClauseDetailFieldID { get; set; }

    }
}
