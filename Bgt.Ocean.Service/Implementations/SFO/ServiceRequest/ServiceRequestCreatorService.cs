using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Job;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Implementations.Adhoc;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.AdhocService;
using Bgt.Ocean.Service.Messagings.ServiceRequest;
using Bgt.Ocean.Service.ModelViews.ActualJobHeader;
using Bgt.Ocean.Service.ModelViews.GenericLog;
using Bgt.Ocean.Service.ModelViews.ServiceRequest;
using Bgt.Ocean.Service.ModelViews.Users;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using static Bgt.Ocean.Service.Mapping.ServiceMapperBootstrapper;

namespace Bgt.Ocean.Service.Implementations.AuditLog.ServiceRequest
{
    #region Interface

    public interface IServiceRequestCreatorService
    {

        SRCreateResponse CreateServiceRequestFLM(SRCreateRequestFLM request);
        SRCreateResponse CreateServiceRequestSLM(SRCreateRequestSLM request);
        SRCreateResponse CreateServiceRequestECash(SRCreateRequestECash request);
        SRCreateResponse CreateServiceRequestTechMeet(SRCreateRequestTechMeet request);

        SRCreateResponse CreateServiceRequestFLM(SRCreateRequestFLM request, SRCreateRequestSFI sfiRequest);
        SRCreateResponse CreateServiceRequestSLM(SRCreateRequestSLM request, SRCreateRequestSFI sfiRequest);
        SRCreateResponse CreateServiceRequestECash(SRCreateRequestECash request, SRCreateRequestSFI sfiRequest);
        SRCreateResponse CreateServiceRequestTechMeet(SRCreateRequestTechMeet request, SRCreateRequestSFI sfiRequest);
        SRCreateResponse CreateServiceRequestMCS(SRCreateRequestMCSCashAddWithSFI request, SRCreateRequestSFI sfiModel);
    }

    #endregion

    public class ServiceRequestCreatorService : IServiceRequestCreatorService
    {
        private readonly IActualJobHeaderService _jobHeaderService;
        private readonly IMasterActualJobHeaderRepository _masterActualJobHeaderRepository;
        private readonly IMasterSiteRepository _masterSiteRepository;
        private readonly ISFOMasterMachineRepository _masterMachineRepository;
        private readonly ISFOSystemDataConfigurationRepository _sfoSystemDataConfiguration;
        private readonly ISFOSystemEnvironmentGlobalRepository _sfoSystemEnvironmentGlobal;
        private readonly ISFOSystemFunctionRepository _sfoSystemFunctionRepository;
        private readonly ISFOTransactionServiceRequestInfoRepository _transactionServiceRequestInfoRepository;
        private readonly ISFOTransactionServiceRequestProblemRepository _transactionServiceRequestProblemRepository;
        private readonly ISFOTransactionServiceRequestRepository _transactionServiceRequestRepository;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly ISystemLog_HistoryErrorRepository _systemLogHistoryError;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemRunningValueGlobalRepository _systemRunningValueGlobalRepository;
        private readonly IMasterUserRepository _masterUserRepository;
        private readonly ISystemTimeZoneRepository _systemTimeZoneRepository;
        private readonly ISFOMasterProblemRepository _masterProblemRepository;
        private readonly ISFOMasterMachineServiceTypeRepository _masterMachineServiceTypeRepository;
        private readonly IGenericLogService _genericLogService;
        private readonly ISFOTransactionServiceRequestEcashRepository _transactionServiceRequestEcashRepository;
        private readonly ISFOApiUserService _sfoApiUserService;
        private readonly IAdhocService _newAdhocService;

        private static object _lockTicketNumber = new object();

        public ServiceRequestCreatorService(
                IActualJobHeaderService jobHeaderService,
                IMasterActualJobHeaderRepository masterActualJobHeaderRepository,
                IMasterSiteRepository masterSiteRepository,
                ISFOMasterMachineRepository masterMachineRepository,
                ISFOSystemDataConfigurationRepository sfoSystemDataConfiguration,
                ISFOSystemEnvironmentGlobalRepository sfoSystemEnvironmentGlobal,
                ISFOSystemFunctionRepository sfoSystemFunctionRepository,
                ISystemRunningValueGlobalRepository systemRunningValueGlobalRepository,
                ISFOTransactionServiceRequestInfoRepository transactionServiceRequestInfoRepository,
                ISFOTransactionServiceRequestProblemRepository transactionServiceRequestProblemRepository,
                ISFOTransactionServiceRequestRepository transactionServiceRequestRepository,
                ISystemEnvironment_GlobalRepository systemEnvironmentGlobal,
                ISystemMessageRepository systemMessageRepository,
                ISystemLog_HistoryErrorRepository systemLogHistoryError,
                IMasterUserRepository masterUserRepository,
                ISystemTimeZoneRepository systemTimeZoneRepository,
                ISFOMasterProblemRepository masterProblemRepository,
                ISFOMasterMachineServiceTypeRepository masterMachineServiceTypeRepository,
                ISFOTransactionServiceRequestEcashRepository transactionServiceRequestEcashRepository,
                IGenericLogService genericLogService,
                ISFOTblTransactionOTCRepository transactionOtcRepository,
                ISystemJobStatusRepository systemJobStatusRepository,
                IObjectComparerService objectComparerService,
                IUnitOfWork<OceanDbEntities> uow,
                ISFOApiUserService sfoApiUserService,
                IAdhocService newAdhocService
            )
        {
            _jobHeaderService = jobHeaderService;
            _masterActualJobHeaderRepository = masterActualJobHeaderRepository;
            _masterMachineRepository = masterMachineRepository;
            _masterSiteRepository = masterSiteRepository;
            _sfoSystemDataConfiguration = sfoSystemDataConfiguration;
            _sfoSystemEnvironmentGlobal = sfoSystemEnvironmentGlobal;
            _sfoSystemFunctionRepository = sfoSystemFunctionRepository;
            _systemMessageRepository = systemMessageRepository;
            _systemRunningValueGlobalRepository = systemRunningValueGlobalRepository;
            _systemLogHistoryError = systemLogHistoryError;
            _transactionServiceRequestInfoRepository = transactionServiceRequestInfoRepository;
            _transactionServiceRequestProblemRepository = transactionServiceRequestProblemRepository;
            _transactionServiceRequestRepository = transactionServiceRequestRepository;
            _masterUserRepository = masterUserRepository;
            _systemTimeZoneRepository = systemTimeZoneRepository;
            _masterProblemRepository = masterProblemRepository;
            _masterMachineServiceTypeRepository = masterMachineServiceTypeRepository;
            _genericLogService = genericLogService;
            _transactionServiceRequestEcashRepository = transactionServiceRequestEcashRepository;
            _uow = uow;
            _sfoApiUserService = sfoApiUserService;
            _newAdhocService = newAdhocService;
        }

