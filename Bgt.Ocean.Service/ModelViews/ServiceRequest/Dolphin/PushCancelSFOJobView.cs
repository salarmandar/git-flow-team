using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.ServiceRequest.Dolphin
{
    public class PushCancelSFOJobView : PushSFOJobView
    {
        public IEnumerable<PushCancelSFOJobBodyView> cancelJobs { get; set; }
    }

    public class PushCancelSFOJobBodyView
    {
        public Guid jobGuid { get; set; }
        public string brinksSiteID { get; set; }
        public string reason { get; set; }
    }
}
