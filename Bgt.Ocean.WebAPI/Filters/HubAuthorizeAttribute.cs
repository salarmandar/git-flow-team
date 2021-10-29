using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Service.Implementations;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Security.Principal;

namespace Bgt.Ocean.WebAPI.Filters
{

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class HubAuthorizeAttribute : AuthorizeAttribute
    {
        public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
        {
            IUserService userService = (IUserService)System.Web.Http.GlobalConfiguration
                .Configuration.DependencyResolver.GetService(typeof(IUserService));

            var token = request.QueryString["auth_key"];
            var appKey = request.QueryString["app_key"];

            if (token.IsEmpty() || appKey.IsEmpty())
                return false;            

            var isValidToken = userService.VerifyTokenKey(token.ToGuid()) && !userService.ExpiredTokenKey(token.ToGuid());
            var isValidAppKey = userService.RetreiveAccessToken(appKey.ToGuid()) != null;

            if (!isValidToken || !isValidAppKey)
                return false;

            return true;
        }

        protected override bool UserAuthorized(IPrincipal user)
        {
            return true;
        }


    }
}
