using Bgt.Ocean.Service.Messagings.MonitorService;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.Monitoring
{
    public class PodMonitoringView
    {
        public IEnumerable<PodMonitorResponse> Data { get; set; }
        public int Total { get; set; }
    }
}
