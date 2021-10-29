using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.StandardTable;
using Bgt.Ocean.Service.Messagings.StandardTable.RunResourceType;
using Bgt.Ocean.WebAPI.External.Filters;
using System;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.External.Controllers.v1
{
    [AuthenticationFilter]
    [RoutePrefix("api/v1/runresourcetypes")]
    public class v1_RunResourceTypeController : ApiControllerBase
    {
        private readonly IRunResourceTypeService _runResourceTypeService;
        private readonly ISystemService _systemService;

        public v1_RunResourceTypeController(IRunResourceTypeService runResourceTypeService, ISystemService systemService)
        {
            _runResourceTypeService = runResourceTypeService;
            _systemService = systemService;
        }

        /// <summary>
        /// API to get Run Resource Type data from Ocean Online
        /// </summary>
        [HttpPost]
        [Route("query")]
        public ResponseQueryRunResourceType Query(RequestQueryRunResourceType request)
        {
            try
            {
                var result = new ResponseQueryRunResourceType();
                var responseValidated = ValidateBaseQueryRequest(request);

                if (responseValidated.responseCode != "1")
                {
                    result.responseCode = responseValidated.responseCode;
                    result.responseMessage = responseValidated.responseMessage;
                    result.rows = 0;

                    return result;
                }

                result = _runResourceTypeService.GetRunResourceTypeList(request);

                return result;
            }
            catch (Exception ex)
            {
                var result = new ResponseQueryRunResourceType
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
