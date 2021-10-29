using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.ModelViews.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bgt.Ocean.Service.Messagings.RunControlService;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Service.ModelViews.Adhoc;
using Bgt.Ocean.Service.ModelViews;
using Bgt.Ocean.Service.Messagings.AdhocService;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable;
using Bgt.Ocean.Service.Implementations.PushToDolphin;
using Bgt.Ocean.Repository.EntityFramework.Repositories.History;
using Bgt.Ocean.Service.Messagings.CustomerLocationService;
using Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable.SitePath;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Customer;
using Bgt.Ocean.Service.Implementations.Hubs;
using Bgt.Ocean.Repository.EntityFramework.Repositories.RouteOptimization;
using System.Security.Cryptography;

namespace Bgt.Ocean.Service.Implementations.Adhoc
{
    public interface IAdhocService
    {
        IEnumerable<OnwardDestinationTypeView> GetOnwardDestinationType();
        IEnumerable<DisplayTextDropDownView> GetTripIndicator(Guid countryGuid);
        IEnumerable<CustomerLocationInternalDepartmentView> GetOnwarddestination_Internal(Guid siteGuid);
        bool InsertOrUpdateHistoryForUpdateJobAdhoc(Guid jobGuid, string jobNo, DateTime? workdate, string userModifed, DateTime? datetimeModified, DateTimeOffset? universalDatetimeModified, bool FlagJobSFO);

        IEnumerable<SpecialCommandView> GetSpecialCommandByCompany(Guid? companyGuid);
        string GenerateJobNo(Guid BrinkSiteGuid);

        #region Create Job
        string GetPrintedReceiptNumber(PrintedReceiptNumberRequest item);
        string createJobNo(string BrinkSiteCode);
        int getMessageID(bool isSFO);
        Adhoc_LOB_ServiceJobTypeView GetLineOfBusinessAndJobType(bool flagNotDelivery, bool flagAdhoc, bool flagFromMapping);

        DetailDestinationForDeliveryReponse GetDetailDestinationForDelivery(Guid siteGuid, Guid? siteGuidDel, Guid? locationGuid, int jobTypeID);


        CreateJobAdHocResponse CreateJobPickUp(CreateJobAdHocRequest request);
        CreateJobAdHocResponse CreateJobDelivery(CreateJobAdHocRequest request);
        CreateJobAdHocResponse CreateJobTransferVault(CreateJobAdHocRequest request);
        CreateJobAdHocResponse CreateJobTransfer(CreateJobAdHocRequest request);

        IEnumerable<SubJobTypeView> GetSubServiceType(Guid? companyGuid, Guid? LobGuid, int? runStatusID, bool pageMasterRoute);

        AdhocJobResponse CheckDuplicateJobsInDay(CreateJobAdHocRequest request);

        SystemMessageView IsThereEmployeeCanDoOTC(CheckEmployeeCanDoOTC modelEmp);

        CreateJobAdHocResponse CheckMachineAssociate(CreateJobAdHocRequest request);

        CreateJobAdHocResponse CreateJobTransferVaultMultiBranch(CreateJobAdHocRequest request);
        CreateJobAdHocResponse CreateJobPickUpMultiBranch(CreateJobAdHocRequest request);
        #endregion

        DetailDestinationForMultibranchReponseJobP GetDetailDestinationForDelivery_MultiBranchJobP(Guid? siteGuid, Guid? locationGuid);
        MultiBrDetailResponse GetAdhocMultiBrDeliveryDetailByOriginLocation(GetMultiBrDestinationDetailRequest req);

        GetAdhocAllCustomerAndLocationResponse GetAdhocAllCustomerBySite(GetAdhocAllCustomerAndLocationRequest req);
        GetAdhocAllCustomerAndLocationResponse GetAdhocAllLocationByCustomer(GetAdhocAllCustomerAndLocationRequest req);
        CreateJobAdHocResponse CheckDailyRunResourceUnderAlarm(int MsgID, Guid languageGuid, params Guid[] dailyRunGuid);

        SystemMessageView UpdateJobOrderInRun(UpdateJobOrderInRunRequest request);
        void UpdatePushToDolPhinWhenCreateJob(UpdateJobOrderInRunRequest request);

