using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class NemoOptimizationRequest
    {
        [JsonProperty("Token")]
        public Guid? Token { get; set; }

        [JsonProperty("Depot")]
        public string Depot { get; set; }

        [JsonProperty("Criteria")]
        public NemoCriteriaRequest Criteria { get; set; }

        [JsonProperty("BranchCode")]
        public string BranchCode { get; set; }

        [JsonProperty("BranchGuid")]
        public Guid BranchGuid { get; set; }

        [JsonProperty("CountryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("CountryGuid")]
        public Guid CountryGuid { get; set; }

        [JsonProperty("ShiftGuid")]
        public Guid ShiftGuid { get; set; }

        [JsonProperty("Shift")]
        public string Shift { get; set; }

        [JsonProperty("ShiftServiceStart")]
        public string ShiftServiceStart { get; set; }

        [JsonProperty("ShiftServiceEnd")]
        public string ShiftServiceEnd { get; set; }

        [JsonProperty("DateStart")]
        public DateTime DateStart { get; set; }

        [JsonProperty("TimeZone")]
        public string TimeZone { get; set; }

        [JsonProperty("RouteOptimizeType")]
        public int RouteOptimizeType { get; set; }

        [JsonProperty("Nodes")]
        public string[] Nodes { get; set; }

        [JsonProperty("Routes")]
        public List<string[]> Routes { get; set; }

        [JsonProperty("ServiceTypes")]
        public string[] ServiceTypes { get; set; }

        [JsonProperty("ParentNodes")]
        public string[] ParentNodes { get; set; }

        [JsonProperty("Unassigned")]
        public string[] Unassigned { get; set; }

        [JsonProperty("Volumes")]
        public decimal[] Volumes { get; set; }

        [JsonProperty("Capacities")]
        public decimal[] Capacities { get; set; }

        [JsonProperty("Liabilities")]
        public decimal[] Liabilities { get; set; }

        [JsonProperty("Orders")]
        public int[] Orders { get; set; }

        [JsonProperty("Jobs")]
        public string[] Jobs { get; set; }

        [JsonProperty("ParentJobs")]
        public string[] ParentJobs { get; set; }

        [JsonProperty("TimeSchedules")]
        public string[] TimeSchedules { get; set; }

        [JsonProperty("DateSchedules")]
        public string[] DateSchedules { get; set; }

        [JsonProperty("ReferenceURL")]
        public string ReferenceURL { get; set; }
    }
}
