using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Service.Implementations;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Bgt.Ocean.WebAPI.External.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is UnauthorizedAccessException)
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            else
            {
                ISystemService logError = (ISystemService)System.Web.Http.GlobalConfiguration
                   .Configuration.DependencyResolver.GetService(typeof(ISystemService));

                logError.CreateHistoryError(actionExecutedContext.Exception);
            }
        }
    }
}
