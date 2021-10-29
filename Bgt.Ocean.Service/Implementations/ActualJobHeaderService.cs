using System;
using System.Linq;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Customer;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using static Bgt.Ocean.Infrastructure.Util.EnumRun;
using Bgt.Ocean.Service.Messagings.RunControlService;
using Bgt.Ocean.Service.ModelViews.ActualJobHeader;
using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Service.Implementations.Adhoc;

namespace Bgt.Ocean.Service.Implementations
{
    #region interface

    public interface IActualJobHeaderService
    {
        /// <summary>
        /// For IncidentController DO NOT DELETE
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool CreateJobForSFO(AdhocJobDetailJobView request);
    }

    #endregion

    public class ActualJobHeaderService : IActualJobHeaderService
    {
        private readonly IMasterActualJobHeaderRepository _masterActualJobHeaderRepository;
        private readonly IMasterActualJobServiceStopLegsRepository _masterActualJobServiceStopLegRepository;
        private readonly IMasterActualJobServiceStopsRepository _masterActualJobServiceStopsRepository;
        private readonly IMasterCustomerContractServiceLocationRepository _masterCustomerContract_ServiceLocationRepository;
        private readonly IMasterCustomerLocationRepository _masterCustomerLocationRepository;
        private readonly IMasterCustomerRepository _masterCustomerRepository;
        private readonly IMasterDailyRunResourceRepository _masterDailyRunResourceRepository;
        private readonly IMasterHistoryActualJobOnDailyRunResourceRepository _masterHistory_ActaulJobOnDailyRunResourceRepository;
        private readonly IMasterHistoryActualJobRepository _masterHistoryActualJobRepository;
        private readonly IMasterSiteRepository _masterSiteRepository;
        private readonly ISystemJobActionsRepository _systemJobActionsRepository;
        private readonly ISystemService _systemService;
        private readonly ISystemServiceJobTypeRepository _systemServiceJobTypeRepository;
        private readonly ISystemServiceStopTypesRepository _systemServiceStopTypesRepository;
        private readonly IAdhocService _adhocService;

        public ActualJobHeaderService(
                IMasterActualJobHeaderRepository masterActualJobHeaderRepository,
                IMasterActualJobServiceStopLegsRepository masterActualJobServiceStopLegRepository,
                IMasterActualJobServiceStopsRepository masterActualJobServiceStopsRepository,
                IMasterCustomerContractServiceLocationRepository masterCustomerContract_ServiceLocationRepository,
                IMasterCustomerLocationRepository masterCustomerLocationRepository,
                IMasterCustomerRepository masterCustomerRepository,
                IMasterDailyRunResourceRepository masterDailyRunResourceRepository,
                IMasterHistoryActualJobOnDailyRunResourceRepository masterHistory_ActaulJobOnDailyRunResourceRepository,
                IMasterHistoryActualJobRepository masterHistoryActualJobRepository,
                IMasterSiteRepository masterSiteRepository,
                ISystemJobActionsRepository systemJobActionsRepository,
                ISystemService systemService,
                ISystemServiceJobTypeRepository systemServiceJobTypeRepository,
                ISystemServiceStopTypesRepository systemServiceStopTypesRepository,
                IAdhocService adhocService
            )
        {
            _masterActualJobHeaderRepository = masterActualJobHeaderRepository;
            _masterActualJobServiceStopLegRepository = masterActualJobServiceStopLegRepository;
            _masterActualJobServiceStopsRepository = masterActualJobServiceStopsRepository;
            _masterCustomerContract_ServiceLocationRepository = masterCustomerContract_ServiceLocationRepository;
            _masterCustomerLocationRepository = masterCustomerLocationRepository;
            _masterCustomerRepository = masterCustomerRepository;
            _masterDailyRunResourceRepository = masterDailyRunResourceRepository;
            _masterHistory_ActaulJobOnDailyRunResourceRepository = masterHistory_ActaulJobOnDailyRunResourceRepository;
            _masterHistoryActualJobRepository = masterHistoryActualJobRepository;
            _masterSiteRepository = masterSiteRepository;
            _systemJobActionsRepository = systemJobActionsRepository;
            _systemService = systemService;
            _systemServiceJobTypeRepository = systemServiceJobTypeRepository;
            _systemServiceStopTypesRepository = systemServiceStopTypesRepository;
            _adhocService = adhocService;
        }

