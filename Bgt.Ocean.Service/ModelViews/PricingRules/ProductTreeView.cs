using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.PricingRules
{
    public class ProductTreeView : TreeViewBase
    {
        /// <summary>
        /// Primary key of product table
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// (Equal or More than 1) = Has child leaf
        /// (0) = No child leaf
        /// </summary>
        public int CountChildLeaf { get; set; }

        /// <summary>
        /// flag for mark this product is parent of selected product; found on move product page
        /// </summary>
        public bool IsParentOfSelectedProduct { get; set; }
        public Guid SystemLineOfBusiness_Guid { get; set; }
        public bool IsProductLOB { get; set; }
        public Guid MasterCountry_Guid { get; set; }
        public Guid BrinksCompany_Guid { get; set; }
        public Guid SystemCashStatus_Guid { get; set; }
        public int CashStatusID { get; set; }
        public string LOBFullName { get; set; }
        public string ProductName { get; set; }
        public string ProductNameDisplay
        {
            get
            {
                if (!string.IsNullOrEmpty(ProductID))
                    return $"{ProductID} - {ProductName}".TrimStart();
                return $"{ProductName}".TrimStart();
            }
        }    
        public int ProductLevel { get; set; }
        public bool FlagProductLeaf { get; set; }
        public bool FlagDisable { get; set; }
        public bool FlagRemove { get; set; }
        public string ProductID { get; set; }
        public string Description { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public Guid? ParentProduct_Guid { get; set; }
        public int ProductIndex { get; set; }
        public IEnumerable<ProductTreeView> ChildProductList { get; set; }
        public IEnumerable<PricingRuleView> PricingRuleList { get; set; }
        public string WordShownInTariff { get; set; }
        public bool FlagPricingRule { get; set; }
        public IEnumerable<Guid> ServiceJobTypeGuids { get; set; }
    }
}
