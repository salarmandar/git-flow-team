using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RouteOptimization;
using Bgt.Ocean.Service.Messagings.AdhocService;
using Bgt.Ocean.Service.Messagings.RunControlService;
using Bgt.Ocean.Service.ModelViews.ActualJobHeader;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Helpers.SystemHelper;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using static Bgt.Ocean.Infrastructure.Util.EnumRun;

namespace Bgt.Ocean.Service.Implementations.Adhoc
{
    public partial class AdhocService : IAdhocService
    {

        public CreateJobAdHocResponse CreateJobPickUp(CreateJobAdHocRequest request)
        {
            try
            {
                #region Variables
                AdhocJobHeaderRequest headDetail = request.AdhocJobHeaderView;
                AdhocLegRequest pickupLeg = request.ServiceStopLegPickup;
                AdhocLegRequest deliveryLeg = request.ServiceStopLegDelivery;
                SystemMessageView MsgViewValidMaxJobs = null;
                List<AdhocJob_Info> arr_JobInfo = new List<AdhocJob_Info>();
                var siteobj = _masterSiteRepository.FindById(pickupLeg.BrinkSiteGuid);

                int statusJob = IntStatusJob.Open;
                int statusRunID = 0; //no status
                Guid serviceStopTypeGuid = _systemServiceStopTypesRepository.GetServiceStopTypeByID(1).Guid; /*Internal ID -> 1 : Service Stop, 2 : Crew break, 3 : Inter Branch */
                Guid Companylocation_LegP = _masterCustomerLocationRepository.GetCompanyGuidBySite(pickupLeg.BrinkSiteGuid);
                Guid Companylocation_LegD = _masterCustomerLocationRepository.GetCompanyGuidBySite(deliveryLeg.BrinkSiteGuid);
                DateTime? workDate = pickupLeg.StrWorkDate_Date.ChangeFromStringToDate(request.DateTimeFormat);
                bool isUseDolphin_LegP = false;
                bool isMCS = request.AdhocJobHeaderView.ServiceJobTypeID == IntTypeJob.MCS;
                #endregion //End Variables

                #region Initial data, Updating Status and return error
                //Check if no Site Guid
                var tblsite_pk = _masterSiteRepository.FindById(pickupLeg.BrinkSiteGuid);
                if (tblsite_pk == null || workDate == null)
                {
                    return CreateResponseFromID(-1, request.LanguagueGuid); //-1 : Data not found
                }
                pickupLeg.BrinkSiteCode = tblsite_pk.SiteCode;  //Updated Site Code at Leg P, No need to use BrinkSiteCode from Leg D
                var maxStop = MaxJobOrderOnDailyRun(pickupLeg.RunResourceGuid);
                if (pickupLeg.arr_LocationGuid == null)
                {
                    var jobInfo = new AdhocJob_Info()
                    {
                        LocationGuid = pickupLeg.LocationGuid.Value, //support old version of CreateJobPickup
                        JobNo = GenerateJobNo(pickupLeg.BrinkSiteGuid),
                        JobGuid = Guid.NewGuid(),
                        LocationSeq = 1 + maxStop,
                        UnassignedBy = request.UserName,
                        UnassignedDate = request.ClientDateTime.DateTime
                    };
                    arr_JobInfo.Add(jobInfo);
                }
                else
                {
                    var multiLoc = MultiLocation(
                        new CreateMultiJobRequest
                        {
                            MasterCustomerLocationGuids = pickupLeg.arr_LocationGuid.Select(s => s.Value),
                            BrinksSiteGuid = pickupLeg.BrinkSiteGuid,
                            MaxStop = maxStop,
                            IsCreateToRun = pickupLeg.RunResourceGuid.HasValue,
                            UnassignedBy = request.UserName,
                            UnassignedDate = request.ClientDateTime.DateTime
                        });
                    arr_JobInfo.AddRange(multiLoc);
                }

                //Check from Run
                if (pickupLeg.RunResourceGuid != null)  /*updated: 2019/08/27 -> TFS#35983 */
                {
                    #region Check Roadnet
                    // get country option : RouteOptimizationDirectoryPath for check freezing        
                    
                    var CountryGuid = _masterSiteRepository.FindById(request.AdhocJobHeaderView.BrinkSiteGuid).MasterCountry_Guid;                    
                    var RoadNetPath = _systemEnvironmentMasterCountryValueRepository.GetSpecificKeyByCountryAndKey( CountryGuid, EnumAppKey.RouteOptimizationDirectoryPath.ToString())?.AppValue1;
                    if (!string.IsNullOrEmpty(RoadNetPath))
                    {
                        var checkRoadnet = _transactionRouteOptimizationHeaderRepository.GetRouteGroupHasOptimize(new RouteOptimizeSearchModel() { RunResourceGuid = new List<Guid> { pickupLeg.RunResourceGuid.Value } });
                        if (checkRoadnet.FlaglockProcess)
                        {                           
                            var messageGet = CreateResponseFromID(-17352, request.LanguagueGuid);
                            messageGet.MessageTextContent = string.Format(messageGet.MessageTextContent, checkRoadnet.returnMessage);
                            return messageGet;
                        }
                    }
                    #endregion

                    var dailyRun = _masterDailyRunResourceRepository.FindById(pickupLeg.RunResourceGuid);
                    if (dailyRun == null)
                    {
                        return CreateResponseFromID(-1, request.LanguagueGuid); //-1 : Data not found
                    }
                    statusRunID = dailyRun.RunResourceDailyStatusID.GetValueOrDefault();

                    //if Run's status is Close Run or Crew Break -> return and show -761
                    if (statusRunID != StatusDailyRun.ReadyRun && statusRunID != StatusDailyRun.DispatchRun)
                    {
                        return CreateResponseFromID(-761, request.LanguagueGuid); //-761: The selected Run Resource No is not in the status that allow to be assigned. Please recheck.
                    }

                    //if This run is already connected from Master Route, we need to break this run and add Run history.
                    DisconnectMasterRouteAndAddRunHistory(dailyRun, request, _masterDailyRunResourceRepository, _masterHistory_DailyRunResource);


                    //Update Status Job From Status Run (Ready/Dispatch) (every jobs from multi selected must be the same)
                    statusJob = (statusRunID == StatusDailyRun.ReadyRun) ? IntStatusJob.OnTruck : IntStatusJob.OnTheWay;

                    //Warn User if there are too many jobs in this Run. (But User can continue)
                    MsgViewValidMaxJobs = ValidateMaxNumberJobs(arr_JobInfo.Count, pickupLeg.RunResourceGuid.Value, workDate.Value, request.LanguagueGuid);
                    isUseDolphin_LegP = _systemEnvironmentMasterCountryRepository.IsUseDolphin(pickupLeg.BrinkSiteGuid, pickupLeg.RunResourceGuid.Value);
                } //End Check from Run

                CreateJobAdHocResponse response = null;
                if (arr_JobInfo.Count == 1)
                {
                    response = getMessageIDFromRouteAndRun(request.ServiceStopLegPickup.RouteName,
                                                                                request.ServiceStopLegPickup.RunResourceName,
                                                                                arr_JobInfo[0].JobNo,
                                                                                request.LanguagueGuid);
                }
                else //if there are more than 1 job, show Message ID : 0
                {
                    response = CreateResponseFromID(0, request.LanguagueGuid);
                }

                response.ValidateMaxNumberJobs = MsgViewValidMaxJobs;                                         //if no Run, this value should be null.
                string strAllJobNo = string.Join(" / ", arr_JobInfo.Select(o => o.JobNo).ToArray());
                response.JobGuid_list = string.Join(" / ", arr_JobInfo.Select(o => o.JobGuid).ToArray());     //to make FrontEnd can debug
                response.JobNo_list = strAllJobNo;
                if (statusRunID == StatusDailyRun.DispatchRun)
                {
                    response.FlagAllowChangeRoute = isUseDolphin_LegP;
                }
                else //ReadyRun
                {
                    response.FlagAllowChangeRoute = false;
                }
                #endregion //End Initial data, Updating Status and return error

                #region Insert to Tables
                using (var transcope = _uow.BeginTransaction())
                {
                    bool isAutoFillContract = getCountryConfigToCheckAutoFillContract(siteobj.MasterCountry_Guid);
                    Guid? ContractGuid = null;
                    if (!isAutoFillContract)  //if not autofill, it means "Validated from FrontEnd" or "Don't use Contract"
                    {
                        ContractGuid = headDetail.ContractGuid; //if headDetail.ContractGuid is null, it should be "Don't use Contract"
                    }

                    List<TblMasterActualJobServiceStopLegs> legs = new List<TblMasterActualJobServiceStopLegs>();
                    foreach (var jobInfo in arr_JobInfo) //one jobInfo is per one location
                    {
                        if (isAutoFillContract) //if autofill, user can choose any locations and it doesn't need to be the same Contracts
                        {
                            ContractGuid = GetContractGuid(jobInfo.LocationGuid, headDetail.LineOfBusiness_Guid, headDetail.ServiceJobTypeGuid, headDetail.SubServiceTypeJobTypeGuid, workDate.Value);
                        } // if "Validated from FrontEnd", it must be the same Contracts of every locations

                        #region Get Printed Receipt
                        var branchCode = _masterCustomerLocationRepository.FindById(jobInfo.LocationGuid).BranchCodeReference;
                        PrintedReceiptNumberRequest itemreceipt = new PrintedReceiptNumberRequest()
                        {
                            CustomerLocation_Guid = jobInfo.LocationGuid,
                            SequenceStop = 1,
                            ServiceStopTransectionDate = workDate,
                            BranchCodeReference = branchCode,
                            SiteCode = pickupLeg.BrinkSiteCode,
                            JobNo = jobInfo.JobNo,
                        };
                        var ReceiptNo = GetPrintedReceiptNumber(itemreceipt);

                        #endregion ///End Get Printed Receipt

                        #region Insert JobHeader
                        request.AdhocJobHeaderView.JobGuid = jobInfo.JobGuid; //still need to use for Cash Added
                        var insertHeader = new TblMasterActualJobHeader
                        {
                            Guid = jobInfo.JobGuid,
                            MasterRouteJobHeader_Guid = null,
                            JobNo = jobInfo.JobNo,
                            SystemStopType_Guid = serviceStopTypeGuid,
                            SystemLineOfBusiness_Guid = headDetail.LineOfBusiness_Guid,
                            SystemServiceJobType_Guid = headDetail.ServiceJobTypeGuid,
                            SaidToContain = headDetail.SaidToContain == null ? 0 : headDetail.SaidToContain,
                            MasterCurrency_Guid = headDetail.CurrencyGuid,
                            Remarks = headDetail.Remarks,
                            DayInVault = 0,
                            InformTime = !string.IsNullOrEmpty(headDetail.strInformTime) ? headDetail.strInformTime.ToTimeDateTime() : "00:00".ToTimeDateTime(),
                            SystemStatusJobID = statusJob,  //Changed depend on Run
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
                            FlagJobInterBranch = headDetail.FlagJobInterBranch,
                            FlagChkOutInterBranchComplete = false,
                            SFOFlagRequiredTechnician = headDetail.SFOFlagRequiredTechnician,
                            FlagUpdateJobManual = false,
                            FlagRequireOpenLock = headDetail.FlagRequireOpenLock,
                            SFOMaxWorkingTime = headDetail.SFOMaxWorkingTime,
                            SFOTechnicianName = headDetail.SFOTechnicianName,
                            SFOTechnicianID = headDetail.SFOTechnicianID,
                            TicketNumber = headDetail.TicketNumber,
                            SFOMaxTechnicianWaitingTime = headDetail.SFOMaxTechnicianWaitingTime,
                            FlagJobSFO = headDetail.FlagJobSFO,  /*added: 2018-01-31 for create job from SFO*/
                            MasterCustomerContract_Guid = ContractGuid,  /*added: 2019/08/27 -> TFS#35983 */
                            CITDeliveryStatus = CheckCITDeliveryStatus(request)
                        };
                        _masterActualJobHeaderRepository.Create(insertHeader);
                        #endregion ///End Insert JobHeader

                        #region Insert TblJobServiceStop
                        var actionJob = _systemJobActionsRepository.FindAll();
                        Guid actionGuidPick = actionJob.FirstOrDefault(o => o.ActionNameAbbrevaition == JobActionAbb.StrPickUp).Guid;
                        Guid actionGuidDel = actionJob.FirstOrDefault(o => o.ActionNameAbbrevaition == JobActionAbb.StrDelivery).Guid;

                        var serviceStopGuidPK = Guid.NewGuid();
                        var serviceStopGuidDEL = Guid.NewGuid();
                        var insertJobServiceStopPK = new TblMasterActualJobServiceStop
                        {
                            Guid = serviceStopGuidPK,
                            MasterActualJobHeader_Guid = jobInfo.JobGuid,
                            CustomerLocationAction_Guid = actionGuidPick,
                            MasterCustomerLocation_Guid = jobInfo.LocationGuid
                        };
                        _masterActualJobServiceStopsRepository.Create(insertJobServiceStopPK);

                        var insertJobServiceStopDEL = new TblMasterActualJobServiceStop
                        {
                            Guid = serviceStopGuidDEL,
                            MasterActualJobHeader_Guid = jobInfo.JobGuid,
                            CustomerLocationAction_Guid = actionGuidDel,
                            MasterCustomerLocation_Guid = Companylocation_LegD
                        };
                        _masterActualJobServiceStopsRepository.Create(insertJobServiceStopDEL);
                        #endregion ///End Insert TblJobServiceStop

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
                        #endregion ///End Insert SpecialCommand

                        #region Insert ServiceStopLegs
                        var WorkDate_Time = !string.IsNullOrEmpty(pickupLeg.StrWorkDate_Time) ? pickupLeg.StrWorkDate_Time.ToTimeDateTime() : "00:00".ToTimeDateTime();
                        List<TblMasterHistory_ActaulJobOnDailyRunResource> historyRun = new List<TblMasterHistory_ActaulJobOnDailyRunResource>();
                        int j = request.AdhocJobHeaderView.FlagJobInterBranch ? 4 : 2;
                        for (int i = 1; i <= j; i++)
                        {
                            var insertStopLeg = new TblMasterActualJobServiceStopLegs
                            {
                                Guid = Guid.NewGuid(),
                                MasterActualJobHeader_Guid = jobInfo.JobGuid,
                                SequenceStop = i,
                                CustomerLocationAction_Guid = i == 1 || i == 3 ? actionGuidPick : actionGuidDel,
                                MasterCustomerLocation_Guid = i == 1 ? jobInfo.LocationGuid : (i == 2 ? Companylocation_LegP : Companylocation_LegD), //In case of interbranch, deliveryLeg.BrinkCompanyGuid should be company_guid of interbranch
                                MasterRouteGroupDetail_Guid = i == 1 || i == 2 ? pickupLeg.RouteGroupDetailGuid : null,
                                ServiceStopTransectionDate = workDate,
                                WindowsTimeServiceTimeStart = WorkDate_Time,
                                WindowsTimeServiceTimeStop = WorkDate_Time,
                                JobOrder = i == 1 ? jobInfo.LocationSeq : 0,
                                SeqIndex = i == 1 ? jobInfo.LocationSeq : 0,
                                MasterRunResourceDaily_Guid = i == 1 || i == 2 ? pickupLeg.RunResourceGuid : null,
                                PrintedReceiptNumber = i == 1 ? ReceiptNo : null,
                                MasterSite_Guid = i == 1 || i == 2 ? pickupLeg.BrinkSiteGuid : deliveryLeg.BrinkSiteGuid,
                                ArrivalTime = null,
                                ActualTime = null,
                                DepartTime = null,
                                FlagCancelLeg = false,
                                FlagNonBillable = i == 1 ? pickupLeg.FlagNonBillable.GetValueOrDefault() : deliveryLeg.FlagNonBillable.GetValueOrDefault(),
                                UnassignedBy = jobInfo.UnassignedBy,
                                UnassignedDatetime = jobInfo.UnassignedDate

                            };
                            if (i == 1) request.ServiceStopLegPickup.LegGuid = insertStopLeg.Guid;
                            if (i == j) insertStopLeg.FlagDestination = true;                                              //Only last leg that must be true
                            legs.Add(insertStopLeg);

                            #region Insert History Run 
                            if (pickupLeg.RunResourceGuid != null)
                            {
                                var insertHistoryRun = new TblMasterHistory_ActaulJobOnDailyRunResource
                                {
                                    Guid = Guid.NewGuid(),
                                    MasterActualJobHeader_Guid = jobInfo.JobGuid,
                                    MasterRunResourceDaily_Guid = pickupLeg.RunResourceGuid,
                                    DateTimeInterChange = request.UniversalDatetime,
                                    MasterActualJobServiceStopLegs_Guid = insertStopLeg.Guid // Ple add 1/8/2016 for use in dolphin
                                };
                                historyRun.Add(insertHistoryRun);

                            }
                            #endregion ///End Insert History Run

                        }

                        _masterHistoryActaulJobOnDailyRunResourceRepository.CreateRange(historyRun);
                        // 13) Insert History           
                        var insertHistory = new TblMasterHistory_ActualJob
                        {
                            Guid = Guid.NewGuid(),
                            MasterActualJobHeader_Guid = jobInfo.JobGuid,
                            MsgID = getMessageID(headDetail.FlagJobSFO), //Created job no. {0} date {1} from Adhoc Job by Ocean Online MVC.
                            MsgParameter = new string[] { jobInfo.JobNo, pickupLeg.StrWorkDate_Date }.ToJSONString(),
                            UserCreated = request.UserName,
                            DatetimeCreated = request.ClientDateTime.DateTime,
                            UniversalDatetimeCreated = request.UniversalDatetime
                        };
                        _masterHistoryActualJobRepository.Create(insertHistory);
                        #endregion ///End Insert ServiceStopLegs

                        #region ## Check and insert sevice type Cash add 
                        if (isMCS)
                        {
                            CreateATMTransaction(request, false);

                            #region ## Show Hide Screen Configuration
                            //TFS#41458
                            CreateJobHideScreenConfiguaration(request);
                            #endregion
                        }
                        #endregion ///End Check and insert sevice type Cash add 

                    } //End foreach(var jobInfo in arr_JobInfo)

                    #region ### Insert Stop
                    CreateAdhocAddSpot(request, legs);
                    _masterActualJobServiceStopLegsRepository.CreateRange(legs);
                    #endregion

                    _uow.Commit();
                    transcope.Complete();
                }          //End Transaction scope
                #endregion //End Insert to Tables

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
                        updatJobOrder.JobHeadGuidList.AddRange(arr_JobInfo.Select(o => o.JobGuid));
                        UpdatePushToDolPhinWhenCreateJob(updatJobOrder);
                    }
                }
                #endregion

