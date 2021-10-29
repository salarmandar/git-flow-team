
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models.RunControl.LiabilityLimitModel;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Messagings.TruckLiabilityLimit;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations.TruckLiabilityLimit
{

    public interface ITruckLiabilityLimitService
    {
        /// <summary>
        /// MOVE JOBS
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        LiabilityLimitResponse IsOverLiabilityLimitWhenExistJobs(LiabilityLimitExistsJobsRequest request);

        /// <summary>
        /// EDIT/UPDATE
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        LiabilityLimitResponse IsOverLiabilityLimitWhenNoExistsItems(LiabilityLimitNoExistsItemsRequest request);

        LiabilityLimitResponse IsOverLialibityLimitWhenNoExistsJobs(LiabilityLimitNoExistsJobsRequest request);

        /// <summary>
        /// Check Run  Percentage Liability
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PercentageLiabilityLimitAlertResponse GetTruckLiabilityLimitPercentageAlert(LiabilityLimitExistsRunRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ConvertBankCleanOutTotalLiabilityResponse GetConvertBankCleanOutTotalLiability(ConvertBankCleanOutTotalLiabilityRequest request);

    }

    public class TruckLiabilityLimitService : ITruckLiabilityLimitService
    {
        private readonly ISystemEnvironmentMasterCountryRepository _systemEnvironmentMasterCountryRepository;
        private readonly IMasterActualJobHeaderRepository _masterActualJobHeaderRepository;
        private readonly IMasterCurrencyRepository _masterCurrencyRepository;
        private readonly IMasterCommodityRepository _masterCommodityRepository;
        private readonly IMasterDailyRunResourceRepository _masterDailyRunResourceRepository;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IBaseRequest _baseRequest;
        public TruckLiabilityLimitService(ISystemEnvironmentMasterCountryRepository systemEnvironmentMasterCountryRepository,
            IMasterActualJobHeaderRepository masterActualJobHeaderRepository,
            IMasterDailyRunResourceRepository masterDailyRunResourceRepository,
            IMasterCurrencyRepository masterCurrencyRepository,
            IMasterCommodityRepository masterCommodityRepository,
            ISystemMessageRepository systemMessageRepository,
            IBaseRequest baseRequest
            )
        {
            _systemEnvironmentMasterCountryRepository = systemEnvironmentMasterCountryRepository;
            _masterActualJobHeaderRepository = masterActualJobHeaderRepository;
            _masterDailyRunResourceRepository = masterDailyRunResourceRepository;
            _masterCurrencyRepository = masterCurrencyRepository;
            _masterCommodityRepository = masterCommodityRepository;
            _systemMessageRepository = systemMessageRepository;
            _baseRequest = baseRequest;
        }

        private bool FlagValidateRunLiabilityLimit { get; set; }
        private bool FlagAllowExceedLiabilityLimit { get; set; }
        private int PercentageLiabilityLimitAlert { get; set; }
        private Guid LanguangeGuid { get; set; }
        private bool GetTruckLitmitConfig(Guid? siteGuid)
        {
            LanguangeGuid = _baseRequest.Data.UserLanguageGuid.GetValueOrDefault();
            FlagValidateRunLiabilityLimit = _systemEnvironmentMasterCountryRepository.FindAppkeyValueByEnumAppkeyName(siteGuid, EnumAppKey.FlagValidateRunLiabilityLimit);
            if (FlagValidateRunLiabilityLimit)
            {
                FlagAllowExceedLiabilityLimit = _systemEnvironmentMasterCountryRepository.FindAppkeyValueByEnumAppkeyName(siteGuid, EnumAppKey.FlagAllowExceedLiabilityLimit);
                var appvalue1 = _systemEnvironmentMasterCountryRepository.FindCountryOptionByEnumAppkeyName(siteGuid, EnumAppKey.PercentageLiabilityLimitAlert)?.AppValue1;
                PercentageLiabilityLimitAlert = string.IsNullOrEmpty(appvalue1) ? 0 : Convert.ToInt16(appvalue1);
            }
            return FlagValidateRunLiabilityLimit;
        }
        private Guid? GetDefaultSiteGuid(Guid? siteGuid, IEnumerable<Guid?> dailyRunGuids)
        {
            if (siteGuid == Guid.Empty || siteGuid == null)
            {
                var target_RunSiteGuid = dailyRunGuids.FirstOrDefault(o => o != Guid.Empty && o.HasValue);
                siteGuid = _masterDailyRunResourceRepository.FindById(target_RunSiteGuid)?.MasterSite_Guid;
            }
            return siteGuid;
        }


        /// <summary>
        /// => TFS#63341: Global ability to manage the truck liability limit  -> IsOverLiabilityLimitWhenExistJobs
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LiabilityLimitResponse IsOverLiabilityLimitWhenExistJobs(LiabilityLimitExistsJobsRequest request)
        {

            var requestModel = request.JobsActionModel;
            requestModel.SiteGuid = GetDefaultSiteGuid(requestModel.SiteGuid, requestModel.RequestList.GetDailyRunTargetGuids());

            var isValidConfig = GetTruckLitmitConfig(requestModel.SiteGuid);
            LiabilityLimitResponse response = new LiabilityLimitResponse();
            response.Message = _systemMessageRepository.FindByMsgId(0, LanguangeGuid).ConvertToMessageView(true);
            if (isValidConfig)
            {
                var rawExistsJobs = _masterDailyRunResourceRepository.GetLiabilityLimitRawJobsData(requestModel.SiteGuid, requestModel.RequestList.Select(o => o));
                var truckLimitDetail = GetJobDeitailInfo(requestModel.SiteGuid, rawExistsJobs).ProcessBulkExceedJobs();

                response.TruckLimitDetail = truckLimitDetail;
                response.Message = IsOverLiabilityLimit(truckLimitDetail.JobDetailList);
            }
            return response;
        }
        /// <summary>
        /// => TFS#63341: Global ability to manage the truck liability limit  -> IsOverLialibityLimitWhenNoExistsJobs
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LiabilityLimitResponse IsOverLiabilityLimitWhenNoExistsItems(LiabilityLimitNoExistsItemsRequest request)
        {
            var requestModel = request.ItemsActionModel;
            var jobsModel = requestModel.RequestList.ConvertToJobsActionModel();
            requestModel.SiteGuid = GetDefaultSiteGuid(requestModel.SiteGuid, jobsModel.GetDailyRunTargetGuids());

            var isValidConfig = GetTruckLitmitConfig(requestModel.SiteGuid);
            LiabilityLimitResponse response = new LiabilityLimitResponse();
            response.Message = _systemMessageRepository.FindByMsgId(0, LanguangeGuid).ConvertToMessageView(true);
            if (isValidConfig)
            {

                var rawExistJob = _masterDailyRunResourceRepository.GetLiabilityLimitRawJobsData(requestModel.SiteGuid, jobsModel);
                rawExistJob = requestModel.RequestList.MergeItemsToExistsJobs(rawExistJob);
                var truckLimitDetail = GetJobDeitailInfo(requestModel.SiteGuid, rawExistJob).ProcessBulkExceedJobs();

                response.TruckLimitDetail = truckLimitDetail;
                response.Message = IsOverLiabilityLimit(truckLimitDetail.JobDetailList);
            }
            return response;
        }
        /// <summary>
        /// => TFS#63341: Global ability to manage the truck liability limit  -> IsOverLialibityLimitWhenNoExistsJobs
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LiabilityLimitResponse IsOverLialibityLimitWhenNoExistsJobs(LiabilityLimitNoExistsJobsRequest request)
        {

            var requestModel = request.NoJobsActionModel;
            requestModel.SiteGuid = GetDefaultSiteGuid(requestModel.SiteGuid, requestModel.RequestList.GetDailyRunTargetGuids());

            var isValidConfig = GetTruckLitmitConfig(requestModel.SiteGuid);
            LiabilityLimitResponse response = new LiabilityLimitResponse();
            response.Message = _systemMessageRepository.FindByMsgId(0, LanguangeGuid).ConvertToMessageView(true);
            if (isValidConfig)
            {
                var jobsActionModel = requestModel.RequestList.ConvertToJobsActionModel();
                var rawExistsJob = _masterDailyRunResourceRepository.GetLiabilityLimitRawJobsData(requestModel.SiteGuid, jobsActionModel);
                var rawNotExistsJob = _masterDailyRunResourceRepository.GetLiabilityLimitRawJobsData(requestModel.SiteGuid, requestModel.RequestList);
                rawExistsJob = rawNotExistsJob.MergeJobsToExistsJobs(rawExistsJob);
                var truckLimitDetail = GetJobDeitailInfo(requestModel.SiteGuid, rawExistsJob).ProcessBulkExceedJobs();

                response.TruckLimitDetail = truckLimitDetail;
                response.Message = IsOverLiabilityLimit(truckLimitDetail.JobDetailList);
            }
            return response;
        }

        /// <summary>
        /// => TFS#63341: Global ability to manage the truck liability limit  -> GetTruckLiabilityLimitPercentageAlert
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PercentageLiabilityLimitAlertResponse GetTruckLiabilityLimitPercentageAlert(LiabilityLimitExistsRunRequest request)
        {
            var model = request.RunsActionModel;
            var isValidConfig = GetTruckLitmitConfig(model.SiteGuid);
            PercentageLiabilityLimitAlertResponse response = new PercentageLiabilityLimitAlertResponse();
            response.Message = _systemMessageRepository.FindByMsgId(0, LanguangeGuid).ConvertToMessageView(true);
            if (isValidConfig)
            {
                var reqList = model.ConvertToJobsActionModel();
                var rawJobData = _masterDailyRunResourceRepository.GetLiabilityLimitRawJobsData(model.SiteGuid, reqList);
                var jobsDetail = GetJobDeitailInfo(model.SiteGuid, rawJobData);

                //total STC per run
                var jobsInRun = jobsDetail.Where(o => o.DailyRunGuid != null)
                                      .GroupBy(o => new { o.DailyRunGuid, o.Target_CurrencyGuid, o.Target_RunLibilityLimit, o.Target_CurrencyAbb })
                                      .Select(g => new
                                      {
                                          Target_DailyRunGuid = g.Key.DailyRunGuid,
                                          Target_RunLibilityLimit = g.Key.Target_RunLibilityLimit,
                                          Target_CurrencyGuid = g.Key.Target_CurrencyGuid,
                                          Target_CurrencyAbbr = g.Key.Target_CurrencyAbb,
                                          Target_DailyRunSTC = g.Sum(o => o.TotalJobSTC)
                                      }).ToList();


                var appvalue1 = _systemEnvironmentMasterCountryRepository.FindCountryOptionByEnumAppkeyName(model.SiteGuid, EnumAppKey.PercentageLiabilityLimitAlert)?.AppValue1;
                var percentageLiabilityLimitAlert = string.IsNullOrEmpty(appvalue1) ? 0 : Convert.ToInt16(appvalue1);

                var result = jobsInRun.Select(r =>
                 {
                     double? currentPercentageLiability = 0;
                     var isOverPercentageLiabilityLimitAlert = false;

                     if (percentageLiabilityLimitAlert > 0 && r.Target_RunLibilityLimit > 0)
                     {

                         currentPercentageLiability = ((r.Target_DailyRunSTC / r.Target_RunLibilityLimit) * 100);
                         isOverPercentageLiabilityLimitAlert = r.Target_DailyRunGuid.HasValue
                                                               && r.Target_RunLibilityLimit != 0
                                                               && currentPercentageLiability > percentageLiabilityLimitAlert;


                     }

                     return new PercentageLiabilityLimitAlertResult
                     {
                         Target_CurrencyGuid = r.Target_CurrencyGuid,
                         Target_CurrencyAbbr = r.Target_CurrencyAbbr,
                         Target_DailyRunGuid = r.Target_DailyRunGuid,
                         Target_DailyRunSTC = r.Target_DailyRunSTC,
                         Target_CurrentPercentageLiability = currentPercentageLiability,
                         FlagPercentageLiabilityLimitAlert = isOverPercentageLiabilityLimitAlert,
                         Target_RunLibilityLimit = r.Target_RunLibilityLimit
                     };
                 });

                response.RunDetail = result;
            }
            return response;
        }

        private IEnumerable<JobDetailResult> GetJobDeitailInfo(Guid? siteGuid, IEnumerable<RawJobDataView> jobModels)
        {
            var currentExchange = _masterCurrencyRepository.GetCurrencyExchangeList(siteGuid).ToList();
            var template = _masterCommodityRepository.GetAllCommodityBySite(siteGuid);

            //total STC and covert Exchange per job
            var allJobs = jobModels.Select(o => o.CalculateJobSTC(currentExchange, template)).ToList();
            return allJobs;
        }
        private SystemMessageView IsOverLiabilityLimit(IEnumerable<JobDetailResult> exceedJobs)
        {
            SystemMessageView result = null;
            var displayJob = exceedJobs.Where(o => o.FlagDisplayJob);
            if (displayJob.Any(o => o.FlagDisplayJob) && FlagValidateRunLiabilityLimit)
            {
                var allRun = displayJob.Select(o => o.Target_RunNo).Distinct();
                allRun = allRun.To3DotAfterTake(3);

                if (FlagAllowExceedLiabilityLimit)
                {

                    //ถ้า True : Confirmation message, “Run No. {​​​​​​​​Run No. 1, Run No. 2, …}​​​​​​​​ liability has been reached the limit. Do you want to continue?”
                    result = _systemMessageRepository.FindByMsgId(6110, LanguangeGuid).ConvertToMessageView();
                    result.MessageTextContent = string.Format(result.MessageTextContent, string.Join(",", allRun.Select(o => o).Distinct()));
                    result.IsWarning = true;
                }
                else
                {
                    //Alert message (Reached limit), “Run No. {Run No. 1, Run No. 2, …} liability has been reached the limit.”
                    result = _systemMessageRepository.FindByMsgId(-17341, LanguangeGuid).ConvertToMessageView();
                    result.MessageTextContent = string.Format(result.MessageTextContent, string.Join(",", allRun.Select(o => o).Distinct()));
                    result.IsWarning = true;
                }
            }
            else
            {
                result = _systemMessageRepository.FindByMsgId(0, LanguangeGuid).ConvertToMessageView(true);
            }
            return result;
        }

        public ConvertBankCleanOutTotalLiabilityResponse GetConvertBankCleanOutTotalLiability(ConvertBankCleanOutTotalLiabilityRequest request)
        {
            ConvertBankCleanOutTotalLiabilityResponse response = new ConvertBankCleanOutTotalLiabilityResponse();
            var siteGuid = request.JobModel.SiteGuid;
            var isValidConfig = GetTruckLitmitConfig(siteGuid);
            response.Message = _systemMessageRepository.FindByMsgId(-1, LanguangeGuid).ConvertToMessageView(false);
            if (isValidConfig)
            {
                var jobModels = request.JobModel.RawJobs;
                var currentExchange = _masterCurrencyRepository.GetCurrencyExchangeList(siteGuid).ToList();
                var template = _masterCommodityRepository.GetAllCommodityBySite(siteGuid);
                var rawData = _masterActualJobHeaderRepository.GetJobBCODetail(jobModels);
                //total Liability STC and covert Exchange per job
                var jobDetails = rawData.Select(o => o.CalculateJobSTC(currentExchange, template)).ToList();
                response.TotalLiabilities_STC = jobDetails.Sum(o => o.TotalLiabilities_STC);
                response.TotalCommodities_STC = jobDetails.Sum(o => o.TotalCommodities_STC);
                response.Total_STC = jobDetails.Sum(o => o.TotalJobSTC);
                response.Target_CurrencyAbb = jobDetails.FirstOrDefault()?.Target_CurrencyAbb;
                response.Message = _systemMessageRepository.FindByMsgId(0, LanguangeGuid).ConvertToMessageView(true);
            }
            return response;
        }
    }
}