        #region Public

        public SRCreateResponse CreateServiceRequestFLM(SRCreateRequestFLM request)
        {
            return BaseCreateServiceRequest(request);
        }

        public SRCreateResponse CreateServiceRequestMCS(SRCreateRequestMCSCashAddWithSFI request, SRCreateRequestSFI sfiModel)
        {
            return BaseCreateServiceRequest(request, serviceRequestView =>
            {
                serviceRequestView.ServiceTypeId = ServiceJobTypeHelper.ServiceJobTypeMCS;
                serviceRequestView.ServiceTypeGuid = ServiceJobTypeHelper.ServiceJobTypeMCSGuid.ToGuid();
            });
        }

        public SRCreateResponse CreateServiceRequestECash(SRCreateRequestECash request)
        {
            return BaseCreateServiceRequest(request, serviceRequestView =>
            {
                serviceRequestView.ECashViewList = request.ECashViewList.ConvertToEcashView();
                serviceRequestView.ServiceTypeId = ServiceJobTypeHelper.ServiceJobTypeEcashID;
                serviceRequestView.ServiceTypeGuid = ServiceJobTypeHelper.ServiceJobTypeECASHGuid.ToGuid();
            });
        }

        public SRCreateResponse CreateServiceRequestTechMeet(SRCreateRequestTechMeet request)
        {
            return BaseCreateServiceRequest(request, serviceRequestView =>
            {
                serviceRequestView.TechMeetInformation = request.ConvertToTechMeet();
                serviceRequestView.ServiceTypeId = ServiceJobTypeHelper.ServiceJobTypeTechmeetID;
                serviceRequestView.ServiceTypeGuid = ServiceJobTypeHelper.ServiceJobTypeTMGuid.ToGuid();
            });
        }

        public SRCreateResponse CreateServiceRequestSLM(SRCreateRequestSLM request)
        {
            return BaseCreateServiceRequest(request, serviceRequestView =>
            {
                serviceRequestView.ServiceTypeId = ServiceJobTypeHelper.ServiceJobTypeSLM;
                serviceRequestView.ServiceTypeGuid = ServiceJobTypeHelper.ServiceJobTypeSLMGuid.ToGuid();
            });
        }

        public SRCreateResponse CreateServiceRequestFLM(SRCreateRequestFLM request, SRCreateRequestSFI sfiRequest)
        {
            return BaseCreateServiceRequest(request, serviceRequestView =>
            {
                serviceRequestView.SFIModelView = MapperService.Map<SFIModelView>(sfiRequest);
            });
        }

        public SRCreateResponse CreateServiceRequestSLM(SRCreateRequestSLM request, SRCreateRequestSFI sfiRequest)
        {
            return BaseCreateServiceRequest(request, serviceRequestView =>
            {
                serviceRequestView.ServiceTypeId = ServiceJobTypeHelper.ServiceJobTypeSLM;
                serviceRequestView.ServiceTypeGuid = ServiceJobTypeHelper.ServiceJobTypeSLMGuid.ToGuid();

                serviceRequestView.SFIModelView = MapperService.Map<SFIModelView>(sfiRequest);
            });
        }

        public SRCreateResponse CreateServiceRequestECash(SRCreateRequestECash request, SRCreateRequestSFI sfiRequest)
        {
            return BaseCreateServiceRequest(request, serviceRequestView =>
            {
                serviceRequestView.ECashViewList = request.ECashViewList.ConvertToEcashView();
                serviceRequestView.ServiceTypeId = ServiceJobTypeHelper.ServiceJobTypeEcashID;
                serviceRequestView.ServiceTypeGuid = ServiceJobTypeHelper.ServiceJobTypeECASHGuid.ToGuid();

                serviceRequestView.SFIModelView = MapperService.Map<SFIModelView>(sfiRequest);
            });
        }

        public SRCreateResponse CreateServiceRequestTechMeet(SRCreateRequestTechMeet request, SRCreateRequestSFI sfiRequest)
        {
            return BaseCreateServiceRequest(request, serviceRequestView =>
            {
                serviceRequestView.TechMeetInformation = request.ConvertToTechMeet();
                serviceRequestView.ServiceTypeId = ServiceJobTypeHelper.ServiceJobTypeTechmeetID;
                serviceRequestView.ServiceTypeGuid = ServiceJobTypeHelper.ServiceJobTypeTMGuid.ToGuid();

                serviceRequestView.SFIModelView = MapperService.Map<SFIModelView>(sfiRequest);
            });
        }