        SystemMessageView CreateHeaderOtcByJobGuids(IEnumerable<Guid> jobGuids);
        CreateJobAdHocResponse CreateResponseFromID(int MsgID, Guid languageGuid);
    }
    public partial class AdhocService : IAdhocService
    {
        private readonly IMasterCustomerRepository _masterCustomerRepository;
        private readonly IMasterCustomerLocationRepository _masterCustomerLocationRepository;
        private readonly IMasterCustomerContractRepository _masterCustomerContractRepository;
        private readonly IMasterCustomerContractServiceLocationRepository _masterCustomerContract_ServiceLocationRepository;
        private readonly ISystemLineOfBusinessRepository _systemLineOfBusinessRepository;
        private readonly IMasterActualJobHeaderRepository _masterActualJobHeaderRepository;
        private readonly IMasterActualJobServiceStopsRepository _masterActualJobServiceStopsRepository;
        private readonly IMasterActualJobServiceStopLegsRepository _masterActualJobServiceStopLegsRepository;
        private readonly ISystemRunningValueGlobalRepository _systemRunningValueGlobalRepository;
        private readonly IMasterActualJobServiceStopSpecialCommandRepository _masterActualJobServiceStopSpecialCommandsRepository;
        private readonly IMasterHistoryActualJobOnDailyRunResourceRepository _masterHistoryActaulJobOnDailyRunResourceRepository;
        private readonly IMasterHistoryActualJobRepository _masterHistoryActualJobRepository;
        private readonly IMasterHistory_DailyRunResourceRepository _masterHistory_DailyRunResource;
        private readonly IMasterSiteRepository _masterSiteRepository;
        private readonly IMasterSitePathRepository _masterSitePathRepository;

