using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.RouteOptimization
{

    public class MasterRouteSummaryView
    {
        public int MasterDayOfWeek_Sequence { get; set; }
        public string MasterDayOfWeek_Name { get; set; }
        public int WeekTypeInt { get; set; }
        public string WeekTypeName { get; set; }
        public Guid RouteGuid { get; set; }
        public string MasterRouteName { get; set; }
        public Guid RouteGroupGuid { get; set; }
        public string MasterRouteGroupName { get; set; }
        public Guid RouteGroupDetailGuid { get; set; }
        public string MasterRouteGroupDetailName { get; set; }
        public string Holiday { get; set; }
        public int Stops { get; set; }
        public string StrStops { get; set; }
        public int Jobs { get; set; }
        public string StrJobs { get; set; }
        public int Locations { get; set; }
        public string StrLocations { get; set; }
        public string OptimizationStatus { get; set; }
        public int OptimizationStatusId { get; set; }
    }

    public class MasterRouteSummaryResponse
    {
        public IEnumerable<MasterRouteSummaryView> MasterRouteSummaryList { get; set; }

        public int Total { get; set; }
    }

   
}
