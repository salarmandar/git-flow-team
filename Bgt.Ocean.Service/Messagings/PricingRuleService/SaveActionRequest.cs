using Bgt.Ocean.Service.ModelViews.PricingRules;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.PricingRuleService
{
    public class SaveActionRequest : RequestBase
    {
        public Guid? Guid { get; set; }
        public Guid ChargeCategory_Guid { get; set; }
        public decimal BaseRate { get; set; }
        public Guid BasisPricingVariable_Guid { get; set; }
        public int SeqNo { get; set; }
        public bool IsVoid { get; set; }
        public bool IsProgressive { get; set; }

        /// <summary>
        /// for progressive
        /// </summary>
        public Guid? SystemPricingVariable_Guid { get; set; }

        /// <summary>
        /// for condition
        /// </summary>
        public Guid? SystemPricingCriteria_Guid { get; set; }

        public IEnumerable<Action_ChargeView> Action_ChargeList { get; set; } = new List<Action_ChargeView>();
    }
}
