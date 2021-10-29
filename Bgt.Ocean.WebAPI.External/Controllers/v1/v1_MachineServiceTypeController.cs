using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.StandardTable;
using Bgt.Ocean.Service.Messagings.StandardTable.MachineServiceType;
using Bgt.Ocean.WebAPI.External.Filters;
using System;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.External.Controllers.v1
{
    [AuthenticationFilter]
    [RoutePrefix("api/v1/machineservicetypes")]
    public class v1_MachineServiceTypeController : ApiControllerBase
    {
        private readonly IMachineServiceTypeService _machineServiceTypeService;
        private readonly ISystemService _systemService;

        public v1_MachineServiceTypeController(IMachineServiceTypeService machineServiceTypeService, ISystemService systemService)
        {
            _machineServiceTypeService = machineServiceTypeService;
            _systemService = systemService;
        }

        /// <summary>
        /// API to get Machine Service Type data from Ocean Online
        /// </summary>
        [HttpPost]
        [Route("query")]
        public ResponseQueryMachineServiceType Query(RequestQueryMachineServiceType request)
        {
            try
            {
                var result = new ResponseQueryMachineServiceType();
                var responseValidated = ValidateBaseQueryRequest(request);

                if (responseValidated.responseCode != "1")
                {
                    result.responseCode = responseValidated.responseCode;
                    result.responseMessage = responseValidated.responseMessage;
                    result.rows = 0;

                    return result;
                }

                result = _machineServiceTypeService.GetMachineServiceTypeList(request);

                return result;
            }
            catch (Exception ex)
            {
                var result = new ResponseQueryMachineServiceType
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
