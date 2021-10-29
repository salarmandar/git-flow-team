using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Service.Messagings.AdhocService;
using Bgt.Ocean.Service.Messagings.RunControlService;
using Bgt.Ocean.Service.ModelViews.ActualJobHeader;
using Bgt.Ocean.Service.ModelViews.Adhoc;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using static Bgt.Ocean.Infrastructure.Util.EnumRun;

namespace Bgt.Ocean.Service.Implementations.Adhoc
{
    public partial class AdhocService
    {
        public Adhoc_LOB_ServiceJobTypeView GetLineOfBusinessAndJobType(bool flagNotDelivery, bool flagAdhoc, bool flagFromMapping)
        {
            var languageGuid = ApiSession.UserLanguage_Guid.Value;

            Adhoc_LOB_ServiceJobTypeView lob_servicetype = new Adhoc_LOB_ServiceJobTypeView();
            List<int> typeJobDeliveryID = new List<int> { IntTypeJob.D, IntTypeJob.AC, IntTypeJob.AE, IntTypeJob.BCD, IntTypeJob.BCD_MultiBr, IntTypeJob.MCS};

            //Loop all groups of LOB
            var typeJobInLob = _systemServiceJobTypeLOBRepository.Func_LineOfBusinessJobTypeByFlagAdhocJob(languageGuid);

            //TFS#68707 : From non-preadvise not display service type BCO group
            if (flagFromMapping)
            {
                typeJobInLob = typeJobInLob.Where(e => e.ServiceJobTypeID != IntTypeJob.BCP).ToList();
            }

            foreach (var grp in typeJobInLob.GroupBy(lob => lob.LOBGuid))
            {
                var LOB = new Adhoc_LOB
                {
                    LOBGuid = grp.Key,
                    LOBFullName = grp.First().LOBFullName,

                };
                lob_servicetype.list_LOB.Add(LOB);

                List<LineOfBusinessJobTypeByFlagAdhocJobResult> filter_jobtypes;
                if (flagNotDelivery)// From Mapping 
                {
                    filter_jobtypes = grp.Where(e => !typeJobDeliveryID.Contains(e.ServiceJobTypeID.Value)).ToList();
                }
                else if (flagAdhoc)
                {
                    filter_jobtypes = grp.Where(e => e.ServiceJobTypeID.Value != IntTypeJob.BCD && e.ServiceJobTypeID.Value != IntTypeJob.BCD_MultiBr).ToList();
                }
                else //Not Mapping and Adhoc
                {
                    filter_jobtypes = grp.ToList();
                }

                //Loop all types in group
                foreach (var jobtype in filter_jobtypes)
                {
                    var LOB_ServiceType = new Adhoc_LOB_ServiceJobType
                    {
                        LOBGuid = jobtype.LOBGuid,
                        TypeName_DisplayText = jobtype.DisplayTextJobTypeName,
                        JobTypeGuid = jobtype.JobTypeGuid,
                        ServiceJobTypeID = jobtype.ServiceJobTypeID
                    };
                    lob_servicetype.list_LOB_ServiceType.Add(LOB_ServiceType);
                }
            }
            return lob_servicetype;
        }

        public IEnumerable<SubJobTypeView> GetSubServiceType(Guid? companyGuid, Guid? LobGuid, int? runStatusID, bool pageMasterRoute)
        {
            var jobType = _systemServiceJobTypeRepository.FindAllAsQueryable();
            var subServiceType = _masterSubServiceTypeRepository.FindAllAsQueryable();
            var data = from job in jobType
                       join sub in subServiceType on job.Guid equals sub.SystemServiceJobType_Guid
                       where sub.SystemLineOfBusiness_Guid == LobGuid && sub.FlagDisable == false
                       select new SubJobTypeView
                       {
                           JobTypeGuid = sub.SystemServiceJobType_Guid,
                           ServiceJobTypeID = job.ServiceJobTypeID,
                           SubJobTypeGuid = sub.Guid,
                           SubServiceTypeName = sub.SubServiceTypeName
                       };

            if (!pageMasterRoute)
            {
                if (runStatusID == StatusDailyRun.DispatchRun)
                {
                    List<int?> typeJobDeliveryID = new List<int?> { IntTypeJob.D, IntTypeJob.AC, IntTypeJob.AE };
                    data = data.Where(e => typeJobDeliveryID.Contains(e.ServiceJobTypeID));
                }
                else
                {
                    data = data.Where(e => e.ServiceJobTypeID != IntTypeJob.BCD);
                }
            }

            return data;
        }

