using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.QuotationService
{
    public class GetPricingRuleTreeViewReqeust
    {
        [Required]
        public Guid BrinksCompany_Guid { get; set; }
        public Dictionary<Guid, bool> ProductSelectedGuids { get; set; }
        public List<Guid> ProductGuidsForDisable { get; set; }

        /// <summary>
        /// For quotation with contract need to send this parameter to get treeview of pricing rule without pricing rule in contract 
        /// </summary>
        public Guid? Contract_Guid { get; set; }
    }
}
