using System;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class MasterRouteOptimizationView
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
    }
}