        public CreateJobAdHocResponse CreateJobTransferVault(CreateJobAdHocRequest request)
        {
            try
            {
                DateTime? WorkDate_leg_P = null, WorkDate_leg_D = null;
                AdhocJobHeaderRequest headDetail = request.AdhocJobHeaderView;
                AdhocLegRequest pickupLeg = request.ServiceStopLegPickup;
                AdhocLegRequest deliveryLeg = request.ServiceStopLegDelivery;
                AdhocTempData tempdata = new AdhocTempData(request);


                headDetail.JobGuid = tempdata.JobGuid = Guid.NewGuid();
                headDetail.JobNo = tempdata.JobNo = GenerateJobNo(pickupLeg.BrinkSiteGuid);
                if (pickupLeg.FlagNonBillable.GetValueOrDefault() && deliveryLeg.FlagNonBillable.GetValueOrDefault())       //Should be validated from FrontEnd that both 2 legs have NonBillable
                {
                    var tblmsg = _systemMessageRepository.FindByMsgId(-998, request.LanguagueGuid); //Cannot be saved
                    return new CreateJobAdHocResponse(tblmsg);
                }
                using (var transcope = _uow.BeginTransaction())
                {
                    /*Internal ID -> 1 : Service Stop, 2 : Crew break, 3 : Inter Branch */
                    Guid serviceStopTypeGuid = _systemServiceStopTypesRepository.GetServiceStopTypeByID(1).Guid;

                    #region Get Site code
                    var tblsite_pk = _masterSiteRepository.FindById(pickupLeg.BrinkSiteGuid);
                    if (tblsite_pk == null)
                    {
                        //-1 : Data not found
                        var tblmsg = _systemMessageRepository.FindByMsgId(-1, request.LanguagueGuid);
                        return new CreateJobAdHocResponse(tblmsg);
                    }
                    var tblsite_del = _masterSiteRepository.FindById(deliveryLeg.BrinkSiteGuid);
                    if (tblsite_del == null)
                    {
                        //-1 : Data not found
                        var tblmsg = _systemMessageRepository.FindByMsgId(-1, request.LanguagueGuid);
                        return new CreateJobAdHocResponse(tblmsg);
                    }
                    pickupLeg.BrinkSiteCode = tblsite_pk.SiteCode;          //headDetail.BrinkSiteGuid from jobheader should be from User's default setting
                    deliveryLeg.BrinkSiteCode = tblsite_del.SiteCode;



                    //these are Locations of Brink's companies. 
                    var Companylocation_LegP = _masterCustomerLocationRepository.GetCompanyGuidBySite(pickupLeg.BrinkSiteGuid);
                    var Companylocation_LegD = _masterCustomerLocationRepository.GetCompanyGuidBySite(deliveryLeg.BrinkSiteGuid);
                    #endregion End Get Site code

                    #region Update RunningValue
                    DateTimeOffset date_offset = request.UniversalDatetime;
                    WorkDate_leg_P = pickupLeg.StrWorkDate_Date.ChangeFromStringToDate(request.DateTimeFormat);
                    WorkDate_leg_D = deliveryLeg.StrWorkDate_Date.ChangeFromStringToDate(request.DateTimeFormat);
                    if (WorkDate_leg_P == null || WorkDate_leg_D == null)
                    {
                        return CreateResponseFromID(-1, request.LanguagueGuid); //-1 : Data not found
                    }

                    #endregion End Update RunningValue

                    #region Check StatusJob

                    //// This Block check status Dispatched Run to return TV(D) only
                    if (deliveryLeg.RunResourceGuid != null) //TV(D) has Run 
                    {
                        var dailyRun = _masterDailyRunResourceRepository.FindById(deliveryLeg.RunResourceGuid);
                        if (dailyRun == null)
                        {
                            return CreateResponseFromID(-1, request.LanguagueGuid); //-1 : Data not found
                        }

                        //TV(D) cannot change status when create in Run but cannot create in Closed Run
                        var statusRunID = dailyRun.RunResourceDailyStatusID.GetValueOrDefault();
                        if (statusRunID == StatusDailyRun.ClosedRun)
                        {
                            //-761: The selected Run Resource No is not in the status that allow to be assigned. Please recheck.
                            var tblmsg = _systemMessageRepository.FindByMsgId(-761, request.LanguagueGuid);
                            return new CreateJobAdHocResponse(tblmsg);
                        }

                        //if This run is already connected from Master Route, we need to break this run and add Run history.
                        DisconnectMasterRouteAndAddRunHistory(dailyRun, request, _masterDailyRunResourceRepository, _masterHistory_DailyRunResource);


                        tempdata.IsUseDolphin_LegD = _systemEnvironmentMasterCountryRepository.IsUseDolphin(deliveryLeg.BrinkSiteGuid, deliveryLeg.RunResourceGuid.Value);
                        tempdata.ValidateMaxNumberJobs = ValidateMaxNumberJobs(1, deliveryLeg.RunResourceGuid.Value, WorkDate_leg_D.Value, request.LanguagueGuid);
                    }

                    //// This Block is to update data
                    int? StatusJob = IntStatusJob.Open; //if no Run, status job is Open
                    if (pickupLeg.RunResourceGuid != null)       /*updated: 2019/09/03 -> TFS#35983 */
                    {
                        var dailyRun = _masterDailyRunResourceRepository.FindById(pickupLeg.RunResourceGuid);
                        if (dailyRun == null)
                        {
                            return CreateResponseFromID(-1, request.LanguagueGuid); //-1 : Data not found
                        }

                        var statusRunID = dailyRun.RunResourceDailyStatusID.GetValueOrDefault();
                        switch (statusRunID)
                        {
                            case StatusDailyRun.ReadyRun:
                                StatusJob = IntStatusJob.OnTruckPickUp;
                                break;
                            case StatusDailyRun.DispatchRun:
                                StatusJob = IntStatusJob.OnTheWayPickUp;
                                tempdata.isDispatchRun = true; //Can be true only for TV(P)
                                break;
                            case StatusDailyRun.ClosedRun:
                                //-761: The selected Run Resource No is not in the status that allow to be assigned. Please recheck.
                                var tblmsg = _systemMessageRepository.FindByMsgId(-761, request.LanguagueGuid);
                                return new CreateJobAdHocResponse(tblmsg);
                        }

                        //if This run is already connected from Master Route, we need to break this run and add Run history.
                        DisconnectMasterRouteAndAddRunHistory(dailyRun, request, _masterDailyRunResourceRepository, _masterHistory_DailyRunResource);


                        tempdata.ValidateMaxNumberJobs = ValidateMaxNumberJobs(1, pickupLeg.RunResourceGuid.Value, WorkDate_leg_P.Value, request.LanguagueGuid);
                        tempdata.IsUseDolphin_LegP = _systemEnvironmentMasterCountryRepository.IsUseDolphin(pickupLeg.BrinkSiteGuid, pickupLeg.RunResourceGuid.Value);
                        if (deliveryLeg.RunResourceGuid != null && tempdata.ValidateMaxNumberJobs == null) //TV(P) is not max and TV(D) has Run
                        {
                            tempdata.ValidateMaxNumberJobs = ValidateMaxNumberJobs(1, deliveryLeg.RunResourceGuid.Value, WorkDate_leg_D.Value, request.LanguagueGuid);
                        }
                        tempdata.getMsgId_ForAdhocJob_LegP();
                    }
                    else if (deliveryLeg.RunResourceGuid != null) //TV(P) has no Run but TV(D) has Run
                    {
                        tempdata.getMsgId_ForAdhocJob_LegD();
                    }
                    else //Both TV(P) and TV(D) have no Run
                    {
                        tempdata.getMsgId_ForAdhocJob_LegP();
                    }
                    #endregion End Check StatusJob

                    #region Calculate dayinvault
                    if (WorkDate_leg_P != WorkDate_leg_D)
                    {
                        TimeSpan countDays = WorkDate_leg_D.GetValueOrDefault() - WorkDate_leg_P.GetValueOrDefault();
                        headDetail.DayInVaults = countDays.Days;
                    }
                    #endregion End Calculate dayinvault

                    bool isAutoFillContract = getCountryConfigToCheckAutoFillContract(tblsite_pk.MasterCountry_Guid);
                    Guid? ContractGuid = null;
                    if (!isAutoFillContract)  //if not autofill, it means "Validated from FrontEnd" or "Don't use Contract"
                    {
                        ContractGuid = headDetail.ContractGuid; //if headDetail.ContractGuid is null, it should be "Don't use Contract"
                    }

                    if (isAutoFillContract) //if autofill, user can choose any locations, so we seek from P, if cannot seek from P then seek from D
                    {
                        if (pickupLeg.LocationGuid != null)
                            ContractGuid = GetContractGuid(pickupLeg.LocationGuid.Value, headDetail.LineOfBusiness_Guid, headDetail.ServiceJobTypeGuid, headDetail.SubServiceTypeJobTypeGuid, WorkDate_leg_P.Value);
                        if (ContractGuid == null && deliveryLeg.LocationGuid != null)
                            ContractGuid = GetContractGuid(deliveryLeg.LocationGuid.Value, headDetail.LineOfBusiness_Guid, headDetail.ServiceJobTypeGuid, headDetail.SubServiceTypeJobTypeGuid, WorkDate_leg_D.Value);
                    }

                    #region Insert JobHeader
                    var insertHeader = new TblMasterActualJobHeader
                    {
                        Guid = headDetail.JobGuid.GetValueOrDefault(),
                        MasterRouteJobHeader_Guid = null, //--> Has value when create from master route
                        JobNo = headDetail.JobNo,
                        SystemStopType_Guid = serviceStopTypeGuid,
                        SystemLineOfBusiness_Guid = headDetail.LineOfBusiness_Guid,
                        SystemServiceJobType_Guid = headDetail.ServiceJobTypeGuid,
                        SaidToContain = headDetail.SaidToContain == null ? 0 : headDetail.SaidToContain,
                        MasterCurrency_Guid = headDetail.CurrencyGuid,
                        Remarks = headDetail.Remarks,
                        DayInVault = headDetail.DayInVaults,
                        MasterCustomerContract_Guid = ContractGuid,                              //added: 2019/08/27  -> TFS#35983
                        InformTime = !string.IsNullOrEmpty(headDetail.strInformTime) ? headDetail.strInformTime.ToTimeDateTime() : "00:00".ToTimeDateTime(),
                        SystemStatusJobID = StatusJob,
                        TransectionDate = WorkDate_leg_P,
                        FlagJobProcessDone = false,
                        OnwardDestinationType = deliveryLeg.OnwardTypeID,
                        OnwardDestination_Guid = deliveryLeg.OnwardDestGuid,
                        //FlagBgsAirport = request.FlagBgsAirport, --> cancel
                        UserCreated = request.UserName,
                        DatetimeCreated = request.ClientDateTime.DateTime,
                        UniversalDatetimeCreated = request.UniversalDatetime,
                        FlagJobClose = false,
                        FlagJobReadyToVault = false,
                        FlagMissingStop = false,
                        FlagJobDiscrepancies = false,
                        FlagNonDelivery = false,
                        FlagCancelAll = false,
                        FlagPickupAlready = false,
                        FlagSyncToMobile = 0,
                        FlagPickedupAfterInterchange = false,
                        SystemTripIndicator_Guid = deliveryLeg.TripIndicatorGuid,
                        FlagTotalValuePerJob = false,
                        MasterSubServiceType_Guid = headDetail.SubServiceTypeJobTypeGuid,
                        FlagJobInterBranch = (pickupLeg.BrinkSiteGuid != deliveryLeg.BrinkSiteGuid),
                        FlagChkOutInterBranchComplete = false,
                        SFOFlagRequiredTechnician = false,
                        FlagUpdateJobManual = false,
                        FlagRequireOpenLock = false
                    };
                    _masterActualJobHeaderRepository.Create(insertHeader);
                    #endregion End Insert JobHeader

                    var actionJob = _systemJobActionsRepository.FindAll();
                    Guid actionGuidPick = actionJob.FirstOrDefault(o => o.ActionNameAbbrevaition == JobActionAbb.StrPickUp).Guid;
                    Guid actionGuidDel = actionJob.FirstOrDefault(o => o.ActionNameAbbrevaition == JobActionAbb.StrDelivery).Guid;

                    #region Insert TblJobServiceStop
                    var serviceStopGuidPK = Guid.NewGuid();
                    var serviceStopGuidDEL = Guid.NewGuid();
                    var insertJobServiceStopPK = new TblMasterActualJobServiceStop
                    {
                        Guid = serviceStopGuidPK,
                        MasterActualJobHeader_Guid = headDetail.JobGuid,
                        CustomerLocationAction_Guid = actionGuidPick,
                        MasterCustomerLocation_Guid = pickupLeg.LocationGuid
                    };
                    _masterActualJobServiceStopsRepository.Create(insertJobServiceStopPK);

                    var insertJobServiceStopDEL = new TblMasterActualJobServiceStop
                    {
                        Guid = serviceStopGuidDEL,
                        MasterActualJobHeader_Guid = headDetail.JobGuid,
                        CustomerLocationAction_Guid = actionGuidDel,
                        MasterCustomerLocation_Guid = deliveryLeg.LocationGuid
                    };
                    _masterActualJobServiceStopsRepository.Create(insertJobServiceStopDEL);
                    #endregion End Insert TblJobServiceStop

                    #region Insert SpecialCommand
                    if (pickupLeg.SpecialCommandGuid != null)
                    {
                        var tblSpecialCommand = pickupLeg.SpecialCommandGuid.Select(o => new TblMasterActualJobServiceStopSpecialCommand
                        {
                            Guid = Guid.NewGuid(),
                            MasterActualJobServiceStop_Guid = serviceStopGuidPK,
                            MasterSpecialCommand_Guid = o
                        });
                        _masterActualJobServiceStopSpecialCommandsRepository.CreateRange(tblSpecialCommand);
                    }
                    if (deliveryLeg.SpecialCommandGuid != null)
                    {
                        var tblSpecialCommand = deliveryLeg.SpecialCommandGuid.Select(o => new TblMasterActualJobServiceStopSpecialCommand
                        {
                            Guid = Guid.NewGuid(),
                            MasterActualJobServiceStop_Guid = serviceStopGuidDEL,
                            MasterSpecialCommand_Guid = o
                        });
                        _masterActualJobServiceStopSpecialCommandsRepository.CreateRange(tblSpecialCommand);
                    }
                    #endregion End Insert SpecialCommand

                    #region Insert ServiceStopLegs
                    var WorkDate_Time_P = !string.IsNullOrEmpty(pickupLeg.StrWorkDate_Time) ? pickupLeg.StrWorkDate_Time.ToTimeDateTime() : "00:00".ToTimeDateTime();
                    var WorkDate_Time_D = !string.IsNullOrEmpty(deliveryLeg.StrWorkDate_Time) ? deliveryLeg.StrWorkDate_Time.ToTimeDateTime() : "00:00".ToTimeDateTime();

                    string ReceiptNo = "";
                    int maxStop = 0;
                    DateTime? clientDate = request.ClientDateTime.DateTime;
                    List<TblMasterActualJobServiceStopLegs> legs = new List<TblMasterActualJobServiceStopLegs>();
                    for (int i = 1; i <= 4; i++)
                    {
                        #region Get Printed Receipt
                        ReceiptNo = "";
                        if (i == 1 || i == 4)
                        {
                            var branchCode = _masterCustomerLocationRepository.FindById(i == 1 ? pickupLeg.LocationGuid : deliveryLeg.LocationGuid).BranchCodeReference;
                            PrintedReceiptNumberRequest itemreceipt = new PrintedReceiptNumberRequest();
                            itemreceipt.CustomerLocation_Guid = i == 1 ? pickupLeg.LocationGuid.Value : deliveryLeg.LocationGuid.Value;
                            itemreceipt.SequenceStop = i;
                            itemreceipt.ServiceStopTransectionDate = i == 1 ? WorkDate_leg_P : WorkDate_leg_D;
                            itemreceipt.BranchCodeReference = branchCode;
                            itemreceipt.SiteCode = i == 1 ? pickupLeg.BrinkSiteCode : deliveryLeg.BrinkSiteCode;
                            itemreceipt.JobNo = request.AdhocJobHeaderView.JobNo;
                            ReceiptNo = GetPrintedReceiptNumber(itemreceipt);
                        }

                        #endregion End Get Printed Receipt
                        Guid legGuid = Guid.NewGuid();
                        if (i == 1)
                        {
                            pickupLeg.LegGuid = legGuid;
                            maxStop = MaxJobOrderOnDailyRun(pickupLeg.RunResourceGuid) + 1;
                        }
                        else if (i == 4)
                        {
                            deliveryLeg.LegGuid = legGuid;
                            maxStop = (headDetail.DayInVaults > 0 || insertHeader.FlagJobInterBranch ? 1 : 2) + MaxJobOrderOnDailyRun(deliveryLeg.RunResourceGuid);
                        }
                        var insertStopLeg = new TblMasterActualJobServiceStopLegs
                        {
                            Guid = legGuid,
                            MasterActualJobHeader_Guid = insertHeader.Guid,
                            SequenceStop = i,
                            MasterRouteGroupDetail_Guid = (i <= 2 ? pickupLeg.RouteGroupDetailGuid : deliveryLeg.RouteGroupDetailGuid),
                            ServiceStopTransectionDate = (i <= 2 ? WorkDate_leg_P : WorkDate_leg_D),
                            WindowsTimeServiceTimeStart = (i <= 2 ? WorkDate_Time_P : WorkDate_Time_D),
                            WindowsTimeServiceTimeStop = (i <= 2 ? WorkDate_Time_P : WorkDate_Time_D),
                            MasterRunResourceDaily_Guid = i == 1 || i == 2 ? pickupLeg.RunResourceGuid : deliveryLeg.RunResourceGuid,
                            PrintedReceiptNumber = string.IsNullOrEmpty(ReceiptNo) ? null : ReceiptNo,
                            MasterSite_Guid = (i <= 2 ? pickupLeg.BrinkSiteGuid : deliveryLeg.BrinkSiteGuid),
                            ArrivalTime = null,
                            ActualTime = null,
                            DepartTime = null,
                            FlagCancelLeg = false
                        };
                        switch (i) //Switch Leg (1-4)
                        {
                            case 1:
                                insertStopLeg.FlagNonBillable = pickupLeg.FlagNonBillable.GetValueOrDefault();
                                insertStopLeg.CustomerLocationAction_Guid = actionGuidPick;
                                insertStopLeg.MasterCustomerLocation_Guid = pickupLeg.LocationGuid;
                                insertStopLeg.JobOrder = maxStop;
                                insertStopLeg.SeqIndex = maxStop;
                                if (!pickupLeg.RunResourceGuid.HasValue)
                                {
                                    insertStopLeg.UnassignedBy = request.UserName;
                                    insertStopLeg.UnassignedDatetime = clientDate;
                                }
                                break;
                            case 2:
                                insertStopLeg.CustomerLocationAction_Guid = actionGuidDel;
                                insertStopLeg.MasterCustomerLocation_Guid = Companylocation_LegP;
                                break;
                            case 3:
                                insertStopLeg.CustomerLocationAction_Guid = actionGuidPick;
                                insertStopLeg.MasterCustomerLocation_Guid = Companylocation_LegD;
                                break;
                            default: //Leg 4
                                insertStopLeg.FlagNonBillable = deliveryLeg.FlagNonBillable.GetValueOrDefault();
                                insertStopLeg.CustomerLocationAction_Guid = actionGuidDel;
                                insertStopLeg.MasterCustomerLocation_Guid = deliveryLeg.LocationGuid;
                                insertStopLeg.FlagDestination = true;//Only last leg that must be true
                                insertStopLeg.JobOrder = maxStop;
                                insertStopLeg.SeqIndex = 1;
                                if (!deliveryLeg.RunResourceGuid.HasValue)
                                {
                                    insertStopLeg.UnassignedBy = request.UserName;
                                    insertStopLeg.UnassignedDatetime = clientDate;
                                }
                                break;
                        }
                        legs.Add(insertStopLeg);

                        #region Insert History
                        Guid? runGuid = (i == 1 ? pickupLeg.RunResourceGuid : deliveryLeg.RunResourceGuid);
                        if (runGuid != null)
                        {
                            var insertHistoryRun = new TblMasterHistory_ActaulJobOnDailyRunResource
                            {
                                Guid = Guid.NewGuid(),
                                MasterActualJobHeader_Guid = insertHeader.Guid,
                                MasterRunResourceDaily_Guid = runGuid.GetValueOrDefault(),
                                DateTimeInterChange = date_offset, // Ple add 1/8/2016 for use in dolphin
                                MasterActualJobServiceStopLegs_Guid = insertStopLeg.Guid    // Ple add 1/8/2016 for use in dolphin
                            };
                            _masterHistoryActaulJobOnDailyRunResourceRepository.Create(insertHistoryRun);
                        }


                        #endregion End Insert History
                    }
                    CreateAdhocAddSpot(request, legs);
                    _masterActualJobServiceStopLegsRepository.CreateRange(legs);

                    #endregion End Insert ServiceStopLegs
                    var insertHistoryJob = new TblMasterHistory_ActualJob
                    {
                        Guid = Guid.NewGuid(),
                        MasterActualJobHeader_Guid = insertHeader.Guid, //headDetail.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                        MsgID = getMessageID(headDetail.FlagJobSFO), //Created job no. {0} date {1} from Adhoc Job by Ocean Online MVC.
                        MsgParameter = new string[] { headDetail.JobNo, pickupLeg.StrWorkDate_Date }.ToJSONString(),
                        UserCreated = request.UserName,
                        DatetimeCreated = request.ClientDateTime.DateTime,
                        UniversalDatetimeCreated = request.UniversalDatetime
                    };
                    _masterHistoryActualJobRepository.Create(insertHistoryJob);

                    _uow.Commit();
                    transcope.Complete();
                } //End Transaction scope

                #region ## Update JobOrder and Push to dolphin.
                if (!pickupLeg.RunResourceGuid.IsNullOrEmpty())
                {
                    UpdateJobOrderInRunRequest updatJobOrder = new UpdateJobOrderInRunRequest()
                    {
                        ClientDateTime = request.ClientDateTime.DateTime,
                        LanguageGuid = request.LanguagueGuid,
                        UserModified = request.UserName,
                        WorkDate = WorkDate_leg_P,
                        SiteGuid = pickupLeg.BrinkSiteGuid,
                        FlagReorder = false,
                        RunDailyGuid = pickupLeg.RunResourceGuid.GetValueOrDefault()

                    };
                    UpdateJobOrderInRun(updatJobOrder);
                    if (!request.AdhocJobHeaderView.FlagPageFromMapping) //From Non-Mapping Page
                    {
                        updatJobOrder.JobHeadGuidList = new List<Guid>() { headDetail.JobGuid.GetValueOrDefault() };
                        UpdatePushToDolPhinWhenCreateJob(updatJobOrder);
                    }
                }

                if (!deliveryLeg.RunResourceGuid.IsNullOrEmpty())
                {
                    UpdateJobOrderInRunRequest updateJobOrder = new UpdateJobOrderInRunRequest()
                    {
                        ClientDateTime = request.ClientDateTime.DateTime,
                        LanguageGuid = request.LanguagueGuid,
                        UserModified = request.UserName,
                        WorkDate = WorkDate_leg_D,
                        SiteGuid = deliveryLeg.BrinkSiteGuid,
                        FlagReorder = false,
                        RunDailyGuid = deliveryLeg.RunResourceGuid.GetValueOrDefault()

                    };
                    UpdateJobOrderInRun(updateJobOrder);
                    if (!request.AdhocJobHeaderView.FlagPageFromMapping) //From Non-Mapping Page
                    {

                        updateJobOrder.JobHeadGuidList = new List<Guid>() { headDetail.JobGuid.GetValueOrDefault() };
                        UpdatePushToDolPhinWhenCreateJob(updateJobOrder);
                    }
                }
                #endregion End ## Update JobOrder and Push to dolphin.

                #region ## Create OTC
                //=> TFS#53385:Ability to generate the OTC codes for both legs of Transfer job -> CreateJobTransferVault [NEW]
                _OTCRunControlService.CreateHeaderOtcByJobGuids(new Guid[] { headDetail.JobGuid.GetValueOrDefault() });
                #endregion


                var tblmsg_r = _systemMessageRepository.FindByMsgId(tempdata.MsgId_ForAdhocJob, request.LanguagueGuid);
                tempdata.ServiceJobTypeId = request.AdhocJobHeaderView.ServiceJobTypeID;
                return new CreateJobAdHocResponse(tblmsg_r, tempdata);
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);

                var tblmsg = _systemMessageRepository.FindByMsgId(-186, request.LanguagueGuid);
                return new CreateJobAdHocResponse(tblmsg);
            }
        }

