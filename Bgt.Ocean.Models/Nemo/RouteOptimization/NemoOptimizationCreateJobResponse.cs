using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class NemoOptimizationCreateJobResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public List<Guid> Guid { get; set; } = new List<Guid>();
    }
}
