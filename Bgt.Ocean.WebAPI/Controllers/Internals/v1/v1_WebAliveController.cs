using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.WebAPI.Filters;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers
{
    [ActionFilter]
    public class v1_WebAliveController : ApiController
    {
        #region Objects & Variables
        private readonly ISystemService _systemService;
        #endregion

        public v1_WebAliveController(ISystemService systemService)
        {
            _systemService = systemService;
        }

        /// <summary>
        /// Check Application Status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Status()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { API = true, DB = _systemService.CheckExistData() }, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { API = true, DB = false, Message = ex }, Configuration.Formatters.JsonFormatter);
            }

        }
    }
}