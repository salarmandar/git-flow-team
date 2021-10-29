using Bgt.Ocean.WebAPI.Filters;
using Bgt.Ocean.WebAPI.App_Start;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Net.Http.Headers;

namespace Bgt.Ocean.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.MessageHandlers.Add(new MethodOverrideHandler());
            config.MessageHandlers.Add(new LogRequestAndResponseHandler());
            config.Services.Replace(typeof(IHttpControllerSelector), new ApiVersioningSelector(config));
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Re‌​ferenceLoopHandling = ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new DecimalConverter());
            config.Filters.Add(new ExceptionHandlerFilterAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Custom route
            Routing_Account(config);
            Routing_Report(config);
            Routing_WebAlive(config);

            // Default routes
            config.Routes.MapHttpRoute(
                name: "InternalDefaultAPI",
                routeTemplate: "api/{version}/{access}/{controller}/{action}",
                defaults: new { controller = "{version}_{controller}", id = RouteParameter.Optional }
            );
        }

        #region Custom route
        public static void Routing_Account(HttpConfiguration config)
        {
            #region v1
            config.Routes.MapHttpRoute(
               name: "v1_account_verifyEmail",
               routeTemplate: "api/v1/Account/verifyEmail",
               defaults: new { controller = "v1_Account", action = "verifyEmail" }
            );

            config.Routes.MapHttpRoute(
               name: "v1_account_register",
               routeTemplate: "api/v1/Account/register",
               defaults: new { controller = "v1_Account", action = "register" }
            );

            config.Routes.MapHttpRoute(
                name: "v1_account_register_old",
                routeTemplate: "api/v1/register",
                defaults: new { controller = "v1_Account", action = "register" }
            );

            config.Routes.MapHttpRoute(
               name: "v1_account_getUserDomainData",
               routeTemplate: "api/v1/Account/getUserDomainData",
               defaults: new { controller = "v1_Account", action = "getUserDomainData" }
            );

            config.Routes.MapHttpRoute(
               name: "v1_account_changePassword",
               routeTemplate: "api/v1/Account/changePassword",
               defaults: new { controller = "v1_Account", action = "changePassword" }
            );

            config.Routes.MapHttpRoute(
               name: "v1_account_resetPassword",
               routeTemplate: "api/v1/Account/resetPassword",
               defaults: new { controller = "v1_Account", action = "resetPassword" }
            );

            config.Routes.MapHttpRoute(
               name: "v1_account_forgotPassword",
               routeTemplate: "api/v1/Account/forgotPassword",
               defaults: new { controller = "v1_Account", action = "forgotPassword" }
            );
            #endregion
            #region v2
            config.Routes.MapHttpRoute(
               name: "v2_account_verifyTokenKey",
               routeTemplate: "api/v2/Account/verifyTokenKey",
               defaults: new { controller = "v2_Account", action = "verifyTokenKey" }
            );

            config.Routes.MapHttpRoute(
               name: "v2_account_register",
               routeTemplate: "api/v2/Account/register",
               defaults: new { controller = "v2_Account", action = "register" }
            );

            config.Routes.MapHttpRoute(
               name: "v2_account_expiredTokenKey",
               routeTemplate: "api/v2/Account/expiredTokenKey",
               defaults: new { controller = "v2_Account", action = "expiredTokenKey" }
            );

            config.Routes.MapHttpRoute(
               name: "v2_account_changelanguage",
               routeTemplate: "api/v2/Account/changelanguage",
               defaults: new { controller = "v2_Account", action = "changelanguage" }
            );
            #endregion
        }

        public static void Routing_Report(HttpConfiguration config)
        {
            #region v1
            config.Routes.MapHttpRoute(
               name: "DownloadProductivityMexicoFile",
               routeTemplate: "api/Report/DownloadProductivityMexicoFile",
               defaults: new { controller = "v1_Report", action = "DownloadProductivityMexicoFile" }
            );
            #endregion
        }

        public static void Routing_WebAlive(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
               name: "v1_webalive_status",
               routeTemplate: "api/v1/WebAlive/Status",
               defaults: new { controller = "v1_WebAlive", action = "Status" }
            );
        }
        #endregion
    }
}
