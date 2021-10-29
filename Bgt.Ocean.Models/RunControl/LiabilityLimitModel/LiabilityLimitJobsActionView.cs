using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.RunControl.LiabilityLimitModel
{
    public class LiabilityLimitJobsAction
    {
        public Guid? DailyRunGuid_Target { get; set; }
        public Guid? DailyRunGuid_Source { get; set; }
        public IEnumerable<RawExistJobView> JobGuids { get; set; } = new List<RawExistJobView>();
    }
    public class LiabilityLimitItemsAction
    {
        public Guid? DailyRunGuid_Target { get; set; }
        public RawItemsView JobItems { get; set; } = new RawItemsView();
    }

    public class LiabilityLimitNoJobsAction
    {
        public Guid? DailyRunGuid_Target { get; set; }
        public RawJobDataView JobData { get; set; } = new RawJobDataView();
    }
}
