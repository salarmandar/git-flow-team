using System;

namespace Bgt.Ocean.Service.ModelViews.PreVault
{
    public class RouteGroupRunResourceModelView
    {
        public Guid DailyRunGuid { get; set; }
        public string DailyRunName { get; set; }
        public string RouteGroupDetailName { get; set; }
        public string DisplayText { get; set; }
        public Guid RouteGroupGuid { get; set; }
        public string RouteGroupName { get; set; }
    }
}
