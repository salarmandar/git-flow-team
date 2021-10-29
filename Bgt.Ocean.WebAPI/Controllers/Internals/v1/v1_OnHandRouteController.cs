using Bgt.Ocean.Service.Implementations.MasterRoute;
using Bgt.Ocean.Service.Messagings.MasterRouteService;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_OnHandRouteController : ApiControllerBase
    {
        private readonly IOnHandRouteService _onHandRouteService;

        public v1_OnHandRouteController(IOnHandRouteService onHandRouteService)
        {
            _onHandRouteService = onHandRouteService;
        }


        #region GET
        [HttpPost]
        public OnHandRouteResponse GetOnHandRouteDetail(OnHandMasterRouteRequest request)
        {
            return _onHandRouteService.GetDetailJob(request);
        }

        [HttpPost]
        public OnHandRouteSummaryResponse GetOnHandSummary(OnHandMasterRouteRequest request)
        {
            return _onHandRouteService.GetOnHandRouteSummary(request);
        }
        #endregion
    }
}