        private readonly IOTCRunControlService _OTCRunControlService;
        private readonly ISystemService _systemService;
        private readonly ISystemOnwardDestinationTypesRepository _systemOnwardDestinationTypesRepository;
        private readonly IMasterCustomerLocationInternalDepartmentRepository _masterCustomerLocationInternalDepartmentRepository;
        private readonly ISystemInternalDepartmentTypesRepository _systemInternalDepartmentTypesRepository;
        private readonly ISystemServiceJobTypeLOBRepository _systemServiceJobTypeLOBRepository;
        private readonly ISystemJobActionsRepository _systemJobActionsRepository;
        private readonly ISystemServiceJobTypeRepository _systemServiceJobTypeRepository;
        private readonly IMasterSpecialCommandRepository _masterSpecialCommandRepository;
        private readonly ISystemTripIndicatorRepository _systemTripIndicatorRepository;
        private readonly ISystemServiceStopTypesRepository _systemServiceStopTypesRepository;
        private readonly IMasterDailyRunResourceRepository _masterDailyRunResourceRepository;
        private readonly IMasterSubServiceTypeRepository _masterSubServiceTypeRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IMasterDenominationRepository _masterDenominationRepository;
        private readonly ISFOMasterMachineCassetteRepository _sfoMasterMachineCassetteRepository;
        private readonly ISFOMasterMachineRepository _sfoMasterMachineRepository;
        private readonly IPushToDolphinService _pushToDolphinService;
        private readonly IMasterRouteGroupDetailRepository _masterRouteGroupDetailRepository;
        private readonly ISystemEnvironmentMasterCountryRepository _systemEnvironmentMasterCountryRepository;
        private readonly ISFOMasterMachineLockTypeRepository _sfoMasterMachineLockTypeRepository;
        private readonly ISystemEnvironment_GlobalRepository _systemEnvironment_GlobalRepository;
        private readonly ISFOMasterOTCLockModeRepository _sfoMasterOTCLockModeRepository;
        private readonly IMasterCustomerLocationLocationDestinationRepository _masterCustomerLocation_LocationDestinationRepository;
        private readonly IMasterActualJobActualCountRepository _masterActualJobActualCountRepository;
        private readonly IMasterActualJobCashAddRepository _masterActualJobCashAddRepository;
        private readonly IMasterActualJobCashReturnRepository _masterActualJobCashReturnRepository;
        private readonly IMasterActualJobMachineReportRepository _masterActualJobMachineReportRepository;
        private readonly IMasterActualJobSumActualCountRepository _masterActualJobSumActualCountRepository;
        private readonly IMasterActualJobSumCashAddRepository _masterActualJobSumCashAddRepository;
        private readonly IMasterActualJobSumCashReturnRepository _masterActualJobSumCashReturnRepository;
        private readonly IMasterActualJobSumMachineReportRepository _masterActualJobSumMachineReportRepository;
        private readonly IMasterActualJobHeaderCapabilityRepository _masterActualJobHeaderCapabilityRepository;
        private readonly ISystemEnvironmentMasterCountryValueRepository _systemEnvironmentMasterCountryValueRepository;
        private readonly IMasterCustomerJobHideScreenRepository _masterCustomerJobHideScreenRepository;
        private readonly IMasterActualJobHideScreenMappingRepository _masterActualJobHideScreenMappingRepository;
        private readonly IAlarmHubService _alarmHubService;
        private readonly IMasterActualJobMCSCoinCashReturnRepository _masterActualJobMCSCoinCashReturnRepository;
        private readonly IMasterActualJobMCSCoinSuspectFakeRepository _masterActualJobMCSCoinSuspectFakeRepository;
        private readonly IMasterActualJobMCSCoinMachineBalanceRepository _masterActualJobMCSCoinMachineBalanceRepository;
        private readonly IMasterActualJobMCSCoinBulkNoteCollectRepository _masterActualJobMCSCoinBulkNoteCollectRepository;
        private readonly IMasterActualJobMCSRecyclingActualCountRepository _masterActualJobMCSRecyclingActualCountRepository;
        private readonly IMasterActualJobMCSRecyclingCashRecyclingRepository _masterActualJobMCSRecyclingCashRecyclingRepository;
        private readonly IMasterActualJobMCSRecyclingMachineReportRepository _masterActualJobMCSRecyclingMachineReportRepository;
        private readonly IMasterMCSBulkDepositReportRepository _masterActualJobMCSBulkDepositReportRepository;
        private readonly IMasterMCSBulkSuspectFakeRepository _masterActualJobMCSBulkSuspectFakeRepository;
        private readonly IMasterActualJobMCSBulkJammedRepository _masterActualJobMCSBulkJammedRepository;
        private readonly IMasterMCSBulkRetractRepository _masterActualJobMCSBulkRetractRepository;
        private readonly IMasterActualJobMCSCoinCashAddRepository _masterActualJobMCSCoinCashAddRepository;
        private readonly ITransactionRouteOptimizationHeaderRepository _transactionRouteOptimizationHeaderRepository;
        public AdhocService(
            IMasterCustomerRepository masterCustomerRepository,
            IMasterCustomerLocationRepository masterCustomerLocationRepository,
            IMasterActualJobHeaderRepository masterActualJobHeaderRepository,
            IMasterActualJobServiceStopsRepository masterActualJobServiceStopsRepository,
            IMasterActualJobServiceStopLegsRepository masterActualJobServiceStopLegsRepository,
            ISystemRunningValueGlobalRepository systemRunningValueGlobalRepository,
            IMasterActualJobServiceStopSpecialCommandRepository masterActualJobServiceStopSpecialCommandsRepository,
            IMasterHistoryActualJobOnDailyRunResourceRepository masterHistoryActaulJobOnDailyRunResourceRepository,
            IMasterHistoryActualJobRepository masterHistoryActualJobRepository,
            IMasterHistory_DailyRunResourceRepository masterHistory_DailyRunResource,
            IMasterSiteRepository masterSiteRepository,
            IOTCRunControlService OTCRunControlService,
            ISystemService systemService,
            ISystemOnwardDestinationTypesRepository systemOnwardDestinationTypesRepository,
            IMasterCustomerLocationInternalDepartmentRepository masterCustomerLocationInternalDepartmentRepository,
            ISystemInternalDepartmentTypesRepository systemInternalDepartmentTypesRepository,
            ISystemServiceJobTypeLOBRepository systemServiceJobTypeLOBRepository,
            ISystemJobActionsRepository systemJobActionsRepository,
            ISystemServiceJobTypeRepository systemServiceJobTypeRepository,
            IMasterSpecialCommandRepository masterSpecialCommandRepository,
            ISystemTripIndicatorRepository systemTripIndicatorRepository,
            ISystemServiceStopTypesRepository systemServiceStopTypesRepository,
            IMasterDailyRunResourceRepository masterDailyRunResourceRepository,
            IMasterSubServiceTypeRepository masterSubServiceTypeRepository,
            IUnitOfWork<OceanDbEntities> uow,
            ISystemMessageRepository systemMessageRepository,
            IMasterDenominationRepository masterDenominationRepository,
            ISFOMasterMachineCassetteRepository sfoMasterMachineCassetteRepository,
            ISFOMasterMachineRepository sfoMasterMachineRepository,
            IPushToDolphinService pushToDolphinService,
            IMasterRouteGroupDetailRepository masterRouteGroupDetailRepository,
            ISystemEnvironmentMasterCountryRepository systemEnvironmentMasterCountryRepository,
            ISFOMasterMachineLockTypeRepository sfoMasterMachineLockTypeRepository,
            ISystemEnvironment_GlobalRepository systemEnvironment_GlobalRepository,
            ISFOMasterOTCLockModeRepository sfoMasterOTCLockModeRepository,
            IMasterCustomerLocationLocationDestinationRepository masterCustomerLocation_LocationDestinationRepository,
            IMasterActualJobSumMachineReportRepository masterActualJobSumMachineReportRepository,
            IMasterActualJobActualCountRepository masterActualJobActualCountRepository,
            IMasterActualJobCashAddRepository masterActualJobCashAddRepository,
            IMasterActualJobCashReturnRepository masterActualJobCashReturnRepository,
            IMasterActualJobMachineReportRepository masterActualJobMachineReportRepository,
            IMasterActualJobSumActualCountRepository masterActualJobSumActualCountRepository,
            IMasterActualJobSumCashAddRepository masterActualJobSumCashAddRepository,
            IMasterActualJobSumCashReturnRepository masterActualJobSumCashReturnRepository,
            IMasterActualJobHeaderCapabilityRepository masterActualJobHeaderCapabilityRepository,
            IMasterSitePathRepository masterSitePathRepository,
            ISystemEnvironmentMasterCountryValueRepository systemEnvironmentMasterCountryValueRepository,
            IMasterCustomerContractServiceLocationRepository masterCustomerContract_ServiceLocationRepository,
            ISystemLineOfBusinessRepository systemLineOfBusinessRepository,
            IMasterCustomerContractRepository masterCustomerContractRepository,
            IMasterCustomerJobHideScreenRepository masterCustomerJobHideScreenRepository,
            IMasterActualJobHideScreenMappingRepository masterActualJobHideScreenMappingRepository,
            IAlarmHubService alarmHubService,
            IMasterActualJobMCSCoinCashReturnRepository masterActualJobMCSCoinCashReturnRepository,
            IMasterActualJobMCSCoinSuspectFakeRepository masterActualJobMCSCoinSuspectFakeRepository,
            IMasterActualJobMCSCoinMachineBalanceRepository masterActualJobMCSCoinMachineBalanceRepository,
            IMasterActualJobMCSCoinBulkNoteCollectRepository masterActualJobMCSCoinBulkNoteCollectRepository,
            IMasterActualJobMCSCoinCashAddRepository masterActualJobMCSCoinCashAddRepository,
            IMasterActualJobMCSRecyclingActualCountRepository masterActualJobMCSRecyclingActualCountRepository,
            IMasterActualJobMCSRecyclingCashRecyclingRepository masterActualJobMCSRecyclingCashRecyclingRepository,
            IMasterActualJobMCSRecyclingMachineReportRepository masterActualJobMCSRecyclingMachineReportRepository,
            IMasterActualJobMCSBulkJammedRepository masterActualJobMCSBulkJammedRepository,
            IMasterMCSBulkDepositReportRepository masterActualJobMCSBulkDepositReportRepository,
            IMasterMCSBulkSuspectFakeRepository masterActualJobMCSBulkSuspectFakeRepository,
            IMasterMCSBulkRetractRepository masterActualJobMCSBulkRetractRepository,
            ITransactionRouteOptimizationHeaderRepository transactionRouteOptimizationHeaderRepository
        )
        {
            _systemLineOfBusinessRepository = systemLineOfBusinessRepository;
            _masterCustomerRepository = masterCustomerRepository;
            _masterCustomerLocationRepository = masterCustomerLocationRepository;
            _masterCustomerContract_ServiceLocationRepository = masterCustomerContract_ServiceLocationRepository;
            _masterActualJobHeaderRepository = masterActualJobHeaderRepository;
            _masterActualJobServiceStopsRepository = masterActualJobServiceStopsRepository;
            _masterActualJobServiceStopLegsRepository = masterActualJobServiceStopLegsRepository;
            _systemRunningValueGlobalRepository = systemRunningValueGlobalRepository;
            _masterActualJobServiceStopSpecialCommandsRepository = masterActualJobServiceStopSpecialCommandsRepository;
            _masterHistoryActaulJobOnDailyRunResourceRepository = masterHistoryActaulJobOnDailyRunResourceRepository;
            _masterHistoryActualJobRepository = masterHistoryActualJobRepository;
            _masterHistory_DailyRunResource = masterHistory_DailyRunResource;
            _masterSiteRepository = masterSiteRepository;
            _OTCRunControlService = OTCRunControlService;
            _systemService = systemService;
            _systemOnwardDestinationTypesRepository = systemOnwardDestinationTypesRepository;
            _masterCustomerLocationInternalDepartmentRepository = masterCustomerLocationInternalDepartmentRepository;
            _systemInternalDepartmentTypesRepository = systemInternalDepartmentTypesRepository;
            _systemServiceJobTypeLOBRepository = systemServiceJobTypeLOBRepository;
            _systemJobActionsRepository = systemJobActionsRepository;
            _systemServiceJobTypeRepository = systemServiceJobTypeRepository;
            _masterSpecialCommandRepository = masterSpecialCommandRepository;
            _systemTripIndicatorRepository = systemTripIndicatorRepository;
            _systemServiceStopTypesRepository = systemServiceStopTypesRepository;
            _masterDailyRunResourceRepository = masterDailyRunResourceRepository;
            _masterSubServiceTypeRepository = masterSubServiceTypeRepository;
            _uow = uow;
            _systemMessageRepository = systemMessageRepository;
            _masterDenominationRepository = masterDenominationRepository;
            _sfoMasterMachineCassetteRepository = sfoMasterMachineCassetteRepository;
            _sfoMasterMachineRepository = sfoMasterMachineRepository;
            _pushToDolphinService = pushToDolphinService;
            _masterRouteGroupDetailRepository = masterRouteGroupDetailRepository;
            _systemEnvironmentMasterCountryRepository = systemEnvironmentMasterCountryRepository;
            _sfoMasterMachineLockTypeRepository = sfoMasterMachineLockTypeRepository;
            _systemEnvironment_GlobalRepository = systemEnvironment_GlobalRepository;
            _sfoMasterOTCLockModeRepository = sfoMasterOTCLockModeRepository;
            _masterCustomerLocation_LocationDestinationRepository = masterCustomerLocation_LocationDestinationRepository;
            _masterActualJobSumMachineReportRepository = masterActualJobSumMachineReportRepository;
            _masterActualJobActualCountRepository = masterActualJobActualCountRepository;
            _masterActualJobCashAddRepository = masterActualJobCashAddRepository;
            _masterActualJobCashReturnRepository = masterActualJobCashReturnRepository;
            _masterActualJobMachineReportRepository = masterActualJobMachineReportRepository;
            _masterActualJobSumActualCountRepository = masterActualJobSumActualCountRepository;
            _masterActualJobSumCashAddRepository = masterActualJobSumCashAddRepository;
            _masterActualJobSumCashReturnRepository = masterActualJobSumCashReturnRepository;
            _masterActualJobHeaderCapabilityRepository = masterActualJobHeaderCapabilityRepository;
            _masterSitePathRepository = masterSitePathRepository;
            _systemEnvironmentMasterCountryValueRepository = systemEnvironmentMasterCountryValueRepository;
            _masterCustomerContractRepository = masterCustomerContractRepository;
            _masterCustomerJobHideScreenRepository = masterCustomerJobHideScreenRepository;
            _masterActualJobHideScreenMappingRepository = masterActualJobHideScreenMappingRepository;
            _alarmHubService = alarmHubService;
            _masterActualJobMCSCoinCashReturnRepository = masterActualJobMCSCoinCashReturnRepository;
            _masterActualJobMCSCoinSuspectFakeRepository = masterActualJobMCSCoinSuspectFakeRepository;
            _masterActualJobMCSCoinMachineBalanceRepository = masterActualJobMCSCoinMachineBalanceRepository;
            _masterActualJobMCSCoinBulkNoteCollectRepository = masterActualJobMCSCoinBulkNoteCollectRepository;
            _masterActualJobMCSCoinCashAddRepository = masterActualJobMCSCoinCashAddRepository;
            _masterActualJobMCSRecyclingActualCountRepository = masterActualJobMCSRecyclingActualCountRepository;
            _masterActualJobMCSRecyclingCashRecyclingRepository = masterActualJobMCSRecyclingCashRecyclingRepository;
            _masterActualJobMCSRecyclingMachineReportRepository = masterActualJobMCSRecyclingMachineReportRepository;
            _masterActualJobMCSBulkJammedRepository = masterActualJobMCSBulkJammedRepository;
            _masterActualJobMCSBulkDepositReportRepository = masterActualJobMCSBulkDepositReportRepository;
            _masterActualJobMCSBulkSuspectFakeRepository = masterActualJobMCSBulkSuspectFakeRepository;
            _masterActualJobMCSBulkRetractRepository = masterActualJobMCSBulkRetractRepository;
            _transactionRouteOptimizationHeaderRepository = transactionRouteOptimizationHeaderRepository;
        }
        public SystemMessageView CreateHeaderOtcByJobGuids(IEnumerable<Guid> jobGuids) {

            return _OTCRunControlService.CreateHeaderOtcByJobGuids(jobGuids);
        }

