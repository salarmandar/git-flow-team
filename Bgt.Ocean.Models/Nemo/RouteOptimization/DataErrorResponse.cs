using Newtonsoft.Json;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class DataErrorResponse
    {
        [JsonProperty("Row")]
        public int Row { get; set; }

        [JsonProperty("Error")]
        public string Error { get; set; }
    }
}
