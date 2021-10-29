using Bgt.Ocean.Models.BaseModel;
using System;

namespace Bgt.Ocean.Models.RouteOptimization
{
    public class DailyRouteSummaryRequest : PagingBase
    {
        public Guid SiteGuid { get; set; }
        public DateTime WorkDate { get; set; }
    }
}
