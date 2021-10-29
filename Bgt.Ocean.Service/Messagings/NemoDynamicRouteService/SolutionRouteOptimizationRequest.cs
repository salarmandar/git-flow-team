using Bgt.Ocean.Infrastructure.Storages;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.NemoDynamicRouteService
{
    /// <summary>
    /// Request for Nemo Solution Route Optimization.
    /// </summary>
    public class SolutionRouteOptimizationRequest : RequestBase
    {
        public string Token { get; set; }
        public Guid CountryGuid { get; set; }
        public Guid BranchGuid { get; set; }
        public Guid ShiftGuid { get; set; }
        public DateTime DateStart { get; set; }
        public string Shift { get; set; }
        public bool Success { get; set; }
        public int Node { get; set; }
        public int Route { get; set; }
        public decimal Distance { get; set; }
        public int DurationTime { get; set; }
        public int TravelTime { get; set; }
        public int WaitTime { get; set; }
        public IEnumerable<RoutesResponse> Routes { get; set; }
        public IEnumerable<UnservedResponse> Unserved { get; set; }
        public string Duration { get; set; }
        public string Travel { get; set; }
        public string Wait { get; set; }
        public Guid RouteOptimizeTaskGuid { get; set; }
        public int Status { get; set; }
        public string Execution { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public Guid LanguagueGuid { get; set; } = ApiSession.UserLanguage_Guid.GetValueOrDefault();

        // new properties unknown errors from NEMO
        // properties used to identify internal server errors in NEMO
        public string Message { get; set; }
        public string Data { get; set; }
        public IEnumerable<string> ErrorDetail { get; set; }
    }

    public class RoutesResponse
    {
        public IEnumerable<string> ServiceTypes { get; set; }
        public IEnumerable<string> ParentNodes { get; set; }
        public IEnumerable<string> Jobs { get; set; }
        public decimal Distance { get; set; }
        public int Count { get; set; }
        public IEnumerable<string> Paths { get; set; }
        public IEnumerable<string> Arrivals { get; set; }
        public IEnumerable<string> Departures { get; set; }
        public IEnumerable<decimal> Distances { get; set; }
        public IEnumerable<string> Travels { get; set; }
        public IEnumerable<string> Waits { get; set; }
        public IEnumerable<decimal> Capacities { get; set; }
        public IEnumerable<decimal> Volumes { get; set; }
        public IEnumerable<decimal> Liabilities { get; set; }
        public IEnumerable<int> ServiceTimes { get; set; }
        public IEnumerable<int> TravelTimes { get; set; }
        public IEnumerable<int> WaitTimes { get; set; }
        public IEnumerable<decimal> ArrivalTimes { get; set; }
        public IEnumerable<decimal> DepartureTimes { get; set; }
        public IEnumerable<string> Services { get; set; }
        public string Vehicle { get; set; }
        public decimal Liability { get; set; }
        public decimal Capacity { get; set; }
        public decimal Volume { get; set; }
        public IEnumerable<string> Latitudes { get; set; }
        public IEnumerable<string> Longitudes { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Duration { get; set; }
        public int DurationTime { get; set; }
        public string Travel { get; set; }
        public int TravelTime { get; set; }
        public string Wait { get; set; }
        public int WaitTime { get; set; }
        public string Description { get; set; }
        public int QuantityStops { get; set; }
    }

    public class UnservedResponse
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string ServiceTypeCode { get; set; }
        public decimal Liability { get; set; }
        public decimal Capacity { get; set; }
        public decimal Volume { get; set; }
    }
}