        #endregion

        #region Private function

        #region Util

        private string GetNewTicketNumber()
        {
            lock (_lockTicketNumber)
            {
                var runningId = _systemRunningValueGlobalRepository.GetServiceRequestRunning();
                var runningValue = runningId.RunningVaule1 + 1;

                runningId.RunningVaule1 = runningValue;
                _systemRunningValueGlobalRepository.Modify(runningId);
                _uow.Commit();

                var ticketNumber = $"{runningValue.ToString("D5")}{SecureHelper.GenerateRandomNumber()}";
                return ticketNumber;
            }
        }

        #endregion

        #region Create Service Request

        #region Create Job

        private AdhocJobDetailJobView PrepareDataForCreateJobHeader(Guid machineTypeGuid, ServiceRequestView serviceRequestView)
        {
            var r = new AdhocJobDetailJobView();
            var jobDetail = new AdhocJobHeaderView();
            var siteInfo = _masterSiteRepository.FindById(serviceRequestView.BrinkSiteGuid);
            var userInfo = _sfoApiUserService.GetUserByConfiguration(serviceRequestView.CountryGuid, SFOSystemDataConfigurationDataKeyHelper.SFO_API_USER);
            var configuration = _sfoSystemDataConfiguration.GetByDataKey(new string[]
            {
                "SERVICE_REQUEST_CREATE_JOB_DEFAULT_VALUE"
            }, userInfo.MasterCountry_Guid.Value).FirstOrDefault();
            var machineId = (from machine in _masterMachineRepository.FindAllAsQueryable()
                             where machine.Guid == serviceRequestView.MachineGuid
                             select machine.MachineID)
                            .SingleOrDefault();

            DateTime? workDateSiteTime = _sfoSystemFunctionRepository.Func_CalculateTime(serviceRequestView.DateTimeServiceDate.Value, SystemTimeZoneHelper.UTC, siteInfo.TimeZoneID.GetValueOrDefault());

            r.CurrencyGuid = userInfo.DefaultMasterCurrencyGuid;
            r.SpecialCommandGuidDEL = null;

            jobDetail.JobGuid = Guid.NewGuid();
            jobDetail.DayInVaults = 0;
            jobDetail.LineOfBusiness_Guid = machineTypeGuid == MachineTypeHelper.ATMMachineGuid.ToGuid()
                ? LineOfBusinessHelper.ATMGuid.ToGuid()
                : LineOfBusinessHelper.CompuSafeGuid.ToGuid();
            jobDetail.ServiceTypeID = serviceRequestView.ServiceTypeId;
            jobDetail.ServiceTypeGuid = serviceRequestView.ServiceTypeGuid;
            jobDetail.Remarks = GetJobHeaderRemark(serviceRequestView.TicketNumber, serviceRequestView.ServiceTypeId, machineId, serviceRequestView.ReportedDescription ?? serviceRequestView.ReportedIncidentDescription);
            jobDetail.StatusJob = serviceRequestView.StatusId;
            jobDetail.SaidToContain = 0;
            jobDetail.FlagJobInterBranch = false;

            UserModifyDetailView UserModifyDetailView = new UserModifyDetailView();
            UserModifyDetailView.DatetimeModified = DateTime.Now;
            UserModifyDetailView.UniversalDatetimeModified = DateTime.Now;
            UserModifyDetailView.LanguageGuid = userInfo.UserLanguageGuid;
            UserModifyDetailView.UserModifed = userInfo.UserName;

            r.UserModifyDetailView = UserModifyDetailView;


            jobDetail.InformTime = string.IsNullOrEmpty(serviceRequestView.DateTimeNotifiedTime)
                ? "00:00".ToTimeDateTime()
                : _sfoSystemFunctionRepository.Func_CalculateTime(serviceRequestView.DateTimeNotifiedTime.ToTimeDateTime(), SystemTimeZoneHelper.UTC, siteInfo.TimeZoneID.GetValueOrDefault());

            r.AdhocJobHeaderView = jobDetail;
            r.FlagJobSFO = true;
            // Customer Information
            r.BrinksSite_Guid = serviceRequestView.BrinkSiteGuid;
            r.BrinksSiteCode = serviceRequestView.BrinkSiteCode;
            r.CustomerGuid = serviceRequestView.CustomerGuid;

            // WorkDate is ServiceDate (convert to Site Time Zone)
            // TblMasterActualJobServiceStopLegs.ServiceStopTransectionDate
            r.WorkDate = workDateSiteTime ?? serviceRequestView.DateTimeServiceDate;
            r.Time = (workDateSiteTime ?? serviceRequestView.DateTimeServiceDate).Value.ToString("HH:mm").ToTimeDateTime();

            // Brink's Information
            r.BrinksCompanyGuid = serviceRequestView.BrinkSiteGuid;

            //leg D
            r.BrinksSite_GuidDEL = serviceRequestView.BrinkSiteGuid;


            //ModeJobLegs
            var modeLeg1 = _sfoSystemEnvironmentGlobal.FindByAppKey("ModeJobLegs").AppValue1.ToInt();
            var modeLeg2 = _sfoSystemEnvironmentGlobal.FindByAppKey("ModeJobLegs").AppValue2.ToInt();

            //mode 1 = Pick Up (SequenceStop 1) => Company
            r.CompanylocationGuidDEL = modeLeg1 == 1 ? serviceRequestView.BrinksCompanyGuid : serviceRequestView.MachineGuid;// Brinks location SequenceStop 1
            r.LocationGuid = modeLeg2 == 1 ? serviceRequestView.BrinksCompanyGuid : serviceRequestView.MachineGuid; //SequenceStop 2 MachineGuid

            r.CustomerGuidDEL = serviceRequestView.CustomerGuid;


            r.FlagJobSFO = true;
            r.TicketNumber = serviceRequestView?.TicketNumber;
            r.SFOMaxWorkingTime = configuration?.DataValue1.ToInt() ?? 0;
            r.SFOTechnicianID = configuration?.DataValue2 ?? "0000000";
            r.SFOMaxTechnicianWaitingTime = configuration?.DataValue3.ToInt();
            r.SFOFlagRequiredTechnician = serviceRequestView?.ServiceTypeId == ServiceJobTypeHelper.ServiceJobTypeTechmeetID;
            r.SFOTechnicianName = serviceRequestView?.TechMeetInformation?.TechMeetName;
            r.FlagRequireOpenLock = _masterMachineRepository.IsMachineHasLock(serviceRequestView.MachineGuid);

            return r;

        }