        #region Get Onward Type
        public IEnumerable<OnwardDestinationTypeView> GetOnwardDestinationType()
        {
            return _systemOnwardDestinationTypesRepository.FindAll(o => o.FlagDisable == false)
                .Select(o => new OnwardDestinationTypeView
                {
                    Guid = o.Guid,
                    OnwardDestinationName = o.OnwardDestinationName,
                    OnwardDestinationTypeID = o.OnwardDestinationTypeID
                });
        }

        public IEnumerable<CustomerLocationInternalDepartmentView> GetOnwarddestination_Internal(Guid siteGuid)
        {
            return _masterCustomerLocationInternalDepartmentRepository.GetInternalDeptOnwardDestination(siteGuid).ConvertToCustomerLocationInternalDept();
        }
        #endregion

        #region Create Job From Adhoc
        public string GetPrintedReceiptNumber(PrintedReceiptNumberRequest item)
        {
            item.BranchCodeReference = String.IsNullOrEmpty(item.BranchCodeReference) ? "9999" : item.BranchCodeReference;
            String Date_Formate = "yyyyMMdd";
            String Date = ((DateTime)item.ServiceStopTransectionDate).ToString(Date_Formate);
            String LocCode = item.BranchCodeReference.Length > 4 ? item.BranchCodeReference.Substring(0, 4) : item.BranchCodeReference;
            String SiteCode = String.IsNullOrEmpty(item.SiteCode) ? String.Empty : item.SiteCode;
            String JobID = item.JobNo == null ? "0000" : item.JobNo.ToString();
            JobID = JobID.Substring(JobID.Length - 4);

            return Date + SiteCode + LocCode + JobID + item.SequenceStop.ToString();
        }
        public string createJobNo(string BrinkSiteCode)
        {
            var rnd = RandomNumberGenerator.Create();
            byte[] tokenBuffer = new byte[4];      
            rnd.GetBytes(tokenBuffer);
            int ran = Math.Abs(BitConverter.ToInt32(tokenBuffer, 0)) % 10;
            var tbljobrun = _systemRunningValueGlobalRepository.FindAll(o => o.RunningKey.Equals(FixStringRoute.JobNo)).FirstOrDefault();
            tbljobrun.RunningVaule1++;
            _systemRunningValueGlobalRepository.Modify(tbljobrun);
            return string.Format("{0}{1}{2}", BrinkSiteCode, tbljobrun.RunningVaule1, ran); //expect to throw error if tbljobrun is null
        }

