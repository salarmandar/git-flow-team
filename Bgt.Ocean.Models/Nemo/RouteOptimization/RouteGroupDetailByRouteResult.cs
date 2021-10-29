using System;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class RouteGroupDetailByRouteResult
    {
        public Nullable<System.Guid> RouteGroupDetail_Guid { get; set; }
        public Nullable<System.Guid> RouteGroup_Guid { get; set; }
        public string MasterRouteGroupName { get; set; }
        public string MasterRouteGroupDetailName { get; set; }
        public string MasterRouteGroupDetailDisplayText { get; set; }
        public System.Guid MasterRoute_Guid { get; set; }
        public string MasterRouteName { get; set; }
    }
}
