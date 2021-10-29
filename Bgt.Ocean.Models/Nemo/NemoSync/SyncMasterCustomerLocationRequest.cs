using Newtonsoft.Json;
using System;

namespace Bgt.Ocean.Models.Nemo.NemoSync
{
    public class SyncMasterCustomerLocationRequest
    {
        [JsonIgnore]
        public Guid MasterCustomerLocationGuid_Sync { get; set; }

        [JsonIgnore]
        public Guid MasterSiteGuid { get; set; }

        [JsonIgnore]
        public Guid CustomerGuid { get; set; }

        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "branchCode")]
        public string BranchCode { get; set; }

        [JsonProperty(PropertyName = "customerCode")]
        public string CustomerCode { get; set; }

        [JsonProperty(PropertyName = "serviceTypeCode")]
        public string ServiceTypeCode { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public string Longitude { get; set; }

        [JsonProperty(PropertyName = "serviceStart")]
        public string ServiceStart { get { return ServiceStartDate.ToString("HH:mm:ss.0000000"); } }

        [JsonProperty(PropertyName = "serviceEnd")]
        public string ServiceEnd { get { return ServiceEndDate.ToString("HH:mm:ss.0000000"); } }

        [JsonProperty(PropertyName = "serviceStart2")]
        public string ServiceStart2 { get { return ServiceStartDate.ToString("HH:mm:ss.0000000"); } }

        [JsonProperty(PropertyName = "serviceEnd2")]
        public string ServiceEnd2 { get { return ServiceEndDate.ToString("HH:mm:ss.0000000"); } }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "address2")]
        public string Address2 { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "postcode")]
        public string Postcode { get; set; }

        [JsonProperty(PropertyName = "waitTime")]
        public string WaitTime { get; set; }

        [JsonProperty(PropertyName = "serviceTime")]
        public int ServiceTime { get; set; }

        public DateTime ServiceStartDate { get; set; }
        public DateTime ServiceEndDate { get; set; }
    }
}
