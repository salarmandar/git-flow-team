using Bgt.Ocean.Models;
using Bgt.Ocean.Service.ModelViews.PreVault;
using System;
using System.Collections.Generic;
using System.Linq;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.PreVault;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.History;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Consolidation;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.TempReport;
using Bgt.Ocean.Service.Implementations.Hubs;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using Bgt.Ocean.Models.PreVault;
using System.Xml.Linq;
using static Bgt.Ocean.Infrastructure.Helpers.SystemHelper;

namespace Bgt.Ocean.Service.Implementations.Prevault
{
    public interface ICheckOutDepartmentService
    {
        IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> CheckOutDeptItemSealConsolidateGet(CheckOutDepartmentViewModel request);
        CheckOutNonbarcodeModelView CheckOutDeptItemNonbarcodeGet(CheckOutDepartmentViewModel request);
        IEnumerable<CheckOutDeptInternalDeptModelView> InternalDepartmentGet(Guid siteGuid);
        CheckOutDepartmentResponse CheckOutToDepartmentSubmit(CheckOutDepartmentRequest request);
        IEnumerable<RouteGroupRunResourceModelView> GetRouteGroupRunResourceList(Guid siteGuid, string workDate);
    }

    public class CheckOutDepartmentService : ICheckOutDepartmentService
    {
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemService _systemService;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IMasterActualJobHeaderRepository _masterActualJobHeaderRepository;
        private readonly IMasterActualJobItemsSealRepository _masterActualJobItemsSealRepository;
        private readonly IMasterActualJobItemsCommodityRepository _masterActualJobItemsCommodityRepository;
        private readonly IMasterCustomerLocationInternalDepartmentRepository _masterCustomerLocationInternalDepartmentRepository;
        private readonly IMasterHistoryActualJobRepository _masterHistory_ActualJobRepository;
        private readonly IMasterActualJobItemsSeal_ScanHistoryRepository _masterActualJobItemsSeal_ScanHistoryRepository;
        private readonly IMasterHistory_SealRepository _masterHistory_SealRepository;
        private readonly IMasterActualJobItemsCommodity_ScanHistoryRepository _masterActualJobItemsCommodity_ScanHistoryRepository;
        private readonly IMasterConAndDeconsolidate_HeaderRepository _masterConAndDeconsolidate_HeaderRepository;
        private readonly ISystemConAndDeconsolidateStatusRepository _systemConAndDeconsolidateStatusRepository;
        private readonly ITempMainPrevaultReportRepository _tempMainPrevaultReportRepository;
        private readonly ITempPrevaultNonBarcodeReportRepository _tempPrevaultNonBarcodeReportRepository;
        private readonly ITempPrevaultSealReportRepository _tempPrevaultSealReportRepository;
        private readonly ISystemReportStyleRepository _systemReportStyleRepository;
        private readonly IMasterSiteRepository _masterSiteRepository;
        private readonly IMasterActualJobServiceStopLegsRepository _masterActualJobServiceStopLegsRepository;
        private readonly IAlarmHubService _alarmHubService;
        private readonly IMasterDailyRunResourceRepository _masterDailyRunResourceRepository;
        private readonly IMasterCommodityRepository _masterCommodityRepository;
        private readonly ISystemEnvironmentMasterCountryRepository _systemEnvironmentMasterCountryRepository;
        private readonly IMasterCustomerLocationRepository _masterCustomerLocationRepository;


        public CheckOutDepartmentService(
            ISystemService systemService,
            ISystemMessageRepository systemMessageRepository,
            IMasterActualJobHeaderRepository masterActualJobHeaderRepository,
            IMasterActualJobItemsSealRepository masterActualJobItemsSealRepository,
            IMasterActualJobItemsCommodityRepository masterActualJobItemsCommodityRepository,
            IMasterCustomerLocationInternalDepartmentRepository masterCustomerLocationInternalDepartmentRepository,
            IMasterHistoryActualJobRepository masterHistory_ActualJobRepository,
            IMasterActualJobItemsSeal_ScanHistoryRepository masterActualJobItemsSeal_ScanHistoryRepository,
            IMasterHistory_SealRepository masterHistory_SealRepository,
            IMasterCommodityRepository masterCommodityRepository,
            IMasterActualJobItemsCommodity_ScanHistoryRepository masterActualJobItemsCommodity_ScanHistoryRepository,
            IMasterConAndDeconsolidate_HeaderRepository masterConAndDeconsolidate_HeaderRepository,
            ISystemConAndDeconsolidateStatusRepository systemConAndDeconsolidateStatusRepository,
            ITempMainPrevaultReportRepository tempMainPrevaultReportRepository,
            ITempPrevaultNonBarcodeReportRepository tempPrevaultNonBarcodeReportRepository,
            ITempPrevaultSealReportRepository tempPrevaultSealReportRepository,
            ISystemReportStyleRepository systemReportStyleRepository,
            IMasterSiteRepository masterSiteRepository,
            IUnitOfWork<OceanDbEntities> uow,
            IAlarmHubService alarmHubService,
            IMasterActualJobServiceStopLegsRepository masterActualJobServiceStopLegsRepository,
            IMasterDailyRunResourceRepository masterDailyRunResourceRepository,
            ISystemEnvironmentMasterCountryRepository systemEnvironmentMasterCountryRepository,
            IMasterCustomerLocationRepository masterCustomerLocationRepository
            )
        {
            _systemService = systemService;
            _systemMessageRepository = systemMessageRepository;
            _masterActualJobHeaderRepository = masterActualJobHeaderRepository;
            _masterActualJobItemsSealRepository = masterActualJobItemsSealRepository;
            _masterActualJobItemsCommodityRepository = masterActualJobItemsCommodityRepository;
            _masterCustomerLocationInternalDepartmentRepository = masterCustomerLocationInternalDepartmentRepository;
            _masterHistory_ActualJobRepository = masterHistory_ActualJobRepository;
            _masterActualJobItemsSeal_ScanHistoryRepository = masterActualJobItemsSeal_ScanHistoryRepository;
            _masterHistory_SealRepository = masterHistory_SealRepository;
            _masterActualJobItemsCommodity_ScanHistoryRepository = masterActualJobItemsCommodity_ScanHistoryRepository;
            _masterConAndDeconsolidate_HeaderRepository = masterConAndDeconsolidate_HeaderRepository;
            _systemConAndDeconsolidateStatusRepository = systemConAndDeconsolidateStatusRepository;
            _tempMainPrevaultReportRepository = tempMainPrevaultReportRepository;
            _tempPrevaultNonBarcodeReportRepository = tempPrevaultNonBarcodeReportRepository;
            _tempPrevaultSealReportRepository = tempPrevaultSealReportRepository;
            _systemReportStyleRepository = systemReportStyleRepository;
            _masterSiteRepository = masterSiteRepository;
            _alarmHubService = alarmHubService;
            _masterActualJobServiceStopLegsRepository = masterActualJobServiceStopLegsRepository;
            _uow = uow;
            _masterDailyRunResourceRepository = masterDailyRunResourceRepository;
            _masterCommodityRepository = masterCommodityRepository;
            _systemEnvironmentMasterCountryRepository = systemEnvironmentMasterCountryRepository;
            _masterCustomerLocationRepository = masterCustomerLocationRepository;
        }

