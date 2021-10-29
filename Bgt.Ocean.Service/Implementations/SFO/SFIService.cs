//using System.Web.Script.Serialization;

namespace Bgt.Ocean.Service.Implementations.AuditLog
{
    public static class SFIService
    {
        //private static string SFISessionID = "SFISessionID";
        //public static void SetAuthHeader(this RestRequest request)
        //{
        //    string headerName = "Session_ID";
        //    string sessionId = GetToken();

        //    if (!sessionId.IsEmpty()) request.AddHeader(headerName, sessionId);
        //}

        //public static IRestResponse ExecuteWithRetry(this RestClient client, RestRequest request)
        //{
        //    var successStatusCode = new System.Net.HttpStatusCode[]
        //    {
        //        System.Net.HttpStatusCode.OK,
        //        System.Net.HttpStatusCode.Created,
        //        System.Net.HttpStatusCode.Accepted
        //    };
        //    var attemps = 0;
        //    var rawResponse = client.Execute(request);
        //    if (!successStatusCode.Contains(rawResponse.StatusCode))
        //    {
        //        ApplicationStorage.Object.RemoveData(SFISessionID);
        //        request.SetAuthHeader();

        //        rawResponse = client.Execute(request);
        //    }

        //    return rawResponse;
        //}

        //private string GetToken()
        //{
        //    var client = new RestClient(_appSettingService.GetAppSettingByKey("OceanUrl"));
        //    var request = new RestRequest(_appSettingService.GetAppSettingByKey("OceanRegisterUrl"), Method.POST);

        //    var sfiUrlConfig = sfoEnvironmentGlobalRepo.FindByAppKey("UrlSFOIntegration");
        //    var sfiAuthenConfig = sfoEnvironmentGlobalRepo.FindByAppKey("UrlSFOAuthen");

        //    string username = sfiAuthenConfig.AppValue2; // SFI username
        //    string password = sfiAuthenConfig.AppValue3; // SFI password

        //    request.AddHeader("app_key", _appSettingService.GetAppSettingByKey("OceanApiAppKey"));
        //    request.AddJsonBody(new { UserName = _appSettingService.GetAppSettingByKey("OceanApiUsername"), Password = _appSettingService.GetAppSettingByKey("OceanApiPassword") });

        //    var response = client.Execute(request);
        //    var r = response.Content;
        //    var js = new JavaScriptSerializer();
        //    var authenResponseModel = js.Deserialize<AuthenResponse>(r);

        //    return authenResponseModel?.token ?? string.Empty;
        //}

        //private class AuthenResponse
        //{
        //    public string token { get; set; }
        //}

        //private static string GetSessionID()
        //{
        //    //ISFIUtilService sfiUtilService = (ISFIUtilService)System.Web.Http.GlobalConfiguration
        //    //   .Configuration.DependencyResolver.GetService(typeof(ISFIUtilService));

        //    //ISFOTblSystemEnvironment_GlobalRepository sfoEnvironmentGlobalRepo = (ISFOTblSystemEnvironment_GlobalRepository)System.Web.Http.GlobalConfiguration
        //    //    .Configuration.DependencyResolver.GetService(typeof(ISFOTblSystemEnvironment_GlobalRepository));

        //    var sfiUrlConfig = sfoEnvironmentGlobalRepo.FindByAppKey("UrlSFOIntegration");
        //    var sfiAuthenConfig = sfoEnvironmentGlobalRepo.FindByAppKey("UrlSFOAuthen");

        //    string username = sfiAuthenConfig.AppValue2; // SFI username
        //    string password = sfiAuthenConfig.AppValue3; // SFI password

        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    string endcoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));

        //    var client = new RestClient(sfiUrlConfig.AppValue1);
        //    var request = new RestRequest(sfiAuthenConfig.AppValue1, Method.POST);

        //    request.Timeout = Int32.MaxValue;
        //    request.AddHeader("Authorization", "Basic " + endcoded);
        //    request.RequestFormat = DataFormat.Json;

        //    var response = client.Execute(request);
        //    var r = response.Content;

        //    //sfiUtilService
        //    //    .AddAPILog($"SFI Authen via url: {System.IO.Path.Combine(client.BaseUrl.ToString(), sfiAuthenConfig.AppValue1)} with username: {username}, password: {password}. Response: {r}");

        //    var authenResponseModel = js.Deserialize<AuthenResponse>(r);

        //    return authenResponseModel?.Session_ID ?? string.Empty;
        //}

        //private class AuthenResponse
        //{
        //    public string Session_ID { get; set; }
        //}

    }
}
