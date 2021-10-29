using System;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class DailyRunOptimizationView
    {
        public Guid Guid { get; set; }
        public string MasterRouteGroupName { get; set; }
        public string MasterRouteGroupDetailName { get; set; }
        public string VehicleNumberFullName { get; set; }
    }
}
