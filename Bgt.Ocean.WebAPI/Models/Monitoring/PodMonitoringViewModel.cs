using Bgt.Ocean.Service.Messagings.MonitorService;
using System.Collections.Generic;

namespace Bgt.Ocean.WebAPI.Models.Monitoring
{
    public class PodMonitoringViewModel
    {
        public IEnumerable<PodMonitorResponse> Data { get; set; }
        public int Total { get; set; }
    }
}