        #region ### GET ###

        #region Get internal department
        public IEnumerable<CheckOutDeptInternalDeptModelView> InternalDepartmentGet(Guid siteGuid)
        {
            return _masterCustomerLocationInternalDepartmentRepository.GetInternalDeptRoom(siteGuid).ConvertToInternalDeptModelView();
        }

        #endregion

        #region Get Item
        //1. Get seal and consolidate
        public IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> CheckOutDeptItemSealConsolidateGet(CheckOutDepartmentViewModel request)
        {
            DateTime dtWorkDate = request.WorkDate.ChangeStrToDateTime("yyyy-MM-dd");
            var result = _masterActualJobItemsSealRepository.Func_CheckOutDepartment_Seal_Consolidate_Get(
                            request.PrevaultGuid,
                            request.InternalDepartmentGuid,
                            request.FlagIncludeD,
                            request.FlagOWDOnly
                         );

            if (!request.FlagShowAll)
            {
                result = result.Where(e => e.WorkDate == dtWorkDate.Date);

                if (request.DailyRunGuid.HasValue)
                {
                    result = result.Where(e => e.MasterRunResourceDaily_Guid == request.DailyRunGuid);
                }

            }

            return result;
        }

        //2. Get non-barcode
        public CheckOutNonbarcodeModelView CheckOutDeptItemNonbarcodeGet(CheckOutDepartmentViewModel request)
        {
            CheckOutNonbarcodeModelView resultResponse = new CheckOutNonbarcodeModelView();
            DateTime dtWorkDate = request.WorkDate.ChangeStrToDateTime("yyyy-MM-dd");
            var result = _masterActualJobItemsCommodityRepository.Func_CheckOutDepartment_NonBarcode_Get(
                            request.PrevaultGuid,
                            request.InternalDepartmentGuid,
                            request.FlagIncludeD,
                            request.FlagOWDOnly
                         ).ToList();

            if (!request.FlagShowAll)
            {
                result = result.Where(e => e.WorkDate == dtWorkDate.Date).ToList();

                if (request.DailyRunGuid.HasValue)
                {
                    result = result.Where(e => e.MasterRunResourceDaily_Guid == request.DailyRunGuid).ToList();
                }
            }

            if (request.FlagGroupNonbarcode)
            {

                var sumItem = result.GroupBy(x => new { x.MasterCommodity_Guid, x.Location, x.CustomerLocationGuid })
                                              .Select(a => new PrevaultDepartmentBarcodeScanOutResult
                                              {
                                                  MasterCommodity_Guid = a.Key.MasterCommodity_Guid,
                                                  Location = a.Key.Location,
                                                  Quantity = a.Sum(q => q.Quantity)
                                              })
                                              .OrderBy(o => o.Location);



                result = result.GroupBy(g => new { g.CommodityName, g.ColumnInReport, g.MasterCommodity_Guid })
                      .Select(s => new PrevaultDepartmentBarcodeScanOutResult
                      {
                          MasterCommodity_Guid = s.Key.MasterCommodity_Guid,
                          CommodityName = s.Key.CommodityName,
                          Quantity = s.Sum(o => o.Quantity),
                          ColumnInReport = s.Key.ColumnInReport,
                          ItemsInGroup = result.Where(e => e.MasterCommodity_Guid == s.Key.MasterCommodity_Guid).ToList(),
                          ShowItemsInGroup = sumItem.Where(e => e.MasterCommodity_Guid == s.Key.MasterCommodity_Guid).ToList()
                      }).OrderBy(o => o.ColumnInReport != null ? 0 : 1).ThenBy(a => a.ColumnInReport).ToList();
            }

            resultResponse.CheckOutNonbarcodeList = result;
            resultResponse.QtySumNonbarcode = resultResponse.CheckOutNonbarcodeList.Sum(x => x.Quantity);
            return resultResponse;
        }
        #endregion

        #region Get run resource
        public IEnumerable<RouteGroupRunResourceModelView> GetRouteGroupRunResourceList(Guid siteGuid, string workDate)
        {
            DateTime workDateTime = DateTime.Parse(workDate);
            return _masterDailyRunResourceRepository.GetRouteGroupDetailByWorkDate(siteGuid, workDateTime)
                   .Select(x => new RouteGroupRunResourceModelView
                   {
                       DailyRunGuid = x.DailyRunGuid,
                       RouteGroupDetailName = x.RouteGroupDetailName,
                       DailyRunName = x.VehicleNumber,
                       DisplayText = x.RouteGroupDetailName + " - " + x.VehicleNumber + ((x.RunResourceShitf > 1) ? " (" + x.RunResourceShitf + ")" : string.Empty),
                       RouteGroupGuid = x.RouteGroupGuid,
                       RouteGroupName = x.RouteGroupName
                   });
        }
        #endregion

        #endregion ### GET ###

