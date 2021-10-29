using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Service.Implementations.AuditLog;
using Bgt.Ocean.Service.Messagings.RunControlService;
using Bgt.Ocean.Service.ModelViews.GenericLog;
using Bgt.Ocean.Service.ModelViews.ServiceRequest.Dolphin;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations.PushToDolphin
{
    #region interface
    
    public interface ISFOPushToDolphinService
    {
        void PushCancelSFOJobToDolphin(Guid ticketGuid);
        void PushRescheduleSFOJobToDolphin(Guid ticketGuid, Guid dailyRunGuid, DateTime oldWorkDateTime);
    }

    #endregion


    public class SFOPushToDolphinService : ISFOPushToDolphinService
    {
        private const string EE_ACTION_CANCEL = "cancel";
        private const string EE_ACTION_RESCHEDULE = "reschedule";

        private readonly ISFOSystemEnvironmentGlobalRepository _sfoSystemEnvironmentGlobalRepository;
        private readonly ISystemService _systemService;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly IGenericLogService _genericLogService;
        private readonly ISFOApiUserService _sfoApiUserService;
        private readonly IWebAPIUser_TokenRepository _webApiUserTokenRepository;
        private readonly IMasterActualJobHeaderRepository _masterActualJobHeaderRepository;
        private readonly IMasterDailyRunResourceRepository _masterDailyRunResourceRepository;
        private readonly ISFOTransactionServiceRequestRepository _transactionServiceRequestRepository;
        private readonly IReasonCodeRepository _reasonCodeRepository;
        private readonly ISFOSystemFunctionRepository _systemFunctionRepository;
        private readonly IMasterUserRepository _masterUserRepository;
        private readonly IMasterRouteGroupDetailRepository _masterRouteGroupDetailRepository;

        public SFOPushToDolphinService(
                ISFOSystemEnvironmentGlobalRepository sfoSystemEnvironmentGlobalRepository,
                ISystemService systemService,
                IGenericLogService genericLogService,
                IUnitOfWork<OceanDbEntities> uow,
                ISFOApiUserService sfoApiUserService,
                IWebAPIUser_TokenRepository webApiUserTokenRepository,
                IMasterActualJobHeaderRepository masterActualJobHeaderRepository,
                IMasterDailyRunResourceRepository masterDailyRunResourceRepository,
                ISFOTransactionServiceRequestRepository transactionServiceRequestRepository,
                IReasonCodeRepository reasonCodeRepository,
                ISFOSystemFunctionRepository systemFunctionRepository,
                IMasterUserRepository masterUserRepository,
                IMasterRouteGroupDetailRepository masterRouteGroupDetailRepository
            )
        {
            _sfoSystemEnvironmentGlobalRepository = sfoSystemEnvironmentGlobalRepository;
            _systemService = systemService;
            _genericLogService = genericLogService;
            _uow = uow;
            _sfoApiUserService = sfoApiUserService;
            _webApiUserTokenRepository = webApiUserTokenRepository;
            _masterActualJobHeaderRepository = masterActualJobHeaderRepository;
            _masterDailyRunResourceRepository = masterDailyRunResourceRepository;
            _transactionServiceRequestRepository = transactionServiceRequestRepository;
            _reasonCodeRepository = reasonCodeRepository;
            _systemFunctionRepository = systemFunctionRepository;
            _masterUserRepository = masterUserRepository;
            _masterRouteGroupDetailRepository = masterRouteGroupDetailRepository;
        }

        public void PushCancelSFOJobToDolphin(Guid ticketGuid)
        {
            var sr = _transactionServiceRequestRepository.FindById(ticketGuid);
            var reason = _reasonCodeRepository.FindById(sr.MasterReasonType_Guid)?.ReasonTypeName;
            var userInfo = _sfoApiUserService.GetUserByConfiguration(sr.MasterCountryGuid)?.UserName;

            PushCancelSFOJobToDolphin(new PushCancelSFOJobBodyView
            {
                brinksSiteID = sr.TblMasterSite.SiteCode,
                jobGuid = sr.MasterActualJobHeader_Guid.Value,
                reason = reason ?? sr.CancellationReason ?? sr.CancelReason
            }, sr.MasterDailyRunResource_Guid.GetValueOrDefault(), sr.DatetimeModified.ChangeFromDateTimeOffsetToDate().Value, userInfo);
        }

        public void PushRescheduleSFOJobToDolphin(Guid ticketGuid, Guid dailyRunGuid, DateTime oldWorkDateTime)
        {
            try
            {
                var sr = _transactionServiceRequestRepository.FindById(ticketGuid);
                var userInfo = _sfoApiUserService.GetUserByConfiguration(sr.MasterCountryGuid);
                var userTimezone = _masterUserRepository.FindByUserName(userInfo.UserName).TimeZoneID;
                var clientDateTime = _systemFunctionRepository.Func_CalculateTime(DateTime.UtcNow, SystemTimeZoneHelper.UTC, userTimezone.Value);
                var jobGuidList = new Guid[] { sr.MasterActualJobHeader_Guid.Value };
                string routeName = GetRouteName(dailyRunGuid);

                bool isValidJob= IsValidJobBeforePushUpdate(sr.MasterActualJobHeader_Guid.Value);
                if (dailyRunGuid.IsEmpty() || !IsValidRunStatus(dailyRunGuid) || !isValidJob || oldWorkDateTime == DateTime.MinValue)
                    return;

                BeginPush(new PushRescheduleSFOJobView
                {
                    action = EE_ACTION_RESCHEDULE,
                    authen = GetDolphinAuthen(),
                    clientDateTime = clientDateTime.Value.ChangeFromDateToStringSQLFormat(),
                    jobGuid = jobGuidList,
                    oldWorkDate = oldWorkDateTime.ChangeFromDateToStringSQLFormat(),
                    routeName = routeName,
                    runDailyGuid = dailyRunGuid,
                    userAction = userInfo.UserName
                }, jobGuidList);
            }
            catch (Exception err)
            {
                _systemService.CreateHistoryError(err);
            }
        }

        #region Private

        private void PushCancelSFOJobToDolphin(PushCancelSFOJobBodyView jobModel, Guid runDailyGuid, DateTime clientDateTime, string username)
        {
            try
            {
                bool allValid = IsValidJobBeforePushUpdate(jobModel.jobGuid);

                if (runDailyGuid.IsEmpty() || !IsValidRunStatus(runDailyGuid) || !allValid)
                    return;

                BeginPush(new PushCancelSFOJobView
                {
                    action = EE_ACTION_CANCEL,
                    runDailyGuid = runDailyGuid,
                    authen = GetDolphinAuthen(),
                    clientDateTime = clientDateTime.ChangeFromDateToStringSQLFormat(),
                    cancelJobs = new List<PushCancelSFOJobBodyView> { jobModel },
                    userAction = username
                }, new Guid[] { jobModel.jobGuid });                
            }
            catch (Exception err)
            {
                _systemService.CreateHistoryError(err);
            }
        }
        

        private void BeginPush<TRequest>(TRequest requestModel, Guid[] jobHeaderGuidList)
            where TRequest : PushSFOJobView
        {
            try
            {
                var countryGuid = GetCountryGuidByJob(jobHeaderGuidList);
                var userInfo = _sfoApiUserService.GetUserByConfiguration(countryGuid);

                var urlConfig = _sfoSystemEnvironmentGlobalRepository.FindByAppKey("DolphinURL");

                var result = SendRequest(urlConfig.AppValue1, urlConfig.AppValue2, requestModel, Method.POST);
                var msgSuccess = _systemService.GetMessage(2085, userInfo.UserLanguageGuid.Value, new string[] { requestModel.action });
                var msgFailed = _systemService.GetMessage(-2085, userInfo.UserLanguageGuid.Value, new string[] { requestModel.action, result?.Reason ?? result?.Data ?? result?.Error });

                var msg = result != null && result.Success ? msgSuccess.MessageTextContent : msgFailed.MessageTextContent;

                foreach (var jobHeaderGuid in jobHeaderGuidList)
                {
                    CreateTicketLog(jobHeaderGuid, msg, userInfo.UserName);
                }                
            }
            catch (Exception err)
            {
                _systemService.CreateHistoryError(err);
            }
        }

        private Guid GetCountryGuidByJob(Guid[] jobHeaderGuidList)
        {
            var jobData = _masterActualJobHeaderRepository.FindById(jobHeaderGuidList.Distinct().FirstOrDefault());
            return jobData.SFOTblTransactionServiceRequest.FirstOrDefault().TblMasterSite.MasterCountry_Guid;
        }

        private DolphinAPIResponse SendRequest(string baseUrl, string actionUrl, object body, Method method)
        {
            try
            {
                IRestResponse response;
                var client = new RestClient(baseUrl);
                var request = new RestRequest(actionUrl, method);
                string msg = $"SFO-EE {request.Method.ToString()} Request: '{client.BaseUrl}{actionUrl}'";

                request.AddJsonBody(body);

                AddAPILog($"Send {msg}, with body: {JsonConvert.SerializeObject(body)}");

                response = client.Execute(request);

                AddAPILog($"Receive from {msg}, that response: {(response.Content.IsEmpty() ? response.ErrorMessage : response.Content)}");

                var result = JsonConvert.DeserializeObject<DolphinAPIResponse>(response.Content);

                return result == null ? new DolphinAPIResponse { Reason = response.ErrorMessage, Success = false } : result;
            }
            catch (Exception err)
            {
                _systemService.CreateHistoryError(err);
                return new DolphinAPIResponse
                {
                    Reason = err.Message,
                    Success = false
                };
            }

        }

        private void AddAPILog(string msg)
        {
            var username = _webApiUserTokenRepository.FindByTokenId(ApiSession.Application_Token.ToGuid())?.TblWebAPIUser.UserName;
            _systemService.CreateLogActivity(SystemActivityLog.APIActivity, msg, username, SystemHelper.CurrentIpAddress, SystemHelper.SFO_ApplicationGuid);
        }

        private void CreateTicketLog(Guid jobHeaderGuid, string message, string userCreated)
        {
            var genericLogList = new TransactionGenericLogModel();
            genericLogList.DateTimeCreated = DateTime.UtcNow;
            genericLogList.JSONValue = SystemHelper.GetJSONStringByArray<string>(message);
            genericLogList.LabelIndex = null;
            genericLogList.ReferenceValue = jobHeaderGuid.ToString();
            genericLogList.SystemLogCategory_Guid = SFOLogCategoryHelper.ServiceRequestDetailsGuid.ToGuid();
            genericLogList.SystemLogProcess_Guid = SFOProcessHelper.ServiceRequestGuid.ToGuid();
            genericLogList.SystemMsgID = "117";
            genericLogList.UserCreated = userCreated;
            _genericLogService.InsertTransactionGenericLog(genericLogList);
        }

        private bool IsValidJobBeforePushUpdate(Guid jobHeaderGuid)
        {
            var job = _masterActualJobHeaderRepository.FindById(jobHeaderGuid);

            if (job == null) return false;

            return new byte[] { Convert.ToByte(1), Convert.ToByte(7) }.Contains(job.FlagSyncToMobile.GetValueOrDefault());
        }

        private bool IsValidRunStatus(Guid runDailyGuid) => _masterDailyRunResourceRepository.IsRunDispatched(runDailyGuid);

        private DolphinAuthen GetDolphinAuthen()
        {
            var config = _sfoSystemEnvironmentGlobalRepository.FindByAppKey("DolphinAuthen");
            return new DolphinAuthen
            {
                username = config.AppValue1,
                password = config.AppValue2
            };
        }

        private string GetRouteName(Guid dailyRunGuid)
        {
            var dailyRun = _masterDailyRunResourceRepository.FindById(dailyRunGuid);
            var routeGroupDetail = _masterRouteGroupDetailRepository.FindById(dailyRun?.MasterRouteGroup_Detail_Guid);

            return routeGroupDetail?.MasterRouteGroupDetailName;
        }

        private class DolphinAPIResponse
        {
            public bool Success { get; set; }
            public string Reason { get; set; }
            public string Data { get; set; }
            public string Error { get; set; }
        }

        #endregion
    }
}