        private string GetJobHeaderRemark(string ticketNumber, int serviceTypeId, string machineId, string srReportedDescription)
        {
            string remark = "";
            srReportedDescription = srReportedDescription.IsEmpty() ? "-" : srReportedDescription;

            remark = $"Ticket#{ticketNumber}";
            remark += serviceTypeId == ServiceJobTypeHelper.ServiceJobTypeEcashID ? $" | Machine ID: {machineId}" : "";
            remark += $" | Comments: {srReportedDescription}";

            remark = string.Join("", remark.Take(500));

            return remark;
        }

        private CreateJobResponse CreateJob(Guid machineTypeGuid, ServiceRequestView serviceRequestView)
        {
            var jobModel = PrepareDataForCreateJobHeader(machineTypeGuid, serviceRequestView);
            var jobHeaderGuid = jobModel.AdhocJobHeaderView.JobGuid;
            try
            {
                _jobHeaderService.CreateJobForSFO(jobModel);

                _uow.Commit();

                #region ## Create OTC
                //=> TFS#53385:Ability to generate the OTC codes for both legs of Transfer job -> CreateJob [NEW]
                _newAdhocService.CreateHeaderOtcByJobGuids(new Guid[] { jobHeaderGuid.GetValueOrDefault() });
                #endregion

                _uow.Commit();

                return new CreateJobResponse
                {
                    IsSuccess = true,
                    JobHeaderGuid = jobHeaderGuid
                };
            }
            catch (Exception err)
            {
                _systemLogHistoryError.CreateHistoryError(err);
                _masterActualJobHeaderRepository.RemoveSFOJob(jobHeaderGuid.GetValueOrDefault());
                _uow.Commit();

                return new CreateJobResponse
                {
                    IsSuccess = false,
                    JobHeaderGuid = null
                };
            }
        }

        private CreateJobResponse CreateJobMCS(Guid machineTypeGuid, ServiceRequestView serviceRequestView)
        {
            try
            {
                var flmJobDetail = PrepareDataForCreateJobHeader(machineTypeGuid, serviceRequestView);
                var userInfo = _sfoApiUserService.GetUserByConfiguration(serviceRequestView.CountryGuid, SFOSystemDataConfigurationDataKeyHelper.SFO_API_USER);

                var request = new CreateJobAdHocRequest
                {
                    AdhocJobHeaderView = new AdhocJobHeaderRequest
                    {
                        BrinkSiteGuid = flmJobDetail.BrinksSite_Guid,
                        ServiceJobTypeID = serviceRequestView.ServiceTypeId,
                        ServiceJobTypeGuid = serviceRequestView.ServiceTypeGuid,
                        SubServiceTypeJobTypeGuid = SubServiceType.SubServiceTypeATM_CashAdd,
                        InformTime = flmJobDetail.AdhocJobHeaderView.InformTime,
                        strInformTime = flmJobDetail.AdhocJobHeaderView.strInformTime,
                        LineOfBusiness_Guid = flmJobDetail.AdhocJobHeaderView.LineOfBusiness_Guid,
                        FlagJobSFO = false,
                        StatusJob = serviceRequestView.StatusId,
                        Remarks = flmJobDetail.AdhocJobHeaderView.Remarks,
                        TicketNumber = flmJobDetail.TicketNumber,
                        SFOMaxWorkingTime = flmJobDetail.SFOMaxWorkingTime,
                        SFOFlagRequiredTechnician = flmJobDetail.SFOFlagRequiredTechnician,
                        SFOTechnicianID = flmJobDetail.SFOTechnicianID,
                        SFOTechnicianName = flmJobDetail.SFOTechnicianName,
                        SFOMaxTechnicianWaitingTime = flmJobDetail.SFOMaxTechnicianWaitingTime,
                        FlagRequireOpenLock = flmJobDetail.FlagRequireOpenLock
                    },
                    ServiceStopLegPickup = new AdhocLegRequest
                    {
                        BrinkCompanyGuid = flmJobDetail.BrinksCompanyGuid,
                        BrinkSiteGuid = flmJobDetail.BrinksSite_Guid,
                        BrinkSiteCode = flmJobDetail.BrinksSiteCode,
                        CustomerGuid = flmJobDetail.CustomerGuid,
                        LocationGuid = flmJobDetail.LocationGuid,
                        StrWorkDate_Date = flmJobDetail.WorkDate.ChangeFromDateToString(),
                        StrWorkDate_Time = flmJobDetail.WorkDate.ChangFromDateToTimeString()
                    },
                    ServiceStopLegDelivery = new AdhocLegRequest
                    {
                        BrinkCompanyGuid = flmJobDetail.BrinksCompanyGuid,
                        BrinkSiteGuid = flmJobDetail.BrinksSite_GuidDEL,
                        BrinkSiteCode = flmJobDetail.BrinksSiteCodeDEL
                    },
                    UserName = userInfo.UserName,
                    LanguagueGuid = userInfo.UserLanguageGuid.GetValueOrDefault(),
                    UniversalDatetime = DateTime.UtcNow
                };

                var result = _newAdhocService.CreateJobPickUp(request);

                return new CreateJobResponse
                {
                    IsSuccess = true,
                    JobHeaderGuid = result.JobGuid_list.ToGuid()
                };
            }
            catch (Exception err)
            {
                _systemLogHistoryError.CreateHistoryError(err);
                return new CreateJobResponse
                {
                    IsSuccess = false,
                    JobHeaderGuid = null
                };
            }
        }


