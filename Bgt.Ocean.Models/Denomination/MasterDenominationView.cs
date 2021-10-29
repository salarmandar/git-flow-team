using Bgt.Ocean.Infrastructure.Util;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Denomination
{
    public class DenominationHeaderView
    {
        public Guid? LiabilityGuid { get; set; }
        public Guid? CurrencyGuid { get; set; }
        public bool FlagDeletePrevDeno { get; set; }
        public IEnumerable<DenominationDetailView> DenominationList { get; set; }
    }

    public class DenominationDetailView
    {
        public Guid? DenoGuid { get; set; }
        public string DenoName { get; set; }
        public double DenoValue { get; set; }
        public int Qty { get; set; }
        public double Value { get; set; }
        public string Type { get; set; }
        public Guid? DenoUnitGuid { get; set; }
        public Guid? LiabilityDenoGuid { get; set; }
        public EnumState ItemState { get; set; }
        public DenoUnit UnitType { get; set; }
    }
}