        public bool CreateJobForSFO(AdhocJobDetailJobView request)
        {

            // Set Data
            request = SetDataBeforeCreateJob(request);

            // Get Type Action a
            var tblJobActions = _systemJobActionsRepository.FindAll();
            var actionGuidPK = tblJobActions.FirstOrDefault(o => o.ActionNameAbbrevaition.Equals(JobActionAbb.StrPickUp)).Guid;
            var actionGuidDEL = tblJobActions.FirstOrDefault(o => o.ActionNameAbbrevaition.Equals(JobActionAbb.StrDelivery)).Guid;

            return CreateJobPickUp(request, actionGuidPK, actionGuidDEL); //This function is for SF only

        }

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

        #region Private

        private AdhocJobDetailJobView SetDataBeforeCreateJob(AdhocJobDetailJobView request)
        {
            request.AdhocJobHeaderView.ServiceTypeGuid = _systemServiceJobTypeRepository.FindByTypeId(request.AdhocJobHeaderView.ServiceTypeID).Guid;

            #region Preprae data

            var serviceTypeID = request.AdhocJobHeaderView.ServiceTypeID;

            // Get Contract Guid From CustomerLocation #PK
            var tblContractPK = _masterCustomerContract_ServiceLocationRepository.FindAllAsQueryable(o => o.MasterCustomerLocation_Guid.Equals(request.LocationGuid));
            request.ContractGuid = null;
            if (tblContractPK.Count() == 1)
                request.ContractGuid = tblContractPK.Select(o => o.MasterCustomerContract_Guid).FirstOrDefault();

            // Get StopType = Service Stop
            request.AdhocJobHeaderView.ServiceStopTypesGuid = _systemServiceStopTypesRepository.FindAllAsQueryable().FirstOrDefault(o => o.InternalID == 1).Guid;

            // Get Sataus Run and Set SatatusJob
            request.AdhocJobHeaderView.StatusJob = IntStatusJob.Open;
            if (request.RunResoureGuid != null || request.RunResoureGuidDEL != null)
            {
                switch (serviceTypeID)
                {
                    case IntTypeJob.P:
                    case IntTypeJob.P_MultiBr:
                    case IntTypeJob.BCP:
                    case IntTypeJob.FLM:
                    case IntTypeJob.ECash: //added: 2018/01/22
                    case IntTypeJob.FSLM: //added: 2018/01/22
                    case IntTypeJob.TM: //added: 2018/01/22
                    case IntTypeJob.TV:
                    case IntTypeJob.TV_MultiBr:
                    case IntTypeJob.T:
                        if (request.RunResoureGuid != null)
                        {
                            var statusRunID = _masterDailyRunResourceRepository.FindById(request.RunResoureGuid.GetValueOrDefault()).RunResourceDailyStatusID.GetValueOrDefault();
                            request.AdhocJobHeaderView.StatusJob = (statusRunID == StatusDailyRun.ReadyRun ?
                                (serviceTypeID == IntTypeJob.TV || serviceTypeID == IntTypeJob.TV_MultiBr ? IntStatusJob.OnTruckPickUp : IntStatusJob.OnTruck)
                                : (serviceTypeID == IntStatusJob.OnTruck ? IntStatusJob.OnTheWayPickUp : IntStatusJob.OnTheWay));
                        }
                        break;
                    case IntTypeJob.D:
                    case IntTypeJob.AC:
                    case IntTypeJob.AE:
                    case IntTypeJob.BCD:
                    case IntTypeJob.BCD_MultiBr:
                        request.AdhocJobHeaderView.StatusJob = IntStatusJob.Process;
                        break;
                    default:
                        request.AdhocJobHeaderView.StatusJob = IntStatusJob.intNoStatus;
                        break;
                }
            }

            #endregion


            #region CustomerLocation in brinkscompany Leg D : [ Interbranch ]
            if (request.InterbranchGuid != null)
            {
                var resultInterbranchDEL = _masterCustomerLocationRepository.FindAllAsQueryable(e => e.MasterSite_Guid == request.InterbranchGuid)
                        .Join(_masterCustomerRepository.FindAllAsQueryable(e => !e.FlagChkCustomer.Value),
                            CL => CL.MasterCustomer_Guid,
                            C => C.Guid,
                            (CL, C) => new { CusLocation = CL, Customer = C }).Select(o => o.CusLocation.Guid).FirstOrDefault();
                request.InterbranchCompanyLocationGuid = resultInterbranchDEL;
            }
            #endregion

            switch (serviceTypeID)
            {
                case IntTypeJob.P:
                case IntTypeJob.BCP:
                case IntTypeJob.FLM:
                case IntTypeJob.ECash: //added: 2018/01/22
                case IntTypeJob.FSLM: //added: 2018/01/22
                case IntTypeJob.TM: //added: 2018/01/22
                case IntTypeJob.P_MultiBr:

                    #region CustomerLocation in brinkscompany Leg D
                    var resultDEL = _masterCustomerLocationRepository.FindAllAsQueryable(e => e.MasterSite_Guid == request.BrinksSite_GuidDEL)
                            .Join(_masterCustomerRepository.FindAllAsQueryable(e => !e.FlagChkCustomer.Value),
                                CL => CL.MasterCustomer_Guid,
                                C => C.Guid,
                                (CL, C) => new { CusLocation = CL, Customer = C }).Select(o => o.CusLocation.Guid).FirstOrDefault();
                    request.CompanylocationGuidDEL = resultDEL;
                    #endregion
                    request.BrinksSiteCode = _masterSiteRepository.FindById(request.BrinksSite_Guid).SiteCode;
                    break;
                case IntTypeJob.TV:
                case IntTypeJob.TV_MultiBr:
                    #region CustomerLocation in brinkscompany Leg P
                    var _resultPK = _masterCustomerLocationRepository.FindAll(e => e.MasterSite_Guid == request.BrinksSite_Guid)
                            .Join(_masterCustomerRepository.FindAll().Where(e => !e.FlagChkCustomer.GetValueOrDefault()),
                                CL => CL.MasterCustomer_Guid,
                                C => C.Guid,
                                (CL, C) => new { CusLocation = CL, Customer = C }).Select(o => o.CusLocation.Guid).FirstOrDefault();
                    request.CompanylocationGuidPK = _resultPK;

                    // CustomerLocation in brinkscompany Leg D
                    var _resultDEL = _masterCustomerLocationRepository.FindAll(e => e.MasterSite_Guid == request.BrinksSite_GuidDEL)
                            .Join(_masterCustomerRepository.FindAll().Where(e => !e.FlagChkCustomer.GetValueOrDefault()),
                                CL => CL.MasterCustomer_Guid,
                                C => C.Guid,
                                (CL, C) => new { CusLocation = CL, Customer = C }).Select(o => o.CusLocation.Guid).FirstOrDefault();
                    request.CompanylocationGuidDEL = _resultDEL;
                    #endregion

                    #region Calculate dayinvalult

                    if (request.WorkDate != request.WorkDateDEL)
                    {
                        TimeSpan countDays = request.WorkDateDEL.GetValueOrDefault() - request.WorkDate.GetValueOrDefault();
                        request.AdhocJobHeaderView.DayInVaults = countDays.Days;
                    }

                    #endregion

                    #region Get Brinksite code
                    request.BrinksSiteCode = _masterSiteRepository.FindById(request.BrinksSite_Guid).SiteCode;
                    request.BrinksSiteCodeDEL = _masterSiteRepository.FindById(request.BrinksSite_GuidDEL).SiteCode;
                    #endregion
                    break;
                case IntTypeJob.T:
                    request.BrinksSiteCode = _masterSiteRepository.FindById(request.BrinksSite_Guid).SiteCode;
                    request.BrinksSiteCodeDEL = _masterSiteRepository.FindById(request.BrinksSite_GuidDEL).SiteCode;
                    break;
                case IntTypeJob.D:
                case IntTypeJob.AC:
                case IntTypeJob.AE:
                case IntTypeJob.BCD:
                case IntTypeJob.BCD_MultiBr:
                    #region CustomerLocation in brinkscompany Leg P
                    var resultPK = _masterCustomerLocationRepository.FindAll(e => e.MasterSite_Guid == request.BrinksSite_Guid)
                            .Join(_masterCustomerRepository.FindAll().Where(e => !e.FlagChkCustomer.GetValueOrDefault()),
                                CL => CL.MasterCustomer_Guid,
                                C => C.Guid,
                                (CL, C) => new { CusLocation = CL, Customer = C }).Select(o => o.CusLocation.Guid).FirstOrDefault();
                    request.CompanylocationGuidPK = resultPK;
                    #endregion
                    request.BrinksSiteCodeDEL = _masterSiteRepository.FindById(request.BrinksSite_GuidDEL).SiteCode;
                    break;
            }
            if (!request.AdhocJobHeaderView.FlagJobInterBranch)
                request.AdhocJobHeaderView.FlagJobInterBranch = (request.InterbranchGuid != null && request.InterbranchCompanyLocationGuid != null); //sometimes front cannot send check interbranch flag in case of disable in serialize

            return request;
        }

