using System;

namespace Bgt.Ocean.Models.PricingRules
{
    public class SyncChargeCategory_Rule_ValueView
    {
        public System.Guid Guid { get; set; }
        public System.Guid ChargeCategory_Rule_Guid { get; set; }
        public string Value { get; set; }
        public int SeqNo { get; set; }
    }
}
