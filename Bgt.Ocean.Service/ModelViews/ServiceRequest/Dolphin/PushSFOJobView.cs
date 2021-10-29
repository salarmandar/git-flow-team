using Bgt.Ocean.Service.Messagings.RunControlService;
using System;

namespace Bgt.Ocean.Service.ModelViews.ServiceRequest.Dolphin
{
    public class PushSFOJobView
    {
        public string userAction
        {
            get; set;
        }

        public string action { get; set; }
        public Guid runDailyGuid { get; set; }
        public Guid[] jobGuid { get; set; }
        public string clientDateTime { get; set; }
        public DolphinAuthen authen { get; set; }
    }
}
