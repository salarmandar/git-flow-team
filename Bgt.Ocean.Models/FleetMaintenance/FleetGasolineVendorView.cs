using System;

namespace Bgt.Ocean.Models.FleetMaintenance
{
    public class FleetGasolineVendorView
    {
        public Guid Guid { get; set; }
        public string GasolineVendorName { get; set; }
    }

    public class FleetGasolineTypeView
    {
        public Guid Guid { get; set; }
        public string GasolineTypeName { get; set; }
    }

    public class FleetGasolineVendorDefaultView
    {
        public Guid? GasolineVendorGuid { get; set; }
        public Guid? GasolineTypeGuid { get; set; }
        public Decimal? UnitPrice { get; set; }
    }
}
