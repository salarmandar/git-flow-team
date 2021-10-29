using System;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class NemoRouteOptimizationObject
    {
        public Guid NemoQueueGuid { get; set; }
        public Guid DailyGuid { get; set; }
        public Int64 OptimizationOrder { get; set; }
        public Guid BranchGuid { get; set; }
        public string BranchCode { get; set; }
        public Guid CountryGuid { get; set; }
        public string CountryCode { get; set; }
        public Guid ShiftGuid { get; set; }
        public DateTime ShiftServiceStart { get; set; }
        public Nullable<System.DateTime> ShiftServiceEnd { get; set; }
        public Nullable<System.DateTime> DateStart { get; set; }
        public string TimeZone { get; set; }
    }
}
