using System;
using System.Collections.Generic;


namespace Bgt.Ocean.Models.RouteOptimization
{
    public class DailyRouteView
    {
        public Guid Guid { get; set; }
        public Guid RouteGroupGuid { get; set; }
        public string MasterRouteGroupName { get; set; }
        public Guid RouteGroupDetailGuid { get; set; }
        public string MasterRouteGroupDetailName { get; set; }
        public string RunNo { get; set; }
        public string RunStatus { get; set; }
        public int RunStatusId { get; set; }
        public string Balanced { get; set; }
        public DateTime? StartTime { get; set; }
        public string StrStartTime { get; set; }
        public decimal? STC { get; set; }
        public string StrSTCDisplay { get; set; }
        public string Currency { get; set; }
        public int Stops { get; set; }
        public string StrStops { get; set; }
        public int Jobs { get; set; }
        public string StrJobs { get; set; }
        public int Locations { get; set; }
        public string StrLocations { get; set; }
        public string OptimizationStatus { get; set; }
        public int OptimizationStatusId { get; set; }
        public bool FlagShowAllRunStatus { get; set; } = false;
        public bool FlagShowAllOptStatus { get; set; } = false;
        public Guid? JobHeaderGuid { get; set; }
        public Guid? CurrencyOnrunGuid { get; set; }
    }

    public class DailyRouteResponse
    {
        public IEnumerable<DailyRouteView> DailyRouteList { get; set; }

        public int Total { get; set; }
    }

    public class DailyRouteSummaryResponse
    {
        public IEnumerable<DailyRouteView> DailyRouteSummaryList { get; set; }

        public int Total { get; set; }
    }


    public class DailyRouteRequestDetailResponse : RequestManagementDetailResponse
    {
        public DateTime WorkDate { get; set; }

        public Guid RequestTypeGuid { get; set; }

        public string CancelledUser { get; set; }
        public DateTime? CancelledDatetime { get; set; }
        public DateTime? CompletedDatetime { get; set; }

    }

}
