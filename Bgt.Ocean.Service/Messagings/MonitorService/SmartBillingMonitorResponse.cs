using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Email;
using Bgt.Ocean.Service.ModelViews.Monitoring;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.Messagings.MonitorService
{
    public class SmartBillingMonitorResponse
    {
        public IEnumerable<SmartBillingGenerateStatusView> GenerateStatusList { get; set; }
        public int Total { get; set; }
    }

    public class SmartBillingConfigResponse : SmartBillingConfigView
    {
        public Guid ConfigGuid { get; set; }
        public string ScheduleTime { get; set; }
    }

    public class SmartBillingSubmitResponse
    {
        public SystemMessageView MessageVeiw { get; set; }
        public Guid ScheduleSiteGuid { get; set; } 
    }
}
