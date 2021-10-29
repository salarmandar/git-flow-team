using System;

namespace Bgt.Ocean.Models.RunControl
{
    public class RouteGroupDetailRunResourceView
    {
        public Guid DailyRunGuid { get; set; }
        public string VehicleNumber { get; set; }
        public string RouteGroupDetailName { get; set; }
        public int RunResourceShitf { get; set; }
        public Guid RouteGroupGuid { get; set; }
        public string RouteGroupName { get; set; }
    }
}
