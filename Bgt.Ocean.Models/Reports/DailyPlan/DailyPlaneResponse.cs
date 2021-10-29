using System;

namespace Bgt.Ocean.Models.Reports.DailyPlan
{
    public class DailyPlanCustomerResponse
    {
        public Guid Guid { get; set; }
        public string CustomerFullName { get; set; }
    }

    public class DailyPlanRouteGroupResponse
    {
        public Guid Guid { get; set; }
        public string RouteGroupName { get; set; }
    }

    public class DailyPlanRouteGroupDetailResponse
    {
        public Guid Guid { get; set; }
        public string RouteGroupDetailName { get; set; }
    }

    public class DailyPlanDataResponse
    {
        public Guid Guid { get; set; }
        public Guid CustomerGuid { get; set; }
        public string CustomerName { get; set; }
        public string RouteGroupName { get; set; }
        public string RouteGroupDetailName { get; set; }
        public string RunResourceName { get; set; }
    }

    public class DailyPlanEmailResponse
    {
        public Guid CustomerGuid { get; set; }
        public string CustomerName { get; set; }
        public string LocationName { get; set; }
        public string Email { get; set; }
    }
}
