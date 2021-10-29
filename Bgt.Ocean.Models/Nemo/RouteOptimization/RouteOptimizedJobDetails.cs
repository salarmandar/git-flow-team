using System;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class RouteOptimizedJobDetails
    {
        public Guid JobGuid { get; set; }
        public int JobOrder { get; set; }
        public DateTime ScheduleTime { get; set; }
    }
}
