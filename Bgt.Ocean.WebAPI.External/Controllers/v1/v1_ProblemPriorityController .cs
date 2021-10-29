using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.StandardTable;
using Bgt.Ocean.Service.Messagings.StandardTable.ProblemPriority;
using Bgt.Ocean.WebAPI.External.Filters;
using System;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.External.Controllers.v1
{
    [AuthenticationFilter]
    [RoutePrefix("api/v1/problempriorities")]
    public class v1_ProblemPriorityController : ApiControllerBase
    {
        private readonly IProblemPriorityService _problemPriorityService;
        private readonly ISystemService _systemService;

        public v1_ProblemPriorityController(IProblemPriorityService problemPriorityService, ISystemService systemService)
        {
            _problemPriorityService = problemPriorityService;
            _systemService = systemService;
        }

        /// <summary>
        /// API to get Problem Priority data from Ocean Online
        /// </summary>
        [HttpPost]
        [Route("query")]
        public ResponseQueryProblemPriority Query(RequestQueryProblemPriority request)
        {
            try
            {
                var result = new ResponseQueryProblemPriority();
                var responseValidated = ValidateBaseQueryRequest(request);

                if (responseValidated.responseCode != "1")
                {
                    result.responseCode = responseValidated.responseCode;
                    result.responseMessage = responseValidated.responseMessage;
                    result.rows = 0;

                    return result;
                }

                result = _problemPriorityService.GetProblemPriorityList(request);

                return result;
            }
            catch (Exception ex)
            {
                var result = new ResponseQueryProblemPriority
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