        public string GenerateJobNo(Guid BrinkSiteGuid)
        {
            var lastStr = SecureHelper.GenerateRandomNumber();
            string jobNo = "";

            using (var tran = _uow.BeginTransaction())
            {
                var siteCode = _masterSiteRepository.FindById(BrinkSiteGuid);
                if (siteCode != null)
                {
                    var tbljobrun = _systemRunningValueGlobalRepository.FindAll().FirstOrDefault(o => o.RunningKey.Equals(FixStringRoute.JobNo));
                    tbljobrun.RunningVaule1 = tbljobrun.RunningVaule1 + 1;
                    jobNo = string.Format("{0}{1}{2}", siteCode.SiteCode, tbljobrun.RunningVaule1, lastStr);
                    _systemRunningValueGlobalRepository.Modify(tbljobrun);
                    _uow.Commit();
                    tran.Complete();
                }
            }
            return jobNo;
        }

        private List<string> GenerateJobNoMultiJob(Guid BrinkSiteGuid, int jobAmount)
        {
            var rnd = RandomNumberGenerator.Create();
            List<string> jobNo = null;
            int lastJobNo = 0;
            var siteCode = _masterSiteRepository.FindById(BrinkSiteGuid);
            if (siteCode != null && jobAmount > 0)
            {
                using (var tran = _uow.BeginTransaction())
                {
                    var tbljobrun = _systemRunningValueGlobalRepository.FindAll(o => o.RunningKey.Equals(FixStringRoute.JobNo)).FirstOrDefault();
                    tbljobrun.RunningVaule1 += jobAmount;
                    lastJobNo = tbljobrun.RunningVaule1;
                    _systemRunningValueGlobalRepository.Modify(tbljobrun);
                    _uow.Commit();
                    tran.Complete();

                }
                if (lastJobNo != 0)
                {
                    jobNo = new List<string>();
                    StringBuilder lastStr = null;
                    for (int i = 0; i < jobAmount; i++)
                    {
                        lastStr = new StringBuilder();
                        for (int j = 0; j < 3; j++)
                        {
                            byte[] tokenBuffer = new byte[4];
                            rnd.GetBytes(tokenBuffer);
                            int ran = Math.Abs(BitConverter.ToInt32(tokenBuffer, 0)) % 10;
                            lastStr.Append(ran.ToString());
                        }
                        jobNo.Add(string.Format("{0}{1}{2}", siteCode.SiteCode, lastJobNo + i, lastStr));
                    }
                }
                else
                {
                    throw new Exception("Not found lasted JobNo.");
                }
            }
            else
            {
                throw new Exception("Not found site code.");
            }
            return jobNo;
        }

