using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class SolutionRouteOptimizationResponse
    {

        [JsonProperty("CountryGuid")]
        public Guid CountryGuid { get; set; }

        [JsonProperty("BranchGuid")]
        public Guid BranchGuid { get; set; }

        [JsonProperty("ShiftGuid")]
        public Guid ShiftGuid { get; set; }

        [JsonProperty("DateStart")]
        public DateTime DateStart { get; set; }

        [JsonProperty("Shift")]
        public string Shift { get; set; }

        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("Node")]
        public int Node { get; set; }

        [JsonProperty("Route")]
        public int Route { get; set; }

        [JsonProperty("Distance")]
        public decimal Distance { get; set; }

        [JsonProperty("DurationTime")]
        public int DurationTime { get; set; }

        [JsonProperty("TravelTime")]
        public int TravelTime { get; set; }

        [JsonProperty("WaitTime")]
        public int WaitTime { get; set; }

        [JsonProperty("Routes")]
        public IEnumerable<RoutesResponse> Routes { get; set; }

        [JsonProperty("Unserved")]
        public IEnumerable<UnservedResponse> Unserved { get; set; }

        [JsonProperty("Duration")]
        public string Duration { get; set; }

        [JsonProperty("Travel")]
        public string Travel { get; set; }

        [JsonProperty("Wait")]
        public string Wait { get; set; }

        [JsonProperty("RouteOptimizeTaskGuid")]
        public Guid RouteOptimizeTaskGuid { get; set; }

        [JsonProperty("Status")]
        public int Status { get; set; }

        [JsonProperty("Execution")]
        public string Execution { get; set; }

    }

    public class RoutesResponse
    {
        [JsonProperty("ServiceTypes")]
        public IEnumerable<string> ServiceTypes { get; set; }

        [JsonProperty("ParentNodes")]
        public IEnumerable<string> ParentNodes { get; set; }

        [JsonProperty("Jobs")]
        public IEnumerable<string> Jobs { get; set; }

        [JsonProperty("Distance")]
        public decimal Distance { get; set; }

        [JsonProperty("Count")]
        public int Count { get; set; }

        [JsonProperty("Paths")]
        public IEnumerable<string> Paths { get; set; }

        [JsonProperty("Arrivals")]
        public IEnumerable<string> Arrivals { get; set; }

        [JsonProperty("Departures")]
        public IEnumerable<string> Departures { get; set; }

        [JsonProperty("Distances")]
        public IEnumerable<decimal> Distances { get; set; }

        [JsonProperty("Travels")]
        public IEnumerable<string> Travels { get; set; }

        [JsonProperty("Waits")]
        public IEnumerable<string> Waits { get; set; }

        [JsonProperty("Capacities")]
        public IEnumerable<decimal> Capacities { get; set; }

        [JsonProperty("Volumes")]
        public IEnumerable<decimal> Volumes { get; set; }

        [JsonProperty("Liabilities")]
        public IEnumerable<decimal> Liabilities { get; set; }

        [JsonProperty("ServiceTimes")]
        public IEnumerable<int> ServiceTimes { get; set; }

        [JsonProperty("TravelTimes")]
        public IEnumerable<int> TravelTimes { get; set; }

        [JsonProperty("WaitTimes")]
        public IEnumerable<int> WaitTimes { get; set; }

        [JsonProperty("ArrivalTimes")]
        public IEnumerable<decimal> ArrivalTimes { get; set; }

        [JsonProperty("DepartureTimes")]
        public IEnumerable<decimal> DepartureTimes { get; set; }

        [JsonProperty("Services")]
        public IEnumerable<string> Services { get; set; }

        [JsonProperty("Vehicle")]
        public string Vehicle { get; set; }

        [JsonProperty("Liability")]
        public decimal Liability { get; set; }

        [JsonProperty("Capacity")]
        public decimal Capacity { get; set; }

        [JsonProperty("Volume")]
        public decimal Volume { get; set; }

        [JsonProperty("Latitudes")]
        public IEnumerable<string> Latitudes { get; set; }

        [JsonProperty("Longitudes")]
        public IEnumerable<string> Longitudes { get; set; }

        [JsonProperty("Start")]
        public string Start { get; set; }

        [JsonProperty("End")]
        public string End { get; set; }

        [JsonProperty("Duration")]
        public string Duration { get; set; }

        [JsonProperty("DurationTime")]
        public int DurationTime { get; set; }

        [JsonProperty("Travel")]
        public string Travel { get; set; }

        [JsonProperty("TravelTime")]
        public int TravelTime { get; set; }

        [JsonProperty("Wait")]
        public string Wait { get; set; }

        [JsonProperty("WaitTime")]
        public int WaitTime { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }
    }

    public class UnservedResponse
    {
        [JsonProperty("Guid")]
        public Guid Guid { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("ParentCode")]
        public string ParentCode { get; set; }

        [JsonProperty("ServiceTypeCode")]
        public string ServiceTypeCode { get; set; }

        [JsonProperty("Liability")]
        public decimal Liability { get; set; }

        [JsonProperty("Capacity")]
        public decimal Capacity { get; set; }

        [JsonProperty("Volume")]
        public decimal Volume { get; set; }
    }
}
