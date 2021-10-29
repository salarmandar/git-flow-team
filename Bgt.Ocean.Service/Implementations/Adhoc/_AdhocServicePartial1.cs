using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.History;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using Bgt.Ocean.Service.Messagings.AdhocService;
using Bgt.Ocean.Service.ModelViews;
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
        #region Get trip indicator
        public IEnumerable<DisplayTextDropDownView> GetTripIndicator(Guid countryGuid)
        {
            return _systemTripIndicatorRepository.GetTripIndicatorByCountryGuid(countryGuid).Select(x => new DisplayTextDropDownView { Guid = x.Guid, DisplayText = x.IndicatorName });
        }
        #endregion

        public AdhocJobResponse CheckDuplicateJobsInDay(CreateJobAdHocRequest request)
        {
            AdhocJobHeaderRequest headDetail = request.AdhocJobHeaderView;
            AdhocLegRequest pickupLeg = request.ServiceStopLegPickup;
            AdhocLegRequest deliveryLeg = request.ServiceStopLegDelivery;
            AdhocCheckDuplicateJobInDayResult result = null;
            if (headDetail.ServiceJobTypeID == IntTypeJob.P ||
                headDetail.ServiceJobTypeID == IntTypeJob.BCP ||
                headDetail.ServiceJobTypeID == IntTypeJob.P_MultiBr ||
                headDetail.ServiceJobTypeID == IntTypeJob.D
                )           //P, BCP, P Multi, D use new one because it's multi selections
            {
                result = Linq_Adhoc_CheckDuplicateJobInDay(request);
            }
            else
            {
                if (request.AdhocJobHeaderView.JobGuid == null)
                {
                    request.AdhocJobHeaderView.JobGuid = Guid.NewGuid(); //new one can check and it shouldn't be duplicated
                }

                // D,AC,AE ( CustomerGuid && LocationGuid ==  Guid.Empty ** จากหน้าบ้าน -> Don't try to change this variables or input of STORE PROCEDURE, old logic was correct but it's too complicated to understand
                Guid? CustomerGuid = pickupLeg.CustomerGuid != null ? pickupLeg.CustomerGuid : deliveryLeg.CustomerGuid;
                Guid? CusLocationGuid = pickupLeg.LocationGuid != null ? pickupLeg.LocationGuid : deliveryLeg.LocationGuid;
                Guid siteGuid = pickupLeg.BrinkSiteGuid != Guid.Empty ? pickupLeg.BrinkSiteGuid : deliveryLeg.BrinkSiteGuid;

                //when check duplicated, must have Work Date from both 2 legs
                //I expected to check from Job Type ID but it will not work if we have new Job Type ID.
                if (string.IsNullOrEmpty(deliveryLeg.StrWorkDate_Date) && !string.IsNullOrEmpty(pickupLeg.StrWorkDate_Date))
                {
                    deliveryLeg.StrWorkDate_Date = pickupLeg.StrWorkDate_Date;
                }
                if (!string.IsNullOrEmpty(deliveryLeg.StrWorkDate_Date) && string.IsNullOrEmpty(pickupLeg.StrWorkDate_Date))
                {
                    pickupLeg.StrWorkDate_Date = deliveryLeg.StrWorkDate_Date;
                }

                DateTime? WorkDate = pickupLeg.StrWorkDate_Date.ChangeFromStringToDate(request.DateTimeFormat);
                DateTime? WorkDate_del = deliveryLeg.StrWorkDate_Date.ChangeFromStringToDate(request.DateTimeFormat);

                result = _masterActualJobServiceStopLegsRepository.Func_Adhoc_CheckDuplicateJobInDay(
                    siteGuid,
                    WorkDate,
                    WorkDate_del,
                    headDetail.ServiceJobTypeID,
                    headDetail.JobGuid,
                    headDetail.LineOfBusiness_Guid,
                    CustomerGuid,
                    CusLocationGuid,
                    deliveryLeg.CustomerGuid,
                    deliveryLeg.LocationGuid,
                    deliveryLeg.BrinkSiteGuid);
            }

            //result has 2 MsgID depends on Country Configuration
            //-210 it already has information. Can not to insert again.
            //-209 it already has information. Would you like to insert?

            int msgID = 0;
            if (result.FlagDuplicate.GetValueOrDefault())
            {
                msgID = result.FlagJobDupValidate.GetValueOrDefault() ? -210 : -209;
            }
            if (headDetail.ServiceJobTypeID == IntTypeJob.MCS && pickupLeg.LocationGuid != null) //LocationGuid will be replaced by arr_LocationGuid soon
            {
                var machine = _sfoMasterMachineRepository.CheckAssociateMachine(pickupLeg.LocationGuid.GetValueOrDefault());
                if (!machine)
                {
                    msgID = -765;
                    result.FlagDuplicate = false;
                    result.FlagJobDupValidate = true;
                }
            }

            var tblMsg = _systemMessageRepository.FindByMsgId(msgID, request.LanguagueGuid);
            AdhocJobResponse response = new AdhocJobResponse(tblMsg);
            response.FlagDuplicate = result.FlagDuplicate ?? false;
            response.FlagJobDupValidate = result.FlagJobDupValidate ?? false;
            response.JobGuid = request.AdhocJobHeaderView.JobGuid.Value;
            response.JobNo = request.AdhocJobHeaderView.JobNo;
            return response;
        }

        public AdhocCheckDuplicateJobInDayResult Linq_Adhoc_CheckDuplicateJobInDay(CreateJobAdHocRequest request)
        {

            AdhocJobHeaderRequest requestHead = request.AdhocJobHeaderView;
            AdhocLegRequest requestLegP = request.ServiceStopLegPickup;
            AdhocLegRequest requestLegD = request.ServiceStopLegDelivery;

            //when check duplicated, must have Work Date from both 2 legs
            //I expected to check from Job Type ID but it will not work if we have new Job Type ID.
            requestLegD.StrWorkDate_Date = string.IsNullOrEmpty(requestLegD.StrWorkDate_Date) ? requestLegP.StrWorkDate_Date : requestLegD.StrWorkDate_Date;
            requestLegP.StrWorkDate_Date = string.IsNullOrEmpty(requestLegP.StrWorkDate_Date) ? requestLegD.StrWorkDate_Date : requestLegP.StrWorkDate_Date;

            //new one can check and it shouldn't be duplicated
            request.AdhocJobHeaderView.JobGuid = request.AdhocJobHeaderView.JobGuid ?? Guid.NewGuid();

            DateTime? WorkDate_P = requestLegP.StrWorkDate_Date.ChangeFromStringToDate(request.DateTimeFormat);
            DateTime? WorkDate_D = requestLegD.StrWorkDate_Date.ChangeFromStringToDate(request.DateTimeFormat);

            List<Guid> arr_Location = new List<Guid>();
            if (requestLegP.arr_LocationGuid != null)
                arr_Location.AddRange(requestLegP.arr_LocationGuid.Where(o => !o.IsNullOrEmpty()).Select(o => o.GetValueOrDefault()));
            else if (requestLegP.LocationGuid != null)
                arr_Location.Add(requestLegP.LocationGuid.Value); //support old version of CreateJobPickUp
            if (requestLegD.arr_LocationGuid != null)
                arr_Location.AddRange(requestLegD.arr_LocationGuid.Where(o => !o.IsNullOrEmpty()).Select(o => o.GetValueOrDefault()));
            else if (requestLegD.LocationGuid != null)
                arr_Location.Add(requestLegD.LocationGuid.Value); //support old version of CreateJobDelivery

            AdhocCheckDuplicateJobInDayResult result = new AdhocCheckDuplicateJobInDayResult();
            var CountryOp = _systemEnvironment_GlobalRepository.Func_CountryOption_Get("FlagJobDupValidate", requestLegP.BrinkSiteGuid, null).AppValue1;
            result.FlagJobDupValidate = CountryOp != null && CountryOp.Equals("1");

            result.FlagDuplicate = (from head in _masterActualJobHeaderRepository.FindAllAsQueryable()
                                    join legs in _masterActualJobServiceStopLegsRepository.FindAllAsQueryable() on head.Guid equals legs.MasterActualJobHeader_Guid
                                    join cuslo in _masterCustomerLocationRepository.FindAllAsQueryable() on legs.MasterCustomerLocation_Guid equals cuslo.Guid
                                    join cus in _masterCustomerRepository.FindAllAsQueryable() on cuslo.MasterCustomer_Guid equals cus.Guid
                                    where legs.ServiceStopTransectionDate >= WorkDate_P && legs.ServiceStopTransectionDate <= WorkDate_D &&
                                            (legs.MasterSite_Guid == requestLegP.BrinkSiteGuid || legs.MasterSite_Guid == requestLegD.BrinkSiteGuid) &&
                                            head.SystemServiceJobType_Guid == requestHead.ServiceJobTypeGuid &&
                                            head.FlagCancelAll == false &&
                                            head.SystemStatusJobID != 14 && //Status 'Cancel'
                                            legs.MasterActualJobHeader_Guid != requestHead.JobGuid &&
                                            head.SystemLineOfBusiness_Guid == requestHead.LineOfBusiness_Guid &&
                                            arr_Location.Contains(cuslo.Guid) &&
                                            (cuslo.MasterCustomer_Guid == requestLegP.CustomerGuid || cuslo.MasterCustomer_Guid == requestLegD.CustomerGuid) &&
                                            cus.FlagChkCustomer == true
                                    select head).Any();
            return result;
        }


        public SystemMessageView IsThereEmployeeCanDoOTC(CheckEmployeeCanDoOTC modelEmp)
        {
            bool success = true;
            Guid LanguagueGuid = ApiSession.UserLanguage_Guid.GetValueOrDefault();
            var runDetail = _masterDailyRunResourceRepository.FindById(modelEmp.DailyRunResource_Guid);
            if (runDetail.RunResourceDailyStatusID == StatusDailyRun.DispatchRun)
            {
                List<Guid?> arr_Locations = modelEmp.CustomerLocation_Guids.Where(o => !o.IsNullOrEmpty()).ToList();
                if (arr_Locations.Any())
                {
                    var isMachine = _sfoMasterMachineLockTypeRepository
                                   .FileByListId(arr_Locations)
                                   .Where(a => a.LockSeq == 1)
                                   .Select(s => s.SFOMasterMachine_Guid);
                    if (isMachine.Any())
                    {
                        var strMachine = string.Join(",", Array.ConvertAll(isMachine.ToArray(), x => x.ToString()));

                        var noEmployee = _masterDailyRunResourceRepository.Func_IsThereEmployeeCanDoOTC(modelEmp.MasterSite_Guid, modelEmp.DailyRunResource_Guid, strMachine)
                                                                          .Any(a => a.NotEnoughUpper.GetValueOrDefault() > 0);
                        if (noEmployee)
                        {
                            success = false;
                        }
                    }
                }
            }

            if (success)
            {
                var tblmsg = _systemMessageRepository.FindByMsgId(0, LanguagueGuid);
                return new SystemMessageView(tblmsg);
            }
            else
            {
                //-1127 : The run does not have appropriate employee assigned for requesting the OTC. The OTC will not be able to generate if assigned to this run. Do you want to continue assigning?
                var tblmsg = _systemMessageRepository.FindByMsgId(-1127, LanguagueGuid);
                return new SystemMessageView(tblmsg);
            }
        }

        /// <summary>
        /// This is a preventive validation to identify How many jobs are currently admitted in Dolphin without decrease performance.
        /// the user can add as many jobs as he wants, but when he reaches the limit he will receive an alert message.
        /// </summary>
        /// <param name="unAssignedJobs">number of unassigned jobs.</param>
        /// <param name="runGuid">run daily Guid.</param>
        /// <param name="workDate">work date selecteed by the user.</param>
        /// <param name="language">Language Id of the user.</param>
        /// <returns>if the user reaches the limit or not.</returns>
        private SystemMessageView ValidateMaxNumberJobs(int unAssignedJobs, Guid runGuid, DateTime workDate, Guid language)
        {
            int maxNumberJobs = 0;
            var strMaxNum = _systemEnvironment_GlobalRepository.FindByAppKey("MaxNumberJobs")?.AppValue1;

            int.TryParse(strMaxNum, out maxNumberJobs);
            int currentNumberJobs = _masterActualJobHeaderRepository.GetNumberJobsPerRun(runGuid, workDate);
            currentNumberJobs = currentNumberJobs < 0 ? unAssignedJobs : currentNumberJobs + unAssignedJobs;
            if (currentNumberJobs > maxNumberJobs)
            {
                var tblmsg = _systemMessageRepository.FindByMsgId(-726, language);
                return new SystemMessageView(tblmsg);
            }
            else
            {
                return null;
            }
        }

        public DetailDestinationForDeliveryReponse GetDetailDestinationForDelivery(Guid siteGuid, Guid? siteGuidDel, Guid? locationGuid, int jobTypeID)
        {
            DetailDestinationForDeliveryReponse reponse = new DetailDestinationForDeliveryReponse();

            var details = _masterCustomerLocation_LocationDestinationRepository.Func_AdhocDestination(siteGuid, siteGuidDel, locationGuid, jobTypeID).ToList();

            //get first value that has FlagDefaultOnward = true, if none, get first value of any data
            var detailSite = details.FirstOrDefault(e => e.FlagDefaultOnward == true) ?? details.FirstOrDefault();
            if (detailSite != null)
            {
                reponse.SiteGuid = detailSite.SiteGuid.Value;
                reponse.SiteName = detailSite.SiteName;
                reponse.FlagInterBr = detailSite.SiteGuid != siteGuid;
                reponse.FlagDefaultOnward = detailSite.FlagDefaultOnward.GetValueOrDefault();
                reponse.FlagDisableInterBr = detailSite.FlagDisableInterBr.GetValueOrDefault();
            }

            details.Where(e => e.SiteGuid == detailSite.SiteGuid && e.InternalDepartmentGuid != null).ToList().ForEach(
                item =>
                    reponse.ComboDestination.Add(
                        new CustomerLocation_InternalDepartmentView
                        {
                            id = item.InternalDepartmentGuid.Value,
                            text = item.InterDepartmentName,
                            onwardTypeId = 1,
                            flagDefaultOnward = item.FlagDefaultOnward.GetValueOrDefault()
                        })
                    );
            reponse.FlagNoDestination = !details.Any(o => o.CustomerLocationDes != null);
            return reponse;
        }

        private void DisconnectMasterRouteAndAddRunHistory(TblMasterDailyRunResource dailyRun, CreateJobAdHocRequest request, IMasterDailyRunResourceRepository _masterDailyRunResourceRepository, IMasterHistory_DailyRunResourceRepository _masterHistory_DailyRunResource)
        {
            if (!dailyRun.FlagBreakMasterRoute && !dailyRun.FirstMasterRoute_Guid.IsNullOrEmpty())
            {
                dailyRun.FlagBreakMasterRoute = true;

                TblMasterHistory_DailyRunResource dailyHistory = new TblMasterHistory_DailyRunResource()
                {
                    Guid = Guid.NewGuid(),
                    MasterDailyRunResource_Guid = dailyRun.Guid,
                    MsgID = 958,
                    MsgParameter = new string[] { "Create job", "Ad-hoc", request.UserName }.ToJSONString(),
                    UserCreated = request.UserName,
                    DatetimeCreated = request.ClientDateTime.DateTime,
                    UniversalDatetimeCreated = request.UniversalDatetime
                };

                _masterHistory_DailyRunResource.Create(dailyHistory);
                _masterDailyRunResourceRepository.Modify(dailyRun);
            }
        }

        private bool getCountryConfigToCheckAutoFillContract(Guid CountryGuid)
        {
            var objAllowContract = _systemEnvironmentMasterCountryValueRepository.GetSpecificKeyByCountryAndKey(CountryGuid, "FlagAllowCreateJobWithoutContract");
            var objAssociatedContract = _systemEnvironmentMasterCountryValueRepository.GetSpecificKeyByCountryAndKey(CountryGuid, "FlagAssociatedContractNoToJob");

            return (objAllowContract != null && objAssociatedContract != null &&
                objAllowContract.AppValue1 != null && objAssociatedContract.AppValue1 != null &&
                objAllowContract.AppValue1.ToLower().Equals("false") && objAssociatedContract.AppValue1.ToLower().Equals("false"));
        }


        private Guid? GetContractGuid(Guid MasterCustomerLocation_Guid, Guid SystemLineOfBusiness_Guid, Guid SystemServiceJobType_Guid, Guid? SystemSubServiceType_Guid, DateTime WorkDate)
        {

            var query = from cuslo in _masterCustomerLocationRepository.FindAllAsQueryable()
                        join contsvcloc in _masterCustomerContract_ServiceLocationRepository.FindAllAsQueryable() on cuslo.Guid equals contsvcloc.MasterCustomerLocation_Guid into cont_cuslo
                        from cont_cusloResult in cont_cuslo.DefaultIfEmpty()

                        join cont in _masterCustomerContractRepository.FindAllAsQueryable() on cont_cusloResult.MasterCustomerContract_Guid equals cont.Guid into cont_cus
                        from cont_cusResult in cont_cus.DefaultIfEmpty()

                        join lob in _systemLineOfBusinessRepository.FindAllAsQueryable() on cont_cusloResult.SystemLineOfBusiness_Guid equals lob.Guid into cont_cusloLob
                        from cont_cusloLobResult in cont_cusloLob.DefaultIfEmpty()

                        join typ in _systemServiceJobTypeRepository.FindAllAsQueryable() on cont_cusloResult.SystemServiceJobType_Guid equals typ.Guid into cont_cusloTyp
                        from cont_cusloTypResult in cont_cusloTyp.DefaultIfEmpty()

                        join sub in _masterSubServiceTypeRepository.FindAllAsQueryable() on cont_cusloResult.SystemSubServiceType_Guid equals sub.Guid into cont_cusloSub
                        from cont_cusloSubResult in cont_cusloSub.DefaultIfEmpty()

                        where cuslo.Guid == MasterCustomerLocation_Guid &&
                                cont_cusloLobResult.Guid == SystemLineOfBusiness_Guid &&
                                cont_cusloTypResult.Guid == SystemServiceJobType_Guid &&
                                !cont_cusResult.FlagDisable && !cuslo.FlagDisable && cont_cusResult.ContractStartedDate <= WorkDate && cont_cusResult.ContractExpiredDate >= WorkDate
                        select new { CONTRACT = cont_cusloResult, SUBSERVICE = cont_cusloSubResult };
            if (SystemSubServiceType_Guid != null)
            {
                query = query.Where(o => o.SUBSERVICE.Guid == SystemSubServiceType_Guid);
            }
            if (query.Any())
            {
                return query.First().CONTRACT.MasterCustomerContract_Guid; //get first contract that's matched
            }
            return null;
        }


      
    }
}
