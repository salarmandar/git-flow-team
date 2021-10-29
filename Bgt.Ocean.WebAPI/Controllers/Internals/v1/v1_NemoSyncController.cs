using Bgt.Ocean.Service.Implementations.Nemo.NemoSync;
using System.Web.Mvc;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_NemoSyncController : ApiControllerBase
    {
        #region Objects & Variables
        private readonly INemoSyncService _nemoSyncService;
        #endregion

        #region Constuctor
        public v1_NemoSyncController(INemoSyncService nemoSyncService)
        {
            _nemoSyncService = nemoSyncService;
        }
        #endregion

        [HttpPost]
        public bool Index()
        {
            return _nemoSyncService.NemoSyncStart();
        }
    }
}