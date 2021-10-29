using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.PricingRules
{
    public class RuleView
    {
        public Guid Guid { get; set; }
        public Guid ChargeCategory_Guid { get; set; }
        public Guid SystemPricingVariable_Guid { get; set; }
        public Guid SystemOperator_Guid { get; set; }
        public int SeqNo { get; set; }
        public IEnumerable<RuleValueView> ValueList { get; set; }
    }

    public class RuleValueView
    {
        public int SeqNo { get; set; }
        public string Value { get; set; }
    }
}
