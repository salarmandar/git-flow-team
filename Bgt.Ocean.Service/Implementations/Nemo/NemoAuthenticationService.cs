using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Nemo.Authentication;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.Implementations.Nemo
{
    #region Interface
    public interface INemoAuthenticationService
    {
        Task<T> ApiPostAsync<T>(string requestUri, HttpBasicAuthenticator httpBasicAuthenticator);
        Task<T> ApiPostAsync<T>(string requestUri, object jsonData, IEnumerable<RequestHeader> requestHeaders);
        Task<IRestResponse> ApiPostAsync(string requestUri, object jsonData, IEnumerable<RequestHeader> requestHeaders);
        NemoAutenticationResponse GetNemoAutentication();
        NemoAutenticationResponse GetToken();
        int GetErrorAttempt();
    }
    #endregion

    #region Service
    public class NemoAuthenticationService : INemoAuthenticationService
    {
        #region Objects & Variables
        private readonly ISystemEnvironment_GlobalRepository _systemEnvironment_GlobalRepository;

        private static TblSystemEnvironment_Global NEMO_URI_BASE;
        private static TblSystemEnvironment_Global NEMO_URI_AUTHENTICATION;
        private static TblSystemEnvironment_Global NEMO_TIMEOUT_SYNC;
        private static TblSystemEnvironment_Global NEMO_ERROR_ATTEMPT;

        private NemoAutenticationResponse NEMO_TOKEN;
        private int ERROR_ATTEMPT = 5;
        #endregion

        #region Constuctor
        public NemoAuthenticationService(ISystemEnvironment_GlobalRepository systemEnvironment_GlobalRepository)
        {
            _systemEnvironment_GlobalRepository = systemEnvironment_GlobalRepository;

            var nemoSyncBaseURL = Task.Run(() => _systemEnvironment_GlobalRepository.FindByAppKeyAsync("NEMO_URI_BASE"));
            var nemoSyncAuthenURL = Task.Run(() => _systemEnvironment_GlobalRepository.FindByAppKeyAsync("NEMO_URI_AUTHENTICATION"));
            var nemoSyncTimeout = Task.Run(() => _systemEnvironment_GlobalRepository.FindByAppKeyAsync("NEMO_TIMEOUT_SYNC"));
            var nemoSyncAttempt = Task.Run(() => _systemEnvironment_GlobalRepository.FindByAppKeyAsync("NEMO_ERROR_ATTEMPT"));

            NEMO_URI_BASE = nemoSyncBaseURL.Result;
            NEMO_URI_AUTHENTICATION = nemoSyncAuthenURL.Result;
            NEMO_TIMEOUT_SYNC = nemoSyncTimeout.Result;
            NEMO_ERROR_ATTEMPT = nemoSyncAttempt.Result;

            ERROR_ATTEMPT = Convert.ToInt32(NEMO_ERROR_ATTEMPT.AppValue1);
        }
        #endregion

        #region Authentication
        public NemoAutenticationResponse GetNemoAutentication()
        {
            NEMO_TOKEN = null;
            var attempt = 0;
            do
            {
                try
                {
                    NEMO_TOKEN = ApiPostAsync<NemoAutenticationResponse>(NEMO_URI_AUTHENTICATION.AppValue1, new HttpBasicAuthenticator(NEMO_URI_AUTHENTICATION.AppValue2, NEMO_URI_AUTHENTICATION.AppValue3)).Result;
                    attempt++;
                }
                catch (Exception)
                {
                    attempt++;
                }
            }
            while (NEMO_TOKEN == null && attempt < ERROR_ATTEMPT);
            return NEMO_TOKEN;
        }

        public NemoAutenticationResponse GetToken()
        {
            return this.NEMO_TOKEN;
        }

        public int GetErrorAttempt()
        {
            return this.ERROR_ATTEMPT;
        }
        #endregion

        #region Core API
        public async Task<T> ApiPostAsync<T>(string requestUri, HttpBasicAuthenticator httpBasicAuthenticator)
        {
            var client = new RestClient()
            {
                BaseUrl = new Uri(NEMO_URI_BASE.AppValue1),
                Timeout = Convert.ToInt32(NEMO_TIMEOUT_SYNC.AppValue1),
                Authenticator = httpBasicAuthenticator
            };
            var request = new RestRequest(requestUri, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json");

            var tcs = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, res =>
            {
                tcs.SetResult(res);
            });
            return JsonConvert.DeserializeObject<T>(tcs.Task.Result.Content);
        }

        public async Task<T> ApiPostAsync<T>(string requestUri, object jsonData, IEnumerable<RequestHeader> requestHeaders)
        {
            var client = new RestClient()
            {
                BaseUrl = new Uri(NEMO_URI_BASE.AppValue1),
                Timeout = Convert.ToInt32(NEMO_TIMEOUT_SYNC.AppValue1)
            };
            var request = new RestRequest(requestUri, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Token", NEMO_TOKEN.Token);
            if (requestHeaders != null && requestHeaders.Any())
            {
                foreach (var data in requestHeaders)
                {
                    request.AddHeader(data.Name, data.Value);
                }
            }
            request.AddJsonBody(jsonData);

            var tcs = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, res =>
            {
                tcs.SetResult(res);
                
            });
            return JsonConvert.DeserializeObject<T>(tcs.Task.Result.Content);
        }

        public async Task<IRestResponse> ApiPostAsync(string requestUri, object jsonData, IEnumerable<RequestHeader> requestHeaders)
        {
            var client = new RestClient()
            {
                BaseUrl = new Uri(NEMO_URI_BASE.AppValue1),
                Timeout = Convert.ToInt32(NEMO_TIMEOUT_SYNC.AppValue1)
            };
            var request = new RestRequest(requestUri, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Token", NEMO_TOKEN.Token);
            if (requestHeaders != null && requestHeaders.Any())
            {
                foreach (var data in requestHeaders)
                {
                    request.AddHeader(data.Name, data.Value);
                }
            }
            request.AddJsonBody(jsonData);

            var tcs = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, res =>
            {
                tcs.SetResult(res);
            });
            return tcs.Task.Result;
        }
        #endregion
    }
    #endregion

    #region Model
    public class RequestHeader
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    #endregion
}