        #endregion

        #region Create Ticket

        private CreateServiceRequestResponse CreateJobAndServiceRequest(ServiceRequestView serviceRequestView)
        {
            CreateJobResponse resultCreateJob;
            var ticketNumber = GetNewTicketNumber();
            bool isMcs = false;

            serviceRequestView.TicketNumber = ticketNumber;
            _uow.ConfigAutoDetectChanges(false);

            var userInfo = _sfoApiUserService.GetUserByConfiguration(serviceRequestView.CountryGuid);
            var machineInfo = _masterMachineRepository.FindById(serviceRequestView.MachineGuid);

            if (serviceRequestView.ServiceTypeId == ServiceJobTypeHelper.ServiceJobTypeMCS)
            {
                resultCreateJob = CreateJobMCS(machineInfo.SystemMachineType_Guid.Value, serviceRequestView);
                isMcs = true;
            }
            else
            {
                resultCreateJob = CreateJob(machineInfo.SystemMachineType_Guid.Value, serviceRequestView);
            }



            // check result create job
            if (!resultCreateJob.IsSuccess)
            {
                var msg = _systemMessageRepository.FindByMsgId(-312, userInfo.UserLanguageGuid.Value).ConvertToMessageView();
                return new CreateServiceRequestResponse
                {
                    SystemMessageView = msg
                };
            }

            // create service rqeuest
            var resultCreateServiceRequest = CreateServiceRequest(userInfo, machineInfo, serviceRequestView, resultCreateJob.JobHeaderGuid);

            if (resultCreateServiceRequest.SystemMessageView.IsSuccess)
            {
                UpdateSFOInformationToJob(serviceRequestView, resultCreateJob.JobHeaderGuid);

                if (isMcs)
                {
                    UpdateServiceRequestInformationForMCS(resultCreateServiceRequest.ServiceRequestGuid.Value);
                }
            }

            return resultCreateServiceRequest;
        }

        private void UpdateSFOInformationToJob(ServiceRequestView serviceRequestView, Guid? jobHeaderGuid)
        {
            var jobDetail = _masterActualJobHeaderRepository.FindById(jobHeaderGuid.Value);
            jobDetail.TicketNumber = serviceRequestView.TicketNumber;

            _masterActualJobHeaderRepository.Modify(jobDetail);
            _uow.Commit();
        }

        private void UpdateServiceRequestInformationForMCS(Guid serviceRequestGuid)
        {
            var srDetail = _transactionServiceRequestInfoRepository.FindById(serviceRequestGuid);
            srDetail.FlagAllowSafeAccess = true;

            _transactionServiceRequestInfoRepository.Modify(srDetail);
            _uow.Commit();
        }