        public CreateJobAdHocResponse CreateJobTransfer(CreateJobAdHocRequest request)
        {
            try
            {
                DateTime? WorkDate_leg_P = null, WorkDate_Time_P = null, WorkDate_Time_D = null;
                AdhocJobHeaderRequest headDetail = request.AdhocJobHeaderView;
                AdhocLegRequest pickupLeg = request.ServiceStopLegPickup;
                AdhocLegRequest deliveryLeg = request.ServiceStopLegDelivery;
                AdhocTempData tempdata = new AdhocTempData(request);

                headDetail.JobGuid = tempdata.JobGuid = Guid.NewGuid();
                headDetail.JobNo = tempdata.JobNo = GenerateJobNo(pickupLeg.BrinkSiteGuid);
                if (pickupLeg.FlagNonBillable == true && deliveryLeg.FlagNonBillable == true)       //Should be validated from FrontEnd that both 2 legs have NonBillable
                {
                    var tblmsg = _systemMessageRepository.FindByMsgId(-998, request.LanguagueGuid); //Cannot be saved
                    return new CreateJobAdHocResponse(tblmsg);
                }
                using (var transcope = _uow.BeginTransaction())
                {

                    /*Internal ID -> 1 : Service Stop, 2 : Crew break, 3 : Inter Branch */
                    Guid serviceStopTypeGuid = _systemServiceStopTypesRepository.GetServiceStopTypeByID(1).Guid;

                    #region Get Site code
                    var tblsite_pk = _masterSiteRepository.FindById(pickupLeg.BrinkSiteGuid);
                    if (tblsite_pk == null)
                    {
                        //-1 : Data not found
                        var tblmsg = _systemMessageRepository.FindByMsgId(-1, request.LanguagueGuid);
                        return new CreateJobAdHocResponse(tblmsg);
                    }
                    pickupLeg.BrinkSiteCode = tblsite_pk.SiteCode;          //T job should have only one site code, Leg P should be main process.
                    #endregion End Get Site code

                    DateTimeOffset date_offset = request.UniversalDatetime;
                    WorkDate_leg_P = pickupLeg.StrWorkDate_Date.ChangeFromStringToDate(request.DateTimeFormat);
                    WorkDate_Time_P = !string.IsNullOrEmpty(pickupLeg.StrWorkDate_Time) ? pickupLeg.StrWorkDate_Time.ToTimeDateTime() : "00:00".ToTimeDateTime();
                    WorkDate_Time_D = !string.IsNullOrEmpty(deliveryLeg.StrWorkDate_Time) ? deliveryLeg.StrWorkDate_Time.ToTimeDateTime() : "00:00".ToTimeDateTime();
                    if (WorkDate_leg_P == null)
                    {
                        return CreateResponseFromID(-1, request.LanguagueGuid); //-1 : Data not found
                    }

                    int? StatusJob = IntStatusJob.Open;
                    if (pickupLeg.RunResourceGuid != null)               /*updated: 2019/09/03 -> TFS#35983 */
                    {
                        var dailyRun = _masterDailyRunResourceRepository.FindById(pickupLeg.RunResourceGuid);
                        if (dailyRun == null)
                        {
                            return CreateResponseFromID(-1, request.LanguagueGuid); //-1 : Data not found
                        }

                        var statusRunID = dailyRun.RunResourceDailyStatusID.GetValueOrDefault();
                        switch (statusRunID)
                        {
                            case StatusDailyRun.ReadyRun:
                                StatusJob = IntStatusJob.OnTruck;
                                break;
                            case StatusDailyRun.DispatchRun:
                                StatusJob = IntStatusJob.OnTheWay;
                                tempdata.isDispatchRun = true;
                                break;
                            case StatusDailyRun.ClosedRun:
                                //-761: The selected Run Resource No is not in the status that allow to be assigned. Please recheck.
                                var tblmsg = _systemMessageRepository.FindByMsgId(-761, request.LanguagueGuid);
                                return new CreateJobAdHocResponse(tblmsg);
                        }

                        //if This run is already connected from Master Route, we need to break this run and add Run history.
                        DisconnectMasterRouteAndAddRunHistory(dailyRun, request, _masterDailyRunResourceRepository, _masterHistory_DailyRunResource);


                        tempdata.ValidateMaxNumberJobs = ValidateMaxNumberJobs(1, pickupLeg.RunResourceGuid.Value, WorkDate_leg_P.Value, request.LanguagueGuid);
                        tempdata.IsUseDolphin_LegP = _systemEnvironmentMasterCountryRepository.IsUseDolphin(pickupLeg.BrinkSiteGuid, pickupLeg.RunResourceGuid.Value);
                    } // End pickupLeg.RunResourceGuid != null
                    tempdata.getMsgId_ForAdhocJob_LegP();

                    bool isAutoFillContract = getCountryConfigToCheckAutoFillContract(tblsite_pk.MasterCountry_Guid);
                    Guid? ContractGuid = null;
                    if (!isAutoFillContract)  //if not autofill, it means "Validated from FrontEnd" or "Don't use Contract"
                    {
                        ContractGuid = headDetail.ContractGuid; //if headDetail.ContractGuid is null, it should be "Don't use Contract"
                    }

                    if (isAutoFillContract && pickupLeg.LocationGuid != null) //if autofill, user can choose any location from P, so we need to seek from location of Leg P
                    {
                        ContractGuid = GetContractGuid(pickupLeg.LocationGuid.Value, headDetail.LineOfBusiness_Guid, headDetail.ServiceJobTypeGuid, headDetail.SubServiceTypeJobTypeGuid, WorkDate_leg_P.Value);
                    }

                    #region Insert JobHeader
                    var insertHeader = new TblMasterActualJobHeader
                    {
                        Guid = headDetail.JobGuid.Value,
                        MasterRouteJobHeader_Guid = null, //request.AdhocJobHeaderView.MasterRouteJobHeader_Guid,  --> Has value when create from master route 
                        JobNo = headDetail.JobNo,
                        SystemStopType_Guid = serviceStopTypeGuid,
                        SystemLineOfBusiness_Guid = headDetail.LineOfBusiness_Guid,
                        SystemServiceJobType_Guid = headDetail.ServiceJobTypeGuid,
                        SaidToContain = headDetail.SaidToContain == null ? 0 : headDetail.SaidToContain,
                        MasterCurrency_Guid = headDetail.CurrencyGuid,
                        Remarks = headDetail.Remarks,
                        DayInVault = 0,
                        MasterCustomerContract_Guid = ContractGuid,                          //added: 2019/08/27  -> TFS#35983
                        InformTime = !string.IsNullOrEmpty(headDetail.strInformTime) ? headDetail.strInformTime.ToTimeDateTime() : "00:00".ToTimeDateTime(),
                        SystemStatusJobID = StatusJob,
                        TransectionDate = WorkDate_leg_P,
                        FlagJobProcessDone = false,
                        OnwardDestinationType = deliveryLeg.OnwardTypeID,
                        OnwardDestination_Guid = deliveryLeg.OnwardDestGuid,
                        //FlagBgsAirport = headDetail.FlagBgsAirport, --> cancel
                        UserCreated = request.UserName,
                        DatetimeCreated = request.ClientDateTime.DateTime,
                        UniversalDatetimeCreated = request.UniversalDatetime,
                        FlagJobClose = false,
                        FlagJobReadyToVault = false,
                        FlagMissingStop = false,
                        FlagJobDiscrepancies = false,
                        FlagNonDelivery = false,
                        FlagCancelAll = false,
                        FlagPickupAlready = false,
                        FlagSyncToMobile = 0,
                        FlagPickedupAfterInterchange = false,
                        SystemTripIndicator_Guid = deliveryLeg.TripIndicatorGuid,
                        FlagTotalValuePerJob = false,
                        MasterSubServiceType_Guid = headDetail.SubServiceTypeJobTypeGuid,
                        FlagChkOutInterBranchComplete = false,
                        FlagJobInterBranch = false, /*Job Transfer type doesn't have interbrach*/
                        SFOFlagRequiredTechnician = false,
                        FlagUpdateJobManual = false,
                        FlagRequireOpenLock = false
                    };
                    _masterActualJobHeaderRepository.Create(insertHeader);
                    #endregion End Insert JobHeader

                    #region Action Job
                    var actionJob = _systemJobActionsRepository.FindAll();
                    Guid actionGuidPick = actionJob.FirstOrDefault(o => o.ActionNameAbbrevaition == JobActionAbb.StrPickUp).Guid;
                    Guid actionGuidDel = actionJob.FirstOrDefault(o => o.ActionNameAbbrevaition == JobActionAbb.StrDelivery).Guid;
                    #endregion End Action Job

                    #region Insert TblJobServiceStop
                    var serviceStopGuidPK = Guid.NewGuid();
                    var serviceStopGuidDEL = Guid.NewGuid();
                    var insertJobServiceStopPK = new TblMasterActualJobServiceStop
                    {
                        Guid = serviceStopGuidPK,
                        MasterActualJobHeader_Guid = insertHeader.Guid,
                        CustomerLocationAction_Guid = actionGuidPick,
                        MasterCustomerLocation_Guid = pickupLeg.LocationGuid
                    };
                    _masterActualJobServiceStopsRepository.Create(insertJobServiceStopPK);

                    var insertJobServiceStopDEL = new TblMasterActualJobServiceStop
                    {
                        Guid = serviceStopGuidDEL,
                        MasterActualJobHeader_Guid = insertHeader.Guid,
                        CustomerLocationAction_Guid = actionGuidDel,
                        MasterCustomerLocation_Guid = deliveryLeg.LocationGuid
                    };
                    _masterActualJobServiceStopsRepository.Create(insertJobServiceStopDEL);
                    #endregion End Insert TblJobServiceStop

                    #region Insert SpecialCommand
                    if (pickupLeg.SpecialCommandGuid != null)
                    {
                        var tblSpecialCommand = pickupLeg.SpecialCommandGuid.Select(o => new TblMasterActualJobServiceStopSpecialCommand
                        {
                            Guid = Guid.NewGuid(),
                            MasterActualJobServiceStop_Guid = serviceStopGuidPK,
                            MasterSpecialCommand_Guid = o
                        });
                        _masterActualJobServiceStopSpecialCommandsRepository.CreateRange(tblSpecialCommand);
                    }
                    if (deliveryLeg.SpecialCommandGuid != null)
                    {
                        var tblSpecialCommand = deliveryLeg.SpecialCommandGuid.Select(o => new TblMasterActualJobServiceStopSpecialCommand
                        {
                            Guid = Guid.NewGuid(),
                            MasterActualJobServiceStop_Guid = serviceStopGuidDEL,
                            MasterSpecialCommand_Guid = o
                        });
                        _masterActualJobServiceStopSpecialCommandsRepository.CreateRange(tblSpecialCommand);
                    }
                    #endregion End Insert SpecialCommand

                    #region Insert ServiceStopLegs                   

                    #region ## Declare valiable for insert job leg
                    string ReceiptNo = "";
                    Guid? actionGuid = Guid.Empty;
                    Guid? customerLocationGuid = Guid.Empty;
                    string branchCode = "";
                    #endregion
                    int maxStopOnRun = MaxJobOrderOnDailyRun(pickupLeg.RunResourceGuid) + 1;
                    DateTime? clientDate = request.ClientDateTime.DateTime;
                    List<TblMasterActualJobServiceStopLegs> legs = new List<TblMasterActualJobServiceStopLegs>();
                    for (int i = 1; i <= 2; i++)
                    {

                        #region ## Set Data 
                        switch (i)
                        {
                            case 1:
                                {
                                    actionGuid = actionGuidPick;
                                    customerLocationGuid = pickupLeg.LocationGuid;
                                    branchCode = _masterCustomerLocationRepository.FindById(pickupLeg.LocationGuid).BranchCodeReference;
                                    break;
                                }
                            case 2:
                                {
                                    actionGuid = actionGuidDel;
                                    customerLocationGuid = deliveryLeg.LocationGuid;
                                    branchCode = _masterCustomerLocationRepository.FindById(deliveryLeg.LocationGuid).BranchCodeReference;
                                    break;
                                }
                            default:
                                break;
                        }

                        #endregion

                        #region Get Printed Receipt


                        PrintedReceiptNumberRequest itemreceipt = new PrintedReceiptNumberRequest();
                        itemreceipt.CustomerLocation_Guid = customerLocationGuid.Value;
                        itemreceipt.SequenceStop = i;
                        itemreceipt.ServiceStopTransectionDate = WorkDate_leg_P; //should be the same in job T
                        itemreceipt.BranchCodeReference = branchCode;
                        itemreceipt.SiteCode = pickupLeg.BrinkSiteCode;
                        itemreceipt.JobNo = request.AdhocJobHeaderView.JobNo;
                        ReceiptNo = GetPrintedReceiptNumber(itemreceipt);

                        #endregion

                        Guid legGuid = Guid.NewGuid();
                        if (i == 1)
                        {
                            pickupLeg.LegGuid = legGuid;
                        }
                        else
                        {
                            deliveryLeg.LegGuid = legGuid;
                        }

                        var insertStopLeg = new TblMasterActualJobServiceStopLegs
                        {
                            Guid = legGuid,
                            MasterActualJobHeader_Guid = insertHeader.Guid,
                            SequenceStop = i,
                            CustomerLocationAction_Guid = actionGuid,
                            MasterCustomerLocation_Guid = customerLocationGuid,
                            MasterRouteGroupDetail_Guid = pickupLeg.RouteGroupDetailGuid,
                            ServiceStopTransectionDate = WorkDate_leg_P, //both Leg P and D should be the same in Job T
                            WindowsTimeServiceTimeStart = i == 1 ? WorkDate_Time_P : WorkDate_Time_D,
                            WindowsTimeServiceTimeStop = i == 1 ? WorkDate_Time_P : WorkDate_Time_D,
                            JobOrder = maxStopOnRun + (i - 1),
                            SeqIndex = maxStopOnRun,
                            MasterRunResourceDaily_Guid = pickupLeg.RunResourceGuid,
                            PrintedReceiptNumber = ReceiptNo,
                            MasterSite_Guid = pickupLeg.BrinkSiteGuid,
                            ArrivalTime = null,
                            ActualTime = null,
                            DepartTime = null,
                            FlagCancelLeg = false,
                            FlagNonBillable = i == 1 ? pickupLeg.FlagNonBillable.GetValueOrDefault() : deliveryLeg.FlagNonBillable.GetValueOrDefault(),
                            FlagDestination = (i == 2), //Only last Leg must be true
                            UnassignedBy = !pickupLeg.RunResourceGuid.HasValue ? request.UserName : null,
                            UnassignedDatetime = !pickupLeg.RunResourceGuid.HasValue ? clientDate : null

                        };
                        legs.Add(insertStopLeg);
                        #region Insert History Run 

                        Guid? runGuid = pickupLeg.RunResourceGuid; //updated: 2018/03/15
                        if (runGuid != null)
                        {
                            var insertHistoryRun = new TblMasterHistory_ActaulJobOnDailyRunResource
                            {
                                Guid = Guid.NewGuid(),
                                MasterActualJobHeader_Guid = insertHeader.Guid,
                                MasterRunResourceDaily_Guid = runGuid.GetValueOrDefault(),
                                DateTimeInterChange = date_offset, // Ple add 1/8/2016 for use in dolphin
                                MasterActualJobServiceStopLegs_Guid = insertStopLeg.Guid    // Ple add 1/8/2016 for use in dolphin
                            };
                            _masterHistoryActaulJobOnDailyRunResourceRepository.Create(insertHistoryRun);
                        }



                        #endregion

                    }
                    CreateAdhocAddSpot(request, legs);
                    _masterActualJobServiceStopLegsRepository.CreateRange(legs);

                    #endregion End Insert ServiceStopLegs

                    #region Insert TblMasterHistory_ActualJob
                    var insertHistoryJob = new TblMasterHistory_ActualJob
                    {
                        Guid = Guid.NewGuid(),
                        MasterActualJobHeader_Guid = insertHeader.Guid, //headDetail.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                        MsgID = getMessageID(headDetail.FlagJobSFO), //Created job no. {0} date {1} from Adhoc Job by Ocean Online MVC.
                        MsgParameter = new string[] { headDetail.JobNo, pickupLeg.StrWorkDate_Date }.ToJSONString(),
                        UserCreated = request.UserName,
                        DatetimeCreated = request.ClientDateTime.DateTime,
                        UniversalDatetimeCreated = request.UniversalDatetime
                    };
                    _masterHistoryActualJobRepository.Create(insertHistoryJob);
                    #endregion

                    _uow.Commit();
                    transcope.Complete();
                } //End Transaction scope

                #region ## Update JobOrder and Push to dolphin.
                if (!pickupLeg.RunResourceGuid.IsNullOrEmpty())
                {
                    UpdateJobOrderInRunRequest updatJobOrder = new UpdateJobOrderInRunRequest()
                    {
                        ClientDateTime = request.ClientDateTime.DateTime,
                        LanguageGuid = request.LanguagueGuid,
                        UserModified = request.UserName,
                        WorkDate = WorkDate_leg_P,
                        SiteGuid = pickupLeg.BrinkSiteGuid,
                        FlagReorder = false,
                        RunDailyGuid = pickupLeg.RunResourceGuid.GetValueOrDefault()

                    };
                    UpdateJobOrderInRun(updatJobOrder);
                    if (!request.AdhocJobHeaderView.FlagPageFromMapping) //From Non-Mapping Page
                    {
                        updatJobOrder.JobHeadGuidList = new List<Guid>() { headDetail.JobGuid.GetValueOrDefault() };
                        UpdatePushToDolPhinWhenCreateJob(updatJobOrder);
                    }
                }
                #endregion End ## Update JobOrder and Push to dolphin.

                #region ## Create OTC
                //=> TFS#53385:Ability to generate the OTC codes for both legs of Transfer job -> CreateJobTransfer [NEW]
                _OTCRunControlService.CreateHeaderOtcByJobGuids(new Guid[] { headDetail.JobGuid.GetValueOrDefault() });
                #endregion

                var tblmsg_r = _systemMessageRepository.FindByMsgId(tempdata.MsgId_ForAdhocJob, request.LanguagueGuid);
                tempdata.ServiceJobTypeId = request.AdhocJobHeaderView.ServiceJobTypeID;
                return new CreateJobAdHocResponse(tblmsg_r, tempdata);
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);

                var tblmsg = _systemMessageRepository.FindByMsgId(-186, request.LanguagueGuid);
                return new CreateJobAdHocResponse(tblmsg);
            }
        }
    }
}
