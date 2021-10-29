using Newtonsoft.Json;
using System;

namespace Bgt.Ocean.Models.Nemo.NemoSync
{
    public class SyncMasterSiteRequest
    {
        [JsonIgnore]
        public Guid MasterSiteSync_Guid { get; set; }

        [JsonIgnore]
        public Guid? MasterCustomerLocation_Guid { get; set; }

        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "branchName")]
        public string BranchName { get; set; }

        [JsonProperty(PropertyName = "branchCode")]
        public string BranchCode { get; set; }

        [JsonProperty(PropertyName = "timeZoneID")]
        public string TimeZoneID { get; set; }
    }
}