        public bool CreateJobPickUp(AdhocJobDetailJobView request, Guid actionGuidPK, Guid actionGuidDEL)
        {
            try
            {
                #region Update RunningValue
                DateTimeOffset date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                request.AdhocJobHeaderView.JobNo = _adhocService.GenerateJobNo(request.BrinksSite_Guid);
                #endregion

                #region Get Printed Receipt

                var branchCode = _masterCustomerLocationRepository.FindById(request.LocationGuid).BranchCodeReference;
                PrintedReceiptNumberRequest itemreceipt = new PrintedReceiptNumberRequest();
                itemreceipt.CustomerLocation_Guid = request.LocationGuid;
                itemreceipt.SequenceStop = 1;
                itemreceipt.ServiceStopTransectionDate = request.WorkDate;
                itemreceipt.BranchCodeReference = branchCode;
                itemreceipt.SiteCode = request.BrinksSiteCode;
                itemreceipt.JobNo = request.AdhocJobHeaderView.JobNo;
                var ReceiptNo = GetPrintedReceiptNumber(itemreceipt);

                #endregion

                #region Insert JobHeader

                var insertHeader = new TblMasterActualJobHeader
                {
                    Guid = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                    JobNo = request.AdhocJobHeaderView.JobNo,
                    SystemStopType_Guid = request.AdhocJobHeaderView.ServiceStopTypesGuid,
                    SystemLineOfBusiness_Guid = request.AdhocJobHeaderView.LineOfBusiness_Guid,
                    SystemServiceJobType_Guid = request.AdhocJobHeaderView.ServiceTypeGuid,
                    SaidToContain = request.AdhocJobHeaderView.SaidToContain.GetValueOrDefault(),
                    MasterCurrency_Guid = request.CurrencyGuid,
                    Remarks = request.AdhocJobHeaderView.Remarks,
                    MasterRouteJobHeader_Guid = request.AdhocJobHeaderView.MasterRouteJobHeader_Guid,
                    DayInVault = 0,
                    MasterCustomerContract_Guid = request.ContractGuid,
                    InformTime = request.AdhocJobHeaderView.InformTime,
                    SystemStatusJobID = request.AdhocJobHeaderView.StatusJob,
                    TransectionDate = request.WorkDate,
                    FlagJobProcessDone = false,
                    OnwardDestinationType = request.OnwaryTypeID,
                    OnwardDestination_Guid = request.OnwaryDestinationGuid,
                    FlagBgsAirport = request.FlagBgsAirport,
                    UserCreated = request.UserModifyDetailView.UserModifed,
                    DatetimeCreated = request.UserModifyDetailView.DatetimeModified,
                    UniversalDatetimeCreated = request.UserModifyDetailView.UniversalDatetimeModified,
                    FlagJobClose = false,
                    FlagJobReadyToVault = false,
                    FlagMissingStop = false,
                    FlagJobDiscrepancies = false,
                    FlagNonDelivery = false,
                    FlagCancelAll = false,
                    FlagPickupAlready = false,
                    FlagSyncToMobile = 0,
                    FlagPickedupAfterInterchange = false,
                    SystemTripIndicator_Guid = request.TripIndicator_Guid,
                    FlagTotalValuePerJob = false,
                    MasterSubServiceType_Guid = request.AdhocJobHeaderView.SubServiceTypeJobTypeGuid,
                    FlagChkOutInterBranchComplete = false,
                    FlagJobInterBranch = request.AdhocJobHeaderView.FlagJobInterBranch,
                    SFOFlagRequiredTechnician = request.SFOFlagRequiredTechnician,
                    FlagUpdateJobManual = false,
                    FlagRequireOpenLock = request.FlagRequireOpenLock,
                    FlagJobSFO = request.FlagJobSFO,  //added: 2018/01/31
                    TicketNumber = request.TicketNumber,
                    SFOMaxWorkingTime = request.SFOMaxWorkingTime,
                    SFOTechnicianName = request.SFOTechnicianName,
                    SFOTechnicianID = request.SFOTechnicianID,
                    SFOMaxTechnicianWaitingTime = request.SFOMaxTechnicianWaitingTime


                };
                _masterActualJobHeaderRepository.Create(insertHeader);
                #endregion

                #region Insert TblJobServiceStop
                var serviceStopGuidPK = Guid.NewGuid();
                var serviceStopGuidDEL = Guid.NewGuid();
                var insertJobServiceStopPK = new TblMasterActualJobServiceStop
                {
                    Guid = serviceStopGuidPK,
                    MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                    CustomerLocationAction_Guid = actionGuidPK,
                    MasterCustomerLocation_Guid = request.LocationGuid
                };
                _masterActualJobServiceStopsRepository.Create(insertJobServiceStopPK);

                var insertJobServiceStopDEL = new TblMasterActualJobServiceStop
                {
                    Guid = serviceStopGuidDEL,
                    MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                    CustomerLocationAction_Guid = actionGuidDEL,
                    MasterCustomerLocation_Guid = request.CompanylocationGuidDEL
                };
                _masterActualJobServiceStopsRepository.Create(insertJobServiceStopDEL);
                #endregion                

                #region Insert SeviceStopLegs
                var legPKGuid = Guid.NewGuid();
                int j = request.AdhocJobHeaderView.FlagJobInterBranch ? 4 : 2;
                for (int i = 1; i <= j; i++)
                {
                    var locationGuid = (i == 2 ? request.CompanylocationGuidDEL : request.InterbranchCompanyLocationGuid);
                    var siteGuid = (i == 2 ? request.BrinksSite_GuidDEL : request.InterbranchGuid);

                    var insertStopLeg = new TblMasterActualJobServiceStopLegs
                    {
                        Guid = i == 1 ? legPKGuid : Guid.NewGuid(),
                        MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                        SequenceStop = i,
                        CustomerLocationAction_Guid = i == 1 || i == 3 ? actionGuidPK : actionGuidDEL,
                        MasterCustomerLocation_Guid = i == 1 ? request.LocationGuid : locationGuid,
                        MasterRouteGroupDetail_Guid = i == 1 || i == 2 ? request.RouteDetailGuid : null,
                        ServiceStopTransectionDate = request.WorkDate,
                        WindowsTimeServiceTimeStart = request.Time,
                        WindowsTimeServiceTimeStop = request.Time,
                        JobOrder = 0,
                        MasterRunResourceDaily_Guid = i == 1 || i == 2 ? request.RunResoureGuid : null,
                        FlagEarlyFlight = request.FlagEarlyFlight,
                        FlightNo = request.FlightNo,
                        EstimateDepartureTime = request.EstimateDepartureTime,
                        EstimateArrivalTime = request.EstimateArrivalTime,
                        WeightShip = request.WeightShip == null ? 0 : request.WeightShip,
                        Pieces = request.Pieces == null ? 0 : request.Pieces,
                        MawbNumber = request.MawbNumber,
                        NoOfHAWB = request.NoOfHAWB == null ? 0 : request.NoOfHAWB,
                        AirportRemarks = request.AirportRemarks,
                        FlagAirportDocCheck = request.FlagAirportDocCheck.GetValueOrDefault(),
                        PrintedReceiptNumber = i == 1 ? ReceiptNo : null,
                        MasterSite_Guid = i == 1 ? request.BrinksSite_Guid : siteGuid,
                        SystemTransportWay_ID = !request.FlagBgsAirport ? 1 : 2,
                        MasterTrasportWay_Guid = request.MasterTrasportWay_Guid,
                        ArrivalTime = null,
                        ActualTime = null,
                        DepartTime = null,
                        FlagCancelLeg = false
                    };

                    _masterActualJobServiceStopLegRepository.Create(insertStopLeg);

                    #region Insert History Run
                    if (request.RunResoureGuid != null)
                    {
                        var insertHistoryRun = new TblMasterHistory_ActaulJobOnDailyRunResource
                        {
                            Guid = Guid.NewGuid(),
                            MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid,
                            MasterRunResourceDaily_Guid = request.RunResoureGuid.GetValueOrDefault(),
                            DateTimeInterChange = date,
                            MasterActualJobServiceStopLegs_Guid = insertStopLeg.Guid
                        };
                        _masterHistory_ActaulJobOnDailyRunResourceRepository.Create(insertHistoryRun);
                    }
                }

                // 13) Insert History
                var insertHistory = new TblMasterHistory_ActualJob
                {
                    Guid = Guid.NewGuid(),
                    MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                    MsgID = GetMessageID(request.FlagJobSFO), //Created job no. {0} date {1} from Adhoc Job by Ocean Online MVC.
                    MsgParameter = new string[] { request.AdhocJobHeaderView.JobNo, request.WorkDate.GetValueOrDefault().ToString("MM/dd/yyyy") }.ToJSONString(),
                    UserCreated = request.UserModifyDetailView.UserModifed,
                    DatetimeCreated = request.UserModifyDetailView.DatetimeModified,
                    UniversalDatetimeCreated = request.UserModifyDetailView.UniversalDatetimeModified
                };
                _masterHistoryActualJobRepository.Create(insertHistory);
                #endregion

                #endregion

           
                return true;
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);

                return false;
            }
        }
        private int GetMessageID(bool isSFO)
        {
            if (isSFO)
                return 753; //Created job no. {0} date {1} by Service Request.
            return 653;  //Created job no. {0} date {1} from Adhoc Job by Ocean Online MVC.
        }


        #endregion


    }
}
