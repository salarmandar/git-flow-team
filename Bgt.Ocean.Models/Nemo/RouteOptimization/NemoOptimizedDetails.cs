using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class NemoOptimizedDetails
    {
        public Guid NemoTaskGuid { get; set; }
        public decimal Distance { get; set; }
        public int DurationTime { get; set; }
        public int WaitTime { get; set; }
        public List<RouteOptimizedJobDetails> Jobs { get; set; } = new List<RouteOptimizedJobDetails>();
    }
}
