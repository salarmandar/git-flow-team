using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Service.Implementations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Bgt.Ocean.RunControl.WebAPI.App_Start
{
    public class LogRequestAndResponseHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            ISystemService systemService = (ISystemService)System.Web.Http.GlobalConfiguration
                    .Configuration.DependencyResolver.GetService(typeof(ISystemService));

            IWebAPIUser_TokenRepository apiUserTokenRepository = (IWebAPIUser_TokenRepository)System.Web.Http.GlobalConfiguration
                    .Configuration.DependencyResolver.GetService(typeof(IWebAPIUser_TokenRepository));

            // log request body
            string requestBody = await request.Content.ReadAsStringAsync();
            string requestHeader = JsonConvert.SerializeObject(request.Headers);

            LogRequest log;

            try
            {
                log = new LogRequest
                {
                    requestBody = JsonConvert.DeserializeObject(requestBody),
                    requestHeaders = JsonConvert.DeserializeObject(requestHeader)
                };
            }
            catch
            {
                log = new LogRequest
                {
                    requestBody = requestBody,
                    requestHeaders = JsonConvert.DeserializeObject(requestHeader)
                };
            }

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
            if (result.Content != null)
            {
                // once response body is ready, log it
                responseBody = await result.Content.ReadAsStringAsync();
            }
            try
            {
                var apiUser = apiUserTokenRepository.FindByTokenId(authKey.GetValueOrDefault())?.TblWebAPIUser;
                var appGuid = apiUser?.TblWebAPIUserApplication.FirstOrDefault()?.SystemApplication_Guid;

                systemService.CreateAccessAPILogHistory(authKey ?? appKey, _dateStart, request.RequestUri.OriginalString, requestBody, DateTime.Now, responseBody);

                if (appGuid.HasValue)
                {
                    systemService.CreateLogActivity(SystemActivityLog.APIActivity, JsonConvert.SerializeObject(log), apiUser.UserName, SystemHelper.CurrentIpAddress, appGuid.Value);
                }
            }
            catch (Exception ex) { systemService.CreateHistoryError(ex); }

            return result;
        }

        private class LogRequest
        {
            public object requestBody { get; set; }
            public object requestHeaders { get; set; }
        }
    }
}