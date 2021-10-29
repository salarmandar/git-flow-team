using Bgt.Ocean.Models.RunControl;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.ModelViews.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using Bgt.Ocean.Service.Messagings.RunControlService;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models.Masters;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Mobile;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Service.Implementations.Hubs;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Consolidation;
using Newtonsoft.Json;
using Bgt.Ocean.Service.Implementations.Adhoc;
using Bgt.Ocean.Service.Messagings.AdhocService;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.Implementations.TruckLiabilityLimit;
using Bgt.Ocean.Models.RunControl.LiabilityLimitModel;
using Bgt.Ocean.Service.Messagings.TruckLiabilityLimit;
using Bgt.Ocean.Repository.EntityFramework.Repositories.History;
using System.Collections.Concurrent;

namespace Bgt.Ocean.Service.Implementations.RunControl
{
    public interface IRunControlService
    {
        //Product Backlog Item 25284:Run Control: Allow user to override and Close the Run Manually
        SystemMessageView UpdateCloseRunManually(ManuallyCloseRunRequest request);

        //Product Backlog Item 26642:Modify Job Property in Run Control to support Cash Add Workflow
        JobPropertiesResponse GetCashAddJobProperites(JobPropertiesRequest request);

        SystemMessageView IsJobEmptyByDailyRun(Guid dailyRunGuid);
        IEnumerable<string> GetJobCannotClose(Guid runDailyGuid, bool isDolphin);

        SystemMessageView GetValidateJobCannotClose(Guid dailyRunGuid, Guid langGuid, string vehicleNo);

        TruckToTruckTransferResponse TruckToTruckTransferReady(TruckToTruckRequest request, Uri requestUri);
        TruckToTruckHoldoverJobResponse TruckToTruckHoldoverJob(TruckToTruckRequest request, Uri requestUri);
        TruckToTruckTransferResponse TruckToTruckTransferJob(TruckToTruckRequest request, Uri requestUri);
        ValidateTruckLiabilityLimitResponse ValidateTruckLiabilityLimit(ValidateTruckLiabilityLimitRequest request);

        ValidateAssignJobsToRunResponse ValidateAssignJobsToRun(ValidateAssignJobsToRunRequest request);
    }

    public class RunControlService : IRunControlService
    {
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemService _systemService;
        private readonly IAlarmHubService _alarmHubService;
        private readonly IAdhocService _adhocService;
        private readonly ITruckLiabilityLimitService _truckLiabilityLimitService;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IMasterCommodityRepository _masterCommodityRepository;
        private readonly ISystemJobStatusRepository _systemJobStatusRepository;
        private readonly IMasterMCSBulkRetractRepository _masterMCSBulkRetractRepository;
        private readonly IMobileATMCheckListEERepository _mobileATMCheckListEERepository;
        private readonly ISystemServiceJobTypeRepository _systemServiceJobTypeRepository;
        private readonly IMasterActualJobHeaderRepository _masterActualJobHeaderRepository;
        private readonly IMasterHistoryActualJobRepository _masterHistoryActualJobRepository;
        private readonly IMasterActualJobItemsSealRepository _masterActualJobItemsSealRepository;
        private readonly IMasterMCSBulkSuspectFakeRepository _masterMCSBulkSuspectFakeRepository;
        private readonly IMasterActualJobSumCashAddRepository _masterActualJobSumCashAddRepository;
        private readonly IMasterMCSBulkJammedDetailRepository _masterMCSBulkJammedDetailRepository;
        private readonly IMasterMCSBulkDepositReportRepository _masterMCSBulkDepositReportRepository;
        private readonly IMasterActualJobMCSBulkJammedRepository _masterActualJobMCSBulkJammedRepository;
        private readonly IMasterActualJobSumCashReturnRepository _masterActualJobSumCashReturnRepository;
        private readonly IMasterActualJobItemsCommodityRepository _masterActualJobItemsCommodityRepository;
        private readonly IMasterActualJobMCSCoinCashAddRepository _masterActualJobMCSCoinCashAddRepository;
        private readonly IMasterActualJobSumActualCountRepository _masterActualJobSumActualCountRepository;
        private readonly IMasterActualJobServiceStopLegsRepository _masterActualJobServiceStopLegsRepository;
        private readonly IMasterMCSBulkSuspectFakeDetailRepository _masterMCSBulkSuspectFakeDetailRepository;
        private readonly ISystemEnvironmentMasterCountryRepository _systemEnvironmentMasterCountryRepository;
        private readonly IMasterActualJobHeaderCapabilityRepository _masterActualJobHeaderCapabilityRepository;
        private readonly IMasterActualJobSumMachineReportRepository _masterActualJobSumMachineReportRepository;
        private readonly IMasterActualJobItemDiscrapenciesRepository _masterActualJobItemDiscrapenciesRepository;
        private readonly IMasterActualJobMCSCoinCashReturnRepository _masterActualJobMCSCoinCashReturnRepository;
        private readonly IMasterActualJobMCSCoinSuspectFakeRepository _masterActualJobMCSCoinSuspectFakeRepository;
        private readonly IMasterActualJobMCSCoinMachineBalanceRepository _masterActualJobMCSCoinMachineBalanceRepository;
        private readonly IMasterActualJobMCSCoinBulkNoteCollectRepository _masterActualJobMCSCoinBulkNoteCollectRepository;
        private readonly IMasterActualJobMCSRecyclingActualCountRepository _masterActualJobMCSRecyclingActualCountRepository;
        private readonly IMasterActualJobMCSRecyclingCashRecyclingRepository _masterActualJobMCSRecyclingCashRecyclingRepository;
        private readonly IMasterActualJobMCSRecyclingMachineReportRepository _masterActualJobMCSRecyclingMachineReportRepository;
        private readonly IMasterDailyRunResourceRepository _masterDailyRunResource;
        private readonly IMasterConAndDeconsolidate_HeaderRepository _masterConAndDeconsolidateHeaderRepository;
        private readonly IMasterRouteGroupDetailRepository _masterRouteGroupDetailRepository;
        private readonly ISystemConAndDeconsolidateStatusRepository _systemConAndDeconsolidateStatusRepository;
        private readonly IMasterHistoryDailyRunResourceSignatureTruckToTruckTransferRepository _masterHistoryDailyRunResourceSignatureTruckToTruckTransferRepository;
        private readonly IMasterHistory_DailyRunResourceRepository _masterHistoryDailyRunResourceRepository;
        private readonly IMasterHistory_DolphinAssignToAnotherRunRepository _masterHistoryDolphinAssignToAnotherRunRepository;

        public RunControlService(
         IUnitOfWork<OceanDbEntities> uow,
         ISystemService systemService,
         IAlarmHubService alarmHubService,
         IAdhocService adhocService,
         ITruckLiabilityLimitService truckLiabilityLimitService,
         ISystemMessageRepository systemMessageRepository,
         IMasterCommodityRepository masterCommodityRepository,
         ISystemJobStatusRepository systemJobStatusRepository,
         IMasterMCSBulkRetractRepository masterMCSBulkRetractRepository,
         IMobileATMCheckListEERepository mobileATMCheckListEERepository,
         ISystemServiceJobTypeRepository systemServiceJobTypeRepository,
         IMasterActualJobHeaderRepository masterActualJobHeaderRepository,
         IMasterHistoryActualJobRepository masterHistoryActualJobRepository,
         IMasterActualJobItemsSealRepository masterActualJobItemsSealRepository,
         IMasterMCSBulkSuspectFakeRepository masterMCSBulkSuspectFakeRepository,
         IMasterActualJobSumCashAddRepository masterActualJobSumCashAddRepository,
         IMasterMCSBulkJammedDetailRepository masterMCSBulkJammedDetailRepository,
         IMasterMCSBulkDepositReportRepository masterMCSBulkDepositReportRepository,
         IMasterActualJobMCSBulkJammedRepository masterActualJobMCSBulkJammedRepository,
         IMasterActualJobSumCashReturnRepository masterActualJobSumCashReturnRepository,
         IMasterActualJobItemsCommodityRepository masterActualJobItemsCommodityRepository,
         IMasterActualJobMCSCoinCashAddRepository masterActualJobMCSCoinCashAddRepository,
         IMasterActualJobSumActualCountRepository masterActualJobSumActualCountRepository,
         IMasterActualJobServiceStopLegsRepository masterActualJobServiceStopLegsRepository,
         IMasterMCSBulkSuspectFakeDetailRepository masterMCSBulkSuspectFakeDetailRepository,
         ISystemEnvironmentMasterCountryRepository systemEnvironmentMasterCountryRepository,
         IMasterActualJobHeaderCapabilityRepository masterActualJobHeaderCapabilityRepository,
         IMasterActualJobSumMachineReportRepository masterActualJobSumMachineReportRepository,
         IMasterActualJobItemDiscrapenciesRepository masterActualJobItemDiscrapenciesRepository,
         IMasterActualJobMCSCoinCashReturnRepository masterActualJobMCSCoinCashReturnRepository,
         IMasterActualJobMCSCoinSuspectFakeRepository masterActualJobMCSCoinSuspectFakeRepository,
         IMasterActualJobMCSCoinMachineBalanceRepository masterActualJobMCSCoinMachineBalanceRepository,
         IMasterActualJobMCSCoinBulkNoteCollectRepository masterActualJobMCSCoinBulkNoteCollectRepository,
         IMasterActualJobMCSRecyclingActualCountRepository masterActualJobMCSRecyclingActualCountRepository,
         IMasterActualJobMCSRecyclingCashRecyclingRepository masterActualJobMCSRecyclingCashRecyclingRepository,
         IMasterActualJobMCSRecyclingMachineReportRepository masterActualJobMCSRecyclingMachineReportRepository,
         IMasterDailyRunResourceRepository masterDailyRunResourceRepository,
         IMasterConAndDeconsolidate_HeaderRepository masterConAndDeconsolidateHeaderRepository,
         IMasterRouteGroupDetailRepository masterRouteGroupDetailRepository,
         ISystemConAndDeconsolidateStatusRepository systemConAndDeconsolidateStatusRepository,
         IMasterHistoryDailyRunResourceSignatureTruckToTruckTransferRepository masterHistoryDailyRunResourceSignatureTruckToTruckTransferRepository,
         IMasterHistory_DailyRunResourceRepository masterHistoryDailyRunResourceRepository,
         IMasterHistory_DolphinAssignToAnotherRunRepository masterHistoryDolphinAssignToAnotherRunRepository)
        {
            _uow = uow;
            _systemService = systemService;
            _alarmHubService = alarmHubService;
            _adhocService = adhocService;
            _truckLiabilityLimitService = truckLiabilityLimitService;
            _systemMessageRepository = systemMessageRepository;
            _masterCommodityRepository = masterCommodityRepository;
            _systemJobStatusRepository = systemJobStatusRepository;
            _masterMCSBulkRetractRepository = masterMCSBulkRetractRepository;
            _mobileATMCheckListEERepository = mobileATMCheckListEERepository;
            _systemServiceJobTypeRepository = systemServiceJobTypeRepository;
            _masterActualJobHeaderRepository = masterActualJobHeaderRepository;
            _masterHistoryActualJobRepository = masterHistoryActualJobRepository;
            _masterActualJobItemsSealRepository = masterActualJobItemsSealRepository;
            _masterMCSBulkSuspectFakeRepository = masterMCSBulkSuspectFakeRepository;
            _masterActualJobSumCashAddRepository = masterActualJobSumCashAddRepository;
            _masterMCSBulkJammedDetailRepository = masterMCSBulkJammedDetailRepository;
            _masterMCSBulkDepositReportRepository = masterMCSBulkDepositReportRepository;
            _masterActualJobMCSBulkJammedRepository = masterActualJobMCSBulkJammedRepository;
            _masterActualJobSumCashReturnRepository = masterActualJobSumCashReturnRepository;
            _masterActualJobItemsCommodityRepository = masterActualJobItemsCommodityRepository;
            _masterActualJobMCSCoinCashAddRepository = masterActualJobMCSCoinCashAddRepository;
            _masterActualJobSumActualCountRepository = masterActualJobSumActualCountRepository;
            _masterActualJobServiceStopLegsRepository = masterActualJobServiceStopLegsRepository;
            _masterMCSBulkSuspectFakeDetailRepository = masterMCSBulkSuspectFakeDetailRepository;
            _systemEnvironmentMasterCountryRepository = systemEnvironmentMasterCountryRepository;
            _masterActualJobHeaderCapabilityRepository = masterActualJobHeaderCapabilityRepository;
            _masterActualJobSumMachineReportRepository = masterActualJobSumMachineReportRepository;
            _masterActualJobItemDiscrapenciesRepository = masterActualJobItemDiscrapenciesRepository;
            _masterActualJobMCSCoinCashReturnRepository = masterActualJobMCSCoinCashReturnRepository;
            _masterActualJobMCSCoinSuspectFakeRepository = masterActualJobMCSCoinSuspectFakeRepository;
            _masterActualJobMCSCoinMachineBalanceRepository = masterActualJobMCSCoinMachineBalanceRepository;
            _masterActualJobMCSCoinBulkNoteCollectRepository = masterActualJobMCSCoinBulkNoteCollectRepository;
            _masterActualJobMCSRecyclingActualCountRepository = masterActualJobMCSRecyclingActualCountRepository;
            _masterActualJobMCSRecyclingCashRecyclingRepository = masterActualJobMCSRecyclingCashRecyclingRepository;
            _masterActualJobMCSRecyclingMachineReportRepository = masterActualJobMCSRecyclingMachineReportRepository;
            _masterDailyRunResource = masterDailyRunResourceRepository;
            _masterConAndDeconsolidateHeaderRepository = masterConAndDeconsolidateHeaderRepository;
            _masterRouteGroupDetailRepository = masterRouteGroupDetailRepository;
            _systemConAndDeconsolidateStatusRepository = systemConAndDeconsolidateStatusRepository;
            _masterHistoryDailyRunResourceSignatureTruckToTruckTransferRepository = masterHistoryDailyRunResourceSignatureTruckToTruckTransferRepository;
            _masterHistoryDailyRunResourceRepository = masterHistoryDailyRunResourceRepository;
            _masterHistoryDolphinAssignToAnotherRunRepository = masterHistoryDolphinAssignToAnotherRunRepository;
        }

