using Bgt.Ocean.WebAPI.Filters;
using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;

namespace Bgt.Ocean.WebAPI.Hubs
{
    [HubAuthorize]
    public class RouteOptimizationHub : Hub
    {
        public RouteOptimizationHub()
        {

        }

        public void RegisterRouteOptimization(Guid requestGuid)
        {
            Groups.Add(Context.ConnectionId, requestGuid.ToString().ToLower());
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var routeRequest = new RouteRequest(Context);

            Groups.Remove(Context.ConnectionId, routeRequest.RequestGuid);
            return base.OnDisconnected(stopCalled);
        }

        #region Util Class

        private class RouteRequest
        {
            public string RequestGuid { get; }
            public RouteRequest(HubCallerContext context)
            {
                RequestGuid = context.QueryString["userGuid"]?.ToLower();
            }
        }

        #endregion
    }
}