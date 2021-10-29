using Bgt.Ocean.WebAPI.Extends;
using Bgt.Ocean.WebAPI.Filters;
using Bgt.Ocean.WebAPI.Helpers;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers
{
    //[Authorize]
    [TokenFilter]
    [ActionFilter]
    public class ApiControllerBase : ApiController
    {
        protected HttpResponseMessage GetResponseMessageView(SystemMessageView msg, object data = null)
        {
            var response = new HttpSystemMessage(msg);
            response.StatusCode = HttpStatusCode.OK;

            response.Content = new StringContent(data != null ? data?.GetJsonString() : msg.GetJsonString());

            return response;
        }

        /// <summary>
        /// This method will log the exception and returns HttpResponseMessgae with content as its err
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        protected HttpResponseMessage GetErrorResponse(Exception err)
        {
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, err);
        }

        protected HttpResponseMessage GetErrorResponse(HttpStatusCode errorCode, string messageText)
        {
            return Request.CreateErrorResponse(errorCode, messageText);
        }

        protected HttpResponseMessage GetErrorResponse(string messageText)
        {
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, messageText);
        }
    }
}
