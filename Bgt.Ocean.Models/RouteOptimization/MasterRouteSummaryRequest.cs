using Bgt.Ocean.Models.BaseModel;
using System;

namespace Bgt.Ocean.Models.RouteOptimization
{
    public class MasterRouteSummaryRequest : PagingBase
    {
        public Guid SiteGuid { get; set; }
        public Guid DayOfWeekGuid { get; set; }
        public int WeekTypeInt { get; set; }
        public Guid? UserLangquage { get; set; }

    }
}
