using Microsoft.AspNet.SignalR.Hubs;
using System;

namespace Bgt.Ocean.Service.Implementations.Hubs
{
    #region Interface

    public interface IRouteOptimizationBroadcastService
    {
        void NotifyRouteOptimizationStatus(Guid requestGuid, object routeOptimizationStatusModel);
    }

    #endregion

    public class RouteOptimizationBroadcastService : IRouteOptimizationBroadcastService
    {
        private readonly IHubConnectionContext<dynamic> _clients;

        public RouteOptimizationBroadcastService(
                IHubConnectionContext<dynamic> clients
            )
        {
            _clients = clients;
        }

        public void NotifyRouteOptimizationStatus(Guid requestGuid, object routeOptimizationStatusModel)
        {
            _clients.Group(requestGuid.ToString().ToLower()).onRouteOptimizationUpdate(routeOptimizationStatusModel);
        }
    }
}
