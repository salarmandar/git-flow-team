using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.StandardTable;
using Bgt.Ocean.Service.Messagings.StandardTable.MachineSubServiceType;
using Bgt.Ocean.WebAPI.External.Filters;
using System;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.External.Controllers.v1
{
    [AuthenticationFilter]
    [RoutePrefix("api/v1/machinesubservicetypes")]
    public class v1_MachineSubServiceTypeController : ApiControllerBase
    {
        private readonly IMachineSubServiceTypeService _machineSubServiceTypeService;
        private readonly ISystemService _systemService;

        public v1_MachineSubServiceTypeController(IMachineSubServiceTypeService machineSubServiceTypeService, ISystemService systemService)
        {
            _machineSubServiceTypeService = machineSubServiceTypeService;
            _systemService = systemService;
        }

        /// <summary>
        /// API to get Machine Sub Service Type data from Ocean Online
        /// </summary>
        [HttpPost]
        [Route("query")]
        public ResponseQueryMachineSubServiceType Query(RequestQueryMachineSubServiceType request)
        {
            try
            {
                var result = new ResponseQueryMachineSubServiceType();
                var responseValidated = ValidateBaseQueryRequest(request);

                if (responseValidated.responseCode != "1")
                {
                    result.responseCode = responseValidated.responseCode;
                    result.responseMessage = responseValidated.responseMessage;
                    result.rows = 0;

                    return result;
                }

                result = _machineSubServiceTypeService.GetMachineSubServiceTypeList(request);

                return result;
            }
            catch (Exception ex)
            {
                var result = new ResponseQueryMachineSubServiceType
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
