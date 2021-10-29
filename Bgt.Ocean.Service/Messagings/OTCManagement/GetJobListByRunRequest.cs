using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.OTCManagement
{
  public  class GetJobListByRunRequest
    {
        public Guid? LegGuid { get; set; }
        public Guid Guid { get; set; }
        public Guid MasterSite_Guid { get; set; }
        public Guid MasterActualJobHeader_Guid { get; set; }
        public string JobNo { get; set; }
        public string LOBAbbrevaitionName { get; set; }
        public string ServiceJobTypeNameAbb { get; set; }
        public string LocationName { get; set; }
    }
    public class JobListByRunRequest {
        public string DateFormat { get; set; }
        public IEnumerable<GetJobListByRunRequest> JobListRequest {get;set;}
    }
}
