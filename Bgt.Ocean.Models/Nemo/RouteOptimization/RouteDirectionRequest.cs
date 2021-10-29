using Newtonsoft.Json;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class RouteDirectionRequest
    {
        [JsonProperty("CountryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("BranchCode")]
        public string BranchCode { get; set; }

        [JsonProperty("CustomerLocationCode")]
        public string CustomerLocationCode { get; set; }
    }
}
