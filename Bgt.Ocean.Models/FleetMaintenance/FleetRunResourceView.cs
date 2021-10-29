using Bgt.Ocean.Infrastructure.Util;
using System;

namespace Bgt.Ocean.Models.FleetMaintenance
{
    public class FleetRunResourceView
    {
        public Guid? RunGuid { get; set; }
        public string VehicleNumber { get; set; }
        public Guid? ModeOfTransportGuid { get; set; }
        public string ModeOfTransport { get; set; }
        public EnumMOT ModeOfTransportID { get; set; }
        public EnumMaintenanceStatus MaintenanceStatusID { get; set; }
        public string MaintenanceStatus { get; set; }
        public string ImageUrl { get; set; }
    }
}
