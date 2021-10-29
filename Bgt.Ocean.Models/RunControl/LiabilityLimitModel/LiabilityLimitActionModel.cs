
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.RunControl.LiabilityLimitModel
{
    public class ConvertBankCleanOutLiabilityModel
    {
        public Guid? SiteGuid { get; set; }
        public IEnumerable<BankCleanOutJobView> RawJobs { get; set; }
    }

    public class LiabilityLimitRunsActionModel
    {
        public Guid? SiteGuid { get; set; }
        public IEnumerable<Guid?> DailyRunGuids { get; set; }
    }
    public class LiabilityLimitJobsActionModel
    {
        public Guid? SiteGuid { get; set; }
        public IEnumerable<LiabilityLimitJobsAction> RequestList { get; set; }
    }
    public class LiabilityLimitItemsActionModel
    {
        public Guid? SiteGuid { get; set; }
        public IEnumerable<LiabilityLimitItemsAction> RequestList { get; set; }
    }

    public class LiabilityLimitNoJobsActionModel
    {
        public Guid? SiteGuid { get; set; }
        public IEnumerable<LiabilityLimitNoJobsAction> RequestList { get; set; }
    }

}
