using Bgt.Ocean.Service.ModelViews.PricingRules;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.PricingRuleService
{
    public class SaveRuleRequest : RequestBase
    {
        public Guid ChargeCategory_Guid { get; set; }
        public Guid SystemPricingVariable_Guid { get; set; }
        public Guid SystemOperator_Guid { get; set; }
        public int SeqNo { get; set; }
        public IEnumerable<RuleValueView> ValueList  { get; set; }

        /// <summary>
        /// TableName from TblSystemPricingEntity
        /// </summary>
        public string Entity_TableName { get; set; }

        /// <summary>
        /// SQL operator from TblSystemOperator, It must depend on SystemOperator_Guid
        /// </summary>
        public string Operator { get; set; }
    }
}
