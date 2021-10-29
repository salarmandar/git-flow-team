using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace Bgt.Ocean.WebAPI.External
{
    public class ApiVersioningSelector : DefaultHttpControllerSelector
    {
        private HttpConfiguration _HttpConfiguration;
        public ApiVersioningSelector(HttpConfiguration httpConfiguration)
            : base(httpConfiguration)
        {
            _HttpConfiguration = httpConfiguration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            HttpControllerDescriptor controllerDescriptor = null;
            IDictionary<string, HttpControllerDescriptor> controllers = GetControllerMapping();
            IDictionary<string, string> accession = new Dictionary<string, string>();
            accession.Add("ext", "Externals");
            accession.Add("in", "Internals");

            IHttpRouteData routeData = request.GetRouteData();
            if (routeData == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            object apiAccession;
            if (!routeData.Values.TryGetValue("access", out apiAccession))
            {
            }

            object controllerName;
            if (!routeData.Values.TryGetValue("controller", out controllerName))
            {
                controllerName = string.Empty;
            }
            if (controllerName == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            object apiVersion;
            if (!routeData.Values.TryGetValue("version", out apiVersion))
            {
                if (controllers.TryGetValue(controllerName.ToString(), out controllerDescriptor))
                {
                    return controllerDescriptor;
                }
            }

            string newControllerName = String.Concat(apiVersion, "_", controllerName.ToString());
            if (controllers.TryGetValue(newControllerName, out controllerDescriptor))
            {
                if (apiAccession == null)
                    return controllerDescriptor;
                var ac = $"{accession[apiAccession.ToString().ToLower()].ToLower()}.{apiVersion}";
                if (GetLocationPath(controllerDescriptor.ControllerType.Namespace) == ac)
                    return controllerDescriptor;
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        private string GetLocationPath(string name)
        {
            try
            {
                var data = name.Split('.');
                int max_index = data.Length - 1;
                return $"{data[max_index - 1]}.{data[max_index]}".ToLower();
            }
            catch { return "not match"; }
        }
    }
}