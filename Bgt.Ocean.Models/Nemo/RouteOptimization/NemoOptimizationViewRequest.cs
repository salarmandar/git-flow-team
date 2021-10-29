using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class NemoOptimizationViewRequest
    {

        public DateTime WorkDate { get; set; }
        public Guid SiteGuid { get; set; }
        public int OptimizationType { get; set; }
        public List<Guid> RunGuid { get; set; } = new List<Guid>();
        public List<MasterRouteDetail> MasterRoute { get; set; } = new List<MasterRouteDetail>();
    }

    public class MasterRouteDetail
    {
        public Guid RouteGuid { get; set; }
        public Guid RouteDetailGuid { get; set; }
    }
}
