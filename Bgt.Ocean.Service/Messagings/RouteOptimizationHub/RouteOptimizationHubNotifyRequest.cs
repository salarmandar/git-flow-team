using System;

namespace Bgt.Ocean.Service.Messagings.RouteOptimizationHub
{
    public class RouteOptimizationHubNotifyRequest
    {
        public Guid RequestGuid { get; set; }
        public object NotifyStatusModel { get; set; }
    }
}
