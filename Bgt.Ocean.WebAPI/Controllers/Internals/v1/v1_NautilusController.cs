using Bgt.Ocean.Service.Implementations;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_NautilusController : ApiControllerBase
    {
        private readonly IBrinksService _brinksService;

        public v1_NautilusController(IBrinksService brinksService)
        {            
            _brinksService = brinksService;
        }

        /// <summary>
        /// Get currency by country
        /// </summary>
        /// <param name="countryGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCurrencyListByCountry(Guid countryGuid)
        {
            var result = _brinksService.Nautilus_GetCurrencyDependOnCountry(countryGuid);
            if (result != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }

    }
}