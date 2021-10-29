using System;

namespace Bgt.Ocean.Models.RouteOptimization
{

    public class MasterRouteNameRequestModel
    {
        public Guid SiteGuid { get; set; }
        public Guid DayOfWeekGuid { get; set; }
        public Guid WeekGuid { get; set; }
        public bool FlagHoliday { get; set; } = false;
    }

    public class MasterRouteOptimizationRequestModel
    {
        public Guid SiteGuid { get; set; }
    }

    public class MasterRouteOptimizationListRequestModel
    {
        public Guid RequestGuid { get; set; }
    }

    public class MasterRouteOptimizationRequestListRequestModel
    {
        public Guid SiteGuid { get; set; }
        public Guid DayOfWeekGuid { get; set; }
        public Guid WeekGuid { get; set; }
        public Guid MasterRouteGuid { get; set; }
        public bool FlagHoliday { get; set; }
        public bool FlagShowAllStatus { get; set; }
        public int MaxRow { get; set; }
        public Guid? UserLangquage { get; set; }
    }
}
