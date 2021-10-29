using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Service.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bgt.Ocean.WebAPI.App_Start
{
    public class LogRequestAndResponseHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // log request body
            string requestBody = await request.Content.ReadAsStringAsync();

            DateTime _dateStart = DateTime.Now;

            // get app key
            IEnumerable<string> authKeys = null;
            IEnumerable<string> appKeys = null;
            request.Headers.TryGetValues("auth_key", out authKeys);
            request.Headers.TryGetValues("app_key", out appKeys);

            Guid? authKey = authKeys?.FirstOrDefault().ToGuid();
            Guid? appKey = appKeys?.FirstOrDefault().ToGuid();

            string responseBody = string.Empty;

            // let other handlers process the request
            var result = await base.SendAsync(request, cancellationToken);

            if (result != null && result.Content != null)
            { 
                // once response body is ready, log it
                responseBody = await result.Content.ReadAsStringAsync();
            }

            var systemService = (ISystemService)request.GetDependencyScope().GetService(typeof(ISystemService));
            try
            {
                await systemService.CreateAccessAPILogHistoryAsync(authKey ?? appKey, _dateStart, request.RequestUri.OriginalString, requestBody, DateTime.Now, responseBody);
            }
            catch (Exception ex) { systemService.CreateHistoryError(ex); }

            return result;
        }
    }
}