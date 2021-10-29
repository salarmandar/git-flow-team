using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Service.Messagings.AdhocService;
using Bgt.Ocean.Service.Messagings.RunControlService;
using Bgt.Ocean.Service.ModelViews.ActualJobHeader;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using static Bgt.Ocean.Infrastructure.Util.EnumRun;

namespace Bgt.Ocean.Service.Implementations.Adhoc
{
    public partial class AdhocService : IAdhocService
    {

        public CreateJobAdHocResponse CreateJobDelivery(CreateJobAdHocRequest request)
        {
            try
            {

                #region Variables
                AdhocJobHeaderRequest headDetail = request.AdhocJobHeaderView;
                AdhocLegRequest pickupLeg = request.ServiceStopLegPickup;
                AdhocLegRequest deliveryLeg = request.ServiceStopLegDelivery;
                SystemMessageView MsgViewValidMaxJobs = null;
                List<AdhocJob_Info> arr_JobInfo = new List<AdhocJob_Info>();
                var siteobj = _masterSiteRepository.FindById(deliveryLeg.BrinkSiteGuid);

                int statusJob = IntStatusJob.Open;
                int statusRunID = 0; //no status
                Guid serviceStopTypeGuid = _systemServiceStopTypesRepository.GetServiceStopTypeByID(1).Guid; /*Internal ID -> 1 : Service Stop, 2 : Crew break, 3 : Inter Branch */
                Guid Companylocation_LegP = _masterCustomerLocationRepository.GetCompanyGuidBySite(pickupLeg.BrinkSiteGuid);
                ////Guid Companylocation_LegD = _masterCustomerLocationRepository.GetCompanyGuidBySite(deliveryLeg.BrinkSiteGuid);
                DateTime? WorkDate_leg_D = deliveryLeg.StrWorkDate_Date.ChangeFromStringToDate(request.DateTimeFormat);
                bool isUseDolphin_LegD = false;
                #endregion //End Variables

                #region Initial data, Updating Status and return error
                //Check if no Site Guid
                var tblsite_del = _masterSiteRepository.FindById(deliveryLeg.BrinkSiteGuid);
                if (tblsite_del == null || WorkDate_leg_D == null)
                {
                    return CreateResponseFromID(-1, request.LanguagueGuid); //-1 : Data not found
                }
                deliveryLeg.BrinkSiteCode = tblsite_del.SiteCode; //JOB D can get from both Leg P and Leg D because Job D cannot be interbranch

                var maxStop = MaxJobOrderOnDailyRun(deliveryLeg.RunResourceGuid);
                if (deliveryLeg.arr_LocationGuid == null)
                {
                    var jobInfo = new AdhocJob_Info()
                    {
                        LocationGuid = deliveryLeg.LocationGuid.Value, //support old version of CreateJobPickup
                        JobNo = GenerateJobNo(deliveryLeg.BrinkSiteGuid),
                        JobGuid = Guid.NewGuid(),
                        LocationSeq = 1 + maxStop
                    };
                    arr_JobInfo.Add(jobInfo);
                }
                else
                {
                    var multiLoc = MultiLocation(
                        new CreateMultiJobRequest
                        {
                            MasterCustomerLocationGuids = deliveryLeg.arr_LocationGuid.Select(s => s.Value),
                            BrinksSiteGuid = deliveryLeg.BrinkSiteGuid,
                            MaxStop = maxStop,
                            IsCreateToRun = deliveryLeg.RunResourceGuid.HasValue,
                            UnassignedBy = request.UserName,
                            UnassignedDate = request.ClientDateTime.DateTime
                        });
                    arr_JobInfo.AddRange(multiLoc);

                }


                //Check from Run
                if (deliveryLeg.RunResourceGuid != null) /*updated: 2019/08/27 -> TFS#35983 */
                {
                    var dailyRun = _masterDailyRunResourceRepository.FindById(deliveryLeg.RunResourceGuid);
                    if (dailyRun == null)
                    {
                        return CreateResponseFromID(-1, request.LanguagueGuid); //-1 : Data not found
                    }
                    statusRunID = dailyRun.RunResourceDailyStatusID.GetValueOrDefault();

                    //if Run's status is Dispatch, Close Run, Crew Break -> show -761
                    if (statusRunID != StatusDailyRun.ReadyRun)
                    {
                        return CreateResponseFromID(-761, request.LanguagueGuid); //-761: The selected Run Resource No is not in the status that allow to be assigned. Please recheck.
                    }

                    //if This run is already connected from Master Route, we need to break this run and add Run history.
                    DisconnectMasterRouteAndAddRunHistory(dailyRun, request, _masterDailyRunResourceRepository, _masterHistory_DailyRunResource);


                    //Update Status Job  (every jobs from multi selected must be the same)
                    statusJob = IntStatusJob.Process;

                    //Warn User if there are too many jobs in this Run. (But User can continue)
                    MsgViewValidMaxJobs = ValidateMaxNumberJobs(arr_JobInfo.Count, deliveryLeg.RunResourceGuid.Value, WorkDate_leg_D.Value, request.LanguagueGuid);
                    isUseDolphin_LegD = _systemEnvironmentMasterCountryRepository.IsUseDolphin(deliveryLeg.BrinkSiteGuid, deliveryLeg.RunResourceGuid.Value);
                } //End Check from Run

                CreateJobAdHocResponse response = null;
                if (arr_JobInfo.Count == 1)
                {
                    response = getMessageIDFromRouteAndRun(request.ServiceStopLegDelivery.RouteName,
                                                                                request.ServiceStopLegDelivery.RunResourceName,
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
                    response.FlagAllowChangeRoute = isUseDolphin_LegD;
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
                    foreach (var jobInfo in arr_JobInfo)
                    {
                        if (isAutoFillContract) //if autofill, user can choose any locations and it doesn't need to be the same Contracts
                        {
                            ContractGuid = GetContractGuid(jobInfo.LocationGuid, headDetail.LineOfBusiness_Guid, headDetail.ServiceJobTypeGuid, headDetail.SubServiceTypeJobTypeGuid, WorkDate_leg_D.Value);
                        } // if "Validated from FrontEnd", it must be the same Contracts of every locations

                        #region Get Printed Receipt
                        var branchCode = _masterCustomerLocationRepository.FindById(jobInfo.LocationGuid).BranchCodeReference;
                        PrintedReceiptNumberRequest itemreceipt = new PrintedReceiptNumberRequest()
                        {
                            CustomerLocation_Guid = jobInfo.LocationGuid,
                            SequenceStop = 1,
                            ServiceStopTransectionDate = WorkDate_leg_D,
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
                            JobNo = jobInfo.JobNo,
                            SystemStopType_Guid = serviceStopTypeGuid,
                            SystemLineOfBusiness_Guid = headDetail.LineOfBusiness_Guid,
                            SystemServiceJobType_Guid = headDetail.ServiceJobTypeGuid,
                            SaidToContain = headDetail.SaidToContain,
                            MasterCurrency_Guid = headDetail.CurrencyGuid,
                            Remarks = headDetail.Remarks,
                            DayInVault = 0,
                            InformTime = !string.IsNullOrEmpty(headDetail.strInformTime) ? headDetail.strInformTime.ToTimeDateTime() : "00:00".ToTimeDateTime(),
                            OnwardDestinationType = pickupLeg.OnwardTypeID,
                            OnwardDestination_Guid = pickupLeg.OnwardDestGuid,
                            SystemTripIndicator_Guid = pickupLeg.TripIndicatorGuid,
                            SystemStatusJobID = statusJob,
                            TransectionDate = WorkDate_leg_D,
                            FlagJobProcessDone = false,
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
                            FlagTotalValuePerJob = false,
                            MasterSubServiceType_Guid = headDetail.SubServiceTypeJobTypeGuid,
                            FlagJobInterBranch = false, /*Job delivery type doesn't have interbrach*/
                            FlagChkOutInterBranchComplete = false,
                            SFOFlagRequiredTechnician = false,
                            FlagUpdateJobManual = false,
                            FlagRequireOpenLock = false,
                            FlagJobSFO = headDetail.FlagJobSFO,  /*added: 2018-01-31 for create job from SFO*/
                            MasterCustomerContract_Guid = ContractGuid   /*added: 2019/08/27 -> TFS#35983 */
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
                            MasterActualJobHeader_Guid = insertHeader.Guid,
                            CustomerLocationAction_Guid = actionGuidPick,
                            MasterCustomerLocation_Guid = Companylocation_LegP //Location of Brink's company at Leg P
                        };
                        _masterActualJobServiceStopsRepository.Create(insertJobServiceStopPK);

                        var insertJobServiceStopDEL = new TblMasterActualJobServiceStop
                        {
                            Guid = serviceStopGuidDEL,
                            MasterActualJobHeader_Guid = insertHeader.Guid,
                            CustomerLocationAction_Guid = actionGuidDel,
                            MasterCustomerLocation_Guid = jobInfo.LocationGuid
                        };
                        _masterActualJobServiceStopsRepository.Create(insertJobServiceStopDEL);
                        #endregion ///End Insert TblJobServiceStop

                        #region Insert SpecialCommand
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
                        #endregion ///End Insert SpecialCommand

                        #region Insert ServiceStopLegs
                        DateTime deliveryTime = !string.IsNullOrEmpty(deliveryLeg.StrWorkDate_Time) ? deliveryLeg.StrWorkDate_Time.ToTimeDateTime() : "00:00".ToTimeDateTime();
                        for (int i = 1; i <= 2; i++)
                        {
                            var insertStopLeg = new TblMasterActualJobServiceStopLegs
                            {
                                Guid = Guid.NewGuid(),
                                MasterActualJobHeader_Guid = insertHeader.Guid,
                                SequenceStop = i,
                                CustomerLocationAction_Guid = i == 1 ? actionGuidPick : actionGuidDel,
                                MasterCustomerLocation_Guid = i == 1 ? Companylocation_LegP : jobInfo.LocationGuid,
                                MasterRouteGroupDetail_Guid = deliveryLeg.RouteGroupDetailGuid,
                                ServiceStopTransectionDate = WorkDate_leg_D, // DateTime.Parse(deliveryLeg.StrWorkDate_Date), /*Work Date*/
                                WindowsTimeServiceTimeStart = deliveryTime,
                                WindowsTimeServiceTimeStop = deliveryTime,
                                JobOrder = i == 2 ? jobInfo.LocationSeq : 0,
                                SeqIndex = i == 2 ? jobInfo.LocationSeq : 0,
                                MasterRunResourceDaily_Guid = deliveryLeg.RunResourceGuid,
                                PrintedReceiptNumber = i == 1 ? null : ReceiptNo,
                                MasterSite_Guid = i == 1 ? pickupLeg.BrinkSiteGuid : deliveryLeg.BrinkSiteGuid, /* Source : Site of brink , Dest : Site of customer*/
                                ArrivalTime = null,
                                ActualTime = null,
                                DepartTime = null,
                                FlagCancelLeg = false,
                                FlagNonBillable = i == 1 ? pickupLeg.FlagNonBillable.GetValueOrDefault() : deliveryLeg.FlagNonBillable.GetValueOrDefault(),
                                FlagDestination = (i == 2),   //Only last leg that must be true
                                UnassignedBy = jobInfo.UnassignedBy,
                                UnassignedDatetime = jobInfo.UnassignedDate
                            };
                            legs.Add(insertStopLeg);
                            #region Insert History Run 
                            if (deliveryLeg.RunResourceGuid != null)
                            {
                                var insertHistoryRun = new TblMasterHistory_ActaulJobOnDailyRunResource
                                {
                                    Guid = Guid.NewGuid(),
                                    MasterActualJobHeader_Guid = insertHeader.Guid,
                                    MasterRunResourceDaily_Guid = deliveryLeg.RunResourceGuid,
                                    DateTimeInterChange = request.UniversalDatetime,         // Ple add 1/8/2016 for use in dolphin
                                    MasterActualJobServiceStopLegs_Guid = insertStopLeg.Guid // Ple add 1/8/2016 for use in dolphin
                                };
                                _masterHistoryActaulJobOnDailyRunResourceRepository.Create(insertHistoryRun);
                            }
                            #endregion
                        }

                        #region Insert History
                        var insertHistory = new TblMasterHistory_ActualJob
                        {
                            Guid = Guid.NewGuid(),
                            MasterActualJobHeader_Guid = insertHeader.Guid, //headDetail.AdhocJobHeaderView.JobGuid.GetValueOrDefault(),
                            MsgID = getMessageID(headDetail.FlagJobSFO), // Created job no. {0} date {1} from Adhoc Job by Ocean Online MVC.
                            MsgParameter = new string[] { headDetail.JobNo, deliveryLeg.StrWorkDate_Date }.ToJSONString(),
                            UserCreated = request.UserName,
                            DatetimeCreated = request.ClientDateTime.DateTime,
                            UniversalDatetimeCreated = request.UniversalDatetime
                        };
                        _masterHistoryActualJobRepository.Create(insertHistory);
                        #endregion ///End Insert History
                        #endregion ///End Insert ServiceStopLegs

                    } //End foreach(var jobInfo in arr_JobInfo)

                    CreateAdhocAddSpot(request, legs);
                    _masterActualJobServiceStopLegsRepository.CreateRange(legs);

                    _uow.Commit();
                    transcope.Complete();
                }          //End Transaction scope
                #endregion //End Insert to Tables

                #region ## Update JobOrder and Push to dolphin.

                if (!deliveryLeg.RunResourceGuid.IsNullOrEmpty())
                {
                    UpdateJobOrderInRunRequest updatJobOrder = new UpdateJobOrderInRunRequest()
                    {
                        ClientDateTime = request.ClientDateTime.DateTime,
                        LanguageGuid = request.LanguagueGuid,
                        UserModified = request.UserName,
                        WorkDate = WorkDate_leg_D,
                        SiteGuid = deliveryLeg.BrinkSiteGuid,
                        FlagReorder = false,
                        RunDailyGuid = deliveryLeg.RunResourceGuid.GetValueOrDefault()

                    };
                    UpdateJobOrderInRun(updatJobOrder);
                    if (!request.AdhocJobHeaderView.FlagPageFromMapping) //From Non-Mapping Page
                    {
                        updatJobOrder.JobHeadGuidList.AddRange(arr_JobInfo.Select(o => o.JobGuid));
                        UpdatePushToDolPhinWhenCreateJob(updatJobOrder);
                    }
                }
                #endregion End ## Update JobOrder and Push to dolphin.

                #region ## Create OTC
                //=> TFS#53385:Ability to generate the OTC codes for both legs of Transfer job -> CreateJobDelivery [NEW]
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
    }
}
