using Bgt.Ocean.Models.FleetMaintenance;

namespace Bgt.Ocean.Service.Messagings.FleetMaintenance
{
    public class FleetSummaryRequest 
    {
        public SummaryMaintenanceFilter Filters { get; set; }
    }

    public class FleetSummaryResponse : BaseResponse
    {
        public FleetSummaryView Summary { get; set; } = new FleetSummaryView();
    }
}
