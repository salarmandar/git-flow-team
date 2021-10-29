using Newtonsoft.Json;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class NemoCriteriaRequest
    {
        [JsonProperty("MaxNode")]
        public int MaxNode { get; set; }

        [JsonProperty("MaxVehicle")]
        public int MaxVehicle { get; set; }

        [JsonProperty("MaxLiability")]
        public decimal MaxLiability { get; set; }

        [JsonProperty("MaxDistance")]
        public decimal MaxDistance { get; set; }

        [JsonProperty("AverageSpeed")]
        public decimal AverageSpeed { get; set; }

        [JsonProperty("MaxCapacity")]
        public decimal MaxCapacity { get; set; }

        [JsonProperty("MaxVolume")]
        public decimal MaxVolume { get; set; }

        [JsonProperty("Alpha")]
        public int Alpha { get; set; }

        [JsonProperty("Beta")]
        public int Beta { get; set; }

        [JsonProperty("Gamma")]
        public int Gamma { get; set; }

        [JsonProperty("MaxNodeDistance")]
        public decimal MaxNodeDistance { get; set; }

        [JsonProperty("MaxDuration")]
        public int MaxDuration { get; set; }

        [JsonProperty("MaxWait")]
        public int MaxWait { get; set; }

        [JsonProperty("LunchTime")]
        public string LunchTime { get; set; }

        [JsonProperty("LunchDuration")]
        public int LunchDuration { get; set; }

        [JsonProperty("MaxBayLoadingTime")]
        public int MaxBayLoadingTime { get; set; }

        [JsonProperty("MaxBayLoadingVehicle")]
        public int MaxBayLoadingVehicle { get; set; }

        [JsonProperty("Territory")]
        public bool Territory { get; set; }

        [JsonProperty("TerritoryOverlap")]
        public bool TerritoryOverlap { get; set; }

        [JsonProperty("MaxTerritoryOverlapDistance")]
        public int MaxTerritoryOverlapDistance { get; set; }

        [JsonProperty("DistanceUnit")]
        public string DistanceUnit { get; set; }

        [JsonProperty("TrafficFactor")]
        public bool TrafficFactor { get; set; }

        [JsonProperty("TrafficFactors")]
        public string[] TrafficFactors { get; set; }

        [JsonProperty("BreakTimes")]
        public string[] BreakTimes { get; set; }

        [JsonProperty("EarliestDispatchTime")]
        public bool EarliestDispatchTime { get; set; }

        [JsonProperty("TurnaroundTime")]
        public bool TurnaroundTime { get; set; }

        [JsonProperty("TurnaroundLeadTime")]
        public int TurnaroundLeadTime { get; set; }

        [JsonProperty("CostDistance")]
        public decimal CostDistance { get; set; }

        [JsonProperty("CostWorkingHour")]
        public decimal CostWorkingHour { get; set; }

        [JsonProperty("CostOverTimeHour")]
        public decimal CostOverTimeHour { get; set; }

        [JsonProperty("MaxWorkingHour")]
        public decimal MaxWorkingHour { get; set; }

        [JsonProperty("MaxCrew")]
        public int MaxCrew { get; set; }
    }
}