        private CreateServiceRequestResponse CreateServiceRequest(DataStorage userInfo, SFOTblMasterMachine machineInfo, ServiceRequestView serviceRequest, Guid? jobHeaderGuid)
        {

            var utcNow = DateTime.UtcNow;
            var dateStatus = serviceRequest.DateTimeNotified.ToString("yyyyMMdd");
            var datestatusPlus = dateStatus + "1";
            var serviceRequestGuid = Guid.NewGuid();

            var targetBranchInfo = _masterSiteRepository.FindById(serviceRequest.BrinkSiteGuid);

            var userDataTimeZone = _masterUserRepository.FindById(userInfo.UserGuid).TimeZoneID;
            var timeZoneGuid = _systemTimeZoneRepository.FindAll(e => e.TimeZoneID == userDataTimeZone).FirstOrDefault().Guid;

            var slaTime = GetSLATime(serviceRequest.ProblemGuid, machineInfo, serviceRequest.ServiceTypeId);
            var dueDateTimeUtc = slaTime.HasValue ? serviceRequest.DateTimeServiceDate.Value.AddMinutes(slaTime.Value) : new DateTime?();

            try
            {
                #region 1. Create via Store Procedure

                var newSr = new SFOTblTransactionServiceRequest
                {
                    Guid = serviceRequestGuid,
                    DateStatus = datestatusPlus.ToInt(),
                    MasterSite_Guid = targetBranchInfo.Guid,
                    MasterActualJobHeader_Guid = jobHeaderGuid,
                    PreviousTicketStatus_Guid = null,
                    MasterCustomerLocation_Guid = serviceRequest.MachineGuid,
                    SFOMasterCategory_Guid = null,
                    SFOMasterServiceRequestType_Guid = null,
                    SFOMasterPriority_Guid = serviceRequest.PriorityGuid,
                    SFOMasterSource_OpenSource_Guid = serviceRequest.OpenSourceGuid,
                    SFOMasterSource_CloseSource_Guid = serviceRequest.CloseChannelGuid,
                    SFOMasterReason_Guid = null,
                    TicketNumber = serviceRequest.TicketNumber,
                    TicketStatus_Guid = serviceRequest.TicketStatusGuid,
                    DateTimeOpened = utcNow,
                    DateTimeClosed = null,
                    DateTimeDateTimeDown = serviceRequest.DateTimeDown,
                    DateTimeNotified = serviceRequest.DateTimeNotified,
                    RequestedServiceDate = serviceRequest.DateTimeServiceDate,
                    DateTimeServiceDate = serviceRequest.DateTimeServiceDate,
                    DateTimeDueDate = dueDateTimeUtc,
                    CalculatedDueDateTime = dueDateTimeUtc,
                    ContactName = serviceRequest.ContactName,
                    ContactPhone = serviceRequest.ContactPhone,
                    CustomerReferenceNumber = serviceRequest.CustomerReferenceNumber,
                    ResponderReferenceNumber = serviceRequest.ResponderRefNum,
                    EscalationType = null,
                    EscalationLevel = null,
                    EscalationTime = null,
                    ReportedServiceRequestDescription = serviceRequest.ReportedIncidentDescription,


                    // techmeet
                    TechMeetName = serviceRequest.TechMeetInformation?.TechMeetName,
                    TechMeetPhone = serviceRequest.TechMeetInformation?.TechMeetPhone,
                    TechMeetCompanyName = serviceRequest.TechMeetInformation?.TechMeetCompanyName,
                    TechMeetReasonID = serviceRequest.TechMeetInformation?.TechMeetReason,
                    flagTechMeetSecurityRequired = serviceRequest.TechMeetInformation?.TechMeetSecurityRequired,
                    TechMeetSecurityRequired = null,

                    CreatedBy = userInfo.UserName,
                    FlagRescheduleRequest = null,
                    Reason_Guid = null,
                    VendorSolutionID = null,
                    FlagBillableIndicator = null,
                    FlagNotifyNow = serviceRequest.FlagNotify,
                    Technician = null,

                    // datetime in responder status
                    DateTimeAcknowledged = utcNow,
                    DateTimeDispatched = utcNow,
                    DateTimeETA = dueDateTimeUtc,
                    DateTimeDepartureToOnsite = null,
                    DateTimeReportedOnsite = null,
                    DateTimeReportedResolved = null,
                    DateTimeReportedDeparted = null,
                    DateTimeReportedReturned = null,
                    DateTimeCustomerStart = null,
                    DateTimeCustomerDispatched = null,

                    TotalPremiseTime = null,
                    MassServiceRequest_Guid = null,
                    PACNCode = serviceRequest.PACNCode,
                    DateTimeRequestedNotification = null,
                    CancellationReason = null,
                    CustomerCommentOut = null,
                    VendorNetworkID = null,
                    VendorCommentIn = null,
                    VendorCommentOut = null,
                    Owner = null,
                    OwnerSource_Guid = null,
                    FlagClosedOnFirstContact = serviceRequest.FlagClosedOnFirstContact,
                    BrinksLogNumber = serviceRequest.BrinksLog,
                    FlagNonbillableToBranch = serviceRequest.FlagNonBillableToBranch,
                    FlagNonbillableToCustomer = serviceRequest.FlagNonBillableToCustomer,
                    FlagSpecialCharges = serviceRequest.FlagSpecialCharges,
                    ChargeAmount = serviceRequest.ChargeAmount,
                    FlagRescheduleApproved = serviceRequest.FlagReScheduledApproved,
                    FlagDisable = false,
                    UserCreated = userInfo.UserName,
                    DatetimeCreated = utcNow,
                    UniversalDatetimeCreated = utcNow,
                    UserModified = null,
                    DatetimeModified = null,
                    UniversalDatetimeModified = null,
                    TimeZoneLocal_Guid = timeZoneGuid,
                    TimeZoneMachine_Guid = machineInfo.SFOTblMasterCountryTimeZone.SystemTimeZone_Guid,
                    BrinksNote = null,
                    ReasonName = serviceRequest.ReasonName,
                    ResolvedBy = serviceRequest.ResolveBy,
                    FlagStampDispatch = serviceRequest.FlagStampDispatch.GetValueOrDefault(),
                    FlagStampAccept = serviceRequest.FlagStampAccept.GetValueOrDefault(),
                    FlagStampArrive = serviceRequest.FlagStampArrive.GetValueOrDefault(),
                    FlagStampRestore = serviceRequest.FlagStampRestore.GetValueOrDefault(),
                    FlagStampDepart = serviceRequest.FlagStampDepart.GetValueOrDefault(),
                    FlagStampReturn = serviceRequest.FlagStampReturn.GetValueOrDefault(),
                    ResponderName = serviceRequest.ResponderName,
                    ReportedDescription = serviceRequest.ReportedDescription,
                    ParentTicketGuid = null,
                    MasterDailyRunResource_Guid = null,
                    ResponderStatus = null,
                    FlagStampReportedOnsite = serviceRequest.FlagStampReportedOnsite.GetValueOrDefault(),
                    FlagStampETA = serviceRequest.FlagStampETA.GetValueOrDefault(),
                    RescheduleReason = serviceRequest.RescheduleReason,
                    FlagCloseOTC = null,
                    ResponderEmail = serviceRequest.ResponderEmail,
                    ResponderShift = serviceRequest.ResponderShift,
                    FlagATM = serviceRequest.FlagATM,
                    FlagFLM = serviceRequest.FlagFLM,
                    FlagCompuSafe = serviceRequest.FlagCompuSafe,
                    MassRequestID = serviceRequest.MassRequestID,
                    FlagEcash = serviceRequest.FlagEcash,
                    PreviousServiceRequestState_Guid = null,
                    EnvBranchTypeId = serviceRequest.EnvBranchTypeId,
                    FlagBranchVendor = targetBranchInfo.FlagIntegrationSite,
                    SFOMasterMachineServiceType_Guid = serviceRequest.MachineServiceTypeGuid,
                    FlagOnHold = serviceRequest.FlagOnHold,

                    // SFI Information
                    CustomerCommentIn = serviceRequest.SFIModelView?.CustomerCommentIN,
                    CustomerNetworkID = serviceRequest.SFIModelView?.NetworkID,
                    FileReference = serviceRequest.SFIModelView?.FileReference
                };

                _transactionServiceRequestRepository.Create(newSr);

                #endregion

                #region 2. Snap Machine Info

                _transactionServiceRequestInfoRepository.SnapMachineInServiceRequest(serviceRequestGuid, slaTime.GetValueOrDefault());

                #endregion

                #region 3. Add problem

                CreateProblemList(serviceRequestGuid, serviceRequest.ProblemGuid, serviceRequest.MachineServiceTypeGuid.Value, targetBranchInfo.Guid, slaTime, userInfo.UserName);

                #endregion

                #region 4. Add Ecash

                CreateECashList(serviceRequestGuid, serviceRequest.ECashViewList, userInfo.UserName);

                #endregion

                #region 5. Add Log

                CreateLogSRCreate(jobHeaderGuid.Value, serviceRequest.TicketNumber, userInfo.UserName);

                #endregion

                _uow.Commit();

            }
            catch (Exception err)
            {
                try
                {
                    _systemLogHistoryError.CreateHistoryError(err);
                    _masterActualJobHeaderRepository.RemoveSFOJob(jobHeaderGuid.GetValueOrDefault());
                    _uow.Commit();
                }
                catch (Exception ex)
                {
                    _systemLogHistoryError.CreateHistoryError(ex);
                }

                // get exception
                SqlException sqlEx = null;

                sqlEx = err.InnerException?.InnerException != null
                  ? (SqlException)err.InnerException.InnerException
                  : (SqlException)err.InnerException;



                var msgId = new int?[] { 2627 }.Contains(sqlEx?.Number) ? -265 : -312;
                var msgView = _systemMessageRepository.FindByMsgId(msgId, userInfo.UserLanguageGuid.Value).ConvertToMessageView(false);

                return new CreateServiceRequestResponse
                {
                    SystemMessageView = msgView,
                    JobHeaderGuid = null,
                    ServiceRequestGuid = null,
                    TicketNumber = string.Empty
                };
            }

            var msg = _systemMessageRepository.FindByMsgId(500, userInfo.UserLanguageGuid.Value)
                        .ConvertToMessageView(true)
                        .ReplaceTextContentStringFormatWithValue(serviceRequest.TicketNumber);

            return new CreateServiceRequestResponse
            {
                SystemMessageView = msg,
                JobHeaderGuid = jobHeaderGuid,
                ServiceRequestGuid = serviceRequestGuid,
                TicketNumber = serviceRequest.TicketNumber
            };
        }


