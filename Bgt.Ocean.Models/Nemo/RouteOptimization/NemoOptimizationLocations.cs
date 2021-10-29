using System;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class NemoOptimizationLocations
    {
        public Guid DailyRunGuid { get; set; }
        public Guid ShiftGuid { get; set; }
        public Guid JobGuid { get; set; }
        public Guid LocationGuid { get; set; }
        public string LocationCode { get; set; }
        public string ServiceType { get; set; }
        public int JobOrder { get; set; }
    }
}