        #region ### SUBMIT ###
        private IEnumerable<Guid> liabilityGuidList { get; set; }
        public CheckOutDepartmentResponse CheckOutToDepartmentSubmit(CheckOutDepartmentRequest request)
        {
            CheckOutDepartmentResponse responseResult = new CheckOutDepartmentResponse();

            //--------------- Prepare Data ----------------
            IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> sealScaned = request.SealList;
            IEnumerable<PrevaultDepartmentBarcodeScanOutResult> nonbarcodeScaned = request.NonBarcodeList;
            Guid LanguageGuid = ApiSession.UserLanguage_Guid.Value;
            //--------------- Prepare Data ----------------

            CheckOutDeptItemModel itemAllScan = new CheckOutDeptItemModel();

            using (var trans = _uow.BeginTransaction())
            {
                try
                {

                    #region Check scan complete in daily run
                    if (request.DailyRunGuid.HasValue)
                    {
                        bool hasNotScanInDailyRun = HasNotCompleteScanInDailyRun(request.DailyRunGuid.GetValueOrDefault(),
                                                    request.NonBarcodeListNotScan, request.SealListNotScan);
                        if (hasNotScanInDailyRun || (!sealScaned.Any() && !nonbarcodeScaned.Any()))
                        {
                            //Msg alert -860 : Please scan all items in the Daily Run Resource to complete.
                            responseResult.MsgReponse = _systemMessageRepository.FindByMsgId(-860, LanguageGuid).ConvertToMessageView();
                            responseResult.TempReportGuid = Guid.Empty;
                            return responseResult;
                        }
                    }
                    #endregion

                    #region Check scan complete liability
                    var scanLiability = isCompleteScanLiability(sealScaned, request.SealListNotScan);
                    if (scanLiability != null && scanLiability.Any())
                    {
                        //Msg alert -713
                        responseResult.MsgReponse = _systemMessageRepository.FindByMsgId(-713, LanguageGuid).ConvertToMessageView();
                        responseResult.MsgReponse.MessageTextContent = String.Format(responseResult.MsgReponse.MessageTextContent, string.Join(",", scanLiability));
                        responseResult.TempReportGuid = Guid.Empty;
                        return responseResult;
                    }
                    #endregion

                    #region ISA                   
                    if (request.FlagISA && request.SealList.Any())
                    {
                        var resultISA = CheckRequestISA(request);
                        if (resultISA.Flagconfirmcheckout)
                        {
                            return resultISA;
                        }
                    }
                    #endregion

                    #region Get item scan inner consolidate
                    var consolidateList = sealScaned.Where(x => x.ConAndDeconsolidateHeaderGuid.HasValue);
                    if (consolidateList.Any())
                    {
                        itemAllScan = GetItemInnerConsolidate(consolidateList, request.UserName, request.LocalClientDateTime, request.UniversalDatetime);
                    }
                    #endregion                   

                    #region Get all job guid
                    var sealItemScan = sealScaned.Where(o => !o.ConAndDeconsolidateHeaderGuid.HasValue);

                    itemAllScan.SealAllGuid = itemAllScan.SealAllGuid.Union(sealItemScan.Select(o => o.Guid.GetValueOrDefault()));
                    itemAllScan.JobAllGuid = itemAllScan.JobAllGuid.Union(sealItemScan.Select(o => o.JobGuid.GetValueOrDefault()));

                    //Non-barcode
                    if (request.FlagGroupNonBarcode)
                    {
                        var innerGroup = nonbarcodeScaned.SelectMany(o => o.ItemsInGroup).Where(o => o.JobGuid.HasValue);
                        itemAllScan.NonbarcodeAllGuid = itemAllScan.NonbarcodeAllGuid.Union(innerGroup.Select(o => o.Guid));
                        itemAllScan.JobAllGuid = itemAllScan.JobAllGuid.Union(innerGroup.Select(o => o.JobGuid.GetValueOrDefault()));
                    }
                    else
                    {
                        itemAllScan.NonbarcodeAllGuid = itemAllScan.NonbarcodeAllGuid.Union(nonbarcodeScaned.Select(o => o.Guid));
                        itemAllScan.JobAllGuid = itemAllScan.JobAllGuid.Union(nonbarcodeScaned.Select(o => o.JobGuid.GetValueOrDefault()));
                    }

                    //All
                    var jobAllGuid = itemAllScan.JobAllGuid.Distinct();
                    #endregion

                    #region(OO 20.1.1) : Add alarm TFS#44748

                    var dailyRunGuid = _masterActualJobServiceStopLegsRepository
                        .FindAllAsQueryable(o => jobAllGuid.Any(x => x == o.MasterActualJobHeader_Guid)
                        && o.MasterRunResourceDaily_Guid.HasValue).Select(o => (Guid)o.MasterRunResourceDaily_Guid).Distinct().AsEnumerable();

                    var hasAlarmOnDailyRun = _alarmHubService.IsHasAlarm(dailyRunGuid);
                    if (hasAlarmOnDailyRun.Any())
                    {
                        responseResult.MsgReponse = _systemMessageRepository.FindByMsgId(-816, request.LanguageGuid).ConvertToMessageView();
                        return responseResult;
                    }
                    #endregion

                    #region Update Item Seal
                    if (itemAllScan.SealAllGuid.Any())
                    {
                        ScanSeal(itemAllScan.SealAllGuid, request, itemAllScan.SealInCon);
                    }
                    #endregion

                    #region Update Item Non-barcode
                    if (itemAllScan.NonbarcodeAllGuid.Any())
                    {
                        ScanNonBarcode(itemAllScan.NonbarcodeAllGuid, request, itemAllScan.CommInCon);
                    }
                    #endregion

                    #region Update Job Status

                    UpdateStatusJobInDepartment(jobAllGuid, request.UserName, request.LocalClientDateTime, request.UniversalDatetime);

                    #region job status partial
                    List<Guid> jobInterSectJob = new List<Guid>();
                    if (request.SealListNotScan != null || request.NonBarcodeListNotScan != null)
                    {
                        var consolidateNotScan = request.SealListNotScan.Where(x => x.ConAndDeconsolidateHeaderGuid.HasValue).Select(x => x.ConAndDeconsolidateHeaderGuid.Value);

                        IEnumerable<Guid> sealListNotScan = request.SealListNotScan.Where(x => !x.ConAndDeconsolidateHeaderGuid.HasValue)
                            .Select(o => o.JobGuid.GetValueOrDefault());
                        IEnumerable<Guid> nonListNotScan = request.NonBarcodeListNotScan.Select(x => x.JobGuid.GetValueOrDefault());

                        var jobNotScan = new CheckOutDeptItemModel();
                        var allJobNotScan =
                            jobNotScan.JobAllGuid.Union(sealListNotScan)
                            .Union(nonListNotScan)
                            .Union(GetItemInnerConsolidateNotScan(consolidateNotScan).JobAllGuid);

                        jobInterSectJob.AddRange(allJobNotScan.Where(e => itemAllScan.JobAllGuid.Contains(e))); //.ToList();
                        if (jobInterSectJob.Any())
                        {
                            UpdateStatusJobPartial(jobInterSectJob, request.UserName, request.LocalClientDateTime, request.UniversalDatetime);
                        }
                    }
                    #endregion

                    #region Update history
                    IEnumerable<TblMasterHistory_ActualJob> historyJob = jobAllGuid.Select(o => new TblMasterHistory_ActualJob
                    {
                        Guid = Guid.NewGuid(),
                        MasterActualJobHeader_Guid = o,
                        /*64 :Change job status to "In department" by ocean online
                          749 : Change job status to "Partial In department" by ocean online*/
                        MsgID = jobInterSectJob.Any(x => o == x) ? 749 : 64,   //jobNotScanComplete.Any(x => o == x) ? 749 : 64,
                        UserCreated = request.UserName,
                        DatetimeCreated = request.LocalClientDateTime,
                        UniversalDatetimeCreated = request.UniversalDatetime
                    }).ToList();
                    _masterHistory_ActualJobRepository.VirtualCreateRange(historyJob);
                    #endregion

                    #endregion

                    #region Insert Temp Report
                    var SealScanGuid = itemAllScan.SealAllGuid;
                    var NonScanGuid = itemAllScan.NonbarcodeAllGuid;
                    int[] rptStyleId = { 12, 16 };
                    List<TblSystemReportStyle> rptStyle = _systemReportStyleRepository.FindReportStyleByStyleID(rptStyleId).ToList();
                    Guid tempReportGuid = InsertCheckOutDeptTempReport(request, SealScanGuid, NonScanGuid, rptStyle, request.FlagGroupNonBarcode);
                    #endregion

                    _masterActualJobItemsSeal_ScanHistoryRepository.CreateVirtualRangeToDbContext();
                    _masterActualJobItemsSeal_ScanHistoryRepository.CreateVirtualRangeToDbContext();
                    _masterHistory_SealRepository.CreateVirtualRangeToDbContext();
                    _masterHistory_ActualJobRepository.CreateVirtualRangeToDbContext();
                    _tempMainPrevaultReportRepository.CreateVirtualRangeToDbContext();

                    responseResult.MsgReponse = _systemMessageRepository.FindByMsgId(0, LanguageGuid).ConvertToMessageView();
                    responseResult.TempReportGuid = tempReportGuid;

                    _uow.Commit();
                    trans.Complete();

                }
                catch (Exception ex)
                {
                    // OO error logger
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                    responseResult.MsgReponse = _systemMessageRepository.FindByMsgId(-184, LanguageGuid).ConvertToMessageView();
                }
                return responseResult;
            }

        }

