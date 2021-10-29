using Newtonsoft.Json;
using System;

namespace Bgt.Ocean.Models.Nemo.NemoSync
{
    public class SyncSystemServiceJobTypeRequest
    {
        [JsonIgnore]
        public Guid SystemServiceJobTypeGuid { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "Code")]
        public string Code { get; set; }

        public bool IsRepeat { get; set; }
    }
}
