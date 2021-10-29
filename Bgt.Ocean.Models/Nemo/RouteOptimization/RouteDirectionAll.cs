using System.Collections.Generic;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class RouteDirectionAll
    {
        public List<RouteDirectionRequest> RoutePlanned { get; set; } = new List<RouteDirectionRequest>();
        public List<RouteDirectionRequest> RouteOptimized { get; set; } = new List<RouteDirectionRequest>();
    }

    public class RouteDirectionAllResult
    {
        public List<RouteDirectionResult> RoutePlanned { get; set; } = new List<RouteDirectionResult>();
        public List<RouteDirectionResult> RouteOptimized { get; set; } = new List<RouteDirectionResult>();
    }
}
