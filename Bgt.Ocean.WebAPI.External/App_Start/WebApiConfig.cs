using Bgt.Ocean.WebAPI.External.App_Start;
using Bgt.Ocean.WebAPI.External.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Bgt.Ocean.WebAPI.External
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Web API configuration and services
            //config.MessageHandlers.Add(new LogRequestAndResponseHandler());
            config.MessageHandlers.Add(new MethodOverrideHandler());
            //config.Services.Replace(typeof(IHttpControllerSelector), new ApiVersioningSelector(config));
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Re‌​ferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            config.Filters.Add(new ExceptionFilter());

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Default routes
            config.Routes.MapHttpRoute(
                name: "ExternalDefaultAPI",
                routeTemplate: "api/{version}/{access}/{controller}/{action}",
                defaults: new { controller = "{version}_{controller}", id = RouteParameter.Optional }
            );
        }
    }
}
