using System;

namespace Bgt.Ocean.Models.OTCManagement
{
    public class OTCStatusResult
    {
        public Guid Guid { get; set; }
        public string OTCStatusCode { get; set; }
        public string OTCStatusName { get; set; }
        public Guid? MasterActualJobHeader_Guid { get; set; }
    }
}
