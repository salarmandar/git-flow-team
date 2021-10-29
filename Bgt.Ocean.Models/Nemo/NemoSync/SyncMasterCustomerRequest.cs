using Newtonsoft.Json;
using System;

namespace Bgt.Ocean.Models.Nemo.NemoSync
{
    public class SyncMasterCustomerRequest
    {
        [JsonIgnore]
        public Guid MasterSiteGuid { get; set; }

        [JsonIgnore]
        public Guid CustomerGuid { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "branchCode")]
        public string BranchCode { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

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

        [JsonProperty(PropertyName = "contact")]
        public string Contact { get; set; }

        [JsonProperty(PropertyName = "telephone")]
        public string Telephone { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }
    }
}
