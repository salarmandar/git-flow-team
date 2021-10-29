using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.ActualJobHeader
{
    public class CreateJobHeaderOtcRequest
    {
        public Guid CusLocationGuid { get; set; }
        public Guid MasterActualJobHeaderGuid { get; set; }
        public Guid? MaserActualJobLegGuid { get; set; }
        public int ServiceJobId { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DatetimeCreated { get; set; }
        public DateTimeOffset? UniversalDatetimeCreated { get; set; }
        public string OtcBranchName { get; set; }
        public string OtcLockUser { get; set; }
        public string OtcLockMode { get; set; }
    }

    public class OTCCustomerLocationTypeResult
    {
        public Guid Guid { get; set; }
        public int CustomerLocationTypeId { get; set; }
        public string OtcTypeId { get; set; }
    }


    public class CreateJobHeaderOtcMultiCustomerRequest
    {
        public Guid MasterActualJobHeaderGuid { get; set; }
        public int ServiceJobId { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DatetimeCreated { get; set; }
        public DateTimeOffset? UniversalDatetimeCreated { get; set; }
        public string OtcLockUser { get; set; }
        public string OtcLockMode { get; set; }
        public IEnumerable<CreateJobDetailOtcRequest> JobDetail { get; set; }
    }

    public class CreateJobDetailOtcRequest
    {
        public Guid CusLocationGuid { get; set; }
        public Guid? MaserActualJobLegGuid { get; set; }
        public string OtcBranchName { get; set; }
      
    }
}
