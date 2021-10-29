using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class OptimizeNode
    {
        public List<Guid> JobGuids { get; set; }
        public long PlannedOrder { get; set; }
        public int OptimizedOrder { get; set; }
        public string ServiceStopName { get; set; }
        public string EstimatedArrival { get; set; }
        public long EstimatedArrivalSeconds { get; set; }
        public TimeWindow TimeWindow { get; set; }
        public Guid? MasterCustomerLocation_Guid { get; set; }
        public string MasterCustomerLocation_Latitude { get; set; }
        public string MasterCustomerLocation_Longitude { get; set; }
        public string ServiceType { get; set; }
        public string TimeStart { get; set; }
        public string ServiceStopCode { get; set; }
    }
}
