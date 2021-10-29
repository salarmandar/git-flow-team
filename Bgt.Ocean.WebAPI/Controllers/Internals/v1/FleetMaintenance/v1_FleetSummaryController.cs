using Bgt.Ocean.Service.Implementations.FleetMaintenance;
using Bgt.Ocean.Service.Messagings.FleetMaintenance;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_FleetSummaryController : ApiControllerBase
    {
        private readonly IFleetSummaryService _fleetSummaryService;

        public v1_FleetSummaryController(IFleetSummaryService fleetSummaryService)
        {
            _fleetSummaryService = fleetSummaryService;
        }
  
        [HttpPost]
        public FleetSummaryResponse GetSummaryByOption(FleetSummaryRequest req)
        {
            return _fleetSummaryService.GetSummaryByOption(req);
        }
    }
}