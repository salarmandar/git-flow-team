using Bgt.Ocean.Infrastructure.Util;
using System;

namespace Bgt.Ocean.Models.ActualJob
{
    public class DenominationOnMachineCassetteView
    {
        public Guid MachineCassetteGuid { get; set; }
        public int Seq { get; set; }
        public string CasseteName { get; set; }
        public CassetteType CassetteTypeID { get; set; }
        public Guid CassetteTypeIdGuid { get; set; }
        public Guid DeNoGuid { get; set; }
        public string DeNoText { get; set; }
        public double? DeNoValue { get; set; }
        public Guid CurrencyGuid { get; set; }
        public string CurrencyAbb { get; set; }
        public string CurrencyDescription { get; set; }
        public int? Amount { get; set; }

    }
}
