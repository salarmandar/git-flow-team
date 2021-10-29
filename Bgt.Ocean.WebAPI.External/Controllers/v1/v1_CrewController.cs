using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.RunControl;
using Bgt.Ocean.Service.Implementations.SFO;
using Bgt.Ocean.Service.Messagings.RunControlService;
using Bgt.Ocean.WebAPI.External.Filters;
using System;
using System.Globalization;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.External.Controllers.v1
{
    [AuthenticationFilter]
    [RoutePrefix("api/v1/crews")]
    public class v1_CrewController : ApiControllerBase
    {
        private readonly ICrewService _crewService;
        private readonly ISystemService _systemService;

        public v1_CrewController(ICrewService crewService, ISystemService systemService)
        {
            _crewService = crewService;
            _systemService = systemService;
        }

        /// <summary>
        /// API to validate identification of crew on portal
        /// </summary>
        [HttpPost]
        [Route("validatecrewonportal")]
        public ValidateCrewOnPortalResponse ValidateCrewOnPortal(ValidateCrewOnPortalRequest request)
        {
            try
            {
                var result = new ValidateCrewOnPortalResponse();

                if (request == null)
                {
                    result.responseCode = "0";
                    result.responseMessage = "Warning => Please check request body";

                    return result;
                }

                if (string.IsNullOrEmpty(request.crewId))
                {
                    result.responseCode = "0";
                    result.responseMessage = "Warning => Crew ID is required";

                    return result;
                }

                if (string.IsNullOrEmpty(request.runDate))
                {
                    result.responseCode = "0";
                    result.responseMessage = "Warning => Run date is required";

                    return result;
                }
                else
                {
                    try
                    {
                        DateTime.ParseExact(request.runDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        result.responseCode = "0";
                        result.responseMessage = "Warning => Run date is invalid format (Required MM/dd/yyyy)";

                        return result;
                    }
                }

                if (string.IsNullOrEmpty(request.siteCode))
                {
                    result.responseCode = "0";
                    result.responseMessage = "Warning => Site code is required";

                    return result;
                }

                result = _crewService.ValidateCrewOnPortal(request);

                if (result.rows == 0)
                {
                    result.responseCode = "0";
                    result.responseMessage = "Invalid Crew ID and list of crew that are on the dispatched run of the day";

                    return result;
                }

                return result;
            }
            catch (Exception ex)
            {
                var result = new ValidateCrewOnPortalResponse
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
