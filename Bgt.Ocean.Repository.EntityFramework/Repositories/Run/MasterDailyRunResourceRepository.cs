using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl.LiabilityLimitModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Bgt.Ocean.Infrastructure.Helpers;
using System.Data.SqlClient;
using Bgt.Ocean.Repository.EntityFramework.StringQuery.RunResource;
using Bgt.Ocean.Models.Nemo.RouteOptimization;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using Bgt.Ocean.Models.Reports.DailyPlan;
using Bgt.Ocean.Repository.EntityFramework.Extensions;
using Bgt.Ocean.Models.RouteOptimization;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models.RunControl;
using EntityFrameworkExtras.EF6;
using Bgt.Ocean.Infrastructure.Storages;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Run
{
    #region Interface
    public interface IMasterDailyRunResourceRepository : IRepository<TblMasterDailyRunResource>
    {
        IEnumerable<ValidateJobsInRunView> ValidateAssignJobsToRun(IEnumerable<ValidateJobsInRunView> assignJobList);
        List<ValidateCrewOnPortalView> ValidateCrewOnPortal(ValidateCrewOnPortalView_Request request);

        CloseRunManuallyProcedure CloseRunManually(CloseRunManuallyProcedure proc);
        IEnumerable<RunResourceByGroupDetailAndWorkDayResult> Func_RunResourceByGroupDetailAndWorkDay(DateTime? workDate, Guid? routeDetailGuid, Guid? siteGuid);

        IEnumerable<RunControlValidateEMPRequestOTCResult> Func_IsThereEmployeeCanDoOTC(Guid? MasterSite_Guid, Guid? MasterDailyRunResource_Guid, string MasterMachine_Guid);
        string GetDailyRunResourceAndRouteGroupDetail(Guid dailyRunGuid);

        bool IsRunDispatched(Guid dailyRunGuid);

        IEnumerable<DailyRunOptimizationView> GetDailyRun_Optimizations(Guid siteGuid, string workDate);
        DailyRunDetail GetDailyRunDetail(Guid dailyRunGuid);

        ValidateJobCannotCloseResponse GetValidateJobCannotCloseRun(Guid dailyRunGuid, Guid languageguid, string vehicleNo);

        #region Daily Plan
        List<DailyPlanCustomerResponse> GetDailyPlanCustomer(DailyPlanCustomerRequest request);
        List<DailyPlanRouteGroupResponse> GetDailyPlanRouteGroup(DailyPlanRouteGroupRequest request);
        List<DailyPlanRouteGroupDetailResponse> GetDailyPlanRouteDetail(DailyPlanRouteGroupDetailRequest request);
        List<DailyPlanDataResponse> GetDailyPlanDataList(DailyPlanDataRequest request);
        List<DailyPlanEmailResponse> GetDailyPlanEmailList(List<DailyPlanEmailRequest> request);
        #endregion

        #region Truck To Truck Transfer
        TruckToTruckTransferResponse TruckToTruckIsValidRun(Guid oldDailyRunGuid, Guid newDailyRunGuid, Guid languageGuid);
        TruckToTruckTransferResponse TruckToTruckIsValidJobStatus(IEnumerable<TblMasterActualJobHeader> jobList, Guid languageGuid);
        TruckToTruckTransferResponse TruckToTruckIsValidServiceJobType(IEnumerable<TblMasterActualJobHeader> jobList, Guid newDailyRunGuid, Guid languageGuid);
        TruckToTruckTransferResponse TruckToTruckIsValidlegListMustBeInOldRun(List<Guid> legGuidList, Guid oldDailyRunGuid, Guid languageGuid);
        #endregion

        IEnumerable<RouteGroupDetailRunResourceView> GetRouteGroupDetailByWorkDate(Guid siteGuid, DateTime workDate);
        IEnumerable<RouteGroupDetailRunResourceView> GetRouteGroupDetailByRunGuid(Guid runGuid);
        IEnumerable<DailyRouteView> GetDailyRoute_For_Optimization(DailyRouteRequest request, Guid languageGuid, bool validateRunLiabilityLimit);
        IEnumerable<JobUnassignedView> GetUnassigned_For_Optimization(DailyRouteRequest request, Guid languageGuid, bool validateRunLiabilityLimit);

        Guid? FindDefaultCurrencyByUser(Guid? userGuid);
        IEnumerable<RawJobDataView> GetLiabilityLimitRawJobsData(Guid? siteGuid, IEnumerable<LiabilityLimitNoJobsAction> model);
        IEnumerable<RawJobDataView> GetLiabilityLimitRawJobsData(Guid? siteGuid, IEnumerable<LiabilityLimitJobsAction> model);
    }
    #endregion

    public class MasterDailyRunResourceRepository : Repository<OceanDbEntities, TblMasterDailyRunResource>, IMasterDailyRunResourceRepository
    {
        #region Objects & Variables
        private readonly DailyRunResourceQuery dailyRunResourceQuery = new DailyRunResourceQuery();
        private object jobModels;
        #endregion

        public MasterDailyRunResourceRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public List<ValidateCrewOnPortalView> ValidateCrewOnPortal(ValidateCrewOnPortalView_Request request)
        {
            var result = DbContext.Database.Connection
                .Query<ValidateCrewOnPortalView>
                (
                    "Up_OceanOnlineMVC_API_ValidateCrewOnPortal",
                    new
                    {
                        @CrewID = request.CrewID,
                        @RunDate = request.RunDate,
                        @SiteCode = request.SiteCode
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();

            return result;
        }

        public CloseRunManuallyProcedure CloseRunManually(CloseRunManuallyProcedure proc)
        {
            DbContext.Database.ExecuteStoredProcedure(proc);
            return proc;
        }
        public IEnumerable<RunResourceByGroupDetailAndWorkDayResult> Func_RunResourceByGroupDetailAndWorkDay(DateTime? workDate, Guid? routeDetailGuid, Guid? siteGuid)
        {
            return DbContext.Up_OceanOnlineMVC_RunResourceByGroupDetailAndWorkDay_Get(workDate, routeDetailGuid, siteGuid);
        }

        public IEnumerable<RunControlValidateEMPRequestOTCResult> Func_IsThereEmployeeCanDoOTC(Guid? MasterSite_Guid, Guid? MasterDailyRunResource_Guid, string MasterMachine_Guid)
        {
            return DbContext.Up_OceanOnlineMVC_RunControl_ValidateEMPRequestOTC_Get(MasterSite_Guid, MasterDailyRunResource_Guid, MasterMachine_Guid).ToList();
        }
        public string GetDailyRunResourceAndRouteGroupDetail(Guid dailyRunGuid)
        {
            return DbContext.TblMasterDailyRunResource.Where(w => w.Guid == dailyRunGuid)
                           .Join(DbContext.TblMasterRouteGroup_Detail, r => r.MasterRouteGroup_Detail_Guid, d => d.Guid, (r, d) => new { r, d })
                           .Join(DbContext.TblMasterRunResource, rd => rd.r.MasterRunResource_Guid, rs => rs.Guid, (rd, rs) => new
                           {
                               name = rd.d.MasterRouteGroupDetailName + " - " + rs.VehicleNumber
                           }).FirstOrDefault()?.name;
        }

        public bool IsRunDispatched(Guid dailyRunGuid)
        {
            var dailyRun = FindById(dailyRunGuid);
            return dailyRun != null && dailyRun.RunResourceDailyStatusID == DailyRunStatus.DispatchRun;
        }

        public IEnumerable<DailyRunOptimizationView> GetDailyRun_Optimizations(Guid siteGuid, string workDate)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                var command = new SqlCommand(dailyRunResourceQuery.GetDailyRunResource_Ready);
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@SiteGuid", siteGuid));
                parameters.Add(new SqlParameter("@WorkDate", workDate));

                return context.Database.SqlQuery<DailyRunOptimizationView>(command.CommandText, parameters.ToArray()).ToList();
            }
        }

        public DailyRunDetail GetDailyRunDetail(Guid dailyRunGuid)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                var command = new SqlCommand(dailyRunResourceQuery.GetDailyRunResource_Fullname);
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@DailyRunGuid", dailyRunGuid));

                return context.Database.SqlQuery<DailyRunDetail>(command.CommandText, parameters.ToArray()).FirstOrDefault();
            }
        }
        private List<JobStatusCannotCloseRunView> GetCloseRunCondition()
        {
            return new List<JobStatusCannotCloseRunView>()
            {
                new JobStatusCannotCloseRunView{MsgID = -840,JobStatus = 3,FlagHasItem = false},
                new JobStatusCannotCloseRunView{MsgID = -840,JobStatus = 26,FlagHasItem =false},
                new JobStatusCannotCloseRunView{MsgID = -840,JobStatus = 27,FlagActionIsD=true},
                new JobStatusCannotCloseRunView{MsgID = -840,JobStatus = 7,FlagIntermediate = true,FlagActionIsD = true},
                new JobStatusCannotCloseRunView{MsgID = -840,JobStatus = 28,FlagIntermediate = true,FlagActionIsD = true},
                new JobStatusCannotCloseRunView{MsgID = -840,JobStatus = 29,FlagIntermediate = true,FlagActionIsD = true},
                new JobStatusCannotCloseRunView{MsgID = -843,JobStatus = 4},
                new JobStatusCannotCloseRunView{MsgID = -843,JobStatus = 12,FlagActionIsD = true},
                new JobStatusCannotCloseRunView{MsgID = -843,JobStatus = 30},
                new JobStatusCannotCloseRunView{MsgID = -844,JobStatus = 7,FlagHasItem = false},
                new JobStatusCannotCloseRunView{MsgID = -844,JobStatus = 28,FlagHasItem = false},
                new JobStatusCannotCloseRunView{MsgID = -845,JobStatus = 7,FlagDiscrepancies = true},
                new JobStatusCannotCloseRunView{MsgID = -845,JobStatus = 13,FlagDiscrepancies = true},
                new JobStatusCannotCloseRunView{MsgID = -845,JobStatus = 17,FlagDiscrepancies = true},
                new JobStatusCannotCloseRunView{MsgID = -845,JobStatus = 28,FlagDiscrepancies = true},
                new JobStatusCannotCloseRunView{MsgID = -846,JobStatus = 34 , FlagIsDolphinReceive = true,FlagHasItem = false},
                new JobStatusCannotCloseRunView{MsgID = -849,JobStatus = 38},
                new JobStatusCannotCloseRunView{MsgID = -850,JobStatus = 90},
                new JobStatusCannotCloseRunView{MsgID = -851,JobStatus = 104},
                new JobStatusCannotCloseRunView{MsgID = -851,JobStatus = 110},
                new JobStatusCannotCloseRunView{MsgID = -851,JobStatus = 111}
            };
        }

        public ValidateJobCannotCloseResponse GetValidateJobCannotCloseRun(Guid dailyRunGuid, Guid languageguid, string vehicleNo)
        {
            ValidateJobCannotCloseResponse respMsg = new ValidateJobCannotCloseResponse();

            #region ### Check Validate OverageItems Items ###
            if (ValidateItemsOverageWithoutCloseCaseInRunResouce(dailyRunGuid))
            {
                var lang = DbContext.TblSystemMessage.FirstOrDefault(o => o.MsgID == -89 && o.SystemLanguage_Guid == languageguid);
                respMsg.Msg.MsgId = lang.MsgID;
                respMsg.Msg.MsgTitle = lang.MessageTextTitle;
                respMsg.Msg.MsgDetail = lang.MessageTextContent;
                return respMsg;
            }
            #endregion

            #region ### Check Validate Unknown Items ###
            if (ValidateItemsUnknownInRunResource(dailyRunGuid))
            {
                var lang = DbContext.TblSystemMessage.FirstOrDefault(o => o.MsgID == -88 && o.SystemLanguage_Guid == languageguid);
                respMsg.Msg.MsgId = lang.MsgID;
                respMsg.Msg.MsgTitle = lang.MessageTextTitle;
                respMsg.Msg.MsgDetail = lang.MessageTextContent;

                if (string.IsNullOrEmpty(vehicleNo))
                {
                    vehicleNo = DbContext.TblMasterDailyRunResource.Where(w => w.Guid == dailyRunGuid)
                           .Join(DbContext.TblMasterRunResource, d => d.MasterRunResource_Guid, rs => rs.Guid, (d, rs) => new
                           {
                               vNumber = rs.VehicleNumber
                           }).FirstOrDefault()?.vNumber;
                }
                respMsg.Msg.JobNo = new string[] { vehicleNo }.ToJSONString();
                return respMsg;
            }
            #endregion

            #region ### GET JOB CANNOT CLOSE RUN ###
            List<JobStatusCannotCloseRunView> closeRunCondition = GetCloseRunCondition();
            // 11 : In Department
            // Remove Partial // Remove Non delivery, Because Both status have case discrepancies items.
            int[] allowClose = new int[]{IntStatusJob.Department, IntStatusJob.UnableToService,
                                              IntStatusJob.Unrealized, IntStatusJob.VisitWithStamp,IntStatusJob.VisitWithOutStamp,IntStatusJob.NoArrived,IntStatusJob.OnTruckDelivery,IntStatusJob.IntransitInterBranch};

            //list of job status that must check item
            int[] lstCheckItem = new int[] { IntStatusJob.InPreVault, IntStatusJob.InPreVaultPickUp, IntStatusJob.PickedUp, IntStatusJob.ReadyToPreVault, IntStatusJob.CancelAndReturnToPreVault };
            //list of job type that allow close run with no item in run.
            int[] allowCloseJobNoItem = new int[] { IntTypeJob.BCP, IntTypeJob.FLM, IntTypeJob.FSLM, IntTypeJob.TM, IntTypeJob.ECash };

            var lstNotAllow = from leg in DbContext.TblMasterActualJobServiceStopLegs
                              join head in DbContext.TblMasterActualJobHeader on leg.MasterActualJobHeader_Guid equals head.Guid
                              join action in DbContext.TblSystemJobAction on leg.CustomerLocationAction_Guid equals action.Guid
                              join type in DbContext.TblSystemServiceJobType on head.SystemServiceJobType_Guid equals type.Guid
                              where head.FlagCancelAll == false
                              && !allowClose.Any(o => o == head.SystemStatusJobID)
                              && leg.MasterRunResourceDaily_Guid == dailyRunGuid
                              && leg.JobOrder > 0 // GET LEG P Return Value order 0
                              orderby leg.JobOrder, leg.SeqIndex
                              select new JobStatusCannotCloseRunView
                              {
                                  JobNo = head.JobNo,
                                  JobType = type.ServiceJobTypeID,
                                  JobStatus = head.SystemStatusJobID,
                                  FlagHasItem = lstCheckItem.Any(o => o == head.SystemStatusJobID) && (
                                  DbContext.TblMasterActualJobItemsSeal.Any(o => o.MasterActualJobHeader_Guid == head.Guid && o.FlagSealDiscrepancies == false) ||
                                  DbContext.TblMasterActualJobItemsCommodity.Any(o => o.MasterActualJobHeader_Guid == head.Guid &&
                                                                                (o.FlagCommodityDiscrepancies == false || (o.FlagCommodityDiscrepancies == true && o.Quantity > 0)))
                                  ),
                                  FlagDiscrepancies = (bool)head.FlagJobDiscrepancies,
                                  FlagIntermediate = head.FlagIntermediate,
                                  FlagActionIsD = action.ActionNameAbbrevaition == JobActionAbb.StrDelivery,
                                  FlagIsInTransit = (head.SystemStatusJobID == IntStatusJob.IntransitInterBranch && action.ActionNameAbbrevaition == JobActionAbb.StrDelivery),
                                  FlagIsDolphinReceive = (head.SystemStatusJobID == IntStatusJob.WaitingforDolphinReceive)
                              };

            #endregion

            #region ### SET RETURN MESSAGE TO FRONTEND ###
            var tblMsgError = (from job in lstNotAllow.AsEnumerable()
                               join rule in closeRunCondition on job.JobStatus equals rule.JobStatus
                               where rule.Validate(rule, job)
                               // job sfo,bco allow close run with no item 
                               && !allowCloseJobNoItem.Any(o => (o == job.JobType && job.FlagHasItem == false) && job.JobStatus == IntStatusJob.PickedUp)
                               select new { rule.MsgID, job.JobNo }).FirstOrDefault();

            if (tblMsgError != null)
            {
                var lang = DbContext.TblSystemMessage.FirstOrDefault(o => o.MsgID == tblMsgError.MsgID && o.SystemLanguage_Guid == languageguid);
                respMsg.Msg.MsgId = lang.MsgID;
                respMsg.Msg.MsgTitle = lang.MessageTextTitle;
                respMsg.Msg.MsgDetail = lang.MessageTextContent;
                respMsg.Msg.JobNo = new string[] { tblMsgError.JobNo }.ToJSONString();
            }
            else
            {
                var lang = DbContext.TblSystemMessage.FirstOrDefault(o => o.MsgID == 0 && o.SystemLanguage_Guid == languageguid);
                respMsg.Msg.MsgId = lang.MsgID;
                respMsg.Msg.MsgTitle = lang.MessageTextTitle;
                respMsg.Msg.MsgDetail = lang.MessageTextContent;
            }
            #endregion

            return respMsg;
        }

        private bool ValidateItemsUnknownInRunResource(Guid dailyRunGuid)
        {
            bool isHasItem = DbContext.TblMasterActualJobItemUnknow.Any(o => (o.FlagMatchDone == false)
                                                                              && (o.Quantity > 0)
                                                                              && (o.MasterRunResourceDaily_Guid == dailyRunGuid));
            return isHasItem;
        }

        private bool ValidateItemsOverageWithoutCloseCaseInRunResouce(Guid dailyRunGuid)
        {
            bool isHasItem = DbContext.TblMasterActualJobItemDiscrapencies.Any(o => (o.QtyOverage > 0)
                                                                                   && (o.FlagCloseCase == false)
                                                                                   && (o.MasterRunResourceDaily_Guid == dailyRunGuid));
            return isHasItem;
        }

        #region Daily Plan
        public List<DailyPlanCustomerResponse> GetDailyPlanCustomer(DailyPlanCustomerRequest request)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                var workdate = request.StrWorkDate.ChangeFromStringToDate(request.DateFormat);
                var command = new SqlCommand(dailyRunResourceQuery.GetDailyPlanCustomer);
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@SiteGuid", request.SiteGuid));
                parameters.Add(new SqlParameter("@WorkDate", workdate));

                return context.Database.SqlQuery<DailyPlanCustomerResponse>(command.CommandText, parameters.ToArray()).ToList();
            }
        }

        public List<DailyPlanRouteGroupResponse> GetDailyPlanRouteGroup(DailyPlanRouteGroupRequest request)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                DateTime workdate = request.StrWorkDate.ChangeFromStringToDate(request.DateFormat).GetValueOrDefault();
                var command = new SqlCommand(dailyRunResourceQuery.GetDailyPlanRouteGroup);
                List<SqlParameter> parameters = new List<SqlParameter>();
                if (request.CustomerGuid != null && request.CustomerGuid.Any())
                {
                    command.CommandText = command.CommandText.Replace("@@FilterCustomers", "AND cus.Guid IN ({CustomerGuid})");
                    command.AddArrayParameters("CustomerGuid", request.CustomerGuid, parameters);
                }
                else
                {
                    command.CommandText = command.CommandText.Replace("@@FilterCustomers", string.Empty);
                }
                parameters.Add(new SqlParameter("@SiteGuid", request.SiteGuid));
                parameters.Add(new SqlParameter("@WorkDate", workdate));

                return context.Database.SqlQuery<DailyPlanRouteGroupResponse>(command.CommandText, parameters.ToArray()).ToList();
            }
        }

        public List<DailyPlanRouteGroupDetailResponse> GetDailyPlanRouteDetail(DailyPlanRouteGroupDetailRequest request)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                DateTime workdate = request.StrWorkDate.ChangeFromStringToDate(request.DateFormat).GetValueOrDefault();
                var command = new SqlCommand(dailyRunResourceQuery.GetDailyPlanRouteDetail);
                List<SqlParameter> parameters = new List<SqlParameter>();
                if (request.CustomerGuid != null && request.CustomerGuid.Any())
                {
                    command.CommandText = command.CommandText.Replace("@@FilterCustomers", "AND cus.Guid IN ({CustomerGuid})");
                    command.AddArrayParameters("CustomerGuid", request.CustomerGuid, parameters);
                }
                else
                {
                    command.CommandText = command.CommandText.Replace("@@FilterCustomers", string.Empty);
                }
                if (request.RouteGroupGuid != null && request.RouteGroupGuid.Any())
                {
                    command.CommandText = command.CommandText.Replace("@@FilterRouteGroups", "AND rGroup.Guid IN ({RouteGroupGuid})");
                    command.AddArrayParameters("RouteGroupGuid", request.RouteGroupGuid, parameters);
                }
                else
                {
                    command.CommandText = command.CommandText.Replace("@@FilterRouteGroups", string.Empty);
                }
                parameters.Add(new SqlParameter("@SiteGuid", request.SiteGuid));
                parameters.Add(new SqlParameter("@WorkDate", workdate));

                return context.Database.SqlQuery<DailyPlanRouteGroupDetailResponse>(command.CommandText, parameters.ToArray()).ToList();
            }
        }

        public List<DailyPlanDataResponse> GetDailyPlanDataList(DailyPlanDataRequest request)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                DateTime workdate = request.StrWorkDate.ChangeFromStringToDate(request.DateFormat).GetValueOrDefault();
                var command = new SqlCommand(dailyRunResourceQuery.GetDailyPlanDataList);
                List<SqlParameter> parameters = new List<SqlParameter>();
                /* Filter - Customer */
                if (request.CustomerGuid != null && request.CustomerGuid.Any())
                {
                    command.CommandText = command.CommandText.Replace("@@FilterCustomers", "AND cus.Guid IN ({CustomerGuid})");
                    command.AddArrayParameters("CustomerGuid", request.CustomerGuid, parameters);
                }
                else
                {
                    command.CommandText = command.CommandText.Replace("@@FilterCustomers", string.Empty);
                }
                /* Filter - RouteGroup */
                if (request.RouteGroupGuid != null && request.RouteGroupGuid.Any())
                {
                    command.CommandText = command.CommandText.Replace("@@FilterRouteGroups", "AND rGroup.Guid IN ({RouteGroupGuid})");
                    command.AddArrayParameters("RouteGroupGuid", request.RouteGroupGuid, parameters);
                }
                else
                {
                    command.CommandText = command.CommandText.Replace("@@FilterRouteGroups", string.Empty);
                }
                /* Filter - RouteGroupDetail */
                if (request.RouteGroupDetailGuid != null && request.RouteGroupDetailGuid.Any())
                {
                    command.CommandText = command.CommandText.Replace("@@FilterRouteDetails", "AND rDetail.Guid IN ({RouteDetailGuid})");
                    command.AddArrayParameters("RouteDetailGuid", request.RouteGroupDetailGuid, parameters);
                }
                else
                {
                    command.CommandText = command.CommandText.Replace("@@FilterRouteDetails", string.Empty);
                }
                parameters.Add(new SqlParameter("@SiteGuid", request.SiteGuid));
                parameters.Add(new SqlParameter("@WorkDate", workdate));

                return context.Database.SqlQuery<DailyPlanDataResponse>(command.CommandText.Replace("@@MaxRow", request.MaxRow.ToString()), parameters.ToArray()).ToList();
            }
        }

        public List<DailyPlanEmailResponse> GetDailyPlanEmailList(List<DailyPlanEmailRequest> request)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                var command = new SqlCommand(dailyRunResourceQuery.GetDailyPlanEmailList);
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (request.Any())
                {
                    /* Filter - Customer */
                    command.CommandText = command.CommandText.Replace("@@FilterCustomers", "AND cus.Guid IN ({CustomerGuid})");
                    command.AddArrayParameters("CustomerGuid", request.Select(o => o.CustomerGuid).Distinct(), parameters);

                    command.CommandText = command.CommandText.Replace("@@FilterDailyRuns", "AND run.Guid IN ({DailyRunGuid})");
                    command.AddArrayParameters("DailyRunGuid", request.Select(o => o.DailyRunGuid).Distinct(), parameters);

                    return context.Database.SqlQuery<DailyPlanEmailResponse>(command.CommandText, parameters.ToArray()).ToList();
                }
                return null;
            }
        }

        #endregion

        #region TruckToTruck
        public TruckToTruckTransferResponse TruckToTruckIsValidRun(Guid oldDailyRunGuid, Guid newDailyRunGuid, Guid languageGuid)
        {
            var oldDailyRun = DbContext.TblMasterDailyRunResource.FirstOrDefault(e => e.Guid == oldDailyRunGuid);
            var newDailyRun = DbContext.TblMasterDailyRunResource.FirstOrDefault(e => e.Guid == newDailyRunGuid);
            var response = new TruckToTruckTransferResponse();
            TblSystemMessage msg = null;

            if (oldDailyRun != null && newDailyRun != null && (oldDailyRun.RunResourceDailyStatusID != DailyRunStatus.Ready || newDailyRun.RunResourceDailyStatusID != DailyRunStatus.Ready))
            {
                msg = DbContext.TblSystemMessage.FirstOrDefault(e => e.MsgID == -2157 && e.SystemLanguage_Guid == languageGuid);
                response.isSuccess = false;

                if (msg != null)
                    response.message = msg.MessageTextContent;

                return response;
            }
            if (oldDailyRun != null && newDailyRun != null && (oldDailyRun.MasterSite_Guid != newDailyRun.MasterSite_Guid))
            {
                msg = DbContext.TblSystemMessage.FirstOrDefault(e => e.MsgID == -2158 && e.SystemLanguage_Guid == languageGuid);
                response.isSuccess = false;

                if (msg != null)
                    response.message = msg.MessageTextContent;

                return response;
            }
            return response;
        }

        public TruckToTruckTransferResponse TruckToTruckIsValidJobStatus(IEnumerable<TblMasterActualJobHeader> jobList, Guid languageGuid)
        {
            var response = new TruckToTruckTransferResponse();
            var allowedJobStatus = new int?[] { JobStatusHelper.OnTruck, JobStatusHelper.OnTruckPickUp, JobStatusHelper.OnTruckDelivery, JobStatusHelper.InPreVault, JobStatusHelper.InPreVaultPickUp, JobStatusHelper.InPreVaultDelivery };
            var hasNotAllowedJobStatus = jobList.Any(e => !allowedJobStatus.Contains(e.SystemStatusJobID));
            if (hasNotAllowedJobStatus)
            {
                var msg = DbContext.TblSystemMessage.FirstOrDefault(e => e.MsgID == -2159 && e.SystemLanguage_Guid == languageGuid);
                response.isSuccess = false;

                if (msg != null)
                    //Wating update message 21.2
                    response.message = "Only job with status On Truck, On Truck - Pick Up, On Truck - Delivery, In-Pre Vault, In Pre-Vault - Pick Up and In Pre-Vault - Delivery are allowed"; // msg.MessageTextContent;
            }

            return response;
        }

        public TruckToTruckTransferResponse TruckToTruckIsValidlegListMustBeInOldRun(List<Guid> legGuidList, Guid oldDailyRunGuid, Guid languageGuid)
        {
            var response = new TruckToTruckTransferResponse();

            var oldDailyRun = DbContext.TblMasterActualJobServiceStopLegs.Where(e => e.MasterRunResourceDaily_Guid == oldDailyRunGuid).Select(e => e.Guid).AsEnumerable().ToList();

            if (legGuidList.Any(e => !oldDailyRun.Contains(e))) // loop legGuidList check if any leg not contain in run
            {
                var msg = DbContext.TblSystemMessage.FirstOrDefault(e => e.MsgID == -2162 && e.SystemLanguage_Guid == languageGuid);
                response.isSuccess = false;

                if (msg != null)
                    response.message = msg.MessageTextContent;
            }

            return response;
        }


        public TruckToTruckTransferResponse TruckToTruckIsValidServiceJobType(IEnumerable<TblMasterActualJobHeader> jobList, Guid newDailyRunGuid, Guid languageGuid)
        {
            var response = new TruckToTruckTransferResponse();

            #region Validate service type is not allowed
            var allowedServiceType = new int?[] { IntTypeJob.P, IntTypeJob.D, IntTypeJob.T, IntTypeJob.TV, IntTypeJob.BCD, IntTypeJob.BCP };
            var hasNotAllowedServiceType = jobList.Any(e => !allowedServiceType.Contains(e.JobTypeID));
            if (hasNotAllowedServiceType)
            {
                var msg = DbContext.TblSystemMessage.FirstOrDefault(e => e.MsgID == -2160 && e.SystemLanguage_Guid == languageGuid);
                response.isSuccess = false;

                if (msg != null)
                    response.message = msg.MessageTextContent;

                return response;
            }
            #endregion

            #region Validate same TV job cannot in the same run
            IEnumerable<Guid> jobGuidList = jobList.Where(e => e.JobTypeID == IntTypeJob.TV).Select(x => x.Guid);
            var hasSameTVjob = DbContext.TblMasterActualJobServiceStopLegs.Any(e => e.MasterRunResourceDaily_Guid == newDailyRunGuid
                                && jobGuidList.Contains((Guid)e.MasterActualJobHeader_Guid) && (e.SequenceStop == 1 || e.FlagDestination));
            if (hasSameTVjob)
            {
                var msg = DbContext.TblSystemMessage.FirstOrDefault(e => e.MsgID == -2161 && e.SystemLanguage_Guid == languageGuid);
                response.isSuccess = false;

                if (msg != null)
                    response.message = msg.MessageTextContent;

                return response;
            }
            #endregion

            return response;
        }
        #endregion

        public IEnumerable<RouteGroupDetailRunResourceView> GetRouteGroupDetailByWorkDate(Guid siteGuid, DateTime workDate)
        {
            return DbContext.TblMasterDailyRunResource.Where(e => e.WorkDate != null && (DateTime)e.WorkDate == workDate.Date
                                                       && e.MasterSite_Guid == siteGuid && e.FlagDisable != true)
                           .Join(DbContext.TblMasterRunResource.Where(e => e.Flag3Party == false),
                           dr => dr.MasterRunResource_Guid,
                           rr => rr.Guid,
                           (dr, rr) => new { dr.Guid, dr.MasterRouteGroup_Detail_Guid, dr.MasterRunResourceShift, rr.VehicleNumber })
                           .Join(DbContext.TblMasterRouteGroup_Detail,
                           drr => drr.MasterRouteGroup_Detail_Guid,
                           rgd => rgd.Guid,
                           (ddr, rgd) => new { ddr, rgd.MasterRouteGroupDetailName, rgd.MasterRouteGroup_Guid })
                           .Join(DbContext.TblMasterRouteGroup,
                           rungp => rungp.MasterRouteGroup_Guid,
                           rg => rg.Guid,
                           (rungp, rg) => new RouteGroupDetailRunResourceView
                           {
                               DailyRunGuid = rungp.ddr.Guid,
                               RouteGroupDetailName = rungp.MasterRouteGroupDetailName,
                               VehicleNumber = rungp.ddr.VehicleNumber,
                               RunResourceShitf = (int)rungp.ddr.MasterRunResourceShift,
                               RouteGroupGuid = rg.Guid,
                               RouteGroupName = rg.MasterRouteGroupName
                           }).OrderBy(o => o.VehicleNumber).OrderBy(o => o.RunResourceShitf).AsEnumerable();

        }

        public IEnumerable<RouteGroupDetailRunResourceView> GetRouteGroupDetailByRunGuid(Guid runGuid)
        {
            return DbContext.TblMasterDailyRunResource.Where(e => e.Guid == runGuid)
                           .Join(DbContext.TblMasterRunResource.Where(e => e.Flag3Party == false),
                           dr => dr.MasterRunResource_Guid,
                           rr => rr.Guid,
                           (dr, rr) => new { dr.Guid, dr.MasterRouteGroup_Detail_Guid, dr.MasterRunResourceShift, rr.VehicleNumber })
                           .Join(DbContext.TblMasterRouteGroup_Detail,
                           drr => drr.MasterRouteGroup_Detail_Guid,
                           rgd => rgd.Guid,
                           (ddr, rgd) => new { ddr, rgd.MasterRouteGroupDetailName, rgd.MasterRouteGroup_Guid })
                           .Join(DbContext.TblMasterRouteGroup,
                           rungp => rungp.MasterRouteGroup_Guid,
                           rg => rg.Guid,
                           (rungp, rg) => new RouteGroupDetailRunResourceView
                           {
                               DailyRunGuid = rungp.ddr.Guid,
                               RouteGroupDetailName = rungp.MasterRouteGroupDetailName,
                               VehicleNumber = rungp.ddr.VehicleNumber,
                               RunResourceShitf = (int)rungp.ddr.MasterRunResourceShift,
                               RouteGroupGuid = rg.Guid,
                               RouteGroupName = rg.MasterRouteGroupName
                           }).OrderBy(o => o.VehicleNumber).OrderBy(o => o.RunResourceShitf).AsEnumerable();

        }

        private IEnumerable<TblMasterCurrency_ExchangeRate> GetCurrencyExchangeList(Guid contryGuid)
        {
            var global_exchange = DbContext.TblMasterCurrency_ExchangeRate.Where(o => !o.FlagDisable && o.MasterCountry_Guid == null).ToList();
            var local_exchange = DbContext.TblMasterCurrency_ExchangeRate.Where(o => !o.FlagDisable && o.MasterCountry_Guid == contryGuid).ToList();


            var include_global = global_exchange.Where(o => !local_exchange.Any(t => t.MasterCurrencySource_Guid == o.MasterCurrencySource_Guid && t.MasterCurrencyTarget_Guid == o.MasterCurrencyTarget_Guid))
                                                .Select(o => { o.MasterCountry_Guid = contryGuid; return o; });

            var merge_exchange = local_exchange.Union(include_global);

            return merge_exchange;
        }

        public IEnumerable<DailyRouteView> GetDailyRoute_For_Optimization(DailyRouteRequest request, Guid languageGuid, bool validateRunLiabilityLimit)
        {
            string currencyOfUser = "";
            var countryGuid = DbContext.TblMasterSite.FirstOrDefault(e => e.Guid == request.SiteGuid).MasterCountry_Guid;
            var currentExchange = GetCurrencyExchangeList(countryGuid);
            var Template = DbContext.GetAllCommodityBySite(request.SiteGuid, countryGuid).ToList();

            var result = (from DailyRun in DbContext.TblMasterDailyRunResource.Where(e => e.WorkDate != null && (DateTime)e.WorkDate == request.WorkDate.Date
                                                       && e.MasterSite_Guid == request.SiteGuid && e.FlagDisable != true)
                          join RouteGroupDetail in DbContext.TblMasterRouteGroup_Detail on DailyRun.MasterRouteGroup_Detail_Guid equals RouteGroupDetail.Guid
                          join RouteGroup in DbContext.TblMasterRouteGroup on RouteGroupDetail.MasterRouteGroup_Guid equals RouteGroup.Guid
                          join MasterRun in DbContext.TblMasterRunResource on DailyRun.MasterRunResource_Guid equals MasterRun.Guid
                          join ServiceStopLeg in DbContext.TblMasterActualJobServiceStopLegs.Select(e => new { e.MasterActualJobHeader_Guid, e.MasterRunResourceDaily_Guid, e.MasterCustomerLocation_Guid, e.WindowsTimeServiceTimeStart }).Distinct() on DailyRun.Guid equals ServiceStopLeg.MasterRunResourceDaily_Guid
                          into ServiceStopLegleft
                          from ServiceStopLeg in ServiceStopLegleft.DefaultIfEmpty()
                          join JobHeader in DbContext.TblMasterActualJobHeader on ServiceStopLeg.MasterActualJobHeader_Guid equals JobHeader.Guid
                          join Currency in DbContext.TblMasterCurrency on MasterRun.LiabilityLimitCurrency_Guid equals Currency.Guid
                          into Currencyleft
                          from Currency in Currencyleft.DefaultIfEmpty()
                          join RouteOp in DbContext.TblSystemRouteOptimizationStatus on DailyRun.SystemRouteOptimizationStatus_Guid equals RouteOp.Guid
                          into RouteOpleft
                          from RouteOp in RouteOpleft.DefaultIfEmpty(DbContext.TblSystemRouteOptimizationStatus.Where(e => e.RouteOptimizationStatusID == 0).FirstOrDefault())
                          join RouteOpLang in DbContext.TblSystemDisplayTextControlsLanguage.Where(e => e.SystemLanguageGuid == languageGuid) on RouteOp.SystemDisplayTextControlsLanguage_Guid equals RouteOpLang.Guid
                          into RouteOpLangleft
                          from RouteOpLang in RouteOpLangleft.DefaultIfEmpty()
                          join CusLo in DbContext.TblMasterCustomerLocation on ServiceStopLeg.MasterCustomerLocation_Guid equals CusLo.Guid
                          into CusLoleft
                          from CusLo in CusLoleft.DefaultIfEmpty()
                          join Cus in DbContext.TblMasterCustomer on CusLo.MasterCustomer_Guid equals Cus.Guid
                          into Cusleft
                          from Cus in Cusleft.DefaultIfEmpty()
                          select new
                          {
                              DailyRun.Guid,
                              DailyRun.MasterRunResourceShift,
                              DailyRun.RunResourceDailyStatusID,
                              DailyRun.FlagRouteBalanceDone,
                              DailyRun.StartTime,
                              RouteGroupDetailGuid = RouteGroupDetail.Guid,
                              RouteGroupDetail.MasterRouteGroupDetailName,
                              RouteGroupGuid = RouteGroup.Guid,
                              RouteGroup.MasterRouteGroupName,
                              MasterRun.VehicleNumber,
                              ServiceStopLeg,
                              JobHeader.SaidToContain,
                              Currency.MasterCurrencyAbbreviation,
                              RouteOpLang.DisplayText,
                              RouteOp.RouteOptimizationStatusID,
                              Cus.FlagChkCustomer,
                              jobHeaderGuid = JobHeader.Guid,
                              CurrencyOnrun = MasterRun.LiabilityLimitCurrency_Guid
                          }).GroupBy(e => new { e.jobHeaderGuid, e.Guid, e.MasterRunResourceShift, e.RunResourceDailyStatusID, e.FlagRouteBalanceDone, e.StartTime, e.RouteGroupDetailGuid, e.MasterRouteGroupDetailName, e.RouteGroupGuid, e.MasterRouteGroupName, e.VehicleNumber, e.CurrencyOnrun }).AsEnumerable().Select(e => new DailyRouteView()
                          {
                              Guid = e.Key.Guid,
                              RouteGroupGuid = e.Key.RouteGroupGuid,
                              MasterRouteGroupName = e.Key.MasterRouteGroupName,
                              RouteGroupDetailGuid = e.Key.RouteGroupDetailGuid,
                              MasterRouteGroupDetailName = e.Key.MasterRouteGroupDetailName,
                              RunNo = (e.Key.MasterRunResourceShift > 1) ? e.Key.VehicleNumber + " (S" + e.Key.MasterRunResourceShift + ")" : e.Key.VehicleNumber,
                              RunStatusId = e.Key.RunResourceDailyStatusID.Value,
                              Balanced = e.Key.FlagRouteBalanceDone ? "Yes" : "No",
                              StartTime = e.Key.StartTime,
                              Currency = (e.Select(f => f.MasterCurrencyAbbreviation).Distinct().Count() > 1) ? "*" : e.FirstOrDefault().MasterCurrencyAbbreviation,
                              OptimizationStatus = e.FirstOrDefault().DisplayText,
                              OptimizationStatusId = e.FirstOrDefault().RouteOptimizationStatusID,
                              Jobs = (e.FirstOrDefault().ServiceStopLeg != null) ? e.Select(f => f.ServiceStopLeg?.MasterActualJobHeader_Guid).Distinct().Count() : 0,
                              Locations = (e.FirstOrDefault().ServiceStopLeg != null) ? e.Where(f => f.ServiceStopLeg.MasterCustomerLocation_Guid.HasValue && f.FlagChkCustomer.GetValueOrDefault()).Select(g => g.ServiceStopLeg?.MasterCustomerLocation_Guid).Distinct().Count() : 0,
                              Stops = (e.FirstOrDefault().ServiceStopLeg != null) ? e.Where(f => f.ServiceStopLeg.MasterCustomerLocation_Guid.HasValue && f.FlagChkCustomer.GetValueOrDefault()).Select(g => g.ServiceStopLeg?.MasterCustomerLocation_Guid.ToString() + "_" + g.ServiceStopLeg?.WindowsTimeServiceTimeStart.ToString()).Distinct().Count() : 0,
                              JobHeaderGuid = e.Key.jobHeaderGuid,
                              CurrencyOnrunGuid = e.Key.CurrencyOnrun
                          });

            var jobGuidList = result.Select(e => e.JobHeaderGuid).ToList();
            var ItemLiability = DbContext.TblMasterActualJobItemsLiability.Where(e => jobGuidList.Contains(e.MasterActualJobHeader_Guid))
                                .Select(o => new ItemsLibilityView
                                {
                                    DocCurrencyGuid = o.MasterCurrency_Guid,
                                    Liability = o.Liability ?? 0,
                                    LibilityGuid = o.Guid,
                                    JobGuid = o.MasterActualJobHeader_Guid
                                }).ToList();
            List<CurrencyOfDailyRunResourceView> currencyOnRun = null;
            if (!validateRunLiabilityLimit)
            {
                var userCurrency = DbContext.TblMasterUser.FirstOrDefault(o => o.Guid == ApiSession.UserGuid);
                currencyOfUser = DbContext.TblMasterCurrency.FirstOrDefault(f => f.Guid == userCurrency.MasterCurrency_Default_Guid).MasterCurrencyAbbreviation;

                var checkOneCurrency = ItemLiability.Where(w => w.DocCurrencyGuid.HasValue)
                    .GroupBy(g => new { g.JobGuid, g.DocCurrencyGuid }).Select(s => new
                    {
                        JobGuid = s.Key.JobGuid.Value,
                        DocCurrencyGuid = s.Key.DocCurrencyGuid.Value
                    })?.ToList();
                var currencyFromLiability = checkOneCurrency
                        .Join(DbContext.TblMasterCurrency
                        , cc => cc.DocCurrencyGuid
                        , c => c.Guid,
                        (cc, c) =>
                        new
                        {
                            cc.JobGuid,
                            c.MasterCurrencyAbbreviation,
                            c.Guid
                        })?.ToList();

                //Group by currency by run
                var dr = result.Select(
                    s => new
                    {
                        s.Guid,
                        s.JobHeaderGuid
                    });
                currencyOnRun = (from r in dr
                                 join c in currencyFromLiability on r.JobHeaderGuid equals c.JobGuid
                                 group new { r, c } by new { r.Guid, c.MasterCurrencyAbbreviation } into grc
                                 where grc.Any()
                                 select new CurrencyOfDailyRunResourceView
                                 {
                                     DailyRunGuid = grc.Key.Guid,
                                     CurrencyAbb = grc.Key.MasterCurrencyAbbreviation
                                 }
                  )?.ToList();
            }


            var Commudity = DbContext.TblMasterActualJobItemsCommodity.Where(e => jobGuidList.Contains(e.MasterActualJobHeader_Guid))
                            .Select(o => new ItemsCommodityView
                            {
                                ActualCommodityGuid = o.Guid,
                                CommodityGuid = o.MasterCommodity_Guid,
                                FlagCommodityDiscrepancies = o.FlagCommodityDiscrepancies ?? false,
                                Quantity = o.Quantity ?? 0,
                                QuantityActual = o.QuantityActual ?? 0,
                                QuantityExpected = o.QuantityExpected ?? 0,
                                JobGuid = o.MasterActualJobHeader_Guid
                            }).ToList();

            return result.Select(e =>
            {
                e.STC = (decimal)(new RawJobDataView()
                {
                    Target_CurrencyGuid = e.CurrencyOnrunGuid,
                    JobItems = new RawItemsView()
                    {
                        Commodities = Commudity.Where(o => o.JobGuid == e.JobHeaderGuid),
                        Liabilities = ItemLiability.Where(o => o.JobGuid == e.JobHeaderGuid)
                    },
                }).CalculateJobSTC(currentExchange, Template, validateRunLiabilityLimit).TotalJobSTC;
                if (!validateRunLiabilityLimit)
                {
                    var currency = currencyOnRun.Count(c=>c.DailyRunGuid == e.Guid);
                    if (currency == 1)
                    {
                        e.Currency = currencyOnRun.FirstOrDefault(f => f.DailyRunGuid == e.Guid).CurrencyAbb; ;
                    }
                    else if (currency > 1)
                    {
                        e.Currency = "*";
                    }
                    else
                    {
                        e.Currency = currencyOfUser;
                    }
                }
                return e;
            }).GroupBy(e => e.Guid).Select(e => new DailyRouteView()
            {
                Guid = e.Key,
                RouteGroupGuid = e.FirstOrDefault().RouteGroupGuid,
                MasterRouteGroupName = e.FirstOrDefault().MasterRouteGroupName,
                RouteGroupDetailGuid = e.FirstOrDefault().RouteGroupDetailGuid,
                MasterRouteGroupDetailName = e.FirstOrDefault().MasterRouteGroupDetailName,
                RunNo = e.FirstOrDefault().RunNo,
                RunStatusId = e.FirstOrDefault().RunStatusId,
                RunStatus = GetRunStatus(e.FirstOrDefault().RunStatusId),
                Balanced = e.FirstOrDefault().Balanced,
                StartTime = e.FirstOrDefault().StartTime,
                STC = e.Sum(f => f.STC),
                Currency = e.FirstOrDefault().Currency,
                OptimizationStatus = e.FirstOrDefault().OptimizationStatus,
                OptimizationStatusId = e.FirstOrDefault().OptimizationStatusId,
                Jobs = e.Sum(f => f.Jobs),
                Locations = e.Sum(f => f.Locations),
                Stops = e.Sum(f => f.Stops),
                StrStops = e.Sum(f => f.Stops).ToString(),
                StrJobs = e.Sum(f => f.Jobs).ToString(),
                StrLocations = e.Sum(f => f.Locations).ToString(),
                StrStartTime = e.FirstOrDefault().StartTime.Value.ToString("HH:mm"),
                StrSTCDisplay = e.Sum(f => f.STC) + " " + e.FirstOrDefault().Currency,
                FlagShowAllRunStatus = (e.FirstOrDefault().RunStatusId != EnumRun.StatusDailyRun.ReadyRun),
                FlagShowAllOptStatus = (!(e.FirstOrDefault().OptimizationStatusId == OptimizationStatusID.NONE || e.FirstOrDefault().OptimizationStatusId == OptimizationStatusID.COMPLETED || e.FirstOrDefault().OptimizationStatusId == OptimizationStatusID.BROKEN || e.FirstOrDefault().OptimizationStatusId == OptimizationStatusID.CANCELED || e.FirstOrDefault().OptimizationStatusId == OptimizationStatusID.FAILED))
            }).OrderBy(e => e.RouteGroupGuid).ThenBy(e => e.RouteGroupDetailGuid).ThenBy(e => e.RunNo);
        }
        private string GetRunStatus(int runId)
        {
            string str = "";
            switch (runId)
            {
                case EnumRun.StatusDailyRun.ReadyRun:
                    str = "Ready";
                    break;
                case EnumRun.StatusDailyRun.ClosedRun:
                    str = "Closed";
                    break;
                case EnumRun.StatusDailyRun.CrewBreak:

                    str = "CrewBreak";
                    break;
                case EnumRun.StatusDailyRun.DispatchRun:
                    str = "Dispatch";
                    break;
                default:
                    str = "Error";
                    break;
            }
            return str;
        }

        public IEnumerable<JobUnassignedView> GetUnassigned_For_Optimization(DailyRouteRequest request, Guid languageGuid, bool validateRunLiabilityLimit)
        {
            var countryGuid = DbContext.TblMasterSite.FirstOrDefault(e => e.Guid == request.SiteGuid).MasterCountry_Guid;
            var currentExchange = GetCurrencyExchangeList(countryGuid);
            var Template = DbContext.GetAllCommodityBySite(request.SiteGuid, countryGuid).ToList();
            var defaultCurrency = DbContext.TblMasterUser.FirstOrDefault(o => o.Guid == ApiSession.UserGuid);
            var defaultCurrencyApp = DbContext.TblMasterCurrency.FirstOrDefault(f => f.Guid == defaultCurrency.MasterCurrency_Default_Guid).MasterCurrencyAbbreviation;
            var result = (from jobHeader in DbContext.TblMasterActualJobHeader.Where(e => e.TransectionDate == request.WorkDate)
                          join Legs in DbContext.TblMasterActualJobServiceStopLegs on jobHeader.Guid equals Legs.MasterActualJobHeader_Guid
                          join CusLo in DbContext.TblMasterCustomerLocation on Legs.MasterCustomerLocation_Guid equals CusLo.Guid
                          join Cus in DbContext.TblMasterCustomer on CusLo.MasterCustomer_Guid equals Cus.Guid
                          join jobAction in DbContext.TblSystemJobAction on Legs.CustomerLocationAction_Guid equals jobAction.Guid
                          join JobType in DbContext.TblSystemServiceJobType on jobHeader.SystemServiceJobType_Guid equals JobType.Guid
                          join LOB in DbContext.TblSystemLineOfBusiness on jobHeader.SystemLineOfBusiness_Guid equals LOB.Guid
                          join RouteOp in DbContext.TblSystemRouteOptimizationStatus on Legs.SystemRouteOptimizationStatus_Guid equals RouteOp.Guid
                          into RouteOpleft
                          from RouteOp in RouteOpleft.DefaultIfEmpty(DbContext.TblSystemRouteOptimizationStatus.Where(e => e.RouteOptimizationStatusID == 0).FirstOrDefault())
                          join RouteOpLang in DbContext.TblSystemDisplayTextControlsLanguage.Where(e => e.SystemLanguageGuid == languageGuid) on RouteOp.SystemDisplayTextControlsLanguage_Guid equals RouteOpLang.Guid
                          into RouteOpLangleft
                          from RouteOpLang in RouteOpLangleft.DefaultIfEmpty()
                          where Cus.FlagChkCustomer.Value && Legs.MasterSite_Guid == request.SiteGuid && Legs.MasterRunResourceDaily_Guid == null
                          select new
                          {
                              JobGuid = Legs.Guid,
                              JobNo = jobHeader.JobNo,
                              STC = jobHeader.SaidToContain,
                              Action = jobAction.ActionNameAbbrevaition,
                              ServiceJobTypeAbbr = JobType.ServiceJobTypeNameAbb,
                              LOBFullName = LOB.LOBFullName,
                              CusLo.BranchName,
                              Cus.CustomerFullName,
                              //MasterCurrencyAbbreviation = validateRunLiabilityLimit ? "" : Currency.MasterCurrencyAbbreviation,
                              RouteOp.RouteOptimizationStatusID,
                              RouteOptimizationStatusName = RouteOpLang.DisplayText,
                              jobHeaderGuid = jobHeader.Guid
                          }).AsEnumerable().GroupBy(e => new { e.jobHeaderGuid, e.JobGuid, e.JobNo, e.STC, e.Action, e.ServiceJobTypeAbbr, e.LOBFullName, e.BranchName, e.CustomerFullName, e.RouteOptimizationStatusID, e.RouteOptimizationStatusName })
                          .Select(e => new JobUnassignedView()
                          {
                              JobGuid = e.Key.JobGuid,
                              Action = e.Key.Action,
                              CustomerLocationDisplay = e.Key.CustomerFullName + " - " + e.Key.BranchName,
                              JobNo = e.Key.JobNo,
                              LOBFullName = e.Key.LOBFullName,
                              OptimizationStatus = e.Key.RouteOptimizationStatusName,
                              OptimizationStatusId = e.Key.RouteOptimizationStatusID,
                              ServiceJobTypeAbbr = e.Key.ServiceJobTypeAbbr,
                              FlagShowAllStatus = (e.Key.RouteOptimizationStatusID != OptimizationStatusID.NONE),
                              JobHeaderGuid = e.Key.jobHeaderGuid
                          });

            var jobGuidList = result.Select(e => e.JobHeaderGuid).ToList();
            var ItemLiability = DbContext.TblMasterActualJobItemsLiability.Where(e => jobGuidList.Contains(e.MasterActualJobHeader_Guid))
                                .Select(o => new ItemsLibilityView
                                {
                                    DocCurrencyGuid = o.MasterCurrency_Guid,
                                    Liability = o.Liability ?? 0,
                                    LibilityGuid = o.Guid,
                                    JobGuid = o.MasterActualJobHeader_Guid
                                }).ToList();
            // check only one currency in job.
            var checkOneCurrency = ItemLiability.Where(w => w.DocCurrencyGuid.HasValue)
                .GroupBy(g => new { g.JobGuid, g.DocCurrencyGuid }).Select(s => new
                {
                    JobGuid = s.Key.JobGuid.Value,
                    DocCurrencyGuid = s.Key.DocCurrencyGuid.Value
                })?.ToList();
            var currencyFromLiability = checkOneCurrency
                    .Join(DbContext.TblMasterCurrency
                    , cc => cc.DocCurrencyGuid
                    , c => c.Guid,
                    (cc, c) =>
                                        new
                                        {
                                            cc.JobGuid,
                                            c.MasterCurrencyAbbreviation,
                                            c.Guid
                                        })?.ToList();


            var Commudity = DbContext.TblMasterActualJobItemsCommodity.Where(e => jobGuidList.Contains(e.MasterActualJobHeader_Guid))
                            .Select(o => new ItemsCommodityView
                            {
                                ActualCommodityGuid = o.Guid,
                                CommodityGuid = o.MasterCommodity_Guid,
                                FlagCommodityDiscrepancies = o.FlagCommodityDiscrepancies ?? false,
                                Quantity = o.Quantity ?? 0,
                                QuantityActual = o.QuantityActual ?? 0,
                                QuantityExpected = o.QuantityExpected ?? 0,
                                JobGuid = o.MasterActualJobHeader_Guid
                            }).ToList();

            return result.Select(e =>
            {
                e.STC = (decimal)(new RawJobDataView()
                {
                    Target_CurrencyGuid = defaultCurrency.MasterCurrency_Default_Guid,
                    JobItems = new RawItemsView()
                    {
                        Commodities = Commudity.Where(o => o.JobGuid == e.JobHeaderGuid),
                        Liabilities = ItemLiability.Where(o => o.JobGuid == e.JobHeaderGuid)
                    }
                }).CalculateJobSTC(currentExchange, Template, validateRunLiabilityLimit).TotalJobSTC;
                if (validateRunLiabilityLimit)
                {
                    e.Currency = defaultCurrencyApp;
                }
                else
                {
                    var count = checkOneCurrency.Count(c => c.JobGuid == e.JobHeaderGuid);
                    if (count == 1)
                    {
                        e.Currency = currencyFromLiability.FirstOrDefault(f => f.JobGuid == e.JobHeaderGuid).MasterCurrencyAbbreviation;
                    }
                    else if (count > 1)
                    {
                        e.Currency = "*";
                    }
                    else
                    {
                        e.Currency = defaultCurrencyApp;
                    }
                }
                return e;
            });

        }


        public Guid? FindDefaultCurrencyByUser(Guid? userGuid)
        {
            return DbContext.TblMasterUser.FirstOrDefault(o => o.Guid == userGuid)?.MasterCurrency_Default_Guid;
        }

        public IEnumerable<RawJobDataView> GetLiabilityLimitRawJobsData(Guid? siteGuid, IEnumerable<LiabilityLimitNoJobsAction> model)
        {
            var userFormatDate = ApiSession.UserFormatDate;
            var _model = model.Select(o =>
            {
                return new { JobGuid = o.JobData.JobGuid, o.DailyRunGuid_Target };
            });
            var defaultCurrencyGuid = FindDefaultCurrencyByUser(ApiSession.UserGuid);

            var dailyRunGuids = model.GetDailyRunTargetGuids();
            var _baseDailyRuns = DbContext.TblMasterDailyRunResource.Where(o => dailyRunGuids.Any(d => o.Guid == d)); // don't ToList
            var routeGuids = _baseDailyRuns.Select(o => o.MasterRouteGroup_Detail_Guid).Distinct();
            var defaultRGD = new { rgGuid = Guid.Empty, rgName = string.Empty, rgdGuid = Guid.Empty, rgdName = string.Empty };
            var allRgd = (from _rg in DbContext.TblMasterRouteGroup
                          join _rgd in DbContext.TblMasterRouteGroup_Detail on _rg.Guid equals _rgd.MasterRouteGroup_Guid
                          where routeGuids.Any(o => o == _rgd.Guid)
                          select new { rgGuid = _rg.Guid, rgName = _rg.MasterRouteGroupName, rgdGuid = _rgd.Guid, rgdName = _rgd.MasterRouteGroupDetailName }).ToList();


            var _baseRun = (from dailyRun in DbContext.TblMasterDailyRunResource.Where(o => dailyRunGuids.Any(d => o.Guid == d))
                            join run in DbContext.TblMasterRunResource on dailyRun.MasterRunResource_Guid equals run.Guid
                            join site in DbContext.TblMasterSite on dailyRun.MasterSite_Guid equals site.Guid
                            select new { d = dailyRun, r = run, s = site }).AsEnumerable()
                          .Select(o =>
                          {
                              var _rgd = allRgd.FirstOrDefault(e => e.rgdGuid == o.d.MasterRouteGroup_Detail_Guid) ?? defaultRGD;
                              var routeDetail = _rgd.rgdName;
                              return new
                              {
                                  DailyRunGuid = o.d.Guid,
                                  o.r.LiabilityLimit,
                                  LiabilityLimitCurrency_Guid = o.r.LiabilityLimitCurrency_Guid ?? defaultCurrencyGuid,
                                  ObjWorkDate = o.d.WorkDate,
                                  RunNo = o.r.VehicleNumber + (o.d.MasterRunResourceShift > 1 ? $" ({o.d.MasterRunResourceShift})" : string.Empty),
                                  RunStatusID = o.d.RunResourceDailyStatusID,
                                  o.d.MasterSite_Guid,
                                  SiteName = o.s.SiteCode + " - " + o.s.SiteName,
                                  RouteDetail = routeDetail
                              };
                          }).ToList();

            var _docCurrGuids = model.SelectMany(o => o.JobData.JobItems.Liabilities.Select(d => d.DocCurrencyGuid)).Distinct();
            var allCurrGuids = _baseRun.Select(o => o.LiabilityLimitCurrency_Guid)
                                 .Union(_docCurrGuids)
                                 .Union(new Guid?[] { defaultCurrencyGuid });

            var locGuids = model.Select(o => o.JobData.LocationGuid);
            var _baseCurr = DbContext.TblMasterCurrency.Where(o => allCurrGuids.Any(c => c == o.Guid)).ToList();
            var _baseLoc = (from loc in DbContext.TblMasterCustomerLocation
                            join cus in DbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cus.Guid
                            where locGuids.Any(o => o == loc.Guid)
                            select new
                            {
                                LocationGuid = loc.Guid,
                                Location = cus.CustomerFullName + " - " + loc.BranchName
                            }).ToList();

            var jobOutRun = model.Select(o => o.JobData).Select(o =>
            {
                var location = _baseLoc.FirstOrDefault(l => l.LocationGuid == o.LocationGuid)?.Location;
                var target_run = _baseRun.FirstOrDefault(r => _model.Any(m => m.DailyRunGuid_Target == r.DailyRunGuid && m.JobGuid == o.JobGuid));
                var target_Curr = _baseCurr.FirstOrDefault(c => c.Guid == target_run?.LiabilityLimitCurrency_Guid);

                o.Target_CurrencyGuid = target_run?.LiabilityLimitCurrency_Guid;
                o.Target_CurrencyAbb = target_Curr?.MasterCurrencyAbbreviation;
                o.Target_DailyRunGuid = target_run?.DailyRunGuid;
                o.Target_DailyRunSiteGuid = target_run?.MasterSite_Guid;
                o.Target_DailyRunSiteName = target_run?.SiteName;
                o.Target_RunLibilityLimit = Convert.ToDouble(target_run?.LiabilityLimit);
                o.Target_RunNo = target_run?.RunNo;
                o.Target_RouteDetail = target_run?.RouteDetail;
                o.Target_WorkDate = target_run?.ObjWorkDate?.ChangeFromDateToString(userFormatDate);
                o.FlagJobOutRun = true;

                //o.JobTypeID
                //o.FlagDestination
                //o.JobStatusID
                //o.JobGuid
                //o.JobNo
                //o.DailyRunGuid
                o.Location = location;
                //o.JobAction
                return o;
            });

            return jobOutRun;
        }



        public IEnumerable<RawJobDataView> GetLiabilityLimitRawJobsData(Guid? siteGuid, IEnumerable<LiabilityLimitJobsAction> model)
        {
            var userFormatDate = ApiSession.UserFormatDate;

            var defaultCurrencyGuid = FindDefaultCurrencyByUser(ApiSession.UserGuid);
            var _model = model.SelectMany(o => o.JobGuids.Select(j => new { JobGuid = j.JobGuid, o.DailyRunGuid_Target, o.DailyRunGuid_Source, j.JobAction }));
            var dailyRunGuids = model.GetDailyRunTargetGuids();
            var _baseDailyRuns = DbContext.TblMasterDailyRunResource.Where(o => dailyRunGuids.Any(d => o.Guid == d)); // don't ToList
            var routeGuids = _baseDailyRuns.Select(o => o.MasterRouteGroup_Detail_Guid).Distinct();
            var defaultRGD = new { rgGuid = Guid.Empty, rgName = string.Empty, rgdGuid = Guid.Empty, rgdName = string.Empty };
            var allRgd = (from _rg in DbContext.TblMasterRouteGroup
                          join _rgd in DbContext.TblMasterRouteGroup_Detail on _rg.Guid equals _rgd.MasterRouteGroup_Guid
                          where routeGuids.Any(o => o == _rgd.Guid)
                          select new { rgGuid = _rg.Guid, rgName = _rg.MasterRouteGroupName, rgdGuid = _rgd.Guid, rgdName = _rgd.MasterRouteGroupDetailName }).ToList();

            var jobGuids = DbContext.TblMasterActualJobServiceStopLegs.Where(o => dailyRunGuids.Any(d => o.MasterRunResourceDaily_Guid == d)).Select(o => o.MasterActualJobHeader_Guid);
            var allJobGuids = model.GetMergeJobGuids(jobGuids);

            var _baseJobs = DbContext.TblMasterActualJobHeader.Where(o => allJobGuids.Any(i => o.Guid == i)).ToList();
            var _baseLegsCus = (from leg in DbContext.TblMasterActualJobServiceStopLegs
                                join loc in DbContext.TblMasterCustomerLocation on leg.MasterCustomerLocation_Guid equals loc.Guid
                                join cus in DbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cus.Guid
                                join action in DbContext.TblSystemJobAction on leg.CustomerLocationAction_Guid equals action.Guid
                                where cus.FlagChkCustomer == true && allJobGuids.Any(o => o == leg.MasterActualJobHeader_Guid)
                                select new { leg, cus, loc, action }).ToList();

            var _baseType = DbContext.TblSystemServiceJobType.ToList();
            var _baseRun = (from dailyRun in _baseDailyRuns
                            join run in DbContext.TblMasterRunResource on dailyRun.MasterRunResource_Guid equals run.Guid
                            join site in DbContext.TblMasterSite on dailyRun.MasterSite_Guid equals site.Guid
                            select new { d = dailyRun, r = run, s = site }).AsEnumerable()
                            .Select(o =>
                            {
                                var _rgd = allRgd.FirstOrDefault(e => e.rgdGuid == o.d.MasterRouteGroup_Detail_Guid) ?? defaultRGD;
                                var routeDetail = _rgd.rgdName;
                                return new
                                {
                                    DailyRunGuid = o.d.Guid,
                                    o.r.LiabilityLimit,
                                    LiabilityLimitCurrency_Guid = o.r.LiabilityLimitCurrency_Guid ?? defaultCurrencyGuid,
                                    ObjWorkDate = o.d.WorkDate,
                                    RunNo = o.r.VehicleNumber + (o.d.MasterRunResourceShift > 1 ? $" (S{o.d.MasterRunResourceShift})" : string.Empty),
                                    RunStatusID = o.d.RunResourceDailyStatusID,
                                    o.d.MasterSite_Guid,
                                    SiteName = o.s.SiteCode + " - " + o.s.SiteName,
                                    RouteDetail = routeDetail
                                };
                            }).ToList();

            var _baseLia = DbContext.TblMasterActualJobItemsLiability.Where(o => allJobGuids.Any(i => o.MasterActualJobHeader_Guid == i)).ToList();
            var _baseComm = DbContext.TblMasterActualJobItemsCommodity.Where(o => allJobGuids.Any(i => o.MasterActualJobHeader_Guid == i)).ToList();
            var allCurrGuids = _baseRun.Select(o => o.LiabilityLimitCurrency_Guid)
                                  .Union(_baseLia.Select(o => o.MasterCurrency_Guid))
                                  .Union(new Guid?[] { defaultCurrencyGuid });
            var _baseCurr = DbContext.TblMasterCurrency.Where(o => allCurrGuids.Any(c => c == o.Guid)).ToList();


            string InterBr = " (Inter Br.)";
            string MultiBr = " (Multi Br.)";

            var _baseJob = (from job in _baseJobs
                            join legCus in _baseLegsCus on job.Guid equals legCus.leg.MasterActualJobHeader_Guid
                            join type in _baseType on job.SystemServiceJobType_Guid equals type.Guid
                            select new { job, legCus, type })
                           .Select(o =>
                           {
                               var jobTypeName = o.job.FlagMultiBranch ? (o.type.ServiceJobTypeNameAbb + MultiBr) : o.type.ServiceJobTypeNameAbb;
                               jobTypeName = o.job.FlagJobInterBranch ? (o.type.ServiceJobTypeNameAbb + InterBr) : jobTypeName;

                               return new RawJobDataView
                               {
                                   JobTypeID = o.type.ServiceJobTypeID ?? 0,
                                   FlagDestination = o.legCus.leg.FlagDestination,
                                   JobStatusID = o.job.SystemStatusJobID ?? 0,
                                   JobGuid = o.job.Guid,
                                   JobNo = o.job.JobNo,
                                   DailyRunGuid = o.legCus.leg.MasterRunResourceDaily_Guid,
                                   Location = o.legCus.cus.CustomerFullName + " - " + o.legCus.loc.BranchName,
                                   JobAction = o.legCus.action.ActionNameAbbrevaition,
                                   JobTypeName = jobTypeName
                               };
                           }).Where(o => o.ExcludeJobs());

            var libilities = (from lia in _baseLia
                              join job in _baseJob on lia.MasterActualJobHeader_Guid equals job.JobGuid
                              select new { job, lia }).AsEnumerable()
                              .Select(o =>
                              {
                                  return new ItemsLibilityView
                                  {
                                      DailyRunGuid = o.job.DailyRunGuid,
                                      DocCurrencyGuid = o.lia.MasterCurrency_Guid,
                                      Liability = Convert.ToDouble(o.lia.Liability),
                                      LibilityGuid = o.lia.Guid,
                                      JobStatusID = o.job.JobStatusID,
                                      JobGuid = o.job.JobGuid
                                  };
                              }).ToList();


            var commodities = (from comm in _baseComm
                               join job in _baseJob on comm.MasterActualJobHeader_Guid equals job.JobGuid
                               select new { job, comm }).AsEnumerable()
                               .Select(o =>
                               {
                                   return new ItemsCommodityView
                                   {
                                       ActualCommodityGuid = o.comm.Guid,
                                       CommodityGuid = o.comm.MasterCommodity_Guid,
                                       FlagCommodityDiscrepancies = o.comm.FlagCommodityDiscrepancies ?? false,
                                       Quantity = o.comm.Quantity ?? 0,
                                       QuantityActual = o.comm.QuantityActual ?? 0,
                                       QuantityExpected = o.comm.QuantityExpected ?? 0,
                                       DailyRunGuid = o.job.DailyRunGuid,
                                       JobGuid = o.job.JobGuid
                                   };
                               }).ToList();

            var TvJob = new int[] { IntTypeJob.TV, IntTypeJob.TV_MultiBr };
            Func<RawJobDataView, bool> ExcludeTV = (o) =>
            {
                bool IsNotExclude = true;
                //Exclude items  TV,Picked Up, On action is D
                if (TvJob.Any(s => s == o.JobTypeID))
                {
                    var ignoredPickedup = o.JobStatusID == IntStatusJob.PickedUp && !o.FlagDestination;
                    if (ignoredPickedup)
                    {
                        o.JobItems = new RawItemsView();
                    }
                    IsNotExclude = _model.Any(m => m.DailyRunGuid_Target == o.Target_DailyRunGuid && m.JobGuid == o.JobGuid && m.JobAction == o.JobAction);
                }
                return IsNotExclude;
            };

            var jobInRun = (from r in _baseRun
                            join j in _baseJob on r.DailyRunGuid equals j.DailyRunGuid into lJob
                            from leftJob in lJob.DefaultIfEmpty()
                            select new { r, j = leftJob })
                            .Select(o =>
                            {
                                var jobitem = new RawItemsView();
                                jobitem.Liabilities = libilities.Where(l => l?.JobGuid == o.j?.JobGuid && l?.DailyRunGuid == o.r?.DailyRunGuid);
                                jobitem.Commodities = commodities.Where(c => c.JobGuid == o.j?.JobGuid && c.DailyRunGuid == o.r?.DailyRunGuid);

                                var target_Curr = _baseCurr.FirstOrDefault(c => c.Guid == o.r.LiabilityLimitCurrency_Guid);
                                return new RawJobDataView
                                {
                                    JobTypeID = (o.j?.JobTypeID) ?? 000,
                                    FlagDestination = (o.j?.FlagDestination) ?? false,
                                    JobStatusID = (o.j?.JobStatusID) ?? 000,
                                    JobGuid = o.j?.JobGuid,
                                    JobNo = o.j?.JobNo,
                                    JobAction = o.j?.JobAction,
                                    JobTypeName = o.j?.JobTypeName,
                                    Location = o.j?.Location,
                                    DailyRunGuid = o.r.DailyRunGuid,
                                    Target_CurrencyGuid = o.r.LiabilityLimitCurrency_Guid,
                                    Target_DailyRunGuid = o.r.DailyRunGuid,
                                    Target_RunLibilityLimit = Convert.ToDouble(o.r.LiabilityLimit),
                                    Target_RunNo = o.r.RunNo,
                                    Target_RouteDetail = o.r.RouteDetail,
                                    Target_WorkDate = o.r.ObjWorkDate.ChangeFromDateToString(userFormatDate),
                                    Target_CurrencyAbb = target_Curr?.MasterCurrencyAbbreviation,
                                    Target_DailyRunSiteGuid = o.r.MasterSite_Guid,
                                    Target_DailyRunSiteName = o.r.SiteName,
                                    FlagJobInRun = true,
                                    JobItems = jobitem
                                };
                            }).ToList();


            var jobOutRun = _baseJob.Where(o => o.DailyRunGuid == null || !_baseRun.Any(r => r.DailyRunGuid == o.DailyRunGuid))
                            .Select(o =>
                            {
                                var jobitem = new RawItemsView();
                                jobitem.Liabilities = libilities.Where(l => l.JobGuid == o.JobGuid);
                                jobitem.Commodities = commodities.Where(c => c.JobGuid == o.JobGuid);

                                var target_run = TvJob.Any(s => s == o.JobTypeID) ?
                                                  _baseRun.FirstOrDefault(r => _model.Any(m => m.DailyRunGuid_Target == r.DailyRunGuid && m.JobGuid == o.JobGuid && m.JobAction == o.JobAction))
                                                : _baseRun.FirstOrDefault(r => _model.Any(m => m.DailyRunGuid_Target == r.DailyRunGuid && m.JobGuid == o.JobGuid));

                                var target_Curr = _baseCurr.FirstOrDefault(c => c.Guid == target_run?.LiabilityLimitCurrency_Guid);
                                o.Target_CurrencyGuid = target_run?.LiabilityLimitCurrency_Guid;
                                o.Target_CurrencyAbb = target_Curr?.MasterCurrencyAbbreviation;
                                o.Target_DailyRunGuid = target_run?.DailyRunGuid;
                                o.Target_DailyRunSiteGuid = target_run?.MasterSite_Guid;
                                o.Target_DailyRunSiteName = target_run?.SiteName;
                                o.Target_RunLibilityLimit = Convert.ToDouble(target_run?.LiabilityLimit);
                                o.Target_RunNo = target_run?.RunNo;
                                o.Target_RouteDetail = target_run?.RouteDetail;
                                o.Target_WorkDate = target_run?.ObjWorkDate?.ChangeFromDateToString(userFormatDate);
                                o.FlagJobOutRun = true;
                                o.FlagDisplayChkBox = _model.Any(m => m.JobGuid == o.JobGuid && m.DailyRunGuid_Source != null); //All has run job
                                o.JobItems = jobitem;
                                return o;
                            }).Where(o => ExcludeTV(o)).ToList();



            return jobInRun.Union(jobOutRun);
        }


        public IEnumerable<ValidateJobsInRunView> ValidateAssignJobsToRun(IEnumerable<ValidateJobsInRunView> assignJobList)
        {
            var result = Enumerable.Empty<ValidateJobsInRunView>();

            #region Validate TV Job
            int[] jobType = new int[] { IntTypeJob.TV, IntTypeJob.TV_MultiBr };
            var jobTV = assignJobList.Where(o => jobType.Any(t => o.JobTypeID == t));
            if (jobTV != null && jobTV.Any())
            {
                var allJobGuids = jobTV.Select(o => o.JobHeaderGuid);
                var allLeg = DbContext.TblMasterActualJobServiceStopLegs_ExCustomer(allJobGuids).ToList();
                var applyRun = allLeg.Select(o =>
                {
                    var dailyRunGuid = jobTV.FirstOrDefault(j => j.JobLegGuid == o.Guid);
                    o.MasterRunResourceDaily_Guid = (dailyRunGuid?.DailyRunGuid) ?? o.MasterRunResourceDaily_Guid;
                    return o;
                });

                var inValidLeg = applyRun.GroupBy(o => new { o.MasterActualJobHeader_Guid, o.MasterRunResourceDaily_Guid })
                                     .Where(o => o.Count() > 1)
                                     .SelectMany(o => o.Select(j => j));

                result = from i in assignJobList
                         join l in inValidLeg on i.JobHeaderGuid equals l.MasterActualJobHeader_Guid
                         select new ValidateJobsInRunView
                         {
                             DailyRunGuid = l.MasterRunResourceDaily_Guid,
                             JobHeaderGuid = l.MasterActualJobHeader_Guid ?? Guid.Empty,
                             JobLegGuid = l.Guid,
                             JobNoID = i.JobNoID,
                             JobTypeID = i.JobTypeID
                         };
            }
            #endregion

            return result;
        }
    }
}
