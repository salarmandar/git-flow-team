using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Service.Messagings.AdhocService;
using Bgt.Ocean.Service.Messagings.CustomerLocationService;
using Bgt.Ocean.Service.Messagings.RunControlService;
using Bgt.Ocean.Service.ModelViews.ActualJobHeader;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using static Bgt.Ocean.Infrastructure.Util.EnumRun;

namespace Bgt.Ocean.Service.Implementations.Adhoc
{
    public partial class AdhocService : IAdhocService
    {
        #region Job TV
        public CreateJobAdHocResponse CreateJobTransferVaultMultiBranch(CreateJobAdHocRequest request)
        {
            try
            {
                DateTime? WorkDate_leg_P = null, WorkDate_leg_D = null;
                AdhocJobHeaderRequest headDetail = request.AdhocJobHeaderView;
                AdhocLegRequest pickupLeg = request.ServiceStopLegPickup;
                AdhocLegRequest deliveryLeg = request.ServiceStopLegDelivery;
                AdhocTempData tempdata = new AdhocTempData(request);
                DateTime? unassignDate = request.ClientDateTime.DateTime;
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
                    var tblsite_del = _masterSiteRepository.FindById(deliveryLeg.BrinkSiteGuid);
                    if (tblsite_del == null)
                    {
                        //-1 : Data not found
                        var tblmsg = _systemMessageRepository.FindByMsgId(-1, request.LanguagueGuid);
                        return new CreateJobAdHocResponse(tblmsg);
                    }
                    pickupLeg.BrinkSiteCode = tblsite_pk.SiteCode;          //headDetail.BrinkSiteGuid from jobheader should be from User's default setting
                    deliveryLeg.BrinkSiteCode = tblsite_del.SiteCode;
                    #endregion //End Get Site code

                    #region Update RunningValue
                    DateTimeOffset date_offset = request.UniversalDatetime;
                    WorkDate_leg_P = pickupLeg.StrWorkDate_Date.ChangeFromStringToDate(request.DateTimeFormat);
                    WorkDate_leg_D = deliveryLeg.StrWorkDate_Date.ChangeFromStringToDate(request.DateTimeFormat);
                    if (WorkDate_leg_P == null || WorkDate_leg_D == null)
                    {
                        return CreateResponseFromID(-1, request.LanguagueGuid); //-1 : Data not found
                    }

                    #endregion //End Update RunningValue

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
                    if (pickupLeg.RunResourceGuid != null)
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
                    #endregion //End Check StatusJob

                    #region Calculate dayinvault
                    if (WorkDate_leg_P != WorkDate_leg_D)
                    {
                        TimeSpan countDays = WorkDate_leg_D.GetValueOrDefault() - WorkDate_leg_P.GetValueOrDefault();
                        headDetail.DayInVaults = countDays.Days;
                    }
                    #endregion //End Calculate dayinvault

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
                        MasterRouteJobHeader_Guid = null,
                        JobNo = headDetail.JobNo,
                        SystemStopType_Guid = serviceStopTypeGuid,
                        SystemLineOfBusiness_Guid = headDetail.LineOfBusiness_Guid,
                        SystemServiceJobType_Guid = headDetail.ServiceJobTypeGuid,
                        SaidToContain = headDetail.SaidToContain == null ? 0 : headDetail.SaidToContain,
                        MasterCurrency_Guid = headDetail.CurrencyGuid,
                        Remarks = headDetail.Remarks,
                        DayInVault = headDetail.DayInVaults,
                        InformTime = !string.IsNullOrEmpty(headDetail.strInformTime) ? headDetail.strInformTime.ToTimeDateTime() : "00:00".ToTimeDateTime(),
                        SystemStatusJobID = StatusJob,
                        TransectionDate = WorkDate_leg_P,
                        FlagJobProcessDone = false,
                        OnwardDestinationType = deliveryLeg.OnwardTypeID,
                        OnwardDestination_Guid = deliveryLeg.OnwardDestGuid,
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
                        FlagJobInterBranch = false,
                        FlagChkOutInterBranchComplete = false,
                        SFOFlagRequiredTechnician = false,
                        FlagUpdateJobManual = false,
                        FlagRequireOpenLock = false,
                        FlagMultiBranch = true,
                        MasterSitePathHeader_Guid = headDetail.SitePathGuid,
                        MasterCustomerContract_Guid = ContractGuid   /*added: 2019/08/27 -> TFS#35983 */
                    };
                    _masterActualJobHeaderRepository.Create(insertHeader);
                    #endregion //End Insert JobHeader

                    #region Insert SpecialCommand
                    var actionJob = _systemJobActionsRepository.FindAll();
                    Guid actionGuidPick = actionJob.FirstOrDefault(o => o.ActionNameAbbrevaition == JobActionAbb.StrPickUp).Guid;
                    Guid actionGuidDel = actionJob.FirstOrDefault(o => o.ActionNameAbbrevaition == JobActionAbb.StrDelivery).Guid;
                    var serviceStopGuidPK = Guid.NewGuid();         //needs to use in Special Command and TblMasterActualJobServiceStop
                    var serviceStopGuidDEL = Guid.NewGuid();        //needs to use in Special Command and TblMasterActualJobServiceStop

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
                    #endregion //End Insert SpecialCommand

                    #region Insert ServiceStopLegs
                    var WorkDate_Time_P = !string.IsNullOrEmpty(pickupLeg.StrWorkDate_Time) ? pickupLeg.StrWorkDate_Time.ToTimeDateTime() : "00:00".ToTimeDateTime();
                    var WorkDate_Time_D = !string.IsNullOrEmpty(deliveryLeg.StrWorkDate_Time) ? deliveryLeg.StrWorkDate_Time.ToTimeDateTime() : "00:00".ToTimeDateTime();

                    Guid[] allSitePathDetails = _masterSitePathRepository.getShortInfoSitePathDetail(headDetail.SitePathGuid).Select(o => o.SiteGuid).ToArray();
                    int maxLeg = (allSitePathDetails.Length * 2) + 2;
                    for (int i = 1; i <= maxLeg; i++)
                    {
                        #region Input TblMasterActualJobServiceStopLegs
                        Guid legGuid = Guid.NewGuid();
                        TblMasterActualJobServiceStop insertStop = null;
                        TblMasterActualJobServiceStopLegs insertStopLeg = null;
                        var branchCode = _masterCustomerLocationRepository.FindById(pickupLeg.LocationGuid).BranchCodeReference;

                        ////////  Seperated by First, Last, Even and Odd write codes here
                        if (i == 1)
                        {
                            PrintedReceiptNumberRequest itemreceipt = new PrintedReceiptNumberRequest()
                            {
                                CustomerLocation_Guid = pickupLeg.LocationGuid.Value,
                                SequenceStop = 1,
                                ServiceStopTransectionDate = WorkDate_leg_P,
                                BranchCodeReference = branchCode,
                                SiteCode = pickupLeg.BrinkSiteCode,
                                JobNo = request.AdhocJobHeaderView.JobNo
                            };

                            int pLegMaxStop = MaxJobOrderOnDailyRun(pickupLeg.RunResourceGuid) + 1;
                            insertStopLeg = new TblMasterActualJobServiceStopLegs
                            {
                                CustomerLocationAction_Guid = actionGuidPick,
                                MasterCustomerLocation_Guid = pickupLeg.LocationGuid,
                                MasterSite_Guid = pickupLeg.BrinkSiteGuid,
                                FlagNonBillable = pickupLeg.FlagNonBillable.GetValueOrDefault(),
                                PrintedReceiptNumber = GetPrintedReceiptNumber(itemreceipt),
                                JobOrder = pLegMaxStop,
                                SeqIndex = pLegMaxStop,
                                UnassignedBy = !pickupLeg.RunResourceGuid.HasValue ? request.UserName : null,
                                UnassignedDatetime = !pickupLeg.RunResourceGuid.HasValue ? unassignDate : null

                            };
                            pickupLeg.LegGuid = legGuid;                                                //Only 1st leg that's still in use.
                            insertStop = new TblMasterActualJobServiceStop                              //similar as TblMasterActualJobServiceStopLegs. Used by Dolphin
                            {
                                Guid = serviceStopGuidPK,                                               //Only 1st leg that's same as Special Command
                                MasterActualJobHeader_Guid = headDetail.JobGuid.GetValueOrDefault(),
                                CustomerLocationAction_Guid = actionGuidPick,
                                MasterCustomerLocation_Guid = pickupLeg.LocationGuid,
                                SequenceOrder = 1
                            };
                        }
                        else if (i == maxLeg)   //Last Leg
                        {
                            PrintedReceiptNumberRequest itemreceipt = new PrintedReceiptNumberRequest()
                            {
                                CustomerLocation_Guid = deliveryLeg.LocationGuid.Value,
                                SequenceStop = maxLeg,
                                ServiceStopTransectionDate = WorkDate_leg_D,
                                BranchCodeReference = branchCode,
                                SiteCode = deliveryLeg.BrinkSiteCode,
                                JobNo = request.AdhocJobHeaderView.JobNo
                            };
                            int dLegMaxStop = MaxJobOrderOnDailyRun(deliveryLeg.RunResourceGuid) + 1;
                            insertStopLeg = new TblMasterActualJobServiceStopLegs
                            {
                                CustomerLocationAction_Guid = actionGuidDel,
                                MasterCustomerLocation_Guid = deliveryLeg.LocationGuid,
                                MasterSite_Guid = deliveryLeg.BrinkSiteGuid,
                                FlagNonBillable = deliveryLeg.FlagNonBillable.GetValueOrDefault(),
                                PrintedReceiptNumber = GetPrintedReceiptNumber(itemreceipt),
                                FlagDestination = true,                                             //Only last leg that must be true
                                JobOrder = dLegMaxStop,
                                SeqIndex = dLegMaxStop,
                                UnassignedBy = !deliveryLeg.RunResourceGuid.HasValue ? request.UserName : null,
                                UnassignedDatetime = !deliveryLeg.RunResourceGuid.HasValue ? unassignDate : null
                            };
                            deliveryLeg.LegGuid = legGuid;                                              //Only last leg that's still in use.
                            insertStop = new TblMasterActualJobServiceStop
                            {
                                Guid = Guid.NewGuid(),
                                MasterActualJobHeader_Guid = headDetail.JobGuid.GetValueOrDefault(),
                                CustomerLocationAction_Guid = actionGuidDel,
                                MasterCustomerLocation_Guid = deliveryLeg.LocationGuid,
                                SequenceOrder = maxLeg
                            };
                        }
                        else  //Not First and Last Legs
                        {
                            int siteIndex = (i / 2) - 1; //Site of leg 2 and 3 will get same siteIndex, also (leg 4 == leg 5) && (leg 6 == leg 7) && ...
                            Guid siteGuid = allSitePathDetails[siteIndex];
                            var Companylocation = _masterCustomerLocationRepository.GetCompanyGuidBySite(siteGuid);
                            if (i % 2 == 1)  //Odd Numbers  (3, 5, 7, 9, ...)
                            {
                                insertStopLeg = new TblMasterActualJobServiceStopLegs
                                {
                                    Guid = Guid.NewGuid(),
                                    CustomerLocationAction_Guid = actionGuidPick,
                                    MasterCustomerLocation_Guid = Companylocation,
                                    MasterSite_Guid = siteGuid,
                                    FlagNonBillable = deliveryLeg.FlagNonBillable.GetValueOrDefault()
                                };
                            }
                            else //Even Numbers (2, 4, 6, 8, ...) 
                            {
                                insertStopLeg = new TblMasterActualJobServiceStopLegs
                                {
                                    Guid = Guid.NewGuid(),
                                    CustomerLocationAction_Guid = actionGuidDel,
                                    MasterCustomerLocation_Guid = Companylocation,
                                    MasterSite_Guid = siteGuid,
                                    FlagNonBillable = deliveryLeg.FlagNonBillable.GetValueOrDefault()
                                };
                            }
                        }

                        ////////  Only two legs from First and Last are different, write codes here
                        if (i <= 2) //2 legs from first
                        {
                            insertStopLeg.MasterRouteGroupDetail_Guid = pickupLeg.RouteGroupDetailGuid;
                            insertStopLeg.ServiceStopTransectionDate = WorkDate_leg_P;
                            insertStopLeg.WindowsTimeServiceTimeStart = WorkDate_Time_P;
                            insertStopLeg.WindowsTimeServiceTimeStop = WorkDate_Time_P;
                            insertStopLeg.MasterRunResourceDaily_Guid = pickupLeg.RunResourceGuid;
                        }
                        if (i >= maxLeg - 1)
                        {
                            insertStopLeg.MasterRouteGroupDetail_Guid = deliveryLeg.RouteGroupDetailGuid;
                            insertStopLeg.ServiceStopTransectionDate = WorkDate_leg_D;
                            insertStopLeg.WindowsTimeServiceTimeStart = WorkDate_Time_D;
                            insertStopLeg.WindowsTimeServiceTimeStop = WorkDate_Time_D;
                            insertStopLeg.MasterRunResourceDaily_Guid = deliveryLeg.RunResourceGuid;
                        }
                        else //not 2 legs from first and last
                        {
                            //insertStopLeg.MasterRouteGroupDetail_Guid = pickupLeg.RouteGroupDetailGuid
                            insertStopLeg.ServiceStopTransectionDate = WorkDate_leg_P;
                            insertStopLeg.WindowsTimeServiceTimeStart = WorkDate_Time_P;
                            insertStopLeg.WindowsTimeServiceTimeStop = WorkDate_Time_P;
                            //insertStopLeg.MasterRunResourceDaily_Guid = pickupLeg.RunResourceGuid
                        }

                        ////////  All legs have same input data, write codes here

                        insertStopLeg.Guid = legGuid;
                        insertStopLeg.MasterActualJobHeader_Guid = insertHeader.Guid;
                        insertStopLeg.SequenceStop = i;
                        insertStopLeg.ArrivalTime = null;
                        insertStopLeg.ActualTime = null;
                        insertStopLeg.DepartTime = null;
                        insertStopLeg.FlagCancelLeg = false;

                        if (insertStop != null)
                            _masterActualJobServiceStopsRepository.Create(insertStop);
                        _masterActualJobServiceStopLegsRepository.Create(insertStopLeg);
                        #endregion Input TblMasterActualJobServiceStopLegs

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
                    #endregion //End Insert ServiceStopLegs

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
                #endregion //End ## Update JobOrder and Push to dolphin.

                #region ## Create OTC
                //=> TFS#53385:Ability to generate the OTC codes for both legs of Transfer job -> CreateJobTransferVaultMultiBranch [NEW]
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


        #endregion Job TV

        #region Job P
        public CreateJobAdHocResponse CreateJobPickUpMultiBranch(CreateJobAdHocRequest request)
        {
            try
            {
                DateTime? workDate;
                AdhocJobHeaderRequest headDetail = request.AdhocJobHeaderView;
                AdhocLegRequest pickupLeg = request.ServiceStopLegPickup;
                AdhocLegRequest deliveryLeg = request.ServiceStopLegDelivery;
                AdhocTempData tempdata = new AdhocTempData(request);
                var siteobj = _masterSiteRepository.FindById(pickupLeg.BrinkSiteGuid);
                bool isMCS = request.AdhocJobHeaderView.ServiceJobTypeID == IntTypeJob.MCS;

                headDetail.JobNo = tempdata.JobNo = GenerateJobNo(pickupLeg.BrinkSiteGuid);
                headDetail.JobGuid = tempdata.JobGuid = Guid.NewGuid();

                using (var transcope = _uow.BeginTransaction())
                {
                    /*Internal ID 1 : Service Stop*/
                    Guid serviceStopTypeGuid = _systemServiceStopTypesRepository.GetServiceStopTypeByID(1).Guid;

                    #region Get Site code   
                    //these are Locations of Brink's companies. 
                    var tblsite_pk = _masterSiteRepository.FindById(pickupLeg.BrinkSiteGuid);
                    pickupLeg.BrinkSiteCode = tblsite_pk.SiteCode;  //No need to use BrinkSiteCode from Leg D
                    if (tblsite_pk == null)
                    {
                        //-1 : Data not found
                        var tblmsg = _systemMessageRepository.FindByMsgId(-1, request.LanguagueGuid);
                        return new CreateJobAdHocResponse(tblmsg);
                    }


                    #endregion End Get Site code

                    DateTimeOffset date = request.UniversalDatetime;
                    workDate = pickupLeg.StrWorkDate_Date.ChangeFromStringToDate(request.DateTimeFormat);
                    int? StatusJob = IntStatusJob.Open;
                    if (pickupLeg.RunResourceGuid != null)
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


                        tempdata.ValidateMaxNumberJobs = ValidateMaxNumberJobs(1, pickupLeg.RunResourceGuid.Value, workDate.Value, request.LanguagueGuid);
                        tempdata.IsUseDolphin_LegP = _systemEnvironmentMasterCountryRepository.IsUseDolphin(pickupLeg.BrinkSiteGuid, pickupLeg.RunResourceGuid.Value);
                    }
                    tempdata.getMsgId_ForAdhocJob_LegP();

                    bool isAutoFillContract = getCountryConfigToCheckAutoFillContract(siteobj.MasterCountry_Guid);
                    Guid? ContractGuid = null;
                    if (!isAutoFillContract)  //if not autofill, it means "Validated from FrontEnd" or "Don't use Contract"
                    {
                        ContractGuid = headDetail.ContractGuid; //if headDetail.ContractGuid is null, it should be "Don't use Contract"
                    }

                    if (isAutoFillContract && pickupLeg.LocationGuid != null) //if autofill, user can choose any locations, so we seek from P, if cannot seek from P then seek from D
                    {
                        ContractGuid = GetContractGuid(pickupLeg.LocationGuid.Value, headDetail.LineOfBusiness_Guid, headDetail.ServiceJobTypeGuid, headDetail.SubServiceTypeJobTypeGuid, workDate.Value);
                    }

                    #region Insert JobHeader
                    var insertHeader = new TblMasterActualJobHeader
                    {
                        Guid = headDetail.JobGuid.GetValueOrDefault(),
                        MasterRouteJobHeader_Guid = null,
                        JobNo = headDetail.JobNo,
                        SystemStopType_Guid = serviceStopTypeGuid,
                        SystemLineOfBusiness_Guid = headDetail.LineOfBusiness_Guid,
                        SystemServiceJobType_Guid = headDetail.ServiceJobTypeGuid,
                        SaidToContain = headDetail.SaidToContain == null ? 0 : headDetail.SaidToContain,
                        MasterCurrency_Guid = headDetail.CurrencyGuid,
                        Remarks = headDetail.Remarks,
                        DayInVault = 0,
                        InformTime = !string.IsNullOrEmpty(headDetail.strInformTime) ? headDetail.strInformTime.ToTimeDateTime() : "00:00".ToTimeDateTime(),
                        SystemStatusJobID = StatusJob,
                        TransectionDate = workDate,
                        FlagJobProcessDone = false,
                        OnwardDestinationType = deliveryLeg.OnwardTypeID,
                        OnwardDestination_Guid = deliveryLeg.OnwardDestGuid,
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
                        FlagJobInterBranch = false,
                        FlagChkOutInterBranchComplete = false,
                        SFOFlagRequiredTechnician = false,
                        FlagUpdateJobManual = false,
                        FlagRequireOpenLock = false,
                        FlagMultiBranch = true,
                        FlagJobSFO = headDetail.FlagJobSFO,  /*added: 2018-01-31 for create job from SFO*/
                        MasterSitePathHeader_Guid = headDetail.SitePathGuid,
                        MasterCustomerContract_Guid = ContractGuid   /*added: 2019/08/27 -> TFS#35983 */
                    };
                    _masterActualJobHeaderRepository.Create(insertHeader);
                    #endregion //End Insert JobHeader

                    #region Insert SpecialCommand
                    var serviceStopGuidPK = Guid.NewGuid(); //needs to use in Special Command and TblMasterActualJobServiceStop
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
                    #endregion //End Insert SpecialCommand

                    #region Insert SeviceStopLegs
                    var WorkDate_Time = !string.IsNullOrEmpty(pickupLeg.StrWorkDate_Time) ? pickupLeg.StrWorkDate_Time.ToTimeDateTime() : "00:00".ToTimeDateTime();

                    #region Action Job
                    var actionJob = _systemJobActionsRepository.FindAll();
                    Guid actionGuidPick = actionJob.FirstOrDefault(o => o.ActionNameAbbrevaition == JobActionAbb.StrPickUp).Guid;
                    Guid actionGuidDel = actionJob.FirstOrDefault(o => o.ActionNameAbbrevaition == JobActionAbb.StrDelivery).Guid;
                    #endregion
                    var maxStop = MaxJobOrderOnDailyRun(pickupLeg.RunResourceGuid) + 1;
                    Guid[] allSitePathDetails = _masterSitePathRepository.getShortInfoSitePathDetail(headDetail.SitePathGuid).Select(o => o.SiteGuid).ToArray();
                    int maxLeg = allSitePathDetails.Length * 2;
                    for (int i = 1; i <= maxLeg; i++)
                    {
                        TblMasterActualJobServiceStop insertStop = null;
                        TblMasterActualJobServiceStopLegs insertStopLeg = null;

                        ////////  Seperated by First, Last, Even and Odd write codes here
                        if (i == 1)
                        {
                            var branchCode = _masterCustomerLocationRepository.FindById(pickupLeg.LocationGuid).BranchCodeReference;
                            PrintedReceiptNumberRequest itemreceipt = new PrintedReceiptNumberRequest()
                            {
                                CustomerLocation_Guid = pickupLeg.LocationGuid.Value,
                                SequenceStop = 1,
                                ServiceStopTransectionDate = workDate,
                                BranchCodeReference = branchCode,
                                SiteCode = pickupLeg.BrinkSiteCode,
                                JobNo = headDetail.JobNo
                            };
                            var ReceiptNo = GetPrintedReceiptNumber(itemreceipt);
                            insertStopLeg = new TblMasterActualJobServiceStopLegs
                            {
                                Guid = Guid.NewGuid(),
                                CustomerLocationAction_Guid = actionGuidPick,                       //odd legs = actionGuidPick, even legs = actionGuidDel
                                MasterCustomerLocation_Guid = pickupLeg.LocationGuid,               //1st leg is CustomerLocation_Guid, Others are BrinkLocation_Guid depends on SitePaths
                                PrintedReceiptNumber = ReceiptNo,                                   //Only 1st leg has value
                                MasterSite_Guid = pickupLeg.BrinkSiteGuid,
                                FlagNonBillable = pickupLeg.FlagNonBillable.GetValueOrDefault(),     //Only 1st leg that's different from others      
                                JobOrder = maxStop,
                                SeqIndex = maxStop,
                            };
                            request.ServiceStopLegPickup.LegGuid = insertStopLeg.Guid;              //Only 1st leg that's still in use.
                            insertStop = new TblMasterActualJobServiceStop          //similar as TblMasterActualJobServiceStopLegs. Used by Dolphin
                            {
                                Guid = serviceStopGuidPK,                                           //Only 1st leg that's same as Special Command
                                MasterActualJobHeader_Guid = headDetail.JobGuid.GetValueOrDefault(),
                                CustomerLocationAction_Guid = actionGuidPick,
                                MasterCustomerLocation_Guid = pickupLeg.LocationGuid,
                                SequenceOrder = 1
                            };
                        }
                        else if (i == maxLeg) //Last Leg
                        {
                            var Companylocation_LegDest = _masterCustomerLocationRepository.GetCompanyGuidBySite(deliveryLeg.BrinkSiteGuid);
                            insertStopLeg = new TblMasterActualJobServiceStopLegs
                            {
                                Guid = Guid.NewGuid(),
                                CustomerLocationAction_Guid = actionGuidDel,
                                MasterCustomerLocation_Guid = Companylocation_LegDest,
                                MasterSite_Guid = deliveryLeg.BrinkSiteGuid,
                                FlagNonBillable = deliveryLeg.FlagNonBillable.GetValueOrDefault(),
                                FlagDestination = true                                              //Only last leg that must be true
                            };
                            insertStop = new TblMasterActualJobServiceStop          //similar as TblMasterActualJobServiceStopLegs. Used by Dolphin
                            {
                                Guid = Guid.NewGuid(),
                                MasterActualJobHeader_Guid = headDetail.JobGuid.GetValueOrDefault(),
                                CustomerLocationAction_Guid = actionGuidDel,
                                MasterCustomerLocation_Guid = Companylocation_LegDest,
                                SequenceOrder = maxLeg
                            };
                        }
                        else  //Not First and Last Legs
                        {
                            int siteIndex = (i / 2) - 1; //Site of leg 2 and 3 will get same siteIndex, also (leg 4 == leg 5) && (leg 6 == leg 7) && ...
                            Guid siteGuid = allSitePathDetails[siteIndex];
                            var Companylocation = _masterCustomerLocationRepository.GetCompanyGuidBySite(siteGuid);
                            if (i % 2 == 1)  //Odd Numbers  (3, 5, 7, 9, ...)
                            {
                                insertStopLeg = new TblMasterActualJobServiceStopLegs
                                {
                                    Guid = Guid.NewGuid(),
                                    CustomerLocationAction_Guid = actionGuidPick,
                                    MasterCustomerLocation_Guid = Companylocation,
                                    MasterSite_Guid = siteGuid,
                                    FlagNonBillable = pickupLeg.FlagNonBillable.GetValueOrDefault()
                                };
                            }
                            else //Even Numbers (2, 4, 6, 8, ...) 
                            {
                                insertStopLeg = new TblMasterActualJobServiceStopLegs
                                {
                                    Guid = Guid.NewGuid(),
                                    CustomerLocationAction_Guid = actionGuidDel,
                                    MasterCustomerLocation_Guid = Companylocation,
                                    MasterSite_Guid = siteGuid,
                                    FlagNonBillable = deliveryLeg.FlagNonBillable.GetValueOrDefault()
                                };
                            }
                        }

                        //////// Only 1st and 2nd legs that have values, write codes here (if not 1st and 2nd, will be null by default)
                        if (i == 1 || i == 2)
                        {
                            insertStopLeg.MasterRouteGroupDetail_Guid = pickupLeg.RouteGroupDetailGuid;
                            insertStopLeg.MasterRunResourceDaily_Guid = pickupLeg.RunResourceGuid;
                            insertStopLeg.UnassignedBy = request.UserName;
                            insertStopLeg.UnassignedDatetime = request.ClientDateTime.DateTime;
                        }

                        ////////  All legs have same input data, write codes here
                        insertStopLeg.MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault();
                        insertStopLeg.SequenceStop = i;
                        insertStopLeg.ServiceStopTransectionDate = workDate;
                        insertStopLeg.WindowsTimeServiceTimeStart = WorkDate_Time;
                        insertStopLeg.WindowsTimeServiceTimeStop = WorkDate_Time;


                        insertStopLeg.ArrivalTime = null;
                        insertStopLeg.ActualTime = null;
                        insertStopLeg.DepartTime = null;
                        insertStopLeg.FlagCancelLeg = false;
                        if (pickupLeg.RunResourceGuid.HasValue)
                        {
                            insertStopLeg.UnassignedBy = request.UserName;
                            insertStopLeg.UnassignedDatetime = request.ClientDateTime.DateTime;
                        }

                        if (insertStop != null)
                            _masterActualJobServiceStopsRepository.Create(insertStop);
                        _masterActualJobServiceStopLegsRepository.Create(insertStopLeg);

                        #region Insert History Run 
                        if (pickupLeg.RunResourceGuid != null)
                        {
                            var insertHistoryRun = new TblMasterHistory_ActaulJobOnDailyRunResource
                            {
                                Guid = Guid.NewGuid(),
                                MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                                MasterRunResourceDaily_Guid = pickupLeg.RunResourceGuid,
                                DateTimeInterChange = date,
                                MasterActualJobServiceStopLegs_Guid = insertStopLeg.Guid // Ple add 1/8/2016 for use in dolphin
                            };
                            _masterHistoryActaulJobOnDailyRunResourceRepository.Create(insertHistoryRun);

                        }
                        #endregion End Insert History Run

                    } // End loop for i = 1 to Maxlegs

                    // 13) Insert History           
                    var insertHistory = new TblMasterHistory_ActualJob
                    {
                        Guid = Guid.NewGuid(),
                        MasterActualJobHeader_Guid = insertHeader.Guid, //headDetail.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                        MsgID = getMessageID(headDetail.FlagJobSFO), //Created job no. {0} date {1} from Adhoc Job by Ocean Online MVC.
                        MsgParameter = new string[] { headDetail.JobNo, pickupLeg.StrWorkDate_Date }.ToJSONString(),
                        UserCreated = request.UserName,
                        DatetimeCreated = request.ClientDateTime.DateTime,
                        UniversalDatetimeCreated = request.UniversalDatetime
                    };
                    _masterHistoryActualJobRepository.Create(insertHistory);
                    #endregion //End Insert SeviceStopLegs

                    #region ## Check and insert sevice type Cash add 
                    if (isMCS)
                    {
                        CreateATMTransaction(request, false);
                    }
                    #endregion

                    _uow.Commit();
                    transcope.Complete();
                } //End Transaction scope
                #region ## Update JobOrder and Push to dolphin.               
                if (!request.ServiceStopLegPickup.RunResourceGuid.IsNullOrEmpty())
                {
                    UpdateJobOrderInRunRequest updatJobOrder = new UpdateJobOrderInRunRequest()
                    {
                        ClientDateTime = request.ClientDateTime.DateTime,
                        LanguageGuid = request.LanguagueGuid,
                        UserModified = request.UserName,
                        WorkDate = workDate,
                        SiteGuid = request.ServiceStopLegPickup.BrinkSiteGuid,
                        FlagReorder = false,
                        RunDailyGuid = request.ServiceStopLegPickup.RunResourceGuid.GetValueOrDefault()
                    };
                    UpdateJobOrderInRun(updatJobOrder);
                    if (!request.AdhocJobHeaderView.FlagPageFromMapping) //From Non-Mapping Page
                    {
                        updatJobOrder.JobHeadGuidList = new List<Guid>() { headDetail.JobGuid.GetValueOrDefault() };
                        UpdatePushToDolPhinWhenCreateJob(updatJobOrder);
                    }
                }
                #endregion

                #region ## Create OTC
                //=> TFS#53385:Ability to generate the OTC codes for both legs of Transfer job -> CreateJobPickUpMultiBranch [NEW]
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


        #endregion //Job P

        public DetailDestinationForMultibranchReponseJobP GetDetailDestinationForDelivery_MultiBranchJobP(Guid? siteGuid, Guid? locationGuid)
        {
            DetailDestinationForMultibranchReponseJobP response = new DetailDestinationForMultibranchReponseJobP();


            Guid? SiteDelivery = null;
            var data = new List<ModelDestinationResponse>();

            var query = (from dest in _masterCustomerLocation_LocationDestinationRepository.FindAllAsQueryable()
                         join depart in _masterCustomerLocationInternalDepartmentRepository.FindAllAsQueryable() on dest.MasterCustomerLocation_InternalDepartment_Guid equals depart.Guid
                         join loc in _masterCustomerLocationRepository.FindAllAsQueryable() on depart.MasterCustomerLocation_Guid equals loc.Guid
                         join sites in _masterSiteRepository.FindAllAsQueryable() on loc.MasterSite_Guid equals sites.Guid
                         join departtype in _systemInternalDepartmentTypesRepository.FindAllAsQueryable() on depart.InternalDepartmentType equals departtype.Guid

                         where dest.MasterCustomerLocation_Guid == locationGuid && !loc.FlagDisable && !depart.FlagDisable && (departtype.InternalDepartmentID == 2 || departtype.InternalDepartmentID == 3) &&
                             loc.MasterSite_Guid != siteGuid
                         select new ModelDestination
                         {
                             SiteGuid = sites.Guid,
                             SiteName = sites.SiteCode + " - " + sites.SiteName,
                             InternalDepartmentGuid = depart.Guid,
                             InterDepartmentName = depart.InterDepartmentName
                         }
                    ).Distinct();
            bool oneSite = query.Select(o => o.SiteGuid).Distinct().Count() == 1;
            bool oneDestination = query.Count() == 1;

            if (oneSite)
            {
                response.SiteGuid = query.First().SiteGuid.ToString();
                response.SiteName = query.First().SiteName;
                SiteDelivery = query.First().SiteGuid;

                if (oneDestination)
                {
                    foreach (var item in query.ToList())
                    {
                        var resultItem = new ModelDestinationResponse { id = item.InternalDepartmentGuid, text = item.InterDepartmentName, onwardTypeId = 1, flagDefaultOnward = true };
                        data.Add(resultItem);
                    }
                }
                else //if more than 1 destination -> all flagDefaultOnward must be false
                {
                    foreach (var item in query.ToList())
                    {
                        var resultItem = new ModelDestinationResponse { id = item.InternalDepartmentGuid, text = item.InterDepartmentName, onwardTypeId = 1, flagDefaultOnward = false };
                        data.Add(resultItem);
                    }
                }

                var desc_internallist = query.Select(o => o.InternalDepartmentGuid).ToList();
                var onwardquery = (from loc in _masterCustomerLocationRepository.FindAllAsQueryable()
                                   join sites in _masterSiteRepository.FindAllAsQueryable() on loc.MasterSite_Guid equals sites.Guid
                                   join depart in _masterCustomerLocationInternalDepartmentRepository.FindAllAsQueryable() on loc.Guid equals depart.MasterCustomerLocation_Guid
                                   join departtype in _systemInternalDepartmentTypesRepository.FindAllAsQueryable() on depart.InternalDepartmentType equals departtype.Guid
                                   where sites.Guid == SiteDelivery && !loc.FlagDisable && !depart.FlagDisable && (departtype.InternalDepartmentID == 2 || departtype.InternalDepartmentID == 3) &&
                                   loc.MasterSite_Guid != siteGuid && !desc_internallist.Contains(depart.Guid)
                                   select new ModelDestination
                                   {
                                       SiteGuid = sites.Guid,
                                       SiteName = sites.SiteCode + " - " + sites.SiteName,
                                       InternalDepartmentGuid = depart.Guid,
                                       InterDepartmentName = depart.InterDepartmentName
                                   }
                                    );
                foreach (var item in onwardquery.ToList()) //other default onwards
                {
                    var resultItem = new ModelDestinationResponse { id = item.InternalDepartmentGuid, text = item.InterDepartmentName, onwardTypeId = 1, flagDefaultOnward = false };
                    data.Add(resultItem);
                }
            }
            else //many sites don't have List of ModelDestinationResponse, return new List<ModelDestinationResponse>()
            {
                response.SiteGuid = "";
                response.SiteName = "";
            }
            response.ComboDestination = data;
            return response;
        }

        public MultiBrDetailResponse GetAdhocMultiBrDeliveryDetailByOriginLocation(GetMultiBrDestinationDetailRequest req)
        {
            MultiBrDetailResponse result = new MultiBrDetailResponse();
            result.LocationDestination = _masterCustomerLocation_LocationDestinationRepository.GetAdhocMultiBrDeliveryDetailByOriginLocation(req.OriginCustomerLocation_Guid, req.OriginSite_Guid, req.SystemServiceJobType_Guid);
            return result;
        }


        public GetAdhocAllCustomerAndLocationResponse GetAdhocAllCustomerBySite(GetAdhocAllCustomerAndLocationRequest req)
        {
            GetAdhocAllCustomerAndLocationResponse result = new GetAdhocAllCustomerAndLocationResponse();
            result.LocationDestination = _masterCustomerLocation_LocationDestinationRepository.GetAdhocAllCustomerBySite(req.Site_Guid, req.OriginSite_Guid);
            return result;
        }

        public GetAdhocAllCustomerAndLocationResponse GetAdhocAllLocationByCustomer(GetAdhocAllCustomerAndLocationRequest req)
        {
            GetAdhocAllCustomerAndLocationResponse result = new GetAdhocAllCustomerAndLocationResponse();
            result.LocationDestination = _masterCustomerLocation_LocationDestinationRepository.GetAdhocAllLocationByCustomer(req.Site_Guid, req.MasterCustomer_Guid, req.OriginSite_Guid);
            return result;
        }
    }
}