        #region ISA

        private CheckOutDepartmentResponse CheckRequestISA(CheckOutDepartmentRequest request)
        {
            CheckOutDepartmentResponse responseResult = new CheckOutDepartmentResponse();
            responseResult.Flagconfirmcheckout = false;
            responseResult.MsgReponse = new ModelViews.Systems.SystemMessageView();
            //If the country configuration “Integrate with ISA” (AppKey: FlagEnableISA) is ON
            var boolFlagEnableISA = _systemEnvironmentMasterCountryRepository.FindAppkeyValueByEnumAppkeyName(request.BrinkSiteGuid, EnumAppKey.FlagEnableISA);

            if (boolFlagEnableISA)
            {
                var siteData = _masterSiteRepository.FindById(request.BrinkSiteGuid);
                var roomData = _masterCustomerLocationInternalDepartmentRepository.FindById(request.InternalDepartmentGuid);
                var configServicePath = _systemEnvironmentMasterCountryRepository.GetValueByAppKeyAndCountry(EnumAppKey.ISAWebServicePath, new List<Guid> { siteData.MasterCountry_Guid, Guid.Empty }).OrderByDescending(e => e.MasterCountry_Guid).FirstOrDefault();
                var ISASecretKey = _systemEnvironmentMasterCountryRepository.GetValueByAppKeyAndCountry(EnumAppKey.ISASecretKey, new List<Guid> { siteData.MasterCountry_Guid, Guid.Empty }).OrderByDescending(e => e.MasterCountry_Guid).FirstOrDefault();

                if (configServicePath.AppValue1 == "") //If there is no service path
                {
                    //Display an error message "Please configure ISA web service path in Country Configuration screen."
                    responseResult.MsgReponse = _systemMessageRepository.FindByMsgId(-17342, request.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(false);
                    responseResult.MsgReponse.IsWarning = true;
                    responseResult.Flagconfirmcheckout = true;
                    return responseResult;
                }

                if (siteData.ISACashCenterCode == null) //if there is no ISACashCenterCode
                {
                    //Display an error message "Please configure cash center code in Brink’s Site standard table."
                    responseResult.MsgReponse = _systemMessageRepository.FindByMsgId(-17351, request.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(false);
                    responseResult.MsgReponse.IsWarning = true;
                    responseResult.Flagconfirmcheckout = true;
                    return responseResult;
                }

                var ListCustomerLocationGuid = request.SealList.Select(e => e.CustomerLocationGuid).Distinct().ToList();
                var ListCustomerLocation = _masterCustomerLocationRepository.FindAllAsQueryable(e => ListCustomerLocationGuid.Contains(e.Guid)).ToList();
                if (ListCustomerLocation.Any(e => string.IsNullOrEmpty(e.ISAClientCode)))//7.1.2.If there is no client code in some location
                {
                    //Display an error message "Please configure ISA Client Code of the below location(S) on Customer Location standard table. · {list of customer location name}"
                    responseResult.CustomerLocationList = ListCustomerLocation.Where(e => string.IsNullOrEmpty(e.ISAClientCode)).Select(e => new CheckOutDepartmentCustomerLocationListModel() { BranchCodeReference = e.BranchCodeReference, BranchName = e.BranchName }).ToList();
                    responseResult.MsgReponse = _systemMessageRepository.FindByMsgId(-17343, request.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(false);
                    responseResult.MsgReponse.IsWarning = true;
                    responseResult.Flagconfirmcheckout = true;
                    return responseResult;
                }

                //7.1.3.Otherwise, send the authentication request and the bag information to ISA. 
                var ISAresult = SendToISA(configServicePath.AppValue1, request, siteData.ISACashCenterCode, ListCustomerLocation, ISASecretKey.AppValue1, roomData.ISAInternalRouteCode);
                if (ISAresult.FlagError) //Call ISA
                {
                    //7.1.3.1.If cannot connect or ISA returns false, then display the below confirmation popup. 
                    //7.1.3.1.2.Message: “Cannot sync data to ISA due to the following error: { return message }.” 
                    //7.1.3.1.1.Title: “Confirmation” 7.1.3.1.3.Cancel button: on click, close the popup.   
                    var message = _systemMessageRepository.FindByMsgId(-17344, request.UserLanguageGuid.GetValueOrDefault()).ConvertToMessageView(false);
                    message.MessageTextContent = string.Format(message.MessageTextContent, ISAresult.errorEx);
                    responseResult.MsgReponse = message;
                    responseResult.MsgReponse.IsWarning = true;
                    responseResult.Flagconfirmcheckout = true;
                    return responseResult;
                }

            }
            //7.1.3.2.If sync successfully, then checks out to department(as is)
            return responseResult;
        }

        private String GetSignedTimeStamp(byte[] secret, String timeStamp)
        {
            var sha256 = System.Security.Cryptography.SHA256.Create();

            // Get the UTC bytes
            var utcNowBytes = Encoding.ASCII.GetBytes(timeStamp);

            var completeBytes = secret.Concat(utcNowBytes).ToArray();

            // Create the hash and get a base64 string of it.
            return Convert.ToBase64String(sha256.ComputeHash(completeBytes));
        }

        private class ISAResult
        {
            public string result { get; set; }
            public string errorEx { get; set; }
            public bool FlagError { get; set; }
        }

        private ISAResult SendToISA(string UrlPath, CheckOutDepartmentRequest request, decimal? CashCentreCode, List<TblMasterCustomerLocation> CusloList, string ISASecretKey, string ISAInternalRouteCode)
        {
            try
            {
                var TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                byte[] bytes = Encoding.Default.GetBytes(ISASecretKey);
                var SignedTimeStamp = GetSignedTimeStamp(bytes, TimeStamp);

                var ISAAuth = new ISAAuthRequest.Envelope
                {
                    Header = new ISAAuthRequest.Header(),
                    Body = new ISAAuthRequest.Body()
                    {
                        Authenticate = new ISAAuthRequest.Authenticate
                        {
                            type = "ApiKey",
                            tokens = new ISAAuthRequest.Token
                            {
                                AuthToken = new List<ISAAuthRequest.AuthToken>()
                                {
                                    new ISAAuthRequest.AuthToken() {
                                        Name = "ApiKey",
                                        Value = "qHDBSK2lwqS59DK4MvW6S2g2e9eeDkz5WHAyJQ8L"
                                    } ,
                                    new ISAAuthRequest.AuthToken() {
                                         Name = "TimeStamp",
                                         Value = TimeStamp
                                    } ,
                                    new ISAAuthRequest.AuthToken() {
                                         Name = "SignedTimeStamp",
                                         Value = SignedTimeStamp
                                    } ,
                                    new ISAAuthRequest.AuthToken() {
                                         Name = "CashCentreCode",
                                         Value = CashCentreCode.Value.ToString()
                                    }
                                }
                            }
                        }
                    }
                };

                XmlSerializer mySerializer = new XmlSerializer(typeof(ISAAuthRequest.Envelope));
                var builder = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = true,
                    OmitXmlDeclaration = true,
                };
                using (var writer = XmlWriter.Create(builder, settings))
                {
                    mySerializer.Serialize(writer, ISAAuth, new XmlSerializerNamespaces());
                }

                var result = CallSOAP(UrlPath + "Services/Reception", "http://tempuri.org/IBaseService/Authenticate", builder.ToString()); // Call Authenticate

                if (result.FlagError)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SOAPErrorModel.WebServiceErrorCollection));
                    var docerror = XDocument.Parse(result.errorEx);
                    var getError = docerror.Root.Descendants("{http://schemas.xmlsoap.org/soap/envelope/}Fault").Elements("detail").Nodes().FirstOrDefault().ToString();
                    using (StringReader reader = new StringReader(getError))
                    {
                        var DeError = (SOAPErrorModel.WebServiceErrorCollection)serializer.Deserialize(reader);
                        return new ISAResult() { FlagError = true, errorEx = DeError.Errors.WebServiceError.Category + " " + DeError.Errors.WebServiceError.Error }; // true = error
                    }
                }
                var doc = XDocument.Parse(result.result);
                XNamespace ns = "http://tempuri.org/";
                var getSession = doc.Root.Descendants(ns + "AuthenticateResponse").Elements(ns + "AuthenticateResult").FirstOrDefault();
                if (!String.IsNullOrEmpty(getSession.Value.ToString()))
                {
                    var ISAPassingInfo = new ISAPassingInfoModel.Envelope
                    {
                        Header = new ISAPassingInfoModel.Header(),
                        Body = new ISAPassingInfoModel.Body()
                        {
                            CreateTrackAndTraceData = new ISAPassingInfoModel.CreateTrackAndTraceData()
                            {
                                token = new ISAPassingInfoModel.Token()
                                {
                                    SessionId = getSession.Value,
                                    TimeStamp = TimeStamp,
                                    SignedTimeStamp = SignedTimeStamp
                                },
                                trackAndTraceData = new ISAPassingInfoModel.trackAndTraceData()
                                {
                                    DeliveryDate = request.LocalClientDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                                    RouteCode = ISAInternalRouteCode,
                                    TrackAndTraceContainers = new ISAPassingInfoModel.TrackAndTraceContainers()
                                    {
                                        TrackAndTraceServiceContainersSM = request.SealList.Select(e => new ISAPassingInfoModel.TrackAndTraceServiceContainersSM()
                                        {
                                            ContainerId = e.SealNo,
                                            Value = Convert.ToDouble(e.STC).ToString("###0"),
                                            ClientCode = CusloList.FirstOrDefault(f => f.Guid == e.CustomerLocationGuid).ISAClientCode ?? "",
                                            ContentHierarchyCode = (e.JobNo == "") ? 2 : 1,
                                            HierarchyGroup = "Standard"
                                        }).ToList()
                                    }
                                }
                            }
                        }
                    };

                    mySerializer = new XmlSerializer(typeof(ISAPassingInfoModel.Envelope));
                    builder = new StringBuilder();
                    using (var writer = XmlWriter.Create(builder, settings))
                    {
                        mySerializer.Serialize(writer, ISAPassingInfo, new XmlSerializerNamespaces());
                    }

                    result = CallSOAP(UrlPath + "Services/Reception", "http://tempuri.org/IReceptionService/CreateTrackAndTraceData", builder.ToString()); // Passing Bag Information
                    if (result.FlagError)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(SOAPErrorModel.WebServiceErrorCollection));
                        var docerror = XDocument.Parse(result.errorEx);
                        var getError = docerror.Root.Descendants("{http://schemas.xmlsoap.org/soap/envelope/}Fault").Elements("detail").Nodes().FirstOrDefault().ToString();
                        using (StringReader reader = new StringReader(getError))
                        {
                            var DeError = (SOAPErrorModel.WebServiceErrorCollection)serializer.Deserialize(reader);
                            return new ISAResult() { FlagError = true, errorEx = DeError.Errors.WebServiceError.Category + " " + DeError.Errors.WebServiceError.Error }; // true = error
                        }
                    }
                    doc = XDocument.Parse(result.result);
                    var getTracking = doc.Root.Descendants(ns + "CreateTrackAndTraceDataResponse").Elements(ns + "CreateTrackAndTraceDataResult").FirstOrDefault();
                    if (!String.IsNullOrEmpty(getTracking.Value.ToString()))
                    {
                        // save trackingCode to jobSeal
                        using (var trans = _uow.BeginTransaction())
                        {
                            var SealGuidList = request.SealList.Select(e => e.Guid.Value).Distinct().ToList();
                            var dbSeal = _masterActualJobItemsSealRepository.FindAllAsQueryable(e => SealGuidList.Contains(e.Guid));
                            foreach (var itemseal in dbSeal)
                            {
                                itemseal.ISATrackingID = getTracking.Value;
                                itemseal.ISACheckoutUser = request.UserName;
                                itemseal.ISACheckoutDateTime = request.LocalClientDateTime;
                            }

                            _uow.Commit();
                            trans.Complete();
                        }
                    }

                }
                return new ISAResult() { FlagError = false }; // true = error
            }
            catch (Exception ex)
            {
                return new ISAResult() { FlagError = true, errorEx = ex.Message }; // true = error
            }
        }

        private ISAResult CallSOAP(string SOAPUrl, string SOAPAction, string xmlBody)
        {
            //Making Web Request    
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SOAPUrl);
            //SOAPAction    
            request.Headers.Add(@"SOAPAction:" + SOAPAction);
            //Content_type    
            request.ContentType = "text/xml;charset=\"utf-8\"";
            request.Accept = "text/xml";
            //HTTP method    
            request.Method = "POST";

            XmlDocument SOAPReqBody = new XmlDocument();
            //SOAP Body Request    
            SOAPReqBody.LoadXml(xmlBody);

            using (Stream stream = request.GetRequestStream())
            {
                SOAPReqBody.Save(stream);
            }
            try
            {
                //Geting response from request    
                using (WebResponse Serviceres = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))
                    {
                        //reading stream    
                        var ServiceResult = rd.ReadToEnd();
                        return new ISAResult() { result = ServiceResult, FlagError = false };
                    }
                }
            }
            catch (WebException wex)
            {
                var pageContent = new StreamReader(wex.Response.GetResponseStream())
                                      .ReadToEnd();
                return new ISAResult() { errorEx = pageContent, FlagError = true };
            }
        }

        #endregion

        #region Check Scan Complete Liability      
        private IEnumerable<string> isCompleteScanLiability(IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> sealScanned, IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> sealNotScan)
        {
            liabilityGuidList = sealScanned.Where(o => o.LiabilityGuid.HasValue).Select(o => o.LiabilityGuid.Value).Distinct();
            IEnumerable<string> sealInLiability = sealNotScan.Where(o => liabilityGuidList.Contains(o.LiabilityGuid.GetValueOrDefault())).Select(x => x.SealNo);
            return sealInLiability;
        }
        #endregion

        #region Scan Item
        private CheckOutDeptItemModel GetItemInnerConsolidate(IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> consolidateItem, string userName, DateTime clientDateTime, DateTimeOffset offsetDatetime)
        {
            CheckOutDeptItemModel itemInConResult = new CheckOutDeptItemModel();

            //Get consolidate status : 8 (Consolidate out vault)
            Guid consolidateStatusGuid = _systemConAndDeconsolidateStatusRepository.GetConsolidateStatus(8);

            //ConsolidationTypeID : 1 (Consolidate Location), 2 (Consolidate Route), 5 (Consolidate Interbranch)
            //Get job inner location consolidate
            itemInConResult.SealInCon = _masterActualJobItemsSealRepository.GetItemInCon(consolidateItem.Select(x => x.ConAndDeconsolidateHeaderGuid.GetValueOrDefault()));
            itemInConResult.CommInCon = _masterActualJobItemsCommodityRepository.GetItemInCon(consolidateItem.Select(x => x.ConAndDeconsolidateHeaderGuid.GetValueOrDefault()));

            var sealInnerCon = itemInConResult.SealInCon.Select(o => new { o.Guid, o.MasterActualJobHeader_Guid });
            var commodityInnerCon = itemInConResult.CommInCon.Select(o => new { o.Guid, o.MasterActualJobHeader_Guid });

            itemInConResult.SealAllGuid = sealInnerCon.Select(o => o.Guid);
            itemInConResult.NonbarcodeAllGuid = commodityInnerCon.Select(o => o.Guid);

            itemInConResult.JobAllGuid = sealInnerCon.Select(o => o.MasterActualJobHeader_Guid.GetValueOrDefault())
                .Union(commodityInnerCon.Select(o => o.MasterActualJobHeader_Guid.GetValueOrDefault()));

            #region Update Status Consolidate
            var consolidateGuid = consolidateItem.Where(o => o.ConAndDeconsolidateHeaderGuid.HasValue).Select(o => o.ConAndDeconsolidateHeaderGuid.Value);
            _masterConAndDeconsolidate_HeaderRepository.UpdateStatusConsolidate_CheckOutDept(consolidateGuid,
                consolidateStatusGuid, userName, clientDateTime, offsetDatetime);

            #endregion

            return itemInConResult;
        }

        private CheckOutDeptItemModel GetItemInnerConsolidateNotScan(IEnumerable<Guid> consolidateItemNotScan)
        {
            CheckOutDeptItemModel itemInConNotScanResult = new CheckOutDeptItemModel();

            //Get consolidate status : 8 (Consolidate out vault)
            //ConsolidationTypeID : 1 (Consolidate Location), 2 (Consolidate Route), 5 (Consolidate Interbranch)
            //Get job inner location consolidate                        
            var sealInnerCon = _masterActualJobItemsSealRepository.GetItemInCon(consolidateItemNotScan);
            var commodityInnerCon = _masterActualJobItemsCommodityRepository.GetItemInCon(consolidateItemNotScan);

            var sealGuid = sealInnerCon.Select(o => o.Guid);
            itemInConNotScanResult.SealAllGuid = sealGuid;
            itemInConNotScanResult.NonbarcodeAllGuid = commodityInnerCon.Select(o => o.Guid);

            var jobInSeal = sealInnerCon.Select(o => o.MasterActualJobHeader_Guid.GetValueOrDefault());
            var jobInCom = commodityInnerCon.Select(o => o.MasterActualJobHeader_Guid.GetValueOrDefault());
            itemInConNotScanResult.JobAllGuid = jobInSeal
                .Union(jobInCom);

            return itemInConNotScanResult;
        }

        private void ScanSeal(IEnumerable<Guid> sealScanedData, CheckOutDepartmentRequest request, IEnumerable<TblMasterActualJobItemsSeal> sealInCon)
        {
            _masterActualJobItemsSealRepository.UpdateSeal(sealScanedData, request.InternalDepartmentGuid, request.UserName, request.LocalClientDateTime, request.UniversalDatetime);

            #region History
            IEnumerable<TblMasterActualJobItemsSeal_ScanHistory> tblhistoryseal = sealScanedData.Select(o => new TblMasterActualJobItemsSeal_ScanHistory
            {
                Guid = Guid.NewGuid(),
                MasterActualJobItemsSeal_Guid = o,
                ClientHostNameScan = SystemHelper.ClientHostName,
                ClientUserNameScan = request.UserName,
                ClientDateTimeScan = request.LocalClientDateTime,
                UniversalDatetimeCreated = request.UniversalDatetime,
                MasterCustomerLocation_InternalDepartment_Guid = request.InternalDepartmentGuid,
                MasterCustomerLocation_InternalDepartmentArea_Guid = null,
                MasterCustomerLocation_InternalDepartmentSubArea_Guid = null
            });
            _masterActualJobItemsSeal_ScanHistoryRepository.VirtualCreateRange(tblhistoryseal);

            var sealWithLocation = sealScanedData.Join(request.SealList.Where(x => !x.ConAndDeconsolidateHeaderGuid.HasValue),
                s => s,
                l => l.Guid,
                (s, l) => new CheckOutDeptSealHistoryModel
                {
                    JobGuid = l.JobGuid,
                    SealGuid = s,
                    SealNo = l.SealNo,
                    LocationGuid = l.CustomerLocationGuid,
                    Location = l.Location,
                    FlagKeyIn = l.FlagKeyIn
                });

            IEnumerable<CheckOutDeptSealHistoryModel> sealWithConLoc = Enumerable.Empty<CheckOutDeptSealHistoryModel>();
            if (sealInCon != null && sealInCon.Any())
            {
                sealWithConLoc = sealInCon.Join(request.SealList.Where(x => x.ConAndDeconsolidateHeaderGuid.HasValue),
                s => s.MasterConAndDeconsolidateHeaderMasterID_Guid,
                m => m.ConAndDeconsolidateHeaderGuid,
                (s, m) => new CheckOutDeptSealHistoryModel
                {
                    JobGuid = s.MasterActualJobHeader_Guid,
                    SealGuid = s.Guid,
                    SealNo = s.SealNo,
                    LocationGuid = m.CustomerLocationGuid,
                    Location = m.Location,
                    FlagKeyIn = m.FlagKeyIn,
                    MasterIDLocGuid = s?.MasterConAndDeconsolidateHeaderMasterID_Guid,
                    MasterIDLocName = s?.Master_ID
                });
            }


            var sealAllWithLocation = sealWithLocation.Union(sealWithConLoc).ToList();

            IEnumerable<TblMasterHistory_ActualJob> tblHistoryjob = sealAllWithLocation.Select(o => new TblMasterHistory_ActualJob
            {
                Guid = Guid.NewGuid(),
                MasterActualJobHeader_Guid = o.JobGuid,
                MsgID = 485,//Scan Out {0}  to internal department : {1} of Brinks site {2}  in Location name {3} by Ocean Online.
                MsgParameter = new string[] { "seal no " + o.SealNo + " (" + o.ScanType + ")",
                    request.InternalDepartmentName, request.BrinkSiteName, o.Location }.ToJSONString(),
                //string.Join(",", "seal no " + o.l.SealNo, request.InternalDepartmentName, request.BrinkSiteName, o.l.Location),
                UserCreated = request.UserName,
                DatetimeCreated = request.LocalClientDateTime,
                UniversalDatetimeCreated = request.UniversalDatetime
            });
            _masterHistory_ActualJobRepository.VirtualCreateRange(tblHistoryjob);

            IEnumerable<TblMasterHistory_Seal> insertHistorySeal = sealAllWithLocation.Select(o => new TblMasterHistory_Seal()
            {
                Guid = Guid.NewGuid(),
                SealNo = o.SealNo,
                MsgID = 485,//Scan Out {0}  to internal department : {1} of Brinks site {2}  in Location name {3} by Ocean Online.
                MsgParameter = new string[] { "seal no " + o.SealNo + " (" + o.ScanType + ")",
                    request.InternalDepartmentName, request.BrinkSiteName, o.Location }.ToJSONString(),
                //string.Join(",", "seal no " + o.l.SealNo, request.InternalDepartmentName, request.BrinkSiteName, o.l.Location),
                UserCreated = request.UserName,
                DatetimeCreated = request.LocalClientDateTime,
                UniversalDatetimeCreated = request.UniversalDatetime,
                //TFS#68700 -> Add for tracking seal
                Site_Guid = request.BrinkSiteGuid,
                MasterActualJobItemsSeal_Guid = o.SealGuid
            });
            _masterHistory_SealRepository.VirtualCreateRange(insertHistorySeal);

            #endregion
        }

        private void ScanNonBarcode(IEnumerable<Guid> nonbarcodeScanedData, CheckOutDepartmentRequest request, IEnumerable<TblMasterActualJobItemsCommodity> commInCon)
        {
            _masterActualJobItemsCommodityRepository.UpdateNonbarcode(nonbarcodeScanedData, request.InternalDepartmentGuid, request.UserName, request.LocalClientDateTime, request.UniversalDatetime);

            #region History
            IEnumerable<TblMasterActualJobItemsCommodity_ScanHistory> tblhistorycomodity = nonbarcodeScanedData.Select(o => new TblMasterActualJobItemsCommodity_ScanHistory()
            {
                Guid = Guid.NewGuid(),
                MasterActualJobItemsCommodity_Guid = o,
                ClientHostNameScan = SystemHelper.ClientHostName,
                ClientUserNameScan = request.UserName,
                ClientDateTimeScan = request.LocalClientDateTime,
                UniversalDatetimeCreated = request.UniversalDatetime,
                MasterCustomerLocation_InternalDepartment_Guid = request.InternalDepartmentGuid,
                MasterCustomerLocation_InternalDepartmentArea_Guid = null,
                MasterCustomerLocation_InternalDepartmentSubArea_Guid = null
            });
            _masterActualJobItemsCommodity_ScanHistoryRepository.VirtualCreateRange(tblhistorycomodity);

            var nonbarcodeList = request.FlagGroupNonBarcode ? request.NonBarcodeList.SelectMany(x => x.ItemsInGroup) : request.NonBarcodeList;
            var nonbarcodeWithLocation = nonbarcodeScanedData.Join(nonbarcodeList,
                n => n,
                l => l.Guid,
                (n, l) => new CheckOutDeptCommodityHistoryModel
                {
                    JobGuid = l.JobGuid,
                    ItemCommGuid = l.Guid,
                    CommodityName = l.CommodityName
                });

            IEnumerable<CheckOutDeptCommodityHistoryModel> nonbarcodeInConWithLocation = Enumerable.Empty<CheckOutDeptCommodityHistoryModel>();
            if (commInCon != null && commInCon.Any())
            {
                nonbarcodeInConWithLocation = commInCon.Join(_masterCommodityRepository.FindAll(),
                n => n.MasterCommodity_Guid,
                c => c.Guid,
                (n, c) => new CheckOutDeptCommodityHistoryModel
                {
                    JobGuid = n.MasterActualJobHeader_Guid,
                    ItemCommGuid = n.Guid,
                    CommodityName = c.CommodityName
                });
            }

            var commodityAllWithLocation = nonbarcodeWithLocation.Union(nonbarcodeInConWithLocation);

            IEnumerable<TblMasterHistory_ActualJob> tblHistoryjob = commodityAllWithLocation.Select(o => new TblMasterHistory_ActualJob()
            {
                Guid = Guid.NewGuid(),
                MasterActualJobHeader_Guid = o.JobGuid,
                MsgID = 76,
                MsgParameter = new string[] { "commodity : " + o.CommodityName, request.InternalDepartmentName }.ToJSONString(),
                //string.Join(", ", "commodity : " + o.CommodityName, request.InternalDepartmentName),
                UserCreated = request.UserName,
                DatetimeCreated = request.LocalClientDateTime,
                UniversalDatetimeCreated = request.UniversalDatetime
            });
            _masterHistory_ActualJobRepository.VirtualCreateRange(tblHistoryjob);
            #endregion
        }
        #endregion

        #region Update Job Status
        private void UpdateStatusJobInDepartment(IEnumerable<Guid> jobGuidInDept, string userModify, DateTime clientDatetime, DateTimeOffset offsetDatetime)
        {
            _masterActualJobHeaderRepository.UpdateJobStatus(jobGuidInDept, 11, userModify, clientDatetime, offsetDatetime);

        }

        private void UpdateStatusJobPartial(IEnumerable<Guid> jobGuidPartial, string userModify, DateTime clientDatetime, DateTimeOffset offsetDatetime)
        {
            _masterActualJobHeaderRepository.UpdateJobStatus(jobGuidPartial, 39, userModify, clientDatetime, offsetDatetime);

        }
        #endregion

        #region Insert temp report
        #region Temp main report
        private Guid InsertCheckOutDeptTempReport(CheckOutDepartmentRequest request, IEnumerable<Guid> SealScanGuid, IEnumerable<Guid> CommScanGuid, List<TblSystemReportStyle> rptStyle, bool FlagGroupNonBarcode)
        {
            int countSealItem = request.SealList.Count(o => o.FlagScan);
            Guid tempMainGuid = Guid.NewGuid();
            TblTempMainPrevaultReport newMainTempReport = new TblTempMainPrevaultReport()
            {
                Guid = tempMainGuid,
                ReceiverName = request.UserName,
                DateTimeScan = request.LocalClientDateTime,
                MasterSiteFrom_Guid = request.BrinkSiteGuid,
                ExpectedQty = countSealItem,
                ActualQty = countSealItem,
                FromName = "Pre Vault",
                ToName = request.InternalDepartmentName,
                RptAction = "PVToInt",
                ScanSeq = 1,
                InternalDepartment_Guid = request.InternalDepartmentGuid,
            };
            _tempMainPrevaultReportRepository.VirtualCreate(newMainTempReport);

            if (SealScanGuid.Any())
            {
                var sealNotCon = request.SealList.Where(o => !o.ConAndDeconsolidateHeaderGuid.HasValue);
                Guid rptStyleId = rptStyle.FirstOrDefault(o => o.ReportStyleID == 12).Guid;
                InsertCheckOutDeptTempReport_Seal(tempMainGuid, SealScanGuid, sealNotCon, rptStyleId);
            }

            if (CommScanGuid.Any())
            {
                var nonNotCon = request.NonBarcodeList;
                Guid rptStyleId = rptStyle.FirstOrDefault(o => o.ReportStyleID == 16).Guid;
                InsertCheckOutDeptTempReport_Nonbarcode(tempMainGuid, CommScanGuid, nonNotCon, request.BrinkSiteGuid, rptStyleId, FlagGroupNonBarcode);
            }

            return tempMainGuid;
        }
        #endregion

        #region Temp seal report
        private void InsertCheckOutDeptTempReport_Seal(Guid tempMainRptGuid, IEnumerable<Guid> sealScanedList, IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> sealNotConList, Guid rptStyleId)
        {
            _tempPrevaultSealReportRepository.InsertTempSealConReport(sealScanedList, tempMainRptGuid, rptStyleId);
            _tempPrevaultSealReportRepository.InsertTempSealReport(sealNotConList, tempMainRptGuid, rptStyleId);
        }
        #endregion

        #region Temp nonbarcode report
        private void InsertCheckOutDeptTempReport_Nonbarcode(Guid tempMainRptGuid, IEnumerable<Guid> nonScanedList, IEnumerable<PrevaultDepartmentBarcodeScanOutResult> nonNotConScanList, Guid siteGuid, Guid rptStyleId, bool FlagGroupNonBarcode)
        {
            Guid countryGuid = _masterSiteRepository.GetCountryGuidByMasterSiteGuid(siteGuid);
            _tempPrevaultNonBarcodeReportRepository.InsertTempNonReport(nonNotConScanList, tempMainRptGuid, rptStyleId, countryGuid, FlagGroupNonBarcode);
            _tempPrevaultNonBarcodeReportRepository.InsertTempNonConReport(nonScanedList, tempMainRptGuid, rptStyleId, countryGuid, FlagGroupNonBarcode);

        }
        #endregion

        #endregion

        //Validate check in partial item when choose route
        private bool HasNotCompleteScanInDailyRun(Guid dailyRunGuid, IEnumerable<PrevaultDepartmentBarcodeScanOutResult> nonNotScan, IEnumerable<PrevaultDepartmentSealConsolidateScanOutResult> sealNotScan)
        {
            //Seal Items
            bool hasSealNotScanInRun = false;
            if (sealNotScan != null)
            {
                hasSealNotScanInRun = sealNotScan.Any(e => e.MasterRunResourceDaily_Guid == dailyRunGuid);
            }

            //Non-barcode Items
            bool hasNonNotScanInRun = false;
            if (nonNotScan != null)
            {
                hasNonNotScanInRun = nonNotScan.Any(e => e.MasterRunResourceDaily_Guid == dailyRunGuid);
            }
            return (hasSealNotScanInRun || hasNonNotScanInRun);
        }

        #endregion ### SUBMIT ###

    }
}