        public int getMessageID(bool isSFO)
        {
            if (isSFO)
                return 753; //Created job no. {0} date {1} by Service Request.
            return 653;  //Created job no. {0} date {1} from Adhoc Job by Ocean Online MVC.
        }

        #region ### Job Pickup     
        #endregion

        #region ### Job Transfer Vault

        #endregion

        #region ### Job Transfer

        #endregion

        #endregion

        #region Update Job From Adhoc
        public bool InsertOrUpdateHistoryForUpdateJobAdhoc(Guid jobGuid, string jobNo, DateTime? workdate, string userModifed, DateTime? datetimeModified, DateTimeOffset? universalDatetimeModified, bool FlagJobSFO)
        {
            int msgID = getMessageID(FlagJobSFO);
            var historyJob = _masterHistoryActualJobRepository.FindByJobMsg(jobGuid, msgID);
            var msgParameter = new string[] { jobNo, workdate.GetValueOrDefault().ToString("MM/dd/yyyy") }.ToJSONString();
            if (historyJob != null)
            {
                if (historyJob.MsgParameter == msgParameter)
                    return true;

                historyJob.MsgParameter = jobNo + "," + workdate.GetValueOrDefault().ToString("MM/dd/yyyy");
                historyJob.UserCreated = userModifed;
                historyJob.DatetimeCreated = datetimeModified;
                historyJob.UniversalDatetimeCreated = universalDatetimeModified;
                _masterHistoryActualJobRepository.Modify(historyJob);
                return true;
            }

            var insertHistory = new TblMasterHistory_ActualJob
            {
                Guid = Guid.NewGuid(),
                MasterActualJobHeader_Guid = jobGuid,
                MsgID = msgID,//Created job no. {0} date {1} from Adhoc Job by Ocean Online MVC.
                MsgParameter = jobNo + "," + workdate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                UserCreated = userModifed,
                DatetimeCreated = datetimeModified,
                UniversalDatetimeCreated = universalDatetimeModified
            };
            _masterHistoryActualJobRepository.Create(insertHistory);
            return true;
        }

