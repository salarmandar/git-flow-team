using Bgt.Ocean.Service.ModelViews.Systems;
using System;

namespace Bgt.Ocean.Service.Messagings.StandardTable.MonitoringNetwork
{
    public class ResponseCreateMonitoringNetwork
    {
        public SystemMessageView systemMessageView { get; set; }
        public Guid monitoringNetwork_Guid { get; set; }
    }
}
