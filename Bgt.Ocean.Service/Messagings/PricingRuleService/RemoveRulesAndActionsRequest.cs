using System;

namespace Bgt.Ocean.Service.Messagings.PricingRuleService
{
    public class RemoveRulesAndActionsRequest
    {
        public Guid ChargeCategory_Guid { get; set; }
        public Guid SystemPricingChargeType_Guid { get; set; }
    }
}
