using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.StandardTable;
using Bgt.Ocean.Service.Messagings.StandardTable.Problem;
using Bgt.Ocean.WebAPI.External.Filters;
using System;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.External.Controllers.v1
{
    [AuthenticationFilter]
    [RoutePrefix("api/v1/problems")]
    public class v1_ProblemController : ApiControllerBase
    {
        private readonly IProblemService _problemService;
        private readonly ISystemService _systemService;

        public v1_ProblemController(IProblemService problemService, ISystemService systemService)
        {
            _problemService = problemService;
            _systemService = systemService;
        }

        /// <summary>
        /// API to get Problem data from Ocean Online
        /// </summary>
        [HttpPost]
        [Route("query")]
        public ResponseQueryProblem Query(RequestQueryProblem request)
        {
            try
            {
                var result = new ResponseQueryProblem();
                var responseValidated = ValidateBaseQueryRequest(request);

                if (responseValidated.responseCode != "1")
                {
                    result.responseCode = responseValidated.responseCode;
                    result.responseMessage = responseValidated.responseMessage;
                    result.rows = 0;

                    return result;
                }

                result = _problemService.GetProblemList(request);

                return result;
            }
            catch (Exception ex)
            {
                var result = new ResponseQueryProblem
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
