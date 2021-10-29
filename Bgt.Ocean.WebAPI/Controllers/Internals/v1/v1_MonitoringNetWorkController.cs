using Bgt.Ocean.Models.StandardTable;
using Bgt.Ocean.Service.Implementations.StandardTable;
using Bgt.Ocean.Service.Messagings.StandardTable.MonitoringNetwork;
using Bgt.Ocean.Service.ModelViews.Systems;
using System.Collections.Generic;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_MonitoringNetWorkController : ApiControllerBase
    {
        private readonly IMoniteringNetWorkService _masterMoniteringNetWorkService;
        public v1_MonitoringNetWorkController(
            IMoniteringNetWorkService masterMoniteringNetWorkService
          )
        {
            _masterMoniteringNetWorkService = masterMoniteringNetWorkService;
        }

        [HttpPost]
        public IEnumerable<MasterMonitoringNetworkView> GetMasterMonitoringNetWorkList(GetMonitoringNetworkRequest request)
        {
            var result = _masterMoniteringNetWorkService.GetMasterMonitoringNetWorkList(request);
            return result;
        }

        [HttpPost]
        public ResponseCreateMonitoringNetwork CreateMonitoringNetwork(MasterMonitoringNetworkView request)
        {
            return _masterMoniteringNetWorkService.CreateMonitoringNetwork(request);
        }

        [HttpPost]
        public ResponseCreateMonitoringNetwork UpdateMonitoringNetwork(MasterMonitoringNetworkView request)
        {
            return _masterMoniteringNetWorkService.UpdateMonitoringNetwork(request);
        }

        [HttpPost]
        public SystemMessageView EnableAndDisableMonitoringNetwork(EnableDisableNetworkRequest request)
        {
            return _masterMoniteringNetWorkService.EnableAndDisableMonitoringNetwork(request);
        }

    }
}