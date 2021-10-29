using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Implementations.Configuration;
using Bgt.Ocean.Service.Messagings.RunControlService;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Service.Implementations.PushToDolphin
{
    public interface IPushToDolphinService
    {
        bool PushToReorderJobs(DolphinReorderJobsRequest request);
        bool UpdateStatusSyncToDolphin(UpdateStatusSyncToDolphinRequest request);
        bool PushJobToDolphin<TRequest>(TRequest request) where TRequest : BaseDolphinAuthen;

        void SetHistoryLogPushToDolphin(SyncToDolphinRequest request);
    }
    public class PushToDolphinService : IPushToDolphinService
    {
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly IMasterActualJobHeaderRepository _masterActualJobHeaderRepository;
        private readonly IMasterHistoryActualJobRepository _masterHistoryActualJobRepository;
        private readonly ISystemService _systemService;
        private readonly IMasterDailyRunResourceRepository _masterDailyRunResourceRepository;
        private readonly ISettingsService _appSetting;
        private readonly ISystemApplicationRepository _systemApplicationRepository;

        public PushToDolphinService(
             IUnitOfWork<OceanDbEntities> uow,
             IMasterActualJobHeaderRepository masterActualJobHeaderRepository,
             ISystemService systemService,
             IMasterHistoryActualJobRepository masterHistoryActualJobRepository,
             IMasterDailyRunResourceRepository masterDailyRunResourceRepository,
             ISettingsService appSetting,
             ISystemApplicationRepository systemApplicationRepository)
        {
            _uow = uow;
            _masterActualJobHeaderRepository = masterActualJobHeaderRepository;
            _systemService = systemService;
            _masterHistoryActualJobRepository = masterHistoryActualJobRepository;
            _masterDailyRunResourceRepository = masterDailyRunResourceRepository;
            _appSetting = appSetting;
            _systemApplicationRepository = systemApplicationRepository;
        }




        public bool PushToReorderJobs(DolphinReorderJobsRequest request)
        {
            var user = ApiSession.UserName;
            request.authen = new DolphinAuthen()     //2019/08/14 -> Change from web.config to SystemEnvironmentGlobal
            {
                username = _appSetting.DolphinUser,
                password = _appSetting.DolphinPwd,
            };

            var url = _appSetting.DolphinUrl;
            var restclient = new RestClient(_appSetting.DolphinHostName);
            var restRequest = new RestRequest(url, Method.POST) { RequestFormat = DataFormat.Json };
            restRequest.AddJsonBody(request);
            string msg = $"{nameof(PushToReorderJobs)} {restRequest.Method.ToString()} Request: '{restclient.BaseUrl}{url}'";
            AddAPILog($"Push To Dolphin : Send {msg}, with body: {JsonConvert.SerializeObject(request)}", 1, user);

            IRestResponse response = restclient.Execute(restRequest);
            AddAPILog($"Push To Dolphin : Receive from {msg}, that response: {response.Content}", 1, user);
            return response.StatusCode == HttpStatusCode.OK;
        }






        public bool UpdateStatusSyncToDolphin(UpdateStatusSyncToDolphinRequest request)
        {
            bool result = false;
            using (var transection = _uow.BeginTransaction())
            {
                try
                {
                    DateTimeOffset date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                    var jobUpdate = _masterActualJobHeaderRepository.FindByListJob(request.ListJobGuid);
                    foreach (var item in jobUpdate)
                    {
                        if (item.SystemStatusJobID == IntStatusJob.CancelledJob)
                            item.FlagSyncToMobile = 88; // 88 is job cancel before mobile receive (EE)

                        item.UserModifed = request.UserModified;
                        item.UniversalDatetimeModified = date;
                        item.DatetimeModified = request.ClientDateTime;
                        _masterActualJobHeaderRepository.Modify(item);

                        var tblHistory = new TblMasterHistory_ActualJob()
                        {
                            Guid = Guid.NewGuid(),
                            MasterActualJobHeader_Guid = item.Guid,
                            MsgID = 275,
                            MsgParameter = item.JobNo + "," + FlagSyncToMobile.GetTextByFlagSync(request.SyncStatusDolphin),
                            UserCreated = request.UserModified,
                            DatetimeCreated = request.ClientDateTime,
                            UniversalDatetimeCreated = date
                        };
                        _masterHistoryActualJobRepository.Create(tblHistory);
                    }

                    _uow.Commit();
                    transection.Complete();

                    result = true;
                }
                catch (Exception ex)
                {

                    // OO error logger
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                }
            }

            return result;
        }
        /// <summary>
        /// Open transaction in method.
        /// </summary>
        /// <param name="request"></param>
        public void SetHistoryLogPushToDolphin(SyncToDolphinRequest request)
        {
            DateTimeOffset date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            var jobUpdate = _masterActualJobHeaderRepository.FindByListJob(request.ListJobGuid);
            string successTxt = request.Success ? "Successfully" : "Unsuccessfully";
            string runDetial = _masterDailyRunResourceRepository.GetDailyRunResourceAndRouteGroupDetail(request.DailyRunGuid.GetValueOrDefault());
            List<TblMasterHistory_ActualJob> acHistList = new List<TblMasterHistory_ActualJob>();
            Parallel.ForEach(jobUpdate, item =>
            {
                if (item.SystemStatusJobID == IntStatusJob.CancelledJob)
                {
                    item.FlagSyncToMobile = 88; // 88 is job cancel before mobile receive (EE)

                        item.UserModifed = request.UserModified;
                    item.UniversalDatetimeModified = date;
                    item.DatetimeModified = request.ClientDateTime;
                    _masterActualJobHeaderRepository.Modify(item);
                }
                var tblHistory = new TblMasterHistory_ActualJob();
                tblHistory.Guid = Guid.NewGuid();
                switch (request.MsgId)
                {
                    case 808://Job has been removed from Run ID {0} {1} by Ocean Online
                            {
                            tblHistory.MsgID = 808;
                            tblHistory.MsgParameter = string.Format("{0},{1}", runDetial, successTxt);
                            break;
                        }
                    case 809://Job information has been update to Dolphin {0} by Ocean Online 
                            {
                            tblHistory.MsgID = 809;
                            tblHistory.MsgParameter = string.Format("{0}", successTxt);
                            break;
                        }
                    case 810://Job has been added to Run ID {0} {1} by Ocean Online 
                            {
                            tblHistory.MsgID = 810;
                            tblHistory.MsgParameter = string.Format("{0},{1}", runDetial, successTxt);
                            break;
                        }
                    case 811://Job has been Canceled from Run ID {0} {1} by Ocean Online.
                            {
                            tblHistory.MsgID = 811;
                            tblHistory.MsgParameter = string.Format("{0},{1}", runDetial, successTxt);
                            break;
                        }
                    case 815://Job has been changed schedule time in Run ID {0} {1} by Ocean Online.
                            {
                            tblHistory.MsgID = 815;
                            tblHistory.MsgParameter = string.Format("{0},{1}", runDetial, successTxt);
                            break;
                        }
                    case 816://Job has been changed workdate from Run ID {0} {1} by Ocean Online.
                            {
                            tblHistory.MsgID = 816;
                            tblHistory.MsgParameter = string.Format("{0},{1}", runDetial, successTxt);
                            break;
                        }
                    default:
                        {
                            tblHistory.MsgID = 275;
                            tblHistory.MsgParameter = item.JobNo + "," + FlagSyncToMobile.GetTextByFlagSync(request.SyncStatusDolphin);
                            break;
                        }
                }
                tblHistory.MasterActualJobHeader_Guid = item.Guid;
                tblHistory.UserCreated = request.UserModified;
                tblHistory.DatetimeCreated = request.ClientDateTime;
                tblHistory.UniversalDatetimeCreated = date;

                acHistList.Add(tblHistory);
            });
            using (var tran = _uow.BeginTransaction())
            {
                _masterHistoryActualJobRepository.CreateRange(acHistList);
                _uow.Commit();
                tran.Complete();
            }

        }

        public bool PushJobToDolphin<TRequest>(TRequest request)
            where TRequest :BaseDolphinAuthen
        {
            var user = ApiSession.UserName;
            request.authen = new DolphinAuthen() //2019/08/14 -> Change from web.config to SystemEnvironmentGlobal
            {
                username = _appSetting.DolphinUser,
                password = _appSetting.DolphinPwd,
            };

            var url = _appSetting.DolphinUrl;
            var restclient = new RestClient(_appSetting.DolphinHostName);
            var restRequest = new RestRequest(url, Method.POST) { RequestFormat = DataFormat.Json };
            restRequest.AddJsonBody(request);
            string msg = $"{nameof(PushJobToDolphin)} {restRequest.Method.ToString()} Request: '{restclient.BaseUrl}{url}'";
            AddAPILog($"Push To Dolphin : Send {msg}, with body: {JsonConvert.SerializeObject(request)}", 1, user);
            IRestResponse response = restclient.Execute(restRequest);
            AddAPILog($"Push To Dolphin : Receive from {msg}, that response: {response.Content}", 1, user);
            return response.StatusCode == HttpStatusCode.OK;
        }
        private void AddAPILog(string msg, int applicationId, string user)
        {

            var appId = _systemApplicationRepository.FindByApplicationID(applicationId);
            _systemService.CreateLogActivity(SystemActivityLog.APIActivity, msg, user, SystemHelper.CurrentIpAddress, appId.Guid);
        }
    }
}