        private void CreateLogSRCreate(Guid jobHeaderGuid, string ticketNumber, string userCreated)
        {
            var genericLogList = new TransactionGenericLogModel();
            genericLogList.DateTimeCreated = DateTime.UtcNow;
            genericLogList.JSONValue = SystemHelper.GetJSONStringByArray(ticketNumber);
            genericLogList.LabelIndex = null;
            genericLogList.ReferenceValue = jobHeaderGuid.ToString();
            genericLogList.SystemLogCategory_Guid = SFOLogCategoryHelper.ServiceRequestDetailsGuid.ToGuid();
            genericLogList.SystemLogProcess_Guid = SFOProcessHelper.ServiceRequestGuid.ToGuid();
            genericLogList.SystemMsgID = "425";
            genericLogList.UserCreated = userCreated;
            _genericLogService.InsertTransactionGenericLog(genericLogList);
        }

        private int? GetSLATime(Guid problemGuid, SFOTblMasterMachine mv, int serviceTypeId)
        {
            var slaTime = 0;
            var masterProblem = _masterProblemRepository.FindById(problemGuid);
            if (masterProblem.FlagNoneSLA) return null;

            if (masterProblem.SLATime.HasValue)
                slaTime = masterProblem.SLATime.Value;
            else
            {
                switch (serviceTypeId)
                {
                    case ServiceJobTypeHelper.ServiceJobTypeSLM: slaTime = (mv.CompuSafeSLATime ?? mv.FLMSLATime).GetValueOrDefault(); break;
                    case ServiceJobTypeHelper.ServiceJobTypeEcashID: slaTime = (mv.ECashSLATime ?? mv.FLMSLATime).GetValueOrDefault(); break;
                    default: slaTime = (mv.FLMSLATime ?? mv.CompuSafeSLATime ?? mv.ECashSLATime).GetValueOrDefault(); break;
                }
            }

            return slaTime;
        }