        #region product Backlog Item 25284:Run Control: Allow user to override and Close the Run Manually
        public SystemMessageView UpdateCloseRunManually(ManuallyCloseRunRequest request)
        {
            try
            {
                if (_alarmHubService.IsHasAlarm(Enumerable.Repeat(request.dailyRun, 1)).Any())
                {
                    return _systemMessageRepository.FindByMsgId(-814, request.languageGuid).ConvertToMessageView();
                }
                var status = _masterActualJobHeaderRepository.CloseJobsAndRun(request.dailyRun, request.userGuid, ApiSession.ClientDatetime.DateTime);
                return _systemMessageRepository.FindByMsgId(status, request.languageGuid).ConvertToMessageView();
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);

                return _systemMessageRepository.FindByMsgId(-184, request.languageGuid).ConvertToMessageView();
            }
        }
        #endregion

        #region Product Backlog Item 26642:Modify Job Property in Run Control to support Cash Add Workflow
        public JobPropertiesResponse GetCashAddJobProperites(JobPropertiesRequest request)
        {
            JobPropertiesResponse result = new JobPropertiesResponse();

            try
            {
                var prop = GetJobPopertiesView(request);

                prop.tabDetail = GetTabDetail(request);
                prop.tabLeg = GetTabLeg(request);
                prop.tabServiceDetail = GetTabSeviceDetail(request);
                prop.tabHistory = GetTabHistory(request);
                result.SVD_AvailableTab = GetSvdAvailableTab(request, prop.tabDetail.Machine_Guid);
                result.JobProperties = prop;

            }
            catch (Exception)
            {
                result.SetMessageView(_systemMessageRepository.FindByMsgId(-184, request.LanguageGuid).ConvertToMessageView());
            }
            return result;
        }

