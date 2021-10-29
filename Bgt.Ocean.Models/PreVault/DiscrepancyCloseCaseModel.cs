using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.PreVault
{
    public class DiscrepancyCloseCaseModel
    {
        public List<Guid> DiscrepancyGuidList { get; set; }
        public bool IsFoundItem { get; set; }
        public string Reason { get; set; }
    }

    public class DiscrepancyCloseCaseLogModel
    {
        public string SealNo { get; set; }
        public string JobNo { get; set; }
        public string SiteName { get; set; }
        public string BranchName { get; set; }
        public string InterDepartmentName { get; set; }     
    }
}