        private void CreateProblemList(Guid serviceRequestGuid, Guid problemGuid, Guid machineServiceTypeGuid, Guid masterSiteGuid, int? slaTime, string userCreated)
        {
            var oldProblem = _transactionServiceRequestProblemRepository.FindAllAsQueryable(e => e.SFOTransactionServiceRequest_Guid == serviceRequestGuid);
            _transactionServiceRequestProblemRepository.RemoveRange(oldProblem);

            var masterProblem = _masterProblemRepository.FindById(problemGuid);
            var masterMachineServiceType = _masterMachineServiceTypeRepository.FindById(machineServiceTypeGuid);

            var srProblem = new SFOTblTransactionServiceRequest_Problem
            {
                Guid = Guid.NewGuid(),
                FlagDisable = false,
                FlagNoneSLA = masterProblem.FlagNoneSLA,
                SLATime = slaTime,
                MachineServiceTypeID = masterMachineServiceType.MachineServiceTypeID,
                MachineServiceTypeName = masterMachineServiceType.MachineServiceTypeName,
                MasterSite_Guid = masterSiteGuid,
                ProblemDescription = masterProblem.ProblemName,
                ProblemID = masterProblem.ProblemID,
                ProblemName = masterProblem.ProblemName,
                SFOMasterProblem_Guid = masterProblem.Guid,
                SFOMasterMachineServiceType_Guid = masterMachineServiceType.Guid,
                SFOTransactionServiceRequest_Guid = serviceRequestGuid,
                DatetimeCreated = DateTime.Now,
                DatetimeModified = DateTime.Now,
                UniversalDatetimeCreated = DateTime.UtcNow,
                UniversalDatetimeModified = DateTime.UtcNow,
                UserCreated = userCreated
            };

            _transactionServiceRequestProblemRepository.Create(srProblem);
        }

        private void CreateECashList(Guid serviceRequestGuid, IEnumerable<ECashView> ecashViewList, string userCreated)
        {
            if (ecashViewList.IsEmpty()) return;

            var dtNow = DateTime.Now;
            var dtUtcNow = DateTime.UtcNow;

            var newEcashList = ecashViewList.Select(e => new SFOTblTransactionServiceRequest_Ecash
            {
                Guid = Guid.NewGuid(),
                SFOTransactionServiceRequest_Guid = serviceRequestGuid,
                Amount = e.Amount,
                Currency = e.Currency,
                Denomination = e.Denomination,
                DenominationQuantity = e.DenominationQuantity,
                DenominationValue = e.DenominationValue,
                Unit = e.Unit,
                FlagDisable = false,
                UserCreated = userCreated,
                DatetimeCreated = dtNow,
                DatetimeModified = dtNow,
                UniversalDatetimeCreated = dtUtcNow,
                UniversalDatetimeModified = dtUtcNow,
            });

            _transactionServiceRequestEcashRepository.CreateRange(newEcashList);
        }

        private SRCreateResponse BaseCreateServiceRequest(SRCreateRequestFLM request, Action<ServiceRequestView> fnSetAttribute = null)
        {
            var machineInfo = _masterMachineRepository.GetMachineByGuid(request.MachineGuid);
            var machineServiceTypeGuid = request.MachineServiceTypeGuid == Guid.Empty
                    ? machineInfo.MachineServiceTypeGuid
                    : request.MachineServiceTypeGuid;

            var servicingBranchInfo = _masterSiteRepository.FindById(machineInfo.SFOServicingBranch_Guid.Value);
            var flmBranchInfo = _masterSiteRepository.FindById(machineInfo.SFOFLMBranch_Guid.Value);
            var targetBranchInfo = flmBranchInfo.FlagIntegrationSite ? flmBranchInfo : servicingBranchInfo;

            var problemInfo = _masterProblemRepository.FindById(request.ProblemGuid);

            var modelCreate = new ServiceRequestView
            {
                // mapping from request
                ContactName = request.ContactName,
                OpenSourceGuid = request.OpenSourceGuid,
                ContactPhone = request.ContactPhone,
                DateTimeDown = request.DateTimeDown,
                DateTimeNotified = request.DateTimeNotified,
                DateTimeServiceDate = request.DateTimeServiceDate,
                DateTimeNotifiedTime = request.DateTimeNotified.GetTime(),
                CustomerReferenceNumber = request.CustomerReferenceNumber,
                MachineGuid = request.MachineGuid,
                ProblemGuid = request.ProblemGuid,
                ReportedIncidentDescription = request.ReportedIncidentDescription,
                ReportedDescription = request.ReportedIncidentDescription,
                CountryGuid = request.MasterCountryGuid,

                BrinksCompanyGuid = machineInfo.BrinksCompany_Guid.Value,
                BrinkSiteCode = machineInfo.SiteCode,
                BrinkSiteGuid = targetBranchInfo.Guid,
                CustomerGuid = machineInfo.CustomerGuid.GetValueOrDefault(),
                EnvBranchTypeId = targetBranchInfo.FlagIntegrationSite
                        ? EnvBranchTypeHelper.FLMBranchID
                        : EnvBranchTypeHelper.ServicingBranchID,
                MachineServiceTypeGuid = machineServiceTypeGuid,
                ServiceTypeId = ServiceJobTypeHelper.ServiceJobTypeFLM,
                ServiceTypeGuid = ServiceJobTypeHelper.ServiceJobTypeFLMGuid.ToGuid(),
                PriorityGuid = problemInfo.SFOMasterPriority_Guid,
                TicketStatusGuid = JobStatusHelper.OpenGuid.ToGuid()
            };

            fnSetAttribute?.Invoke(modelCreate);

            var createResult = CreateJobAndServiceRequest(modelCreate);

            var response = new SRCreateResponse
            {
                IsSuccess = createResult.SystemMessageView.IsSuccess,
                JobHeaderGuid = createResult.JobHeaderGuid,
                Message = createResult.SystemMessageView.MessageTextContent,
                TicketGuid = createResult.ServiceRequestGuid,
                TicketNumber = createResult.TicketNumber
            };

            return response;
        }



        #endregion

        #endregion

        #endregion

        #region Private class

        private class CreateJobResponse
        {
            public bool IsSuccess { get; set; }
            public Guid? JobHeaderGuid { get; set; }
        }



        #endregion
    }


}
