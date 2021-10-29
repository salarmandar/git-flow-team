using System;

namespace Bgt.Ocean.Service.Messagings.PricingRuleService
{
    public class DeleteRequest : RequestBase
    {
        public Guid Guid { get; set; }
        public DateTime? LocalDateTime { get; set; }
    }
}
