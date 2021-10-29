using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.Nemo.RouteOptimization;
using Bgt.Ocean.Repository.EntityFramework.Extensions;
using Bgt.Ocean.Repository.EntityFramework.StringQuery.RouteOptimize;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Nemo
{
    #region Interface
    public interface IMasterNemoQueueRouteOptimizationRepository : IRepository<TblMasterNemoQueueRouteOptimization>
    {
        #region Select
        int GetNextTransactionID();
        List<NemoRouteOptimizationObject> GetOptimizationDailyRun(List<Guid> dailyRunGuid);
        List<NemoOptimizationLocations> GetOptimizationDailyRunLocation(List<Guid> dailyRunGuid);
        List<NemoRouteOptimizationObject> GetOptimizationMasterRoute(Guid routeGuid, Guid routeDetailGuid);
        List<NemoOptimizationLocations> GetOptimizationMasterRouteLocation(Guid routeGuid, Guid routeDetailGuid);
        IEnumerable<NemoOptimizationTransactionResponse> GetOptimizationResult(Guid siteGuid, DateTime workDate, int optimizeMode);
        IEnumerable<TblMasterNemoQueueRouteOptimization> GetOptimizationByTaskGuid(Guid taskGuid);
        TblMasterNemoQueueRouteOptimization GetOptimizationByTaskGuid(Guid taskGuid, Guid runGuid);
        string GetRunInMasterRouteFullname(Guid masterRouteGuid, Guid routeGroupDetailGuid);
        TblMasterCustomerLocation GetDefaultBrinksLocation(Guid optimizeGuid);
        RouteOptimizedLocationDetails GetRouteOptimizedLocationDetails(Guid optimizeGuid);
        int CheckTaskComplete(Guid taskGuid);
        RouteDirectionRequest GetBranchInformation(Guid optimizeGuid);
        #endregion

        #region Insert
        bool SaveOptimizationPlan(List<TblMasterNemoQueueRouteOptimization> plan, List<TblMasterNemoQueueRouteOptimization_Detail> details);
        #endregion

        #region Update
        bool UpdateNemoTaskGuid(Guid optimizationGuid, Guid nemoTaskGuid, EnumRouteOptimization.OptimizationStatus? status);
        bool UpdateOptimizedDetails(NemoOptimizedDetails optimizedData);
        NemoOptimizationResponse UpdateOptimizedAction_Approve(Guid optimizeGuid, string username);
        NemoOptimizationResponse UpdateOptimizedAction_Cancel(Guid optimizeGuid, string username);
        #endregion
    }
    #endregion

    public class MasterNemoQueueRouteOptimizationRepository : Repository<OceanDbEntities, TblMasterNemoQueueRouteOptimization>, IMasterNemoQueueRouteOptimizationRepository
    {
        #region Objects & Variables
        private readonly RouteOptimizeQuery routeOptimizeQuery = new RouteOptimizeQuery();

        private const string PROCESSCODE = "MASTER_ROUTE_JOB";
        private const string CATEGORYCODE = "Seq_ChangeStopSequence";
        private const string PROCESSCODE_MASTERROUTE = "MASTER_ROUTE_TEMPLATE";
        private const string CATEGORYCODE_MASTERROUTE = "Template_EditMasterRoute";


        #endregion

        #region Constuctor
        public MasterNemoQueueRouteOptimizationRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
        #endregion

        #region Select
        public int GetNextTransactionID()
        {
            return DbContext.TblMasterNemoQueueRouteOptimization.Max(o => o.TransactionID).GetValueOrDefault() + 1;
        }

        public List<NemoRouteOptimizationObject> GetOptimizationDailyRun(List<Guid> dailyRunGuid)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                var newList = dailyRunGuid.Select(o => o.ToString()).ToList();
                var command = new SqlCommand(routeOptimizeQuery.GetDailyRunPlans);
                var parameters = command.AddArrayParameters("DailyRunGuid", dailyRunGuid);

                return context.Database.SqlQuery<NemoRouteOptimizationObject>(command.CommandText, parameters.ToArray()).ToList();
            }
        }

        public List<NemoOptimizationLocations> GetOptimizationDailyRunLocation(List<Guid> dailyRunGuid)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                var newList = dailyRunGuid.Select(o => o.ToString()).ToList();
                var command = new SqlCommand(routeOptimizeQuery.GetDailyRunServiceLocations);
                var parameters = command.AddArrayParameters("DailyRunGuid", dailyRunGuid);

                return context.Database.SqlQuery<NemoOptimizationLocations>(command.CommandText, parameters.ToArray()).ToList();
            }
        }

        public List<NemoRouteOptimizationObject> GetOptimizationMasterRoute(Guid routeGuid, Guid routeDetailGuid)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                var command = new SqlCommand(routeOptimizeQuery.GetMasterRoutePlan);
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@RouteGuid", routeGuid));
                parameters.Add(new SqlParameter("@RouteDetailGuid", routeDetailGuid));

                return context.Database.SqlQuery<NemoRouteOptimizationObject>(command.CommandText, parameters.ToArray()).ToList();
            }
        }

        public List<NemoOptimizationLocations> GetOptimizationMasterRouteLocation(Guid routeGuid, Guid routeDetailGuid)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                var command = new SqlCommand(routeOptimizeQuery.GetMasterRouteLocation);
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@RouteGuid", routeGuid));
                parameters.Add(new SqlParameter("@RouteDetailGuid", routeDetailGuid));

                return context.Database.SqlQuery<NemoOptimizationLocations>(command.CommandText, parameters.ToArray()).ToList();
            }
        }

        public IEnumerable<NemoOptimizationTransactionResponse> GetOptimizationResult(Guid siteGuid, DateTime workDate, int optimizeMode)
        {
            var optimization = DbContext.TblMasterNemoQueueRouteOptimization
                                .Where(o => o.MasterSite_Guid == siteGuid
                                    && o.RouteOptimizeType == optimizeMode
                                    && (optimizeMode == 1 || DbFunctions.TruncateTime(o.WorkDate) == DbFunctions.TruncateTime(workDate)))
                                .OrderByDescending(o => o.TransactionID).ThenBy(o => o.OptimizationOrder).Select(o => new NemoOptimizationTransactionResponse()
                                {
                                    NemoQueueOptimization_Guid = o.Guid,
                                    MasterRoute_Guid = o.MasterRoute_Guid,
                                    DailyRunResource_Guid = o.MasterDailyRunResource_Guid.Value,
                                    StatusID = o.StatusQueue,
                                    StatusName = ((EnumRouteOptimization.OptimizationStatus)o.StatusQueue).ToString(),
                                    TransactionID = o.TransactionID.Value,
                                    RequestedBy = o.UserCreated,
                                    RequestedDate = o.DatetimeCreated,
                                    OptimizedDate = o.DateTimeSolutionAsync.Value,
                                    OptimizedDistance = (int)o.OptimizedDistance,
                                    OptimizedTime = o.OptimizedTime,
                                    ApprovedBy = o.UserModifed,
                                    ApprovedDate = o.DatetimeModified
                                }).ToList();

            foreach (var item in optimization)
            {
                item.Stops = DbContext.TblMasterNemoQueueRouteOptimization_Detail.Where(o => o.NemoQueueRouteOptimization_Guid == item.NemoQueueOptimization_Guid)
                                .Join(DbContext.TblMasterCustomerLocation, detail => detail.MasterCustomerLocation_Guid, loc => loc.Guid, (detail, loc) => new NemoOptimizationTransactionDetail
                                {
                                    NemoQueueOptimizationDetail_Guid = detail.NemoQueueRouteOptimization_Guid,
                                    OldLocation_Guid = loc.Guid,
                                    OldLocation_Code = loc.BranchCodeReference,
                                    OldLocation_Name = loc.BranchName,
                                    OldLocation_Lattitude = loc.Latitude,
                                    OldLocation_Longtitude = loc.Longitude,
                                    Sequence = detail.JobOrder,
                                    NewSequence = detail.JobOrderOptmized
                                }).OrderBy(o => o.Sequence).ToList();

                if (item.StatusID != (int)EnumRouteOptimization.OptimizationStatus.Optimizing)
                {
                    var newSequence = item.Stops.Select(o => new { LocationGuid = o.OldLocation_Guid, JobOrder = o.NewSequence }).OrderBy(o => o.JobOrder).ToList();
                    for (int i = 0; i < newSequence.Count; i++)
                    {
                        var thisStop = item.Stops[i];
                        var newStop = newSequence[i];
                        var stopDetail = item.Stops.FirstOrDefault(o => o.OldLocation_Guid == newStop.LocationGuid);
                        thisStop.NewLocation_Guid = newStop.LocationGuid;
                        thisStop.NewSequence = newStop.JobOrder;
                        thisStop.NewLocation_Name = stopDetail.OldLocation_Name;
                        thisStop.NewLocation_Code = stopDetail.OldLocation_Code;
                        thisStop.NewLocation_Lattitude = stopDetail.OldLocation_Lattitude;
                        thisStop.NewLocation_Longtitude = stopDetail.OldLocation_Longtitude;
                    }
                }
            }
            return optimization;
        }

        public IEnumerable<TblMasterNemoQueueRouteOptimization> GetOptimizationByTaskGuid(Guid taskGuid)
        {
            return DbContext.TblMasterNemoQueueRouteOptimization.Where(o => o.RouteOptimizeTaskGuid == taskGuid).OrderBy(o => o.OptimizationOrder);
        }

        public TblMasterNemoQueueRouteOptimization GetOptimizationByTaskGuid(Guid taskGuid, Guid runGuid)
        {
            return DbContext.TblMasterNemoQueueRouteOptimization.FirstOrDefault(o => o.RouteOptimizeTaskGuid == taskGuid && o.MasterDailyRunResource_Guid == runGuid);
        }

        public string GetRunInMasterRouteFullname(Guid masterRouteGuid, Guid routeGroupDetailGuid)
        {
            var masterRouteName = DbContext.TblMasterRoute.FirstOrDefault(o => o.Guid == masterRouteGuid)?.MasterRouteName;
            var routeDetailName = DbContext.TblMasterRouteGroup_Detail.Where(o => o.Guid == routeGroupDetailGuid)
                                    .Join(DbContext.TblMasterRouteGroup, rDetail => rDetail.MasterRouteGroup_Guid, rGroup => rGroup.Guid, (rDetail, rGroup) => rGroup.MasterRouteGroupName + " - " + rDetail.MasterRouteGroupDetailName)
                                    .FirstOrDefault();
            return masterRouteName + " - " + routeDetailName;
        }

        public TblMasterCustomerLocation GetDefaultBrinksLocation(Guid optimizeGuid)
        {
            var optimize = DbContext.TblMasterNemoQueueRouteOptimization.FirstOrDefault(o => o.Guid == optimizeGuid);
            var brinkLocation = DbContext.TblMasterCustomer.Where(o => !o.FlagDisable.Value && !o.FlagChkCustomer.Value)
                                .Join(DbContext.TblMasterCustomerLocation.Where(o => !o.FlagDisable && o.MasterSite_Guid == optimize.MasterSite_Guid),
                                    customer => customer.Guid, location => location.MasterCustomer_Guid, (customer, location) => location).FirstOrDefault();
            return brinkLocation;
        }

        public RouteOptimizedLocationDetails GetRouteOptimizedLocationDetails(Guid optimizeGuid)
        {
            var result = new RouteOptimizedLocationDetails();

            #region Get Brinks Information
            var brinkLocation = GetDefaultBrinksLocation(optimizeGuid);
            var brinksInfo = new LocationInformation()
            {
                Guid = brinkLocation.Guid,
                Code = brinkLocation.BranchCodeReference,
                Name = brinkLocation.BranchName,
                Address = brinkLocation.Address,
                Latitude = brinkLocation.Latitude,
                Longitude = brinkLocation.Longitude,
                Order = 0
            };
            result.Plan.Add(brinksInfo);
            result.Optimized.Add(brinksInfo);
            #endregion

            #region All Locations Information
            var optimizeDetail = DbContext.TblMasterNemoQueueRouteOptimization_Detail.Where(o => o.NemoQueueRouteOptimization_Guid == optimizeGuid)
                                    .Join(DbContext.TblMasterCustomerLocation, detail => detail.MasterCustomerLocation_Guid, location => location.Guid, (detail, location)
                                    => new { Detail = detail, Location = location }).ToList();
            #endregion

            #region Get Plan Information
            foreach (var data in optimizeDetail.OrderBy(o => o.Detail.JobOrder))
            {
                result.Plan.Add(new LocationInformation()
                {
                    Guid = data.Location.Guid,
                    Code = data.Location.BranchCodeReference,
                    Name = data.Location.BranchName,
                    Address = data.Location.Address,
                    Latitude = data.Location.Latitude,
                    Longitude = data.Location.Longitude,
                    Order = data.Detail.JobOrder
                });
            }
            #endregion

            #region Get Optimize Information
            foreach (var data in optimizeDetail.Where(o => o.Detail.JobOrderOptmized.HasValue).OrderBy(o => o.Detail.JobOrderOptmized))
            {
                result.Optimized.Add(new LocationInformation()
                {
                    Guid = data.Location.Guid,
                    Code = data.Location.BranchCodeReference,
                    Name = data.Location.BranchName,
                    Address = data.Location.Address,
                    Latitude = data.Location.Latitude,
                    Longitude = data.Location.Longitude,
                    Order = data.Detail.JobOrderOptmized
                });
            }
            #endregion

            return result;
        }

        public int CheckTaskComplete(Guid taskGuid)
        {
            using (var context = new OceanDbEntities())
            {
                var task = context.TblMasterNemoQueueRouteOptimization.FirstOrDefault(o => o.RouteOptimizeTaskGuid == taskGuid);
                var pendingTask = context.TblMasterNemoQueueRouteOptimization.Any(o => o.StatusQueue == 1 && o.TransactionID == task.TransactionID);
                return pendingTask ? -1 : task.TransactionID.Value;
            }
        }

        public RouteDirectionRequest GetBranchInformation(Guid optimizeGuid)
        {
            var branch = DbContext.TblMasterNemoQueueRouteOptimization.Where(o => o.Guid == optimizeGuid)
                                    .Join(DbContext.TblMasterSite, detail => detail.MasterSite_Guid, site => site.Guid, (detail, site) => new { Detail = detail, Site = site })
                                    .Join(DbContext.TblMasterCountry, detail => detail.Site.MasterCountry_Guid, country => country.Guid, (detail, country) => new RouteDirectionRequest()
                                    {
                                        BranchCode = detail.Site.SiteCode,
                                        CountryCode = country.MasterCountryAbbreviation
                                    })
                                    .FirstOrDefault();

            return branch;
        }
        #endregion

        #region Insert
        public bool SaveOptimizationPlan(List<TblMasterNemoQueueRouteOptimization> plan, List<TblMasterNemoQueueRouteOptimization_Detail> details)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.TblMasterNemoQueueRouteOptimization.AddRange(plan);
                        context.TblMasterNemoQueueRouteOptimization_Detail.AddRange(details);

                        //context.BulkSaveChanges();
                        context.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
        #endregion

        #region Update
        public bool UpdateNemoTaskGuid(Guid optimizationGuid, Guid nemoTaskGuid, EnumRouteOptimization.OptimizationStatus? status)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var data = context.TblMasterNemoQueueRouteOptimization.FirstOrDefault(o => o.Guid == optimizationGuid);
                        if (data != null)
                        {
                            data.RouteOptimizeTaskGuid = nemoTaskGuid;
                            if (status.HasValue)
                            {
                                data.DateTimeShiftAsync = DateTime.Now;
                                data.StatusQueue = (int)status;
                            }
                        }
                        context.SaveChanges();
                        transaction.Commit();
                        return true;

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool UpdateOptimizedDetails(NemoOptimizedDetails optimizedData)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var task = context.TblMasterNemoQueueRouteOptimization.FirstOrDefault(o => o.Guid == optimizedData.NemoTaskGuid);
                        if (task != null)
                        {
                            #region Update Task Status
                            task.StatusQueue = 2;
                            task.DateTimeSolutionAsync = DateTime.Now;
                            task.OptimizedDistance = optimizedData.Distance;
                            task.OptimizedTime = optimizedData.DurationTime;
                            task.OptimizedWaitTime = optimizedData.WaitTime;
                            #endregion

                            #region Update Task Details
                            var locations = context.TblMasterNemoQueueRouteOptimization_Detail.Where(o => o.NemoQueueRouteOptimization_Guid == task.Guid);
                            foreach (var location in locations)
                            {
                                var updateData = optimizedData.Jobs.FirstOrDefault(o => o.JobGuid == location.MasterJob_Guid);
                                if (updateData != null)
                                {
                                    location.JobOrderOptmized = updateData.JobOrder;
                                    location.ScheduleTimeOptimized = updateData.ScheduleTime;
                                }
                            }
                            #endregion
                        }
                        context.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public NemoOptimizationResponse UpdateOptimizedAction_Approve(Guid optimizeGuid, string username)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var task = context.TblMasterNemoQueueRouteOptimization.FirstOrDefault(o => o.Guid == optimizeGuid);
                        if (task != null)
                        {
                            if (task.RouteOptimizeType == 0)
                            {
                                var runStatus = context.TblMasterDailyRunResource.FirstOrDefault(o => o.Guid == task.MasterDailyRunResource_Guid)?.RunResourceDailyStatusID;
                                if (runStatus > 1)
                                {
                                    return new NemoOptimizationResponse() { Success = false, Message = "-5001" };
                                }
                            }

                            var taskDetail = new ConcurrentBag<TblMasterNemoQueueRouteOptimization_Detail>(context.TblMasterNemoQueueRouteOptimization_Detail.Where(o => o.NemoQueueRouteOptimization_Guid == task.Guid && o.JobOrderOptmized.HasValue));
                            if (taskDetail.Any())
                            {
                                #region Daily Run
                                if (task.RouteOptimizeType == 0)
                                {
                                    #region Get Data
                                    var legInRun = new ConcurrentBag<TblMasterActualJobServiceStopLegs>(context.TblMasterActualJobServiceStopLegs.Where(o => o.MasterRunResourceDaily_Guid == task.MasterDailyRunResource_Guid));
                                    var listJobGuid = legInRun.Select(o => o.MasterActualJobHeader_Guid).Distinct();
                                    var jobInRun = new ConcurrentBag<TblMasterActualJobHeader>(context.TblMasterActualJobHeader.Join(listJobGuid, job => job.Guid, jobGuid => jobGuid, (job, jobGuid) => job).Distinct());
                                    var historyJobs = new ConcurrentBag<TblMasterHistory_ActualJob>();
                                    #endregion

                                    #region Start Optimizing
                                    Parallel.ForEach(taskDetail, detail =>
                                    {
                                        var legInGroup = legInRun.Where(o => o.JobOrder == detail.JobOrder && o.MasterCustomerLocation_Guid == detail.MasterCustomerLocation_Guid);
                                        foreach (var leg in legInGroup)
                                        {
                                            #region Create Job History
                                            historyJobs.Add(new TblMasterHistory_ActualJob()
                                            {
                                                Guid = Guid.NewGuid(),
                                                MasterActualJobHeader_Guid = leg.MasterActualJobHeader_Guid,
                                                MsgID = 5098,
                                                MsgParameter = JsonConvert.SerializeObject(new string[] { detail.JobOrder.ToString(), detail.JobOrderOptmized.ToString() }),
                                                UserCreated = username,
                                                DatetimeCreated = DateTime.Now,
                                                UniversalDatetimeCreated = DateTime.UtcNow
                                            });
                                            #endregion

                                            #region Update Modified Job 
                                            var job = jobInRun.FirstOrDefault(o => o.Guid == leg.MasterActualJobHeader_Guid);
                                            if (job != null)
                                            {
                                                job.UserModifed = username;
                                                job.DatetimeModified = DateTime.Now;
                                                job.UniversalDatetimeModified = DateTime.UtcNow;
                                            }
                                            #endregion

                                            #region Update Sequence In Leg
                                            leg.JobOrder = detail.JobOrderOptmized;
                                            #endregion
                                        }
                                    });
                                    #endregion

                                    #region Insert History Data
                                    context.TblMasterHistory_ActualJob.AddRange(historyJobs);
                                    context.TblMasterHistory_DailyRunResource.Add(new TblMasterHistory_DailyRunResource()
                                    {
                                        Guid = Guid.NewGuid(),
                                        MasterDailyRunResource_Guid = task.MasterDailyRunResource_Guid,
                                        MsgID = 5099,
                                        MsgParameter = JsonConvert.SerializeObject(new string[] { task.TransactionID.ToString() }),
                                        UserCreated = username,
                                        DatetimeCreated = DateTime.Now,
                                        UniversalDatetimeCreated = DateTime.UtcNow
                                    });
                                    #endregion
                                }
                                #endregion

                                #region Master Route
                                else if (task.RouteOptimizeType == 1)
                                {
                                    var categoryCodeMasterRoute = context.SFOTblSystemLogCategory.FirstOrDefault(o => o.CategoryCode == CATEGORYCODE_MASTERROUTE);
                                    var processCodeMasterRoute = context.SFOTblSystemLogProcess.FirstOrDefault(o => o.ProcessCode == PROCESSCODE_MASTERROUTE);

                                    var categoryCode = context.SFOTblSystemLogCategory.FirstOrDefault(o => o.CategoryCode == CATEGORYCODE);
                                    var processCode = context.SFOTblSystemLogProcess.FirstOrDefault(o => o.ProcessCode == PROCESSCODE);

                                    #region Get Data
                                    var allJobAndLeg = context.TblMasterRouteJobHeader.Where(o => o.MasterRoute_Guid == task.MasterRoute_Guid)
                                                        .Join(context.TblMasterRouteJobServiceStopLegs.Where(o => o.MasterRouteGroupDetail_Guid == task.MasterDailyRunResource_Guid), job => job.Guid, leg => leg.MasterRouteJobHeader_Guid, (job, leg) => new { Jobs = job, Legs = leg });

                                    var allJobAndLegTV = context.TblMasterRouteJobServiceStopLegs.Where(o => o.MasterRouteDeliveryLeg_Guid == task.MasterRoute_Guid && o.MasterRouteGroupDetail_Guid == task.MasterDailyRunResource_Guid && o.FlagDeliveryLegForTV == true)
                                                            .Join(context.TblMasterRouteJobHeader, leg => leg.MasterRouteJobHeader_Guid, job => job.Guid, (leg, job) => new { Jobs = job, Legs = leg });

                                    var jobInRun = new ConcurrentBag<TblMasterRouteJobHeader>((allJobAndLeg.Select(o => o.Jobs).Union(allJobAndLegTV.Select(o => o.Jobs))).Distinct());
                                    var legInRun = new ConcurrentBag<TblMasterRouteJobServiceStopLegs>((allJobAndLeg.Select(o => o.Legs).Union(allJobAndLegTV.Select(o => o.Legs))).Distinct());

                                    var historyJobs = new ConcurrentBag<TblMasterRouteTransactionLog>();
                                    #endregion

                                    #region Start Optimizing
                                    Parallel.ForEach(taskDetail, detail =>
                                    {
                                        var legInGroup = legInRun.Where(o => o.JobOrder == detail.JobOrder && o.MasterCustomerLocation_Guid == detail.MasterCustomerLocation_Guid);
                                        foreach (var leg in legInGroup)
                                        {
                                            #region Create Job History
                                            historyJobs.Add(new TblMasterRouteTransactionLog()
                                            {
                                                Guid = Guid.NewGuid(),
                                                SystemLogCategory_Guid = categoryCode.Guid,
                                                SystemLogProcess_Guid = processCode.Guid,
                                                ReferenceValue_Guid = detail.MasterJob_Guid.Value,
                                                SystemMsgID = (5098).ToString(),
                                                JSONValue = JsonConvert.SerializeObject(new string[] { detail.JobOrder.ToString(), detail.JobOrderOptmized.ToString() }),
                                                Remark = "Route Optimization",
                                                UserCreated = username,
                                                DatetimeCreated = DateTime.Now,
                                                UniversalDatetimeCreated = DateTime.UtcNow
                                            });
                                            #endregion

                                            #region Update Modified Job 
                                            var job = jobInRun.FirstOrDefault(o => o.Guid == leg.MasterRouteJobHeader_Guid);
                                            if (job != null)
                                            {
                                                job.UserModifed = username;
                                                job.DatetimeModified = DateTime.Now;
                                                job.UniversalDatetimeModified = DateTime.UtcNow;
                                            }
                                            #endregion

                                            #region Update Sequence In Leg
                                            leg.JobOrder = detail.JobOrderOptmized;
                                            #endregion
                                        }
                                    });

                                    historyJobs.Add(new TblMasterRouteTransactionLog()
                                    {
                                        Guid = Guid.NewGuid(),
                                        SystemLogCategory_Guid = categoryCodeMasterRoute.Guid,
                                        SystemLogProcess_Guid = processCodeMasterRoute.Guid,
                                        ReferenceValue_Guid = task.MasterRoute_Guid.Value,
                                        SystemMsgID = (5100).ToString(),
                                        Remark = "Route Optimization",
                                        UserCreated = task.UserCreated,
                                        DatetimeCreated = DateTime.Now,
                                        UniversalDatetimeCreated = DateTime.UtcNow
                                    });
                                    #endregion

                                    #region Insert History Data
                                    context.TblMasterRouteTransactionLog.AddRange(historyJobs);
                                    #endregion
                                }
                                #endregion

                                #region Update Queue
                                task.StatusQueue = (int)EnumRouteOptimization.OptimizationStatus.Accepted;
                                task.UserModifed = username;
                                task.DatetimeModified = DateTime.Now;
                                task.UniversalDatetimeModified = DateTime.UtcNow;
                                #endregion
                            }
                        }

                        context.SaveChanges();
                        transaction.Commit();
                        return new NemoOptimizationResponse() { Success = true }; ;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new NemoOptimizationResponse() { Success = false, Message = ex.Message }; ;
                    }
                }
            }
        }

        public NemoOptimizationResponse UpdateOptimizedAction_Cancel(Guid optimizeGuid, string username)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var task = context.TblMasterNemoQueueRouteOptimization.FirstOrDefault(o => o.Guid == optimizeGuid);
                        if (task != null)
                        {
                            #region Update Task
                            task.StatusQueue = (int)EnumRouteOptimization.OptimizationStatus.Cancelled;
                            task.UserModifed = username;
                            task.DatetimeModified = DateTime.Now;
                            task.UniversalDatetimeModified = DateTime.UtcNow;
                            #endregion
                        }

                        context.SaveChanges();
                        transaction.Commit();
                        return new NemoOptimizationResponse() { Success = true }; ;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new NemoOptimizationResponse() { Success = false, Message = ex.Message }; ;
                    }
                }
            }
        }
        #endregion
    }
}
