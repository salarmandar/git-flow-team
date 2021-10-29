using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Service.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Bgt.Ocean.RunControl.WebAPI.Filters
{
    public sealed class TokenAuthorize : System.Web.Http.AuthorizeAttribute
    {
        public TokenAuthorize()
        { }

        protected override bool IsAuthorized(HttpActionContext httpContext)
        {
            IUserService userService = (IUserService)System.Web.Http.GlobalConfiguration
                .Configuration.DependencyResolver.GetService(typeof(IUserService));

            TokenBox tokenInHeaders = GetAppKeyFromHeaders(httpContext);
            if (tokenInHeaders == null || !tokenInHeaders.App_key.HasValue)
                return false;

            // check from token (internal)
            var app = userService.RetreiveAccessToken(tokenInHeaders.App_key.Value);
            if (app == null)
                return false;

            if (app.TokenExpireDate < DateTime.Now)
            {
                HttpResponseMessage response = httpContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "App key has expired.");
                throw new System.Web.Http.HttpResponseException(response);
            }
                        
            ApiSession.ApplicationNumber = app.ApplicationID;
            ApiSession.Application_Token = app.TokenID.ToString();
            ApiSession.Application_Guid = app.Guid;
            return app.TokenID.HasValue;
        }

        private static TokenBox GetAppKeyFromHeaders(HttpActionContext actionContext)
        {
            TokenBox tokenBox = new TokenBox();
            IEnumerable<string> appValues = null;
            actionContext.Request.Headers.TryGetValues("app_key", out appValues);

            if (!SystemHelper.VerifyNullHeaderKey(appValues))
            {
                tokenBox.App_key = new Guid(appValues.First());
            }

            return tokenBox;
        }

        private class TokenBox
        {
            public Guid? App_key { get; set; }
        }
    }
}
