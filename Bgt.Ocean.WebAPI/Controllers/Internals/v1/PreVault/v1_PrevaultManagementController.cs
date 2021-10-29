using Bgt.Ocean.Service.Implementations.PreVault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_PrevaultManagementController : ApiControllerBase
    {
        private readonly IPrevaultManagementService _prevaultManagementService;

        #region #### DEPENDENCY INJECTION ####
        public v1_PrevaultManagementController(
                IPrevaultManagementService prevaultManagementService
        )
        {
            _prevaultManagementService = prevaultManagementService;
        }

        #endregion

        // GET: PrevaultManagement
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetPrevaultBySite(Guid siteGuid)
        {
            var resp = _prevaultManagementService.GetPrevaultBySite(siteGuid);
            return Request.CreateResponse(HttpStatusCode.OK,resp);
        }

    }
}