using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class NemoOptimizationResponse
    {
        [JsonProperty("Guid")]
        public Guid Guid { get; set; }

        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("ErrorDetail")]
        public IEnumerable<DataErrorResponse> ErrorDetail { get; set; }
    }
}
