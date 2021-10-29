using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.SFO;
using Bgt.Ocean.Services.Messagings.StandardTable.Machine;
using Bgt.Ocean.WebAPI.External.Filters;
using System;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.External.Controllers.v1
{
    [AuthenticationFilter]
    [RoutePrefix("api/v1/machines")]
    public class v1_MachineController : ApiControllerBase
    {
        private readonly IMachineService _machineService;
        private readonly ISystemService _systemService;

        public v1_MachineController(IMachineService machineService, ISystemService systemService)
        {
            _machineService = machineService;
            _systemService = systemService;
        }

        /// <summary>
        /// API to get Machine data from Ocean Online
        /// </summary>
        [HttpPost]
        [Route("query")]
        public ResponseQueryMachine Query(RequestQueryMachine request)
        {
            try
            {
                var result = new ResponseQueryMachine();
                var responseValidated = ValidateBaseQueryRequest(request);

                if (responseValidated.responseCode != "1")
                {
                    result.responseCode = responseValidated.responseCode;
                    result.responseMessage = responseValidated.responseMessage;
                    result.rows = 0;

                    return result;
                }

                result = _machineService.GetMachineList(request);

                return result;
            }
            catch (Exception ex)
            {
                var result = new ResponseQueryMachine
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
