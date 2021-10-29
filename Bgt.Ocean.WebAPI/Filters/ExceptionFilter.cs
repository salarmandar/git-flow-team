using Bgt.Ocean.Service.Implementations;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Bgt.Ocean.WebAPI.Filters
{
    public class ExceptionHandlerFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is UnauthorizedAccessException)
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }    
            else if (actionExecutedContext.Exception.Message == "The operation was canceled.")
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.Gone);
            }
            else
            {
                var logError = (ISystemService)actionExecutedContext.Request.GetDependencyScope().GetService(typeof(ISystemService));                
                logError.CreateHistoryError(actionExecutedContext.Exception);

#if DEBUG
#else
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
#endif
            }
        }
    }
}
