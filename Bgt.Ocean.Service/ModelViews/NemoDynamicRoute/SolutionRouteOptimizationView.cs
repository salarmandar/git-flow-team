using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.NemoDynamicRoute
{
    public class SolutionRouteOptimizationView
    {
        public Guid RouteOptimizeTaskGuid { get; set; }
        public bool Success { get; set; }
        public List<Guid> QueueGuid { get; set; }
        public TblSystemMessage systemMessage { get; set; }
    }
}
