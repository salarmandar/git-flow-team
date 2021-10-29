using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Reports.DailyPlan
{
    public class DailyPlanCustomerRequest
    { 
        public Guid SiteGuid { get; set; }

        public string StrWorkDate { get; set; }
        public string DateFormat { get; set; }
    }

    public class DailyPlanRouteGroupRequest : DailyPlanCustomerRequest
    {
        public IEnumerable<Guid?> CustomerGuid { get; set; }
    }

    public class DailyPlanRouteGroupDetailRequest: DailyPlanRouteGroupRequest
    {
        public IEnumerable<Guid?> RouteGroupGuid { get; set; }
    }

    public class DailyPlanDataRequest : DailyPlanRouteGroupDetailRequest
    {
        public IEnumerable<Guid> RouteGroupDetailGuid { get; set; }
        public int MaxRow { get; set; }
    }

    public class DailyPlanEmailRequest
    {
        public Guid CustomerGuid { get; set; }
        public Guid DailyRunGuid { get; set; }
    }
}
