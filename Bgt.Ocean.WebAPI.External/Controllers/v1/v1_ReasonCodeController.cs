using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.StandardTable;
using Bgt.Ocean.Service.Messagings.StandardTable.ReasonCode;
using Bgt.Ocean.WebAPI.External.Filters;
using System;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.External.Controllers.v1
{
    [AuthenticationFilter]
    [RoutePrefix("api/v1/reasoncodes")]
    public class v1_ReasonCodeController : ApiControllerBase
    {
        private readonly IReasonCodeService _reasonCodeService;
        private readonly ISystemService _systemService;

        public v1_ReasonCodeController(IReasonCodeService reasonCodeService, ISystemService systemService)
        {
            _reasonCodeService = reasonCodeService;
            _systemService = systemService;
        }

        /// <summary>
        /// API to get Reason Code data from Ocean Online
        /// </summary>
        [HttpPost]
        [Route("query")]
        public ResponseQueryReasonCode Query(RequestQueryReasonCode request)
        {
            try
            {
                var result = new ResponseQueryReasonCode();
                var responseValidated = ValidateBaseQueryRequest(request);

                if (responseValidated.responseCode != "1")
                {
                    result.responseCode = responseValidated.responseCode;
                    result.responseMessage = responseValidated.responseMessage;
                    result.rows = 0;

                    return result;
                }

                result = _reasonCodeService.GetReasonCodeList(request);

                return result;
            }
            catch (Exception ex)
            {
                var result = new ResponseQueryReasonCode
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