        #region ### Job Pickup

        #endregion

        #region ### Job Transfer Vault

        #endregion

        #region ### Job Transfer

        #endregion

        #endregion


        public IEnumerable<SpecialCommandView> GetSpecialCommandByCompany(Guid? companyGuid)
        {
            var response = _masterSpecialCommandRepository.FindAll(
                o => !o.FlagDisable && o.MasterCustomer_Guid == companyGuid)
                .Select(o => new SpecialCommandView
                {
                    SpecialCommandGuid = o.Guid,
                    CommandName = o.SpecialCommandName,
                    AbbreviationCommand = o.Abbreviation
                }).OrderBy(o => o.AbbreviationCommand).ToList();
            return response;
        }

        public CreateJobAdHocResponse CheckDailyRunResourceUnderAlarm(int MsgID, Guid languageGuid, params Guid[] dailyRunGuid)
        {
            CreateJobAdHocResponse res = null;
            var alarm = _alarmHubService.IsHasAlarm(dailyRunGuid)?.ToList();
            if (alarm.Any())
            {
                TblSystemMessage tblMsg = _systemMessageRepository.FindByMsgId(MsgID, languageGuid);
                res = new CreateJobAdHocResponse(tblMsg);
                res.IsWarning = true;
            }
            return res;
        }

        public CreateJobAdHocResponse CreateResponseFromID(int MsgID, Guid languageGuid)
        {
            TblSystemMessage tblMsg = _systemMessageRepository.FindByMsgId(MsgID, languageGuid);
            return new CreateJobAdHocResponse(tblMsg);
        }

