using Bgt.Ocean.Service.Implementations.MasterRoute;
using Bgt.Ocean.Service.Messagings.MasterRouteService;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_ManageMasterJobController : ApiControllerBase
    {
        private readonly IManageMasterJobService _manageMasterJobService;
        public v1_ManageMasterJobController(IManageMasterJobService manageMasterJobService) {
            _manageMasterJobService = manageMasterJobService;
        }

        [HttpPost]
        public GetMasterRouteDeliveryLegResponse GetMasterRouteDeliveryLeg(GetMasterRouteDeliveryLegRequest req) {

            return _manageMasterJobService.GetMasterRouteDeliveryLeg(req);
        }

        [HttpGet]
        public void GetRouteGroupDetailByMasterRoute() {

        }

        [HttpGet]
        public void GetDisplayMasterJobs()
        {

        }
    }
}