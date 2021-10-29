using Newtonsoft.Json;
using System;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class RouteOptimizationDetailRequest
    {
        [JsonProperty("Guid")]
        public Guid Guid { get; set; }
    }
}