        private IEnumerable<CashAddAvailableTab> GetSvdAvailableTab(JobPropertiesRequest request, Guid? Machine_Guid)
        {

            List<CashAddAvailableTab> Tabs = new List<CashAddAvailableTab>();

            if (request.TabID == CashAddPropertiesTab.tabDetail)
            {
                #region Prepare Condition
                var capability = _masterActualJobHeaderCapabilityRepository.FindCapabilityIDByJobGuid(request.JobGuid);
                bool flagIsCashAddSubService = _masterActualJobHeaderRepository.FindSubServiceTypeIDByJobGuid(request.JobGuid) == SubServiceTypeHelper.CashAdd;
                bool flagHasTranferSafe = _masterActualJobHeaderRepository.GetMachineTransferSafeModel(Machine_Guid) != null;
                var hidePanels = _masterActualJobHeaderRepository.GetJobScreenMapping(request.JobGuid);

                #endregion

                //Parent Tab
                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabDetail, CashAddPropertiesTab.None, false);
                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabLeg, CashAddPropertiesTab.None, false);
                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabServiceDetail, CashAddPropertiesTab.None, true);
                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabHistory, CashAddPropertiesTab.None, false);

                #region ## Child of Service Detail

                var flagShowCITDelivery = flagHasTranferSafe && flagIsCashAddSubService;
                if (flagShowCITDelivery)
                    //machine has transfer safe associated + Note withdraw + Cash Add
                    CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabSVD_CITDelivery, CashAddPropertiesTab.tabServiceDetail, false);

                foreach (var capId in capability)
                {
                    switch (capId)
                    {
                        case Capability.NoteWithdraw:
                            if (flagIsCashAddSubService)  //Capability = Note Withdraw + Cash Add
                            {
                                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabSVD_NoteWithdraw_MachineReport_ActualCount, CashAddPropertiesTab.tabServiceDetail, false);
                                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabSVD_NoteWithdraw_CashAdd_CashReturn, CashAddPropertiesTab.tabServiceDetail, false);
                            }
                            break;
                        case Capability.Recycling:
                            if (flagIsCashAddSubService) //Capability = Note Withdraw + Cash Add
                            {
                                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabSVD_Recycling_MachineReportWODispense_ActualCount, CashAddPropertiesTab.tabServiceDetail, false);
                                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabSVD_Recycling_CashRecycling, CashAddPropertiesTab.tabServiceDetail, false);
                            }
                            break;
                        case Capability.BulkNoteDeposit:
                            if (flagIsCashAddSubService) //Capability = Note Withdraw + Cash Add
                            {
                                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabSVD_BulkNoteDeposit_DepositReport_Retract, CashAddPropertiesTab.tabServiceDetail, false);
                                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabSVD_BulkNoteDeposit_SuspectFake, CashAddPropertiesTab.tabServiceDetail, false);
                                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabSVD_BulkNoteDeposit_Jammed, CashAddPropertiesTab.tabServiceDetail, false);
                            }
                            break;
                        case Capability.CoinExchange:
                            if (flagIsCashAddSubService) //Capability = Note Withdraw + Cash Add
                            {
                                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabSVD_CoinExchange_MachineBalance_CashAdd, CashAddPropertiesTab.tabServiceDetail, false);
                                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabSVD_CoinExchange_CashReturn_BulkNote, CashAddPropertiesTab.tabServiceDetail, false);
                                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabSVD_CoinExchange_SuspectFake, CashAddPropertiesTab.tabServiceDetail, false);
                            }
                            break;
                        case Capability.SmallBagDeposit: //Capability = Small Bag Deposit + MCS
                            CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabSVD_SmallBagDeposit_SmallBag, CashAddPropertiesTab.tabServiceDetail, false);
                            break;
                        default:
                            break;
                    }
                }

                //MCS Alway Show 
                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabSVD_CapturedCard, CashAddPropertiesTab.tabServiceDetail, false);
                CashAddAvailableTab.CreateTab(Tabs, hidePanels, CashAddPropertiesTab.tabSVD_Checklist, CashAddPropertiesTab.tabServiceDetail, false);
                #endregion
            }

            return Tabs.OrderBy(o => o.TabID);
        }
        private JobPropertiesView GetJobPopertiesView(JobPropertiesRequest request)
        {
            JobPropertiesView prop = new JobPropertiesView();
            string jobStatusName = string.Empty;
            var jobHead = _masterActualJobHeaderRepository.FindById(request.JobGuid);

            if (jobHead != null)
            {
                jobStatusName = _systemJobStatusRepository.FindByStatusID(jobHead.SystemStatusJobID).StatusJobName;
                prop.JobGuid = jobHead.Guid;
                prop.JobNo = jobHead.JobNo;
                prop.JobStatusName = jobStatusName;
            }

            return prop;
        }
        private TabDetailView GetTabDetail(JobPropertiesRequest request)
        {
            TabDetailView detail = new TabDetailView();
            if (request.TabID == CashAddPropertiesTab.tabDetail)
            {
                detail = _masterActualJobHeaderRepository.GetCashAddJobDetail(request.JobGuid);
                if (detail != null)
                {
                    var TranferSafe = _masterActualJobHeaderRepository.GetMachineTransferSafeModel(detail.Machine_Guid);
                    var LockSeq1 = TranferSafe?.SFOTblMasterMachine_LockType.FirstOrDefault(o => o.LockSeq == 1)?.SFOTblSystemLockType.LockTypeName;
                    var LockSeq2 = TranferSafe?.SFOTblMasterMachine_LockType.FirstOrDefault(o => o.LockSeq == 2)?.SFOTblSystemLockType.LockTypeName;
                    var LockType2 = (LockSeq1 + "," + LockSeq2).Trim(',');

                    detail.TranferSafeID = TranferSafe?.MachineID ?? "-";
                    detail.LockType2 = string.IsNullOrWhiteSpace(LockType2) ? "-" : LockType2;
                    detail.WorkDate = detail.ServiceStopTransectionDate.ChangeFromDateToString();
                    detail.ScheduleTime = detail.WindowsTimeServiceTimeStart == null ? "00:00" : detail.WindowsTimeServiceTimeStart.Value.ToString("HH:mm");
                }
            }
            return detail;
        }
        private TabLegView GetTabLeg(JobPropertiesRequest request)
        {
            TabLegView leg = new TabLegView();
            if (request.TabID == CashAddPropertiesTab.tabLeg)
            {
                var legDetail = _masterActualJobServiceStopLegsRepository.GetLegsDetail(request.JobGuid);
                leg.LegList = legDetail.Select(o =>
                               {
                                   o.ActualTime = o.ActualTimeDT.ChangFromDateToTimeString();
                                   o.ArrivalTime = o.ArrivalTimeDT.ChangFromDateToTimeString();
                                   o.DepartTime = o.DepartTimeDT.ChangFromDateToTimeString();
                                   o.WorkDate = o.ServiceStopTransectionDate.ChangeFromDateToString();
                                   return o;
                               });
            }

            return leg;
        }

        private TabServiceDetailView GetTabSeviceDetail(JobPropertiesRequest request)
        {

            TabServiceDetailView result = new TabServiceDetailView();
            IEnumerable<CommodityView> MCtemplate = _masterCommodityRepository.GetCCBySite(request.SiteGuid, false, true);
            var flagOrderMCSAscending = _systemEnvironmentMasterCountryRepository.FindAppkeyValueByEnumAppkeyName(request.SiteGuid, EnumAppKey.FlagOrderMCSAscending);
            switch (request.TabID)
            {
                #region [1] CIT Delivery
                case CashAddPropertiesTab.tabSVD_CITDelivery:
                    SvdCitDelviery CITDel = new SvdCitDelviery();

                    //GET CITDelivery
                    CITDel.LiabilityList = _masterActualJobHeaderRepository.GetCitDeliveryView(request.JobGuid);

                    //SET CITDelivery
                    result.SVD_CITDelivery = CITDel;
                    break;
                #endregion

                #region [2] Note Withdraw
                case CashAddPropertiesTab.tabSVD_NoteWithdraw_MachineReport_ActualCount:

                    //SET MR,AC
                    var MR = _masterActualJobSumMachineReportRepository.GetMachineReportDetail(request.JobGuid);
                    MR.CassetteList = MR.CassetteList.OrderBy(o => o.CassetteSequence);
                    result.SVD_MachineReport = MR;

                    var AC = _masterActualJobSumActualCountRepository.GetActualCountDetail(request.JobGuid);
                    AC.CassetteList = AC.CassetteList.OrderBy(o => o.CassetteSequence);
                    result.SVD_ActualCount = AC;
                    break;
                case CashAddPropertiesTab.tabSVD_NoteWithdraw_CashAdd_CashReturn:

                    //SET CA,CR
                    var CA = _masterActualJobSumCashAddRepository.GetCashAddDetail(request.JobGuid);
                    CA.CassetteList = CA.CassetteList.OrderBy(o => o.CassetteSequence);
                    result.SVD_CashAdd = CA;

                    var CR = _masterActualJobSumCashReturnRepository.GetCashReuturnDetail(request.JobGuid);
                    CR.CassetteList = flagOrderMCSAscending ? CR.CassetteList.OrderBy(o => o.DenominationValue) : CR.CassetteList.OrderByDescending(o => o.DenominationValue);
                    result.SVD_CashReturn = CR;
                    break;

                #endregion

                #region [3] Recycling
                case CashAddPropertiesTab.tabSVD_Recycling_MachineReportWODispense_ActualCount:

                    // 99%
                    var Recycling_MachineReportWODispense = _masterActualJobMCSRecyclingMachineReportRepository.GetRecyclingMachineReportWODispense(request.JobGuid);
                    Recycling_MachineReportWODispense.MachinReportDispenseList = Recycling_MachineReportWODispense.MachinReportDispenseList.OrderBy(o => o.CassetteSequence);
                    result.SVD_Recycling_MachineReportWODispense = Recycling_MachineReportWODispense;

                    // 99%
                    var Recycling_ActualCount = _masterActualJobMCSRecyclingActualCountRepository.GetRecyclingActualCount(request.JobGuid);
                    Recycling_ActualCount.ActualCountList = Recycling_ActualCount.ActualCountList.OrderBy(o => o.CassetteSequence);
                    result.SVD_Recycling_ActualCount = Recycling_ActualCount;

                    break;
                case CashAddPropertiesTab.tabSVD_Recycling_CashRecycling:

                    // 99%
                    var Recycling_CashRecycling = _masterActualJobMCSRecyclingCashRecyclingRepository.GetRecyclingCashRecycling(request.JobGuid);
                    Recycling_CashRecycling.CassetteList = Recycling_CashRecycling.CassetteList.OrderBy(o => o.CassetteSequence);
                    result.SVD_Recycling_CashRecycling = Recycling_CashRecycling;
                    break;
                #endregion

                #region [4] Bulk Note Deposit
                case CashAddPropertiesTab.tabSVD_BulkNoteDeposit_DepositReport_Retract:

                    // 99%
                    var BulkNoteDeposit_DepositCollectionReport = _masterMCSBulkDepositReportRepository.GetBulkNoteDepositDepositReport(request.JobGuid);
                    BulkNoteDeposit_DepositCollectionReport.CassetteList = BulkNoteDeposit_DepositCollectionReport.CassetteList.OrderBy(o => o.CassetteSequence);
                    result.SVD_BulkNoteDeposit_DepositReport = BulkNoteDeposit_DepositCollectionReport;

                    // 99%
                    var BulkNoteDeposit_Retract = _masterMCSBulkRetractRepository.GetBulkNoteDepositRetract(request.JobGuid);
                    BulkNoteDeposit_Retract.DenoList = flagOrderMCSAscending ?
                        BulkNoteDeposit_Retract.DenoList.OrderBy(o => o.DenominationValue) :
                        BulkNoteDeposit_Retract.DenoList.OrderByDescending(o => o.DenominationValue);
                    result.SVD_BulkNoteDeposit_Retract = BulkNoteDeposit_Retract;

                    break;
                case CashAddPropertiesTab.tabSVD_BulkNoteDeposit_SuspectFake:

                    // 99%
                    var BulkNoteDeposit_SuspectFake = _masterMCSBulkSuspectFakeRepository.GetBulkNoteDepositSuspectFake(request.JobGuid);
                    BulkNoteDeposit_SuspectFake.DenoList = flagOrderMCSAscending ?
                        BulkNoteDeposit_SuspectFake.DenoList.OrderBy(o => o.DenominationValue) :
                        BulkNoteDeposit_SuspectFake.DenoList.OrderByDescending(o => o.DenominationValue);
                    result.SVD_BulkNoteDeposit_SuspectFake = BulkNoteDeposit_SuspectFake;

                    // 99%
                    var BulkNoteDeposit_SuspectFakeDetail = _masterMCSBulkSuspectFakeDetailRepository.GetBulkNoteDepositSuspectFakeDetail(request.JobGuid);
                    result.SVD_BulkNoteDeposit_SuspectFakeDetail = BulkNoteDeposit_SuspectFakeDetail;

                    break;
                case CashAddPropertiesTab.tabSVD_BulkNoteDeposit_Jammed:

                    // 99%
                    var BulkNoteDeposit_Jammed = _masterActualJobMCSBulkJammedRepository.GetBulkNoteDepositJammed(request.JobGuid);
                    BulkNoteDeposit_Jammed.DenoList = flagOrderMCSAscending ?
                        BulkNoteDeposit_Jammed.DenoList.OrderBy(o => o.DenominationValue) :
                        BulkNoteDeposit_Jammed.DenoList.OrderByDescending(o => o.DenominationValue);
                    result.SVD_BulkNoteDeposit_Jammed = BulkNoteDeposit_Jammed;

                    // 99%
                    var BulkNoteDeposit_JammedDetail = _masterMCSBulkJammedDetailRepository.GetBulkNoteDepositJammedDetail(request.JobGuid);
                    result.SVD_BulkNoteDeposit_JammedDetail = BulkNoteDeposit_JammedDetail;

                    break;
                #endregion

                #region [5] Coin Exchange
                case CashAddPropertiesTab.tabSVD_CoinExchange_MachineBalance_CashAdd:

                    // 99%
                    var CoinExchange_MachineBalance = _masterActualJobMCSCoinMachineBalanceRepository.GetCoinExchangeMachineBalance(request.JobGuid);
                    CoinExchange_MachineBalance.MachineBalanceHopperList = CoinExchange_MachineBalance.MachineBalanceHopperList.OrderBy(o => o.CassetteSequence);
                    CoinExchange_MachineBalance.MachineBalanceNoteList = CoinExchange_MachineBalance.MachineBalanceNoteList.OrderBy(o => o.CassetteSequence);
                    CoinExchange_MachineBalance.MachineBalanceCoinList = CoinExchange_MachineBalance.MachineBalanceCoinList.OrderBy(o => o.CassetteSequence);
                    result.SVD_CoinExchange_MachineBalance = CoinExchange_MachineBalance;

                    // 99%
                    var CoinExchange_CashAdd = _masterActualJobMCSCoinCashAddRepository.GetCoinExchangeCashAdd(request.JobGuid);
                    CoinExchange_CashAdd.CashAddHopperList = CoinExchange_CashAdd.CashAddHopperList.OrderBy(o => o.CassetteSequence);
                    CoinExchange_CashAdd.CashAddNoteList = CoinExchange_CashAdd.CashAddNoteList.OrderBy(o => o.CassetteSequence);
                    CoinExchange_CashAdd.CashAddCoinList = CoinExchange_CashAdd.CashAddCoinList.OrderBy(o => o.CassetteSequence);
                    result.SVD_CoinExchange_CashAdd = CoinExchange_CashAdd;

                    break;
                case CashAddPropertiesTab.tabSVD_CoinExchange_CashReturn_BulkNote:

                    // 99%
                    var CoinExchange_CashReturn = _masterActualJobMCSCoinCashReturnRepository.GetCoinExchangeCashReturn(request.JobGuid);
                    CoinExchange_CashReturn.CashReturnHopperList = CoinExchange_CashReturn.CashReturnHopperList.OrderBy(o => o.CassetteSequence);
                    CoinExchange_CashReturn.CashReturnNoteList = CoinExchange_CashReturn.CashReturnNoteList.OrderBy(o => o.CassetteSequence);
                    CoinExchange_CashReturn.CashReturnCoinList = CoinExchange_CashReturn.CashReturnCoinList.OrderBy(o => o.CassetteSequence);
                    result.SVD_CoinExchange_CashReturn = CoinExchange_CashReturn;

                    // 99% 
                    var CoinExchange_BulkNoteCollection = _masterActualJobMCSCoinBulkNoteCollectRepository.GetCoinExchangeBulkNote(request.JobGuid);
                    CoinExchange_BulkNoteCollection.BulkNoteCollectionList = flagOrderMCSAscending ?
                        CoinExchange_BulkNoteCollection.BulkNoteCollectionList.OrderBy(o => o.DenominationValue) :
                        CoinExchange_BulkNoteCollection.BulkNoteCollectionList.OrderByDescending(o => o.DenominationValue);
                    result.SVD_CoinExchange_BulkNote = CoinExchange_BulkNoteCollection;
                    break;
                case CashAddPropertiesTab.tabSVD_CoinExchange_SuspectFake:

                    // 99%
                    var SuspectFake_CoinExchange = _masterActualJobMCSCoinSuspectFakeRepository.GetCoinExchangeSuspectFake(request.JobGuid);
                    SuspectFake_CoinExchange.SuspectFakeList = flagOrderMCSAscending ?
                        SuspectFake_CoinExchange.SuspectFakeList.OrderBy(o => o.DenominationValue) :
                        SuspectFake_CoinExchange.SuspectFakeList.OrderByDescending(o => o.DenominationValue);
                    result.SVD_CoinExchange_SuspectFake = SuspectFake_CoinExchange;
                    break;
                #endregion

                #region [6] Small Bag Deposit
                case CashAddPropertiesTab.tabSVD_SmallBagDeposit_SmallBag:
                    var items = _masterActualJobHeaderRepository.GetMasterIDCollection(request.JobGuid, SealTypeID.SmallBagDeposit, MCtemplate);
                    result.SVD_SmallBagDeposit_SmallBag = new SvdSmallBagDepositSmallBag()
                    {
                        ItemsList = items.Where(o => o.MasterID != "-" || o.MasterID_Route != "-")
                    };
                    result.svD_SmallBagDeposit_SmallBagCollection = new SvdSmallBagDepositSmallBagCollection()
                    {
                        ItemsList = items.Where(o => o.MasterID == "-" && o.MasterID_Route == "-").SelectMany(o => o.SealList).Distinct().OrderBy(o => o.SealNo)
                    };
                    break;

                #endregion

                #region [7] Other
                case CashAddPropertiesTab.tabSVD_CapturedCard:
                    //GET CC
                    SvdCapturedCard CC = new SvdCapturedCard();
                    CC.CapturedCardList = _masterActualJobHeaderRepository.GetCaptureCardByJobGuid(request.JobGuid);
                    CC.DelToBankBranchList = _masterActualJobHeaderRepository.GetBankBrachSeal(request.JobGuid, SealTypeID.DeliverToBankBranch);
                    CC.DelToMainBankBranchList = _masterActualJobHeaderRepository.GetBankBrachSeal(request.JobGuid, SealTypeID.DeliverToMainBankBranch);

                    var jobHead = _masterActualJobHeaderRepository.FindById(request.JobGuid);
                    CC.FlagDeliverCardBankBranch = CC.DelToBankBranchList.Any() || jobHead.FlagDeliverCardBankBranch;
                    CC.FlagDeliverCardMainBank = CC.DelToMainBankBranchList.Any() || jobHead.FlagDeliverCardMainBank;
                    //SET CC
                    result.SVD_CapturedCard = CC;
                    break;
                case CashAddPropertiesTab.tabSVD_Checklist:
                    //GET CL
                    SvdChecklist CL = new SvdChecklist();
                    CL.CheckList = _mobileATMCheckListEERepository.GetCheckListEE(request.JobGuid);
                    CL.Remarks = _masterActualJobHeaderRepository.FindById(request.JobGuid)?.Remarks;
                    //SET CL
                    result.SVD_Checklist = CL;
                    break;
                #endregion

                default:
                    break;
            }
            return result;
        }

        private TabServiceDetailView MSCDummy(TabServiceDetailView result, CashAddPropertiesTab TabID)
        {
            switch (TabID)
            {

                #region [3] Recycling
                case CashAddPropertiesTab.tabSVD_Recycling_MachineReportWODispense_ActualCount:

                    //Dummy
                    var Recycling_MachineReportWODispense = AutoFixture.CreateDummy<SvdRecyclingMachineReportWODispense>();
                    Recycling_MachineReportWODispense.MachinReportDispenseList = AutoFixture.CreateDummy<IEnumerable<MRDRCTransectionView>>();
                    result.SVD_Recycling_MachineReportWODispense = Recycling_MachineReportWODispense;

                    //Dummy
                    var Recycling_ActualCount = AutoFixture.CreateDummy<SvdRecyclingActualCount>();
                    Recycling_ActualCount.ActualCountList = AutoFixture.CreateDummy<IEnumerable<ACRCTransectionView>>();
                    result.SVD_Recycling_ActualCount = Recycling_ActualCount;

                    break;
                case CashAddPropertiesTab.tabSVD_Recycling_CashRecycling:

                    //Dummy
                    var Recycling_CashRecycling = AutoFixture.CreateDummy<SvdRecyclingCashRecycling>();
                    Recycling_CashRecycling.CassetteList = AutoFixture.CreateDummy<IEnumerable<RRCTransectionView>>();
                    Recycling_CashRecycling.ReturnCashBagList = AutoFixture.CreateDummy<IEnumerable<SealBagView>>();
                    result.SVD_Recycling_CashRecycling = Recycling_CashRecycling;
                    break;
                #endregion

                #region [4] Bulk Note Deposit
                case CashAddPropertiesTab.tabSVD_BulkNoteDeposit_DepositReport_Retract:

                    //Dummy
                    var BulkNoteDeposit_DepositCollectionReport = AutoFixture.CreateDummy<SvdBulkNoteDepositDepositReport>();
                    BulkNoteDeposit_DepositCollectionReport.CassetteList = AutoFixture.CreateDummy<IEnumerable<DCRBNDTransectionView>>();
                    BulkNoteDeposit_DepositCollectionReport.DepositReturnBagList = AutoFixture.CreateDummy<IEnumerable<SealBagView>>();
                    result.SVD_BulkNoteDeposit_DepositReport = BulkNoteDeposit_DepositCollectionReport;


                    //Dummy
                    var BulkNoteDeposit_Retract = AutoFixture.CreateDummy<SvdBulkNoteDepositRetract>();
                    BulkNoteDeposit_Retract.DenoList = AutoFixture.CreateDummy<IEnumerable<RBNDTransectionView>>();
                    BulkNoteDeposit_Retract.RetractReturnBagList = AutoFixture.CreateDummy<IEnumerable<SealBagView>>();
                    result.SVD_BulkNoteDeposit_Retract = BulkNoteDeposit_Retract;

                    break;
                case CashAddPropertiesTab.tabSVD_BulkNoteDeposit_SuspectFake:

                    //Dummy
                    var BulkNoteDeposit_SuspectFake = AutoFixture.CreateDummy<SvdBulkNoteDepositSuspectFake>();
                    BulkNoteDeposit_SuspectFake.DenoList = AutoFixture.CreateDummy<IEnumerable<SFBNDTransectionView>>();
                    result.SVD_BulkNoteDeposit_SuspectFake = BulkNoteDeposit_SuspectFake;


                    //Dummy
                    var BulkNoteDeposit_SuspectFakeDetail = AutoFixture.CreateDummy<SvdBulkNoteDepositSuspectFakeDetail>();
                    BulkNoteDeposit_SuspectFakeDetail.SuspectFakeDetailList = AutoFixture.CreateDummy<IEnumerable<SuspectFakeDetailView>>();
                    BulkNoteDeposit_SuspectFakeDetail.SuspectFakeBagList = AutoFixture.CreateDummy<IEnumerable<SealBagView>>();
                    result.SVD_BulkNoteDeposit_SuspectFakeDetail = BulkNoteDeposit_SuspectFakeDetail;

                    break;
                case CashAddPropertiesTab.tabSVD_BulkNoteDeposit_Jammed:

                    //Dummy
                    var BulkNoteDeposit_Jammed = AutoFixture.CreateDummy<SvdBulkNoteDepositJammed>();
                    BulkNoteDeposit_Jammed.DenoList = AutoFixture.CreateDummy<IEnumerable<JBNDTransectionView>>();
                    result.SVD_BulkNoteDeposit_Jammed = BulkNoteDeposit_Jammed;

                    //Dummy
                    var BulkNoteDeposit_JammedDetail = AutoFixture.CreateDummy<SvdBulkNoteDepositJammedDetail>();
                    BulkNoteDeposit_JammedDetail.JammedDetailList = AutoFixture.CreateDummy<IEnumerable<JBNDDetailView>>();
                    BulkNoteDeposit_JammedDetail.JammedBagList = AutoFixture.CreateDummy<IEnumerable<SealBagView>>();
                    result.SVD_BulkNoteDeposit_JammedDetail = BulkNoteDeposit_JammedDetail;

                    break;
                #endregion

                #region [5] Coin Exchange
                case CashAddPropertiesTab.tabSVD_CoinExchange_MachineBalance_CashAdd:

                    //Dummy
                    var CoinExchange_MachineBalance = AutoFixture.CreateDummy<SvdCoinExchangeMachineBalance>();
                    CoinExchange_MachineBalance.MachineBalanceCoinList = AutoFixture.CreateDummy<IEnumerable<MBCassetteTransectionView>>();
                    CoinExchange_MachineBalance.MachineBalanceHopperList = AutoFixture.CreateDummy<IEnumerable<MBCassetteTransectionView>>();
                    CoinExchange_MachineBalance.MachineBalanceNoteList = AutoFixture.CreateDummy<IEnumerable<MBCassetteTransectionView>>();
                    result.SVD_CoinExchange_MachineBalance = CoinExchange_MachineBalance;


                    //Dummy
                    var CoinExchange_CashAdd = AutoFixture.CreateDummy<SvdCoinExchangeCashAdd>();
                    CoinExchange_CashAdd.CashAddCoinList = AutoFixture.CreateDummy<IEnumerable<CACassetteTransectionView>>();
                    CoinExchange_CashAdd.CashAddHopperList = AutoFixture.CreateDummy<IEnumerable<CACassetteTransectionView>>();
                    CoinExchange_CashAdd.CashAddNoteList = AutoFixture.CreateDummy<IEnumerable<CACassetteTransectionView>>();
                    result.SVD_CoinExchange_CashAdd = CoinExchange_CashAdd;
                    break;
                case CashAddPropertiesTab.tabSVD_CoinExchange_CashReturn_BulkNote:

                    //Dummy
                    var CoinExchange_CashReturn = AutoFixture.CreateDummy<SvdCoinExchangeCashReturn>();
                    CoinExchange_CashReturn.CashReturnHopperList = AutoFixture.CreateDummy<IEnumerable<CRCXTransectionView>>();
                    CoinExchange_CashReturn.CashReturnNoteList = AutoFixture.CreateDummy<IEnumerable<CRCXTransectionView>>();
                    CoinExchange_CashReturn.ReturnBagList = AutoFixture.CreateDummy<IEnumerable<SealBagView>>();
                    CoinExchange_CashReturn.StayBagList = AutoFixture.CreateDummy<IEnumerable<SealBagView>>();
                    result.SVD_CoinExchange_CashReturn = CoinExchange_CashReturn;


                    //Dummy
                    var CoinExchange_BulkNoteCollection = AutoFixture.CreateDummy<SvdCoinExchangeBulkNote>();
                    CoinExchange_BulkNoteCollection.BulkNoteCollectionList = AutoFixture.CreateDummy<IEnumerable<BNCXTransectionView>>();
                    CoinExchange_BulkNoteCollection.BulkNoteBagList = AutoFixture.CreateDummy<IEnumerable<SealBagView>>();
                    result.SVD_CoinExchange_BulkNote = CoinExchange_BulkNoteCollection;
                    break;
                case CashAddPropertiesTab.tabSVD_CoinExchange_SuspectFake:

                    //Dummy
                    var SuspectFake_CoinExchange = AutoFixture.CreateDummy<SvdCoinExchangeSuspectFake>();
                    SuspectFake_CoinExchange.SuspectFakeList = AutoFixture.CreateDummy<IEnumerable<SuspectFakeTransectionView>>();
                    SuspectFake_CoinExchange.SuspectFakeDetailList = AutoFixture.CreateDummy<IEnumerable<SuspectFakeDetailView>>();
                    SuspectFake_CoinExchange.SuspectFakeBagList = AutoFixture.CreateDummy<IEnumerable<SealBagView>>();
                    result.SVD_CoinExchange_SuspectFake = SuspectFake_CoinExchange;
                    break;
                    #endregion
            }

            return result;
        }

        private TabHistoryView GetTabHistory(JobPropertiesRequest request)
        {
            TabHistoryView history = new TabHistoryView();

            if (request.TabID == CashAddPropertiesTab.tabHistory)
            {

                var tblHistory = _masterHistoryActualJobRepository.FindByJob(request.JobGuid);
                var tblMessage = _systemMessageRepository.FindByLanguage(request.LanguageGuid);
                var result = tblHistory.Join(tblMessage,
                                H => H.MsgID,
                                M => M.MsgID,
                                (H, M) => new { history = H, Massage = M })
                                .OrderBy(o => o.history.UniversalDatetimeCreated)
                                .Select(o => new HistoryView
                                {
                                    Command = o.Massage.MessageTextTitle,
                                    Event = _systemMessageRepository.HistoryMappingParameters(o.Massage.MessageTextContent, o.history.MsgParameter, o.Massage.MsgID),
                                    UserName = o.history.UserCreated,
                                    DatetimeCreated = o.history.DatetimeCreated?.ChangeFromDateToString(request.FormatDateUser),
                                    UniversalDatetimeCreated = o.history.UniversalDatetimeCreated
                                });
                history.HistoryList = result;
            }

            return history;
        }

        #endregion

        #region Job(s) not allow to close run
        public SystemMessageView IsJobEmptyByDailyRun(Guid dailyRunGuid)
        {
            Guid languageGuid = ApiSession.UserLanguage_Guid.Value;
            try
            {
                var listJob = GetJobCannotClose(dailyRunGuid, false);
                if (!listJob.Any())
                {
                    return _systemMessageRepository.FindByMsgId(0, languageGuid).ConvertToMessageView();
                }
                string strJobNo = string.Join(", ", listJob);
                //MsgID -809 : These jobs no. {0}  have no parcels attached. Please unable to service them prior to close the run.
                SystemMessageView msgAlert = _systemMessageRepository.FindByMsgId(-809, languageGuid).ConvertToMessageView();
                msgAlert.MessageTextContent = string.Format(msgAlert.MessageTextContent, strJobNo);
                return msgAlert;
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                return _systemMessageRepository.FindByMsgId(-184, languageGuid).ConvertToMessageView();
            }
        }
        public IEnumerable<string> GetJobCannotClose(Guid runDailyGuid, bool isDolphin)
        {
            IEnumerable<string> jobNoList = new List<string>();
            int[] jobTypeId = new int[] { 0, 2 };
            List<int> jobStatusId = new List<int> { 7, 28 };

            List<TblMasterActualJobServiceStopLegs> legs = _masterActualJobServiceStopLegsRepository.FindAllAsQueryable(l => l.MasterRunResourceDaily_Guid == runDailyGuid
                                                                                       && l.SequenceStop == 1).ToList();
            if (isDolphin)
            {
                var siteGuid = legs.Select(x => x.MasterSite_Guid.GetValueOrDefault()).Distinct().FirstOrDefault();
                bool flgWithOutCheckIn = _systemEnvironmentMasterCountryRepository.IsCloseRunWithOutCheckIn(siteGuid);
                if (flgWithOutCheckIn)
                {
                    jobStatusId.Add(4);
                }
            }

            List<Guid> jobTypeGuid = _systemServiceJobTypeRepository.FindAllAsQueryable(x => jobTypeId.Contains((int)x.ServiceJobTypeID)).Select(x => x.Guid).ToList();
            IEnumerable<TblMasterActualJobHeader> jobList = legs
                                            .Join(_masterActualJobHeaderRepository.FindAllAsQueryable(j => jobStatusId.Contains(j.SystemStatusJobID.Value)
                                                                                       && jobTypeGuid.Contains(j.SystemServiceJobType_Guid.Value))
                                            , l => l.MasterActualJobHeader_Guid
                                            , j => j.Guid
                                            , (l, j) => j).AsEnumerable();

            IEnumerable<Guid> jobAllGuids = jobList.Select(x => x.Guid).Distinct();

            if (jobAllGuids.Any())
            {
                var jobsNotClosecase = _masterActualJobItemDiscrapenciesRepository.FindAllAsQueryable(o => jobAllGuids.Contains(o.MasterActualJobHeader_Guid.Value) && !o.FlagCloseCase == true).Select(o => o.MasterActualJobHeader_Guid.Value);

                if (!jobsNotClosecase.Any() || isDolphin)
                {
                    var itemSealJob = _masterActualJobItemsSealRepository.FindSealByJobList(jobAllGuids)
                                          .Select(x => x.MasterActualJobHeader_Guid.Value);
                    var itemCommJob = _masterActualJobItemsCommodityRepository.FindCommodityByListJob(jobAllGuids)
                                          .Select(x => x.MasterActualJobHeader_Guid.Value);
                    List<Guid> jobsGuid = jobAllGuids.Except(itemSealJob.Union(itemCommJob)).ToList();
                    jobNoList = jobList.Where(x => jobsGuid.Contains(x.Guid)).Select(j => j.JobNo);
                }
            }
            return jobNoList;
        }

        public SystemMessageView GetValidateJobCannotClose(Guid dailyRunGuid, Guid langGuid, string vehicleNo)
        {
            SystemMessageView msg = new SystemMessageView();
            var resp = _masterDailyRunResource.GetValidateJobCannotCloseRun(dailyRunGuid, langGuid, vehicleNo);
            string msgContent = _systemMessageRepository.HistoryMappingParameters(resp.Msg.MsgDetail, resp.Msg.JobNo);
            msg.MsgID = resp.Msg.MsgId;
            msg.IsSuccess = resp.Msg.MsgId == 0;
            msg.MessageTextTitle = resp.Msg.MsgTitle;
            msg.MessageTextContent = msgContent;
            return msg;
        }
        #endregion

        #region Truck to truck for dolphin

        #region -- Update job in run
        private void UpdateJobOrderInRun(TruckToTruckRequest request, IEnumerable<TblMasterActualJobHeader> jobHeader, Guid languageGuid,
            TblMasterDailyRunResource newDailyRun = null)
        {
            if (newDailyRun == null)
            {
                newDailyRun = _masterDailyRunResource.FindById(request.NewDailyRunGuid);
            }

            var jobHeaderList = jobHeader.Select(e => e.Guid).ToList();
            UpdateJobOrderInRunRequest updateJobOrder = new UpdateJobOrderInRunRequest()
            {
                ClientDateTime = request.DatetimeCreated,
                LanguageGuid = languageGuid,
                UserModified = request.UserCreated,
                WorkDate = newDailyRun.WorkDate,
                SiteGuid = newDailyRun.MasterSite_Guid.GetValueOrDefault(),
                FlagReorder = false,
                RunDailyGuid = newDailyRun.Guid

            };
            _adhocService.UpdateJobOrderInRun(updateJobOrder);
            updateJobOrder.JobHeadGuidList = jobHeaderList;

            TblMasterDailyRunResource oldDailyRun = _masterDailyRunResource.FindById(request.OldDailyRunGuid);
            UpdateJobOrderInRunRequest updateOldJobOrder = new UpdateJobOrderInRunRequest()
            {
                ClientDateTime = request.DatetimeCreated,
                LanguageGuid = languageGuid,
                UserModified = request.UserCreated,
                WorkDate = oldDailyRun.WorkDate,
                SiteGuid = oldDailyRun.MasterSite_Guid.GetValueOrDefault(),
                FlagReorder = false,
                RunDailyGuid = oldDailyRun.Guid

            };
            _adhocService.UpdateJobOrderInRun(updateOldJobOrder);
            updateOldJobOrder.JobHeadGuidList = jobHeaderList;
        }
        #endregion

        #region -- API Activity
        private void TruckToTruckCreateAPIActivityRequest(TruckToTruckRequest request, Uri requestUri)
        {
            var msg = $"Request: Get request from Dolphin EE with url: {requestUri}, request:  {JsonConvert.SerializeObject(request)}";
            _systemService.CreateLogActivity(SystemActivityLog.DPOOAPIActivity, msg, request.UserCreated, SystemHelper.CurrentIpAddress, ApiSession.Application_Guid.GetValueOrDefault());
        }
        private void TruckToTruckCreateAPIActivityResponse(TruckToTruckTransferResponse response, string userCreated, Uri requestUri)
        {
            var msg = $"Response: Send response to Dolphin EE with url: {requestUri}, response: {JsonConvert.SerializeObject(response)}";
            _systemService.CreateLogActivity(SystemActivityLog.DPOOAPIActivity, msg, userCreated, SystemHelper.CurrentIpAddress, ApiSession.Application_Guid.GetValueOrDefault());
        }
        #endregion

        #region++ Truck To Truck Ready
        public TruckToTruckTransferResponse TruckToTruckTransferReady(TruckToTruckRequest request, Uri requestUri)
        {
            var enGuid = Guid.Parse("6fa2bd67-0794-4a9e-a13b-2d81ddb574a0");
            var response = new TruckToTruckTransferResponse();
            try
            {
                var jobList = _masterActualJobHeaderRepository.FindByLegGuidList(request.LegGuidList);
                DateTime datetimeCreated = DateTime.Now;
                DateTimeOffset universalDatetimeCreated = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

                #region Insert log API event [Request]
                TruckToTruckCreateAPIActivityRequest(request, requestUri);
                #endregion

                #region Validate both of run statuses must be Ready and site must be same
                var validDailyRun = TruckToTruckIsValidRun(request.OldDailyRunGuid, request.NewDailyRunGuid, enGuid);
                if (!validDailyRun.isSuccess)
                {
                    return validDailyRun;
                }
                #endregion

                #region Validate job status must be On Truck, On Truck (Pick Up), On Truck (Delivery), In-Pre Vault, or In-Pre Vault (Delivery)
                var validJob = TruckToTruckIsValidJobStatus(jobList, enGuid);
                if (!validJob.isSuccess)
                {
                    return validJob;
                }
                #endregion

                #region Validate service type must be P, D, T, TV-P, TV-D, BCD, BCP (Include Interbranch) and same TV job cannot in the same run
                var validServiceType = TruckToTruckIsValidServiceType(jobList, request.NewDailyRunGuid, enGuid);
                if (!validServiceType.isSuccess)
                {
                    return validServiceType;
                }
                #endregion

                #region Validate consolidation in another job that not send in request
                var validCon = TruckToTruckValidateConsolidate(jobList, enGuid);
                if (!validCon.isSuccess)
                {
                    return validCon;
                }
                #endregion

                #region Validate leglist must be in old run
                var validLeg = TruckToTruckIsValidlegListMustBeInOldRun(request.LegGuidList, request.OldDailyRunGuid, enGuid);
                if (!validLeg.isSuccess)
                {
                    return validLeg;
                }
                #endregion

                #region Update Consolidate
                UpdateNewRouteConsolidate(jobList, request.NewDailyRunGuid, request.UserCreated, datetimeCreated, universalDatetimeCreated);
                #endregion

                #region Update Job Status
                TruckToTruckTransfer(jobList, request, datetimeCreated, universalDatetimeCreated);
                #endregion

                #region Insert history truck limit
                InsertHistoryTruckLimit_T2T(request, jobList, universalDatetimeCreated);
                #endregion

                #region Update Job Order
                UpdateJobOrderInRun(request, jobList, enGuid);
                #endregion

                #region Insert log API event [Response]
                TruckToTruckCreateAPIActivityResponse(response, request.UserCreated, requestUri);
                #endregion
                return response;
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);

                response.isSuccess = false;
                response.message = (_systemMessageRepository.FindByMsgId(-184, enGuid)).MessageTextContent;
                return response;
            }
        }

        #region Validate Transfer Ready
        private TruckToTruckTransferResponse TruckToTruckIsValidRun(Guid oldDailyRunGuid, Guid newDailyRunGuid, Guid languageGuid)
        {
            return _masterDailyRunResource.TruckToTruckIsValidRun(oldDailyRunGuid, newDailyRunGuid, languageGuid);
        }

        private TruckToTruckTransferResponse TruckToTruckIsValidJobStatus(IEnumerable<TblMasterActualJobHeader> jobList, Guid languageGuid)
        {
            return _masterDailyRunResource.TruckToTruckIsValidJobStatus(jobList, languageGuid);
        }

        private TruckToTruckTransferResponse TruckToTruckIsValidlegListMustBeInOldRun(List<Guid> legGuidList, Guid oldDailyRunGuid, Guid languageGuid)
        {
            return _masterDailyRunResource.TruckToTruckIsValidlegListMustBeInOldRun(legGuidList, oldDailyRunGuid, languageGuid);
        }

        private TruckToTruckTransferResponse TruckToTruckIsValidServiceType(IEnumerable<TblMasterActualJobHeader> jobList, Guid newDailyRunGuid, Guid languageGuid)
        {
            return _masterDailyRunResource.TruckToTruckIsValidServiceJobType(jobList, newDailyRunGuid, languageGuid);
        }

        private TruckToTruckTransferResponse TruckToTruckValidateConsolidate(IEnumerable<TblMasterActualJobHeader> jobList, Guid languageGuid)
        {
            var response = new TruckToTruckTransferResponse();
            IEnumerable<Guid> jobGuidList = jobList.Select(x => x.Guid);
            bool validJobConsolidate = _masterConAndDeconsolidateHeaderRepository.HasJobNotInCon(jobGuidList);
            if (validJobConsolidate)
            {
                var msg = _systemMessageRepository.FindByMsgId(-2156, languageGuid);
                response.isSuccess = false;
                response.message = msg.MessageTextContent;
            }
            return response;
        }
        #endregion

        #region Update transfer ready
        private void UpdateNewRouteConsolidate(IEnumerable<TblMasterActualJobHeader> jobList, Guid newDailyRunGuid, string userModify, DateTime datetimeModified, DateTimeOffset universalDatetimeModified)
        {
            IEnumerable<Guid> jobsGuid = jobList.Select(x => x.Guid);
            TblMasterDailyRunResource newDailyRun = _masterDailyRunResource.FindById(newDailyRunGuid);
            _masterConAndDeconsolidateHeaderRepository.UpdateConToNewDailyRun(jobsGuid, newDailyRun, datetimeModified, userModify, universalDatetimeModified);
        }

        private void TruckToTruckTransfer(IEnumerable<TblMasterActualJobHeader> jobList, TruckToTruckRequest request, DateTime datetimeCreated, DateTimeOffset universalDatetimeCreated)
        {
            _masterActualJobHeaderRepository.TruckToTruckTransfer(jobList, request.LegGuidList, request.OldDailyRunGuid, request.NewDailyRunGuid, request.UserCreated, request.DatetimeCreated, request.ReceiverSignature, request.SenderGuid, request.ReceiverName, datetimeCreated, universalDatetimeCreated);
        }
        #endregion

        #endregion

        #region++ Truck To Truck Transfer (TV and TV Inter-br)
        public TruckToTruckHoldoverJobResponse TruckToTruckHoldoverJob(TruckToTruckRequest request, Uri requestUri)
        {
            TruckToTruckHoldoverJobResponse response = null;
            DateTimeOffset dateNow = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            var enGuid = Guid.Parse("6fa2bd67-0794-4a9e-a13b-2d81ddb574a0");
            List<TblMasterActualJobHeader> jobsHeader = _masterActualJobHeaderRepository.FindByLegGuidList(request.LegGuidList).ToList();
            var jobGuidList = jobsHeader.Select(o => o.Guid);
            List<TblMasterActualJobServiceStopLegs> legTvDList = _masterActualJobServiceStopLegsRepository.FindDestinationByJobGuid(jobGuidList).ToList();

            TblMasterDailyRunResource newDailyRun = _masterDailyRunResource.FindById(request.NewDailyRunGuid);
            string strNewRouteGrp = _masterRouteGroupDetailRepository.GetFullRouteNameByDailyRun(request.NewDailyRunGuid);
            string strOldRouteGrp = _masterRouteGroupDetailRepository.GetFullRouteNameByDailyRun(request.OldDailyRunGuid);

            int newRunStatus = (int)newDailyRun.RunResourceDailyStatusID;
            using (var trans = _uow.BeginTransaction())
            {
                try
                {
                    #region -- Insert log API event [Request]
                    TruckToTruckCreateAPIActivityRequest(request, requestUri);
                    #endregion

                    #region -- 2.2 validate same TV job cannot be in the same run
                    response = ValidateSameRunTransfer(request.OldDailyRunGuid, request.NewDailyRunGuid, enGuid);
                    if (!response.isSuccess)
                    {
                        return response;
                    }
                    #endregion

                    #region -- 2.1 validate work date of job with work date of destination run
                    var respValidateWorkDate = ValidateDestinationWorkDateSameRun(legTvDList, newDailyRun.WorkDate.GetValueOrDefault(), enGuid);
                    if (!respValidateWorkDate.isSuccess)
                    {
                        response.isSuccess = respValidateWorkDate.isSuccess;
                        response.message = respValidateWorkDate.message;
                        return response;
                    }
                    #endregion

                    #region -- 2.3 validate destination run’s site must match with the TVD’s site
                    response = ValidateDestinationSiteRun(legTvDList, newDailyRun.MasterSite_Guid.GetValueOrDefault(), enGuid);
                    if (!response.isSuccess)
                    {
                        return response;
                    }
                    #endregion

                    #region -- 2.4 validate service type must be TV, TV (Interbranch)
                    response = ValidateJobTypeTV(jobsHeader, enGuid);
                    if (!response.isSuccess)
                    {
                        return response;
                    }
                    #endregion

                    #region -- 2.6 validate tv_p is cancel or unable to service
                    response = ValidateTVPCancelOrUnable(jobsHeader, enGuid);
                    if (!response.isSuccess)
                    {
                        return response;
                    }
                    #endregion

                    #region -- 2.5 validate job status must be picked up, in pre-vault picked up, or in pre-vault delivery
                    response = ValidateTransferJobStatus(jobsHeader, enGuid);
                    if (!response.isSuccess)
                    {
                        return response;
                    }
                    #endregion

                    #region -- 2.7 validate item consolidated in another job
                    var respValidateCon = TruckToTruckValidateConsolidate(jobsHeader, enGuid);
                    if (!respValidateCon.isSuccess)
                    {
                        response.isSuccess = respValidateCon.isSuccess;
                        response.message = respValidateCon.message;
                        return response;
                    }
                    #endregion

                    #region -- 2.8 [Not validate - Already talked with SA (TFS#60657)] validate TVD’s status is on truck - delivery or on the way - delivery 
                    #endregion

                    #region -- update consolidate
                    UpdateConTransferTruckTV(jobGuidList, request, dateNow);
                    #endregion

                    #region -- update job
                    UpdateJobTransferTruckTV(jobsHeader, request, newRunStatus, strOldRouteGrp, strNewRouteGrp, dateNow);
                    #endregion

                    #region -- update leg D
                    UpdateLegTranferTruckTV(legTvDList, newDailyRun, request, strNewRouteGrp, dateNow);
                    #endregion

                    #region -- insert signature history
                    if (!request.ReceiverSignature.IsEmpty())
                    {
                        byte[] bytes = System.Convert.FromBase64String(request.ReceiverSignature);
                        var historySignature = new TblMasterHistory_DailyRunResource_SignatureTruckToTruckTransfer()
                        {
                            Guid = Guid.NewGuid(),
                            OldDailyRun_Guid = request.OldDailyRunGuid,
                            NewDailyRun_Guid = request.NewDailyRunGuid,
                            Sender_Guid = request.SenderGuid,
                            ReceiverName = request.ReceiverName,
                            ReceiverSignature = bytes,
                            DatetimeTransfer = DateTime.Now,
                            UserCreated = request.UserCreated,
                            DatetimeCreated = request.DatetimeCreated,
                            UniversalDatetimeCreated = dateNow
                        };
                        _masterHistoryDailyRunResourceSignatureTruckToTruckTransferRepository.Create(historySignature);
                    }
                    #endregion

                    #region Insert history truck limit
                    InsertHistoryTruckLimit_T2T(request, jobsHeader, dateNow);
                    #endregion

                    response.jobGuidList = jobGuidList.ToList();

                    #region -- Insert log API event [Response]
                    TruckToTruckTVCreateAPIActivityResponse(response, request.UserCreated, requestUri);
                    #endregion

                    _uow.Commit();

                    #region -- update job order in run
                    UpdateJobOrderInRunTVD(request, jobsHeader, newDailyRun, enGuid);
                    #endregion

                }
                catch (Exception ex)
                {
                    // OO error logger
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);

                    response.isSuccess = false;
                    response.message = (_systemMessageRepository.FindByMsgId(-184, enGuid)).MessageTextContent;
                }
            }
            return response;
        }

        #region Activity Truck to truck TV
        private void TruckToTruckTVCreateAPIActivityResponse(TruckToTruckHoldoverJobResponse response, string userCreated, Uri requestUri)
        {
            var msg = $"Response: Send response to Dolphin EE with url: {requestUri}, response: {JsonConvert.SerializeObject(response)}";
            _systemService.CreateLogActivity(SystemActivityLog.DPOOAPIActivity, msg, userCreated, SystemHelper.CurrentIpAddress, ApiSession.Application_Guid.GetValueOrDefault());
        }
        #endregion

        #region Validate
        private TruckToTruckHoldoverJobResponse ValidateSameRunTransfer(Guid oldDailyRun, Guid newDailyRun, Guid languageGuid)
        {
            var resp = new TruckToTruckHoldoverJobResponse();
            bool isNotTransfer = oldDailyRun == newDailyRun;
            if (isNotTransfer)
            {
                var msgId = -2161;
                resp.isSuccess = false;
                resp.message = _systemMessageRepository.FindByMsgId(msgId, languageGuid).MessageTextContent;
            }
            return resp;
        }
        private TruckToTruckHoldoverJobResponse ValidateJobTypeTV(IEnumerable<TblMasterActualJobHeader> jobsHeader, Guid languageGuid)
        {
            //Dolphin send leg of customer
            var resp = new TruckToTruckHoldoverJobResponse();
            var hasNotTVP = jobsHeader.Any(x => x.JobTypeID != IntTypeJob.TV); //Support TV and TV(Inter-branch)
            if (hasNotTVP)
            {
                var msgId = -17327;
                resp.isSuccess = false;
                resp.message = _systemMessageRepository.FindByMsgId(msgId, languageGuid).MessageTextContent;
            }
            return resp;
        }
        private TruckToTruckHoldoverJobResponse ValidateTransferJobStatus(IEnumerable<TblMasterActualJobHeader> jobsHeader, Guid languageGuid)
        {
            var resp = new TruckToTruckHoldoverJobResponse();
            //Job status allow
            List<int> jobStatus = new List<int> { IntStatusJob.PickedUp, IntStatusJob.InPreVaultPickUp, IntStatusJob.InPreVaultDelivery };
            List<int> jobHeaderStatus = jobsHeader.Select(x => x.SystemStatusJobID.GetValueOrDefault()).ToList();
            bool hasNotJobPickup = jobHeaderStatus.Except(jobStatus).Any();
            if (hasNotJobPickup)
            {
                var msgId = -17328;
                resp.isSuccess = false;
                resp.message = _systemMessageRepository.FindByMsgId(msgId, languageGuid).MessageTextContent;
            }
            return resp;
        }
        private TruckToTruckHoldoverJobResponse ValidateTVPCancelOrUnable(IEnumerable<TblMasterActualJobHeader> jobHeader, Guid languageGuid)
        {
            var resp = new TruckToTruckHoldoverJobResponse();
            bool hasJobCancelOrUnable = jobHeader.Any(x => x.FlagCancelAll.GetValueOrDefault() || (x.ReasonUnableToService_Guid.HasValue && x.ReasonUnableToService_Guid.GetValueOrDefault() != Guid.Empty));
            if (hasJobCancelOrUnable)
            {
                var msgId = -17330;
                resp.isSuccess = false;
                resp.message = _systemMessageRepository.FindByMsgId(msgId, languageGuid).MessageTextContent;
            }
            return resp;
        }
        private TruckToTruckHoldoverJobResponse ValidateDestinationSiteRun(IEnumerable<TblMasterActualJobServiceStopLegs> legDList, Guid destinationSiteRun, Guid languageGuid)
        {
            var resp = new TruckToTruckHoldoverJobResponse();
            var destinationSiteJob = legDList.Select(o => o.MasterSite_Guid.GetValueOrDefault());
            bool isDestDiffSite = destinationSiteJob.Any(x => x != destinationSiteRun);
            if (isDestDiffSite)
            {
                var msgId = -17329;
                resp.isSuccess = false;
                resp.message = _systemMessageRepository.FindByMsgId(msgId, languageGuid).MessageTextContent;
            }
            return resp;
        }
        private TruckToTruckHoldoverJobResponse ValidateDestinationWorkDateSameRun(IEnumerable<TblMasterActualJobServiceStopLegs> legDList, DateTime destWorkDate, Guid languageGuid)
        {
            var resp = new TruckToTruckHoldoverJobResponse();
            var legD_WorkDate = legDList.Select(o => o.ServiceStopTransectionDate.GetValueOrDefault());
            bool isDestDiffWorkDate = legD_WorkDate.Any(x => x.Date != destWorkDate.Date);
            if (isDestDiffWorkDate)
            {
                var msgId = -17340;
                resp.isSuccess = false;
                resp.message = _systemMessageRepository.FindByMsgId(msgId, languageGuid).MessageTextContent;
            }
            return resp;
        }
        #endregion

        #region Update
        private void UpdateJobTransferTruckTV(IEnumerable<TblMasterActualJobHeader> jobHeader, TruckToTruckRequest request,
            int newRunStatus, string strOldRouteGrp, string strNewRouteGrp, DateTimeOffset dateNow)
        {
            List<int> statusJobId = new List<int> { IntStatusJob.OnTruckDelivery, IntStatusJob.OnTheWayDelivery };
            List<TblSystemJobStatus> jobStatusList = _systemJobStatusRepository.FindAll(x => statusJobId.Contains((int)x.StatusJobID)).ToList();

            List<TblMasterHistory_ActualJob> historyJob = new List<TblMasterHistory_ActualJob>();
            foreach (var item in jobHeader)
            {
                string strJobStatus = string.Empty;
                switch (newRunStatus)
                {
                    case DailyRunStatus.Ready:
                        item.SystemStatusJobID = IntStatusJob.OnTruckDelivery;
                        strJobStatus = jobStatusList.FirstOrDefault(x => x.StatusJobID == IntStatusJob.OnTruckDelivery).StatusJobName;
                        break;
                    case DailyRunStatus.DispatchRun:
                        item.SystemStatusJobID = IntStatusJob.OnTheWayDelivery;
                        strJobStatus = jobStatusList.FirstOrDefault(x => x.StatusJobID == IntStatusJob.OnTheWayDelivery).StatusJobName;
                        break;
                }
                item.FlagChkOutInterBranchComplete = item.FlagJobInterBranch;   //Dolphin use check.

                TblMasterHistory_ActualJob historyUpdateJob = new TblMasterHistory_ActualJob
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = item.Guid,
                    MsgID = 6106, //Change job status to "{job status}" by Dolphin EE.
                    MsgParameter = strJobStatus,
                    UserCreated = request.UserCreated,
                    DatetimeCreated = request.DatetimeCreated,
                    UniversalDatetimeCreated = dateNow
                };
                historyJob.Add(historyUpdateJob);

                TblMasterHistory_ActualJob historyTruckToTruck = new TblMasterHistory_ActualJob
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = item.Guid,
                    MsgID = 6107,
                    MsgParameter = new string[] { item.JobNo, strOldRouteGrp, strNewRouteGrp }.ToJSONString(),
                    UserCreated = request.UserCreated,
                    DatetimeCreated = request.DatetimeCreated,
                    UniversalDatetimeCreated = dateNow
                };
                historyJob.Add(historyTruckToTruck);
            }
            _masterHistoryActualJobRepository.CreateRange(historyJob);
        }
        private void UpdateLegTranferTruckTV(IEnumerable<TblMasterActualJobServiceStopLegs> legDList, TblMasterDailyRunResource newDailyRun, TruckToTruckRequest request, string strNewRouteGrp, DateTimeOffset dateNow)
        {
            //If TV-D is unassign or another run will update leg to new daily run.
            List<TblMasterHistory_ActualJob> historyJob = new List<TblMasterHistory_ActualJob>();
            //TV-D is unassign
            var listLegDUnassign = legDList.Where(x => !x.MasterRunResourceDaily_Guid.HasValue || x.MasterRunResourceDaily_Guid == Guid.Empty);
            //TV-D is another run
            var listLegDAnotherRun = legDList.Where(x => x.MasterRunResourceDaily_Guid != newDailyRun.Guid && x.MasterRunResourceDaily_Guid.HasValue);
            //Leg before destination
            var listLegDNotInRun = listLegDAnotherRun.Union(listLegDUnassign);
            var jobLegDList = listLegDNotInRun.Select(x => x.MasterActualJobHeader_Guid.GetValueOrDefault());

            var listLegDBrink = _masterActualJobServiceStopLegsRepository.FindByJobGuidList(jobLegDList).Where(x => x.SequenceStop == 3).ToList();

            var listLegDAssignRun = listLegDNotInRun.Union(listLegDBrink).ToList();
            foreach (var item in listLegDAssignRun)
            {
                item.MasterRunResourceDaily_Guid = newDailyRun.Guid;
                item.MasterRouteGroupDetail_Guid = newDailyRun.MasterRouteGroup_Detail_Guid;
                _masterActualJobServiceStopLegsRepository.Modify(item);

                TblMasterHistory_ActualJob historyAssignRun = new TblMasterHistory_ActualJob
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = item.Guid,
                    MsgID = 6108,
                    MsgParameter = new string[] { strNewRouteGrp }.ToJSONString(),
                    UserCreated = request.UserCreated,
                    DatetimeCreated = request.DatetimeCreated,
                    UniversalDatetimeCreated = dateNow
                };
                historyJob.Add(historyAssignRun);
            }
            _masterHistoryActualJobRepository.CreateRange(historyJob);
        }
        private void UpdateConTransferTruckTV(IEnumerable<Guid> jobGuid, TruckToTruckRequest request, DateTimeOffset dateNow)
        {
            var statusConGuid = _systemConAndDeconsolidateStatusRepository.FindOne(o => o.StatusID == 3).Guid;
            var consolidate = _masterConAndDeconsolidateHeaderRepository.GetConsolidateByJobsGuid(jobGuid);
            foreach (var item in consolidate)
            {
                item.MasterDailyRunResource_Guid = request.NewDailyRunGuid;
                item.SystemCoAndDeSolidateStatus_Guid = statusConGuid;
                item.UserModifed = request.UserCreated;
                item.DatetimeModified = request.DatetimeCreated;
                item.UniversalDatetimeModified = dateNow;
                item.FlagInPreVault = false; //for support check out to department
                _masterConAndDeconsolidateHeaderRepository.Modify(item);
            }
        }
        private void UpdateJobOrderInRunTVD(TruckToTruckRequest request, IEnumerable<TblMasterActualJobHeader> jobHeader, TblMasterDailyRunResource newDailyRun, Guid languageGuid)
        {
            var jobHeaderList = jobHeader.Select(e => e.Guid).ToList();
            UpdateJobOrderInRunRequest updateJobOrder = new UpdateJobOrderInRunRequest()
            {
                ClientDateTime = request.DatetimeCreated,
                LanguageGuid = languageGuid,
                UserModified = request.UserCreated,
                WorkDate = newDailyRun.WorkDate,
                SiteGuid = newDailyRun.MasterSite_Guid.GetValueOrDefault(),
                FlagReorder = false,
                RunDailyGuid = newDailyRun.Guid

            };
            _adhocService.UpdateJobOrderInRun(updateJobOrder);
            updateJobOrder.JobHeadGuidList = jobHeaderList;

            TblMasterDailyRunResource oldDailyRun = _masterDailyRunResource.FindById(request.OldDailyRunGuid);
            UpdateJobOrderInRunRequest updateOldJobOrder = new UpdateJobOrderInRunRequest()
            {
                ClientDateTime = request.DatetimeCreated,
                LanguageGuid = languageGuid,
                UserModified = request.UserCreated,
                WorkDate = oldDailyRun.WorkDate,
                SiteGuid = oldDailyRun.MasterSite_Guid.GetValueOrDefault(),
                FlagReorder = false,
                RunDailyGuid = oldDailyRun.Guid

            };
            _adhocService.UpdateJobOrderInRun(updateOldJobOrder);
            updateOldJobOrder.JobHeadGuidList = jobHeaderList;
        }
        #endregion

        #endregion

        #region ++ Truck To Truck Transfer Job
        public TruckToTruckTransferResponse TruckToTruckTransferJob(TruckToTruckRequest request, Uri requestUri)
        {
            DateTimeOffset dateNow = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            var enGuid = Guid.Parse("6fa2bd67-0794-4a9e-a13b-2d81ddb574a0");
            var response = new TruckToTruckTransferResponse();
            using (var trans = _uow.BeginTransaction())
            {
                try
                {
                    var legReqList = _masterActualJobServiceStopLegsRepository.FindByLegGuidList(request.LegGuidList).ToList();
                    var jobList = _masterActualJobHeaderRepository.FindByLegGuidList(request.LegGuidList).ToList();
                    DateTime datetimeCreated = DateTime.Now;
                    DateTimeOffset universalDatetimeCreated = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

                    #region Run resource detail
                    List<Guid> dailyRunGuidList = new List<Guid> { request.OldDailyRunGuid, request.NewDailyRunGuid };
                    IEnumerable<TblMasterDailyRunResource> DailyRun = _masterDailyRunResource.FindAll(o => dailyRunGuidList.Contains(o.Guid));

                    TblMasterDailyRunResource oldDailyRun = DailyRun.FirstOrDefault(x => x.Guid == request.OldDailyRunGuid);
                    string strOldRouteGrp = _masterRouteGroupDetailRepository.GetFullRouteNameByDailyRun(request.OldDailyRunGuid);

                    TblMasterDailyRunResource newDailyRun = DailyRun.FirstOrDefault(x => x.Guid == request.NewDailyRunGuid);
                    string strNewRouteGrp = _masterRouteGroupDetailRepository.GetFullRouteNameByDailyRun(request.NewDailyRunGuid);
                    #endregion

                    #region Insert log API event [Request]
                    TruckToTruckCreateAPIActivityRequest(request, requestUri);
                    #endregion

                    #region 2.1 Validate consolidation in another job that not send in request (ID: -17360)   / -2156
                    var validCon = TruckToTruckValidateConsolidate(jobList, enGuid);
                    if (!validCon.isSuccess)
                    {
                        validCon.message = _systemMessageRepository.FindByMsgId(-17360, enGuid).MessageTextContent;
                        return validCon;
                    }
                    #endregion

                    #region 2.2 Validate both of run statuses must be dispatched  (ID: -17361)
                    var validDispath = ValidateBothRunDispatchTransferJob(oldDailyRun.RunResourceDailyStatusID, newDailyRun.RunResourceDailyStatusID, enGuid);
                    if (!validDispath.isSuccess)
                    {
                        return validDispath;
                    }
                    #endregion

                    #region 2.3 Validate both run same site  (ID: -17362)
                    var validRunSameSite = ValidateBothRunSameSiteTransferJob(oldDailyRun.MasterSite_Guid, newDailyRun.MasterSite_Guid, enGuid);
                    if (!validRunSameSite.isSuccess)
                    {
                        return validRunSameSite;
                    }
                    #endregion

                    #region 2.4 Validate only job with status picked up,in pre-vault, in pre-vault picked up, or in pre-vault delivery, Return To Pre - Vault are allowed  (ID: -17363)
                    var validJobStatus = ValidateJobStatusTransferJob(jobList, enGuid);
                    if (!validJobStatus.isSuccess)
                    {
                        return validJobStatus;
                    }
                    #endregion

                    #region 2.5 Service type must be P, D, T, TV-P, TV-D, BCD, BCP (Include Interbranch and multibranch) (ID: -17364)
                    var validJobTypeAllow = ValidateJobTypeTransferJob(jobList, enGuid);
                    if (!validJobTypeAllow.isSuccess)
                    {
                        return validJobTypeAllow;
                    }
                    #endregion

                    #region 2.6 Same TV job cannot in the same run  (ID: -17365)
                    var validTVSameRun = ValidateJobTypeTVTransferJob(jobList, request.OldDailyRunGuid, request.NewDailyRunGuid, enGuid);
                    if (!validTVSameRun.isSuccess)
                    {
                        return validTVSameRun;
                    }
                    #endregion

                    #region 2.7.Transfer job cannot in the different run (ID: -17366)
                    var validTDiffRun = ValidateJobTypeTTransferJob(jobList, legReqList, enGuid);
                    if (!validTDiffRun.isSuccess)
                    {
                        return validTDiffRun;
                    }
                    #endregion

                    #region Update Consolidate
                    UpdateNewRouteConsolidate(jobList, request.NewDailyRunGuid, request.UserCreated, datetimeCreated, universalDatetimeCreated);
                    #endregion

                    #region Update run for leg and job
                    UpdateLegTranferJob(legReqList, newDailyRun);
                    UpdateJobTransferJob(newDailyRun, jobList, request, dateNow, strNewRouteGrp, strOldRouteGrp);
                    InsertSignatureHistoryTransferJob(request, dateNow);
                    InsertLogDolphinAssignToAnotherRun(jobList, request.OldDailyRunGuid);
                    #endregion

                    #region Update history truck limit
                    InsertHistoryTruckLimit_T2T(request, jobList, dateNow);
                    #endregion

                    #region Insert log API event [Response]
                    TruckToTruckCreateAPIActivityResponse(response, request.UserCreated, requestUri);
                    #endregion

                    _uow.Commit();

                    #region Update Job Order
                    UpdateJobOrderInRun(request, jobList, enGuid, newDailyRun: newDailyRun);
                    #endregion

                    response.message = _systemMessageRepository.FindByMsgId(0, enGuid).MessageTextContent;
                    return response;
                }
                catch (Exception ex)
                {
                    // OO error logger
                    _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);

                    response.isSuccess = false;
                    response.message = (_systemMessageRepository.FindByMsgId(-184, enGuid)).MessageTextContent;
                    return response;
                }
            }
        }

        #region -- Validate truck to truck transfer job
        private TruckToTruckTransferResponse ValidateBothRunDispatchTransferJob(int? oldDailyRunStatus, int? newDailyRunStatus, Guid languageGuid)
        {
            var resp = new TruckToTruckTransferResponse();
            //Both of run statuses must be Dispatched
            bool isBothRunDispatch = (oldDailyRunStatus.GetValueOrDefault() == DailyRunStatus.DispatchRun) && (newDailyRunStatus.GetValueOrDefault() == DailyRunStatus.DispatchRun);
            if (!isBothRunDispatch)
            {
                resp.isSuccess = false;
                resp.message = _systemMessageRepository.FindByMsgId(-17361, languageGuid).MessageTextContent;
            }
            return resp;
        }
        private TruckToTruckTransferResponse ValidateBothRunSameSiteTransferJob(Guid? oldDailyRunSiteGuid, Guid? newDailyRunSiteGuid, Guid languageGuid)
        {
            var resp = new TruckToTruckTransferResponse();
            //Both of run site must be same:
            bool isBothRunSameSite = oldDailyRunSiteGuid.Value == newDailyRunSiteGuid.Value;
            if (!isBothRunSameSite)
            {
                resp.isSuccess = false;
                resp.message = _systemMessageRepository.FindByMsgId(-17362, languageGuid).MessageTextContent;
            }
            return resp;
        }
        private TruckToTruckTransferResponse ValidateJobStatusTransferJob(IEnumerable<TblMasterActualJobHeader> jobsHeader, Guid languageGuid)
        {
            var resp = new TruckToTruckHoldoverJobResponse();
            //Job status allow
            List<int> jobStatus = new List<int> { IntStatusJob.PickedUp, IntStatusJob.InPreVault, IntStatusJob.InPreVaultPickUp, IntStatusJob.InPreVaultDelivery, IntStatusJob.ReturnToPreVault };
            List<int> jobHeaderStatus = jobsHeader.Select(x => x.SystemStatusJobID.GetValueOrDefault()).ToList();
            bool hasNotJobPickup = jobHeaderStatus.Except(jobStatus).Any();
            if (hasNotJobPickup)
            {
                resp.isSuccess = false;
                resp.message = _systemMessageRepository.FindByMsgId(-17363, languageGuid).MessageTextContent;
            }
            return resp;
        }
        private TruckToTruckTransferResponse ValidateJobTypeTransferJob(IEnumerable<TblMasterActualJobHeader> jobsHeader, Guid languageGuid)
        {
            var resp = new TruckToTruckTransferResponse();
            List<int> jobType = new List<int> { IntTypeJob.P, IntTypeJob.P_MultiBr, IntTypeJob.D, IntTypeJob.T, IntTypeJob.TV, IntTypeJob.TV_MultiBr, IntTypeJob.BCD, IntTypeJob.BCD_MultiBr, IntTypeJob.BCP };
            var hasNotAllowJobType = jobsHeader.Where(o => !jobType.Contains(o.JobTypeID)).Any();
            if (hasNotAllowJobType)
            {
                resp.isSuccess = false;
                resp.message = _systemMessageRepository.FindByMsgId(-17364, languageGuid).MessageTextContent;
            }
            return resp;
        }
        private TruckToTruckTransferResponse ValidateJobTypeTVTransferJob(IEnumerable<TblMasterActualJobHeader> jobsHeader, Guid? oldDailyRun, Guid? newDailyRun, Guid languageGuid)
        {
            var resp = new TruckToTruckTransferResponse();

            //find job in new run.
            var jobInNewDailyRun = _masterActualJobServiceStopLegsRepository.FindByDailyRun(newDailyRun)
                                   .Select(o => o.MasterActualJobHeader_Guid.GetValueOrDefault()).Distinct();
            var jobInOldDailyRun = jobsHeader.Where(x => x.JobTypeID == IntTypeJob.TV || x.JobTypeID == IntTypeJob.TV_MultiBr).Select(o => o.Guid).Distinct();
            var hasSameJobInRun = jobInNewDailyRun.Intersect(jobInOldDailyRun).Any();
            if (hasSameJobInRun)
            {
                resp.isSuccess = false;
                resp.message = _systemMessageRepository.FindByMsgId(-17365, languageGuid).MessageTextContent;
            }
            return resp;
        }
        private TruckToTruckTransferResponse ValidateJobTypeTTransferJob(IEnumerable<TblMasterActualJobHeader> jobsHeader, IEnumerable<TblMasterActualJobServiceStopLegs> LegList, Guid languageGuid)
        {
            var resp = new TruckToTruckTransferResponse();
            var jobLegs = LegList
                          .Join(jobsHeader.Where(x => x.JobTypeID == IntTypeJob.T),
                          l => l.MasterActualJobHeader_Guid,
                          j => j.Guid,
                          (l, j) => new { jobGuid = j.Guid, legGuid = l.Guid })
                          .GroupBy(g => g.jobGuid)
                          .Select(o => new { jobGuid = o.Key, countLeg = o.Count() });

            var hasJobTDiffRun = jobLegs.Any(o => o.countLeg < 2);
            if (hasJobTDiffRun)
            {
                resp.isSuccess = false;
                resp.message = _systemMessageRepository.FindByMsgId(-17366, languageGuid).MessageTextContent;
            }
            return resp;
        }
        #endregion

        #region -- Update transfer job
        private void UpdateLegTranferJob(IEnumerable<TblMasterActualJobServiceStopLegs> LegReqList, TblMasterDailyRunResource newDailyRun)
        {
            var jobLeg = LegReqList.Select(o => o.MasterActualJobHeader_Guid.GetValueOrDefault());
            var legAll = _masterActualJobServiceStopLegsRepository.FindByJobGuidList(jobLeg);

            foreach (var leg in LegReqList)
            {
                leg.MasterRunResourceDaily_Guid = newDailyRun.Guid;
                leg.MasterRouteGroupDetail_Guid = newDailyRun.MasterRouteGroup_Detail_Guid.GetValueOrDefault();
                leg.JobOrder = 0;
                leg.SeqIndex = 0;

                int seqStopLeg = leg.SequenceStop.Value % 2 == 0 ? leg.SequenceStop.Value - 1 : leg.SequenceStop.Value + 1;
                var legBrink = legAll.FirstOrDefault(o => o.SequenceStop == seqStopLeg);
                legBrink.MasterRunResourceDaily_Guid = newDailyRun.Guid;
                legBrink.MasterRouteGroupDetail_Guid = newDailyRun.MasterRouteGroup_Detail_Guid.GetValueOrDefault();
                legBrink.JobOrder = 0;
                legBrink.SeqIndex = 0;

                _masterActualJobServiceStopLegsRepository.Modify(leg);
                _masterActualJobServiceStopLegsRepository.Modify(legBrink);
            }
        }
        private void UpdateJobTransferJob(TblMasterDailyRunResource newDailyRun, IEnumerable<TblMasterActualJobHeader> jobsHeader, TruckToTruckRequest request, DateTimeOffset dateNow, string newRunDetail, string oldRunDetail)
        {
            var jobStatusOntruck = new List<int> { IntStatusJob.OnTruck, IntStatusJob.OnTruckPickUp, IntStatusJob.OnTruckDelivery };
            var jobStatusOntheWay = new List<int> { IntStatusJob.OnTheWay, IntStatusJob.OnTheWayPickUp, IntStatusJob.OnTheWayDelivery };

            List<TblMasterHistory_ActualJob> historyJob = new List<TblMasterHistory_ActualJob>();
            List<TblMasterHistory_DailyRunResource> historyRun = new List<TblMasterHistory_DailyRunResource>();

            foreach (var job in jobsHeader)
            {
                if (newDailyRun.RunResourceDailyStatusID == DailyRunStatus.DispatchRun && jobStatusOntruck.Any(x => x == job.SystemStatusJobID))
                {
                    switch (job.SystemStatusJobID)
                    {
                        case IntStatusJob.OnTruck:
                            job.SystemStatusJobID = IntStatusJob.OnTheWay;
                            break;
                        case IntStatusJob.OnTruckPickUp:
                            job.SystemStatusJobID = IntStatusJob.OnTheWayPickUp;
                            break;
                        case IntStatusJob.OnTruckDelivery:
                            job.SystemStatusJobID = IntStatusJob.OnTheWayDelivery;
                            break;
                    }
                }

                if (newDailyRun.RunResourceDailyStatusID == DailyRunStatus.Ready && jobStatusOntheWay.Any(x => x == job.SystemStatusJobID))
                {
                    switch (job.SystemStatusJobID)
                    {
                        case IntStatusJob.OnTheWay:
                            job.SystemStatusJobID = IntStatusJob.OnTruck;
                            break;
                        case IntStatusJob.OnTheWayPickUp:
                            job.SystemStatusJobID = IntStatusJob.OnTruckPickUp;
                            break;
                        case IntStatusJob.OnTheWayDelivery:
                            job.SystemStatusJobID = IntStatusJob.OnTruckDelivery;
                            break;
                    }
                }

                job.DatetimeModified = request.DatetimeCreated;
                job.UniversalDatetimeModified = dateNow;
                job.UserModifed = request.UserCreated;
                _masterActualJobHeaderRepository.Modify(job);

                var hisJob = new TblMasterHistory_ActualJob()
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = job.Guid,
                    MsgID = 6139,
                    MsgParameter = new string[] { job.JobNo, newRunDetail }.ToJSONString(),
                    UserCreated = request.UserCreated,
                    DatetimeCreated = request.DatetimeCreated,
                    UniversalDatetimeCreated = dateNow
                };
                historyJob.Add(hisJob);

                var hisNewRun = new TblMasterHistory_DailyRunResource()
                {
                    Guid = Guid.NewGuid(),
                    MasterDailyRunResource_Guid = request.NewDailyRunGuid,
                    MsgID = 6138,
                    MsgParameter = new string[] { job.JobNo, oldRunDetail, newRunDetail }.ToJSONString(),
                    UserCreated = request.UserCreated,
                    DatetimeCreated = request.DatetimeCreated,
                    UniversalDatetimeCreated = dateNow
                };
                historyRun.Add(hisNewRun);

                var hisOldRun = new TblMasterHistory_DailyRunResource()
                {
                    Guid = Guid.NewGuid(),
                    MasterDailyRunResource_Guid = request.OldDailyRunGuid,
                    MsgID = 6138,
                    MsgParameter = new string[] { job.JobNo, oldRunDetail, newRunDetail }.ToJSONString(),
                    UserCreated = request.UserCreated,
                    DatetimeCreated = request.DatetimeCreated,
                    UniversalDatetimeCreated = dateNow
                };
                historyRun.Add(hisOldRun);
            }
            _masterHistoryDailyRunResourceRepository.CreateRange(historyRun);
            _masterHistoryActualJobRepository.CreateRange(historyJob);
        }
        private void InsertSignatureHistoryTransferJob(TruckToTruckRequest request, DateTimeOffset dateNow)
        {
            #region Insert signature history
            if (!request.ReceiverSignature.IsEmpty())
            {
                byte[] bytesSignature = System.Convert.FromBase64String(request.ReceiverSignature);
                var historySignature = new TblMasterHistory_DailyRunResource_SignatureTruckToTruckTransfer()
                {
                    Guid = Guid.NewGuid(),
                    OldDailyRun_Guid = request.OldDailyRunGuid,
                    NewDailyRun_Guid = request.NewDailyRunGuid,
                    Sender_Guid = request.SenderGuid,
                    ReceiverName = request.ReceiverName,
                    ReceiverSignature = bytesSignature,
                    DatetimeTransfer = request.DatetimeCreated,
                    UserCreated = request.UserCreated,
                    DatetimeCreated = request.DatetimeCreated,
                    UniversalDatetimeCreated = dateNow
                };
                _masterHistoryDailyRunResourceSignatureTruckToTruckTransferRepository.Create(historySignature);
            }
            #endregion
        }
        private void InsertLogDolphinAssignToAnotherRun(IEnumerable<TblMasterActualJobHeader> jobsHeader, Guid oldDailyRun)
        {
            #region Insert log DolphinAssignToAnotherRun
            var historyDolphin = new ConcurrentBag<TblMasterHistory_DolphinAssignToAnotherRun>();

            foreach (var job in jobsHeader)
            {
                historyDolphin.Add(new TblMasterHistory_DolphinAssignToAnotherRun()
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = job.Guid,
                    MasterRunResourceDaily_Guid = oldDailyRun,
                    FlagDolphinRemove = false
                });
            }
            _masterHistoryDolphinAssignToAnotherRunRepository.CreateRange(historyDolphin);
            #endregion
        }
        #endregion

        #endregion

        #region++ Validate Truck Liability Limit
        public ValidateTruckLiabilityLimitResponse ValidateTruckLiabilityLimit(ValidateTruckLiabilityLimitRequest request)
        {
            //Target run
            var dailyRun = _masterDailyRunResource.FindById(request.NewDailyRunGuid);
            //Source leg
            var jobInLegList = _masterActualJobServiceStopLegsRepository.FindByLegGuidList(request.LegGuidList);

            LiabilityLimitJobsActionModel runRequestList = new LiabilityLimitJobsActionModel()
            {
                SiteGuid = dailyRun?.MasterSite_Guid,
                RequestList = new List<LiabilityLimitJobsAction> {
                    new LiabilityLimitJobsAction {
                        DailyRunGuid_Target = request.NewDailyRunGuid,
                        DailyRunGuid_Source = request.OldDailyRunGuid,
                        JobGuids = jobInLegList.Select(o => new RawExistJobView { JobGuid = o.MasterActualJobHeader_Guid, JobAction = o.ActionNameAbbr })
                    }
                }
            };
            var req = new LiabilityLimitExistsJobsRequest() { JobsActionModel = runRequestList };

            var result = _truckLiabilityLimitService.IsOverLiabilityLimitWhenExistJobs(req);
            return new ValidateTruckLiabilityLimitResponse()
            {
                isSuccess = result.Message.IsSuccess,
                message = result.Message.MessageTextContent,
                flagExceed = result.TruckLimitDetail.FlagHasExceedJob
            };
        }
        private void InsertHistoryTruckLimit_T2T(TruckToTruckRequest request, IEnumerable<TblMasterActualJobHeader> jobHeaders, DateTimeOffset dateNow)
        {
            if (!string.IsNullOrEmpty(request.TruckLimitReasonName))
            {
                var historyJob = jobHeaders.Select(o => new TblMasterHistory_ActualJob
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = o.Guid,
                    MsgID = 6141,
                    MsgParameter = new string[] { request.TruckLimitReasonName + (string.IsNullOrEmpty(request.TruckLimitComment) ?
                                                 string.Empty : " (" + request.TruckLimitComment + ")"), "Transfer Jobs"}.ToJSONString(),
                    UserCreated = request.UserCreated,
                    DatetimeCreated = request.DatetimeCreated,
                    UniversalDatetimeCreated = dateNow
                });
                _masterHistoryActualJobRepository.CreateRange(historyJob);

                string jobNoList = string.Join(",", jobHeaders.Select(o => o.JobNo));
                List<Guid> listDailyRun = new List<Guid> { request.OldDailyRunGuid, request.NewDailyRunGuid };
                var historyRun = listDailyRun.Select(o => new TblMasterHistory_DailyRunResource
                {
                    Guid = Guid.NewGuid(),
                    MasterDailyRunResource_Guid = o,
                    MsgID = 6140,
                    MsgParameter = new string[] { jobNoList, "Transfer Jobs" }.ToJSONString(),
                    UserCreated = request.UserCreated,
                    DatetimeCreated = request.DatetimeCreated,
                    UniversalDatetimeCreated = dateNow,
                });
                _masterHistoryDailyRunResourceRepository.CreateRange(historyRun);
            }
        }
        #endregion

        #endregion

        #region Validate Assign Jobs to Run
        /// <summary>
        /// => TFS#67763:[Run Control] - TV job is able to assign into the same run -> ValidateAssignJobsToRun
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ValidateAssignJobsToRunResponse ValidateAssignJobsToRun(ValidateAssignJobsToRunRequest request)
        {
            var result = new ValidateAssignJobsToRunResponse();
            var userLanguage_Guid = ApiSession.UserLanguage_Guid.GetValueOrDefault();
            var message = _systemMessageRepository.FindByMsgId(0, userLanguage_Guid).ConvertToMessageView(true);
            try
            {
                var assignJobList = _masterDailyRunResource.ValidateAssignJobsToRun(request.AssignJobList);
                if (assignJobList.Any())
                {
                    var jobIDS = assignJobList.Select(o => o.JobNoID).Distinct();
                    jobIDS = jobIDS.To3DotAfterTake(3);
                    message = _systemMessageRepository.FindByMsgId(-17368, userLanguage_Guid).ConvertToMessageView(false);
                    message.MessageTextContent = string.Format(message.MessageTextContent, string.Join(",", jobIDS.Select(o => o).Distinct()));
                    message.IsWarning = true;
                }
                result.SetMessageView(message);
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                message = _systemMessageRepository.FindByMsgId(-184, userLanguage_Guid).ConvertToMessageView(false);
                result.SetMessageView(message);
            }
            return result;
        }
        #endregion

    }
}