        private CreateJobAdHocResponse getMessageIDFromRouteAndRun(string RouteName, string RunName, string JobNo, Guid languageGuid)
        {
            List<string> arr_message_vars = new List<string>();
            int MsgID = 0;
            if (!string.IsNullOrEmpty(RunName)) //have Run
            {
                MsgID = 321; //Job ID <b>{0}</b> has been generated and assigned to Run Resource No. <b>{1}</b>. (MessageTextTitle: Saved)
                arr_message_vars.Add(JobNo);       ////{0}
                arr_message_vars.Add(RunName);     ////{1}
            }
            else if (!string.IsNullOrEmpty(RouteName)) //No Run but have Route
            {
                MsgID = 320; //The Route <b>{0}</b> is not available for this day, The Job ID <b>{1}</b> will be unassigned job. (MessageTextTitle: Saved)
                arr_message_vars.Add(RouteName);   ////{0}
                arr_message_vars.Add(JobNo);       ////{1}
            }
            else //User didn't choose Route and Run
            {
                MsgID = 322; //Job ID <b>{0}</b> has been generated. (MessageTextTitle: Saved)
                arr_message_vars.Add(JobNo);       ////{0}
            }

            TblSystemMessage tblMsg = _systemMessageRepository.FindByMsgId(MsgID, languageGuid);
            CreateJobAdHocResponse adhocResponse = new CreateJobAdHocResponse(tblMsg);
            adhocResponse.IsSuccess = true;
            adhocResponse.MessageTextContent = string.Format(tblMsg.MessageTextContent, arr_message_vars.ToArray());
            return adhocResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="refStop"></param>
        private void CreateAdhocAddSpot(CreateJobAdHocRequest req, List<TblMasterActualJobServiceStopLegs> refStop)
        {
            if (req.flagAddSpot)
            {
                int? jobOrderSpot = 0;
                var actionD_Guid = _systemJobActionsRepository.FindByAbbrevaition(JobActionAbb.StrDelivery).Guid;
                IEnumerable<TblMasterActualJobServiceStopLegs> ls = null;
                int[] jobtype = new int[] { IntTypeJob.T, IntTypeJob.TV, IntTypeJob.TV_MultiBr };

                Func<TblMasterActualJobServiceStopLegs, bool> IsMergeCase = (refLeg) =>
                {
                    //merge case by default ===> TV-D,T-D and Job Same Location in run ** must be update by job reorder function
                    refLeg.JobOrder = 100000;
                    refLeg.SeqIndex = 100000;

                    var flagMerge = true;
                    var hasDailyRun = refLeg.MasterRunResourceDaily_Guid != null;
                    var ActionD = refLeg.CustomerLocationAction_Guid == actionD_Guid;
                    var IsT_TV_Delivery = jobtype.Any(t => t == req.AdhocJobHeaderView.ServiceJobTypeID) && ActionD;
                    var IsAllLegP = !(IsT_TV_Delivery || !hasDailyRun);
                    if (IsAllLegP)
                    {
                        ls = ls ?? _masterActualJobServiceStopLegsRepository.FindByDailyRun(refLeg.MasterRunResourceDaily_Guid).ToList();
                        var hasSamelocation = ls.Any(o => o.MasterCustomerLocation_Guid == refLeg.MasterCustomerLocation_Guid);
                        if (!hasSamelocation)
                        {
                            jobOrderSpot = !ActionD ? req.ServiceStopLegPickup.jobOrderSpot : req.ServiceStopLegDelivery.jobOrderSpot;
                            refLeg.JobOrder = jobOrderSpot;
                            refLeg.SeqIndex = 9999;
                            flagMerge = false;
                        }
                    }
                    return flagMerge;
                };

                var allSpotLeg = refStop.Select(refLeg => new { IsMergeCase = IsMergeCase(refLeg), refLeg });
                var spotLegs = allSpotLeg.Where(refLeg => !refLeg.IsMergeCase).Select((o, i) => { o.refLeg.JobOrder += i; return o.refLeg; });
                if (spotLegs.Any())
                {
                    var minInSpotLegs = spotLegs.Min(o => o.JobOrder);
                    var maxInSpotLegs = spotLegs.Max(o => o.JobOrder);
                    var shiftOrderLegs = ls.Where(o => o.JobOrder >= minInSpotLegs);

                    foreach (var l in shiftOrderLegs)
                    {
                        l.JobOrder += maxInSpotLegs;
                        //update leg
                        _masterActualJobServiceStopLegsRepository.Modify(l);
                    }

                    if (shiftOrderLegs.Any())
                    {
                        var jobGuids = shiftOrderLegs.Select(o => o.MasterActualJobHeader_Guid).Distinct();
                        var jh = _masterActualJobHeaderRepository.FindByListJob(jobGuids);
                        foreach (var h in jh)
                        {
                            var alreadySyncToMobile = ConstFlagSyncToMobile.AlreadySyncToMobile == h.FlagSyncToMobile;
                            //update head
                            h.FlagJobReOrder = true;
                            h.FlagSyncToMobile = alreadySyncToMobile ? ConstFlagSyncToMobile.UpdateToMobile : h.FlagSyncToMobile;
                            _masterActualJobHeaderRepository.Modify(h);
                        }
                    }

                }

            }
        }
    }
}
