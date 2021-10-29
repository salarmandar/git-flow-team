using System;

namespace Bgt.Ocean.Models.OTCManagement
{
   public class GetJobListByRunModel
    {
        public Guid Guid { get; set; }
        public Guid MasterSite_Guid { get; set; }
        public Guid MasterActualJobHeader_Guid { get; set; }
        public string JobNo { get; set; }
        public string LOBAbbrevaitionName { get; set; }
        public string ServiceJobTypeNameAbb { get; set; }
        public string LocationName { get; set; }
    }
}