                #region ## Create OTC
                //=> TFS#53385:Ability to generate the OTC codes for both legs of Transfer job -> CreateJobPickUp [NEW]
                _OTCRunControlService.CreateHeaderOtcByJobGuids(arr_JobInfo.Select(o => o.JobGuid).Distinct());
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                // OO error logger
                _systemService.CreateHistoryError(ex, SystemHelper.CurrentPageUri, SystemHelper.CurrentIpAddress, false);
                return CreateResponseFromID(-186, request.LanguagueGuid);
            }
        }
        private int? CheckCITDeliveryStatus(CreateJobAdHocRequest request)
        {
            return request.AdhocJobHeaderView.ServiceJobTypeID == IntTypeJob.MCS ? (int?)CITDeliveryStatus.Inprogress : default(int?);
        }
        private void CreateJobHideScreenConfiguaration(CreateJobAdHocRequest request)
        {
            var headDetail = request.AdhocJobHeaderView;
            var jobHideScreenList = _masterCustomerJobHideScreenRepository.GetJobHideScreenConfig(request.ServiceStopLegPickup.CustomerGuid, headDetail.LineOfBusiness_Guid, headDetail.ServiceJobTypeGuid, headDetail.SubServiceTypeJobTypeGuid);
            var jobHideScreenConfig = jobHideScreenList.Select(o => new TblMasterActualJobHideScreenMapping
            {
                Guid = Guid.NewGuid(),
                MasterActualJobHeader_Guid = request.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                SystemJobHideScreen_Guid = o.SystemJobHideScreen_Guid,
                UserCreated = request.UserName,
                DatetimeCreated = request.ClientDateTime.DateTime,
                UniversalDatetimeCreated = request.UniversalDatetime,
                SystemJobHideField_Guid = o.SystemJobHideField_Guid,
            });
            _masterActualJobHideScreenMappingRepository.CreateRange(jobHideScreenConfig);
        }
    }
}

