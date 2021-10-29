using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.Hubs;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.RouteOptimizationHub;
using System;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_RouteOptimizationHubController : ApiControllerBase
    {
        private readonly IRouteOptimizationBroadcastService _routeOptBroadcastService;
        private readonly ISystemService _systemService;

        public v1_RouteOptimizationHubController(
                IRouteOptimizationBroadcastService routeOptBroadcastService,
                ISystemService systemService
            )
        {
            _routeOptBroadcastService = routeOptBroadcastService;
            _systemService = systemService;
        }

        [HttpPost]
        public BaseResponse NotifyRouteOptimizationStatus(RouteOptimizationHubNotifyRequest request)
        {
            try
            {
                _routeOptBroadcastService.NotifyRouteOptimizationStatus(request.RequestGuid, request.NotifyStatusModel);
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "Notify Route Optimization Success",
                    MsgID = 1,
                    Title = "Route Optimization hub"
                };

            }
            catch (Exception err)
            {
                _systemService.CreateHistoryError(err);
                return new BaseResponse
                {
                     IsSuccess = false,
                     Message = err.Message,
                     MsgID = -1,
                    Title = "Route Optimization hub"
                };
            }

        }
    }
}