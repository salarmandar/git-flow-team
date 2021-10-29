using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.StandardTable;
using Bgt.Ocean.Service.Messagings.StandardTable.RouteGroup;
using Bgt.Ocean.WebAPI.External.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.External.Controllers.v1
{
    [AuthenticationFilter]
    [RoutePrefix("api/v1/routegroups")]
    public class v1_RouteGroupController : ApiControllerBase
    {
        private readonly IRouteGroupService _routeGroupService;
        private readonly ISystemService _systemService;


        public v1_RouteGroupController(IRouteGroupService routeGroupService, ISystemService systemService)
        {
            _routeGroupService = routeGroupService;
            _systemService = systemService;
        }

        /// <summary>
        /// API to get Route Group data from Ocean Online
        /// </summary>
        [HttpPost]
        [Route("query")]
        public ResponseQueryRouteGroup Query(RequestQueryRouteGroup request)
        {
            try
            {
                var result = new ResponseQueryRouteGroup();
                var responseValidated = ValidateBaseQueryRequest(request);

                if (responseValidated.responseCode != "1")
                {
                    result.responseCode = responseValidated.responseCode;
                    result.responseMessage = responseValidated.responseMessage;
                    result.rows = 0;

                    return result;
                }

                result = _routeGroupService.GetRouteGroupList(request);

                return result;
            }
            catch (Exception ex)
            {
                var result = new ResponseQueryRouteGroup
                {
                    responseCode = "-1",
                    responseMessage = "Error => Please contact administrator",
                    rows = 0
                };

                _systemService.CreateHistoryError(ex);

                return result;
            }
        }

   
    }
}
