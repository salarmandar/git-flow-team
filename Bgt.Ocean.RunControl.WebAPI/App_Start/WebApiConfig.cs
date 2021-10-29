using Bgt.Ocean.RunControl.WebAPI.Filters;
using Bgt.Ocean.RunControl.WebAPI.App_Start;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Dispatcher;
using System.Net.Http.Headers;

namespace Bgt.Ocean.RunControl.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute("*", "*", "GET,POST");
            config.EnableCors(cors);

            // Web API configuration and services
            //config.MessageHandlers.Add(new LogRequestAndResponseHandler());
            config.MessageHandlers.Add(new MethodOverrideHandler());
            config.MessageHandlers.Add(new LogRequestAndResponseHandler());
            config.Services.Replace(typeof(IHttpControllerSelector), new ApiVersioningSelector(config));
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Re‌​ferenceLoopHandling = ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));
            config.Filters.Add(new ExceptionFilter());

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Custom route
            //Routing_Account(config);
            //Routing_Report(config);

            // Default routes
            //config.Routes.MapHttpRoute(
            //    name: "InternalDefaultAPI",
            //    routeTemplate: "api/{version}/{access}/{controller}/{action}",
            //    defaults: new { controller = "{version}_{controller}", id = RouteParameter.Optional }
            //);
        }
    }
}
