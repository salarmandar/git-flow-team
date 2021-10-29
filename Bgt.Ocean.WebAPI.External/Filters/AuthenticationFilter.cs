using System;
using System.Net;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web.Configuration;
using System.Text;

namespace Bgt.Ocean.WebAPI.External.Filters
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                var authToken = actionContext.Request.Headers.Authorization.Parameter;
                var decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(authToken));
                var apiToken = string.Concat(decodedToken.Substring(0, decodedToken.IndexOf(":")).ToLower(), decodedToken.Substring(decodedToken.IndexOf(":") + 1));
                var apiAuthen = string.Concat(WebConfigurationManager.AppSettings["Username"].ToLower(), WebConfigurationManager.AppSettings["Password"]);

                if (apiToken.Equals(apiAuthen) == false)
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, $"Invalid username and password");
                }
            }
            catch
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, $"Unauthorized");
            }
        }
    }
}