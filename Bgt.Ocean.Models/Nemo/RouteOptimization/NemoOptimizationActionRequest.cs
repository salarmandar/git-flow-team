using System;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class NemoOptimizationActionRequest
    {
        public Guid OptimizeGuid { get; set; }
        public int OptimizeAction { get; set; }
    }
}
