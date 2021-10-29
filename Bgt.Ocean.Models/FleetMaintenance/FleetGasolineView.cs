
using System;

namespace Bgt.Ocean.Models.FleetMaintenance
{
    public class FleetGasolineView
    {
        public Guid RunResourceGasolineGuid { get; set; }
        public DateTime TopUpDate { get; set; }
        public decimal TopUpAmount { get; set; }
        public string GasolineVendorName { get; set; }
        public string GasolineName { get; set; }
        public string CurrencyAmount { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public DateTime? DatetimeModified { get; set; }
        public bool FlagDisable { get; set; }
        public double TopUpQty { get; set; }
        public string TopUpQtyUnit { get; set; }
    }

    public class FleetGasolineOperatorView
    {
        public int Id { get; set; }
        public string Operator { get; set; }
    }

   
}
