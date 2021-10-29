using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.RunControl;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using static Bgt.Ocean.Infrastructure.Util.EnumRun;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    #region Interface

    public interface IMasterActualJobServiceStopLegsRepository : IRepository<TblMasterActualJobServiceStopLegs>
    {
        IEnumerable<TblMasterActualJobServiceStopLegs> FindByJobHeader(Guid jobGuid);

        AdhocCheckDuplicateJobInDayResult Func_Adhoc_CheckDuplicateJobInDay(Guid? siteGuid, DateTime? workdate, DateTime? workdateDev, int jobTypeID, Guid? jobGuid, Guid? lobGuid, Guid? customerGuid, Guid? cusLocationGuid, Guid? customerDevGuid, Guid? cusLocationDevGuid, Guid? siteDevGuid);
        IEnumerable<JobLegsView> GetJobDetailForUpdateOtc(Guid jobHeadGuid);
        IEnumerable<RunControlJobLegsDetailByJobGetResult> Func_GetJobLegsDetailByJob(Guid jobGuid, string formatDate, Guid? siteGuid);
        IEnumerable<LegView> GetLegsDetail(Guid jobGuid);
        IEnumerable<TblMasterActualJobServiceStopLegs> FindByDailyRun(Guid? dailyRunGuid);
        IEnumerable<TblMasterActualJobServiceStopLegs> FindDestinationByJobGuid(IEnumerable<Guid> jobGuid);
        IEnumerable<TblMasterActualJobServiceStopLegs> FindByJobGuidList(IEnumerable<Guid> jobGuid);
        IEnumerable<TblMasterActualJobServiceStopLegs> GetLegsForOptimizationByDailyRun(List<Guid> jobGuid);
        IEnumerable<TblMasterActualJobServiceStopLegs> GetLegsForOptimizationByDailyRun(Guid jobGuid);
        IEnumerable<TblMasterActualJobServiceStopLegs> GetALLLegsByLegGuid(List<Guid> LegGuid);
        IEnumerable<TblMasterActualJobServiceStopLegs> FindByLegGuidList(IEnumerable<Guid> legsGuid);
        IEnumerable<TblMasterActualJobServiceStopLegs> FindByDailyRunAndSite(Guid dailyRunGuid);
    }

    #endregion

    public class MasterActualJobServiceStopLegsRepository : Repository<OceanDbEntities, TblMasterActualJobServiceStopLegs>, IMasterActualJobServiceStopLegsRepository
    {
        public MasterActualJobServiceStopLegsRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterActualJobServiceStopLegs> FindByDailyRun(Guid? dailyRunGuid)
        {
            return (from l in DbContext.TblMasterActualJobServiceStopLegs
                    join h in DbContext.TblMasterActualJobHeader on l.MasterActualJobHeader_Guid equals h.Guid
                    where h.SystemStatusJobID != IntStatusJob.CancelledJob && l.MasterRunResourceDaily_Guid == dailyRunGuid
                    select l).Distinct();
        }

        public IEnumerable<TblMasterActualJobServiceStopLegs> FindByDailyRunAndSite(Guid dailyRunGuid)
        {
            return DbContext.TblMasterActualJobServiceStopLegs.Where(o => o.MasterRunResourceDaily_Guid == dailyRunGuid);
        }

        public IEnumerable<TblMasterActualJobServiceStopLegs> FindByJobHeader(Guid jobGuid)
        {
            return DbContext.TblMasterActualJobServiceStopLegs.Where(e => e.MasterActualJobHeader_Guid == jobGuid);
        }

        public AdhocCheckDuplicateJobInDayResult Func_Adhoc_CheckDuplicateJobInDay(Guid? siteGuid, DateTime? workdate, DateTime? workdateDev, int jobTypeID, Guid? jobGuid, Guid? lobGuid, Guid? customerGuid, Guid? cusLocationGuid, Guid? customerDevGuid, Guid? cusLocationDevGuid, Guid? siteDevGuid)
        {
            return DbContext.Up_OceanOnlineMVC_Adhoc_CheckDuplicateJobInDay_Get(siteGuid, workdate, workdateDev, jobTypeID, jobGuid, lobGuid, customerGuid, cusLocationGuid, customerDevGuid, cusLocationDevGuid, siteDevGuid).FirstOrDefault();
        }

        public IEnumerable<JobLegsView> GetJobDetailForUpdateOtc(Guid jobHeadGuid)
        {
            // Status 2 : dispatch
            int[] allowRunStatus = new int[] { StatusDailyRun.DispatchRun };
            var jobLegs = DbContext.TblMasterActualJobServiceStopLegs.Where(w => w.MasterActualJobHeader_Guid == jobHeadGuid).Join(
                DbContext.TblMasterDailyRunResource.Where(w => allowRunStatus.Contains(w.RunResourceDailyStatusID ?? 0)),
                        leg => leg.MasterRunResourceDaily_Guid, run => run.Guid,
                        (leg, run) => new { leg });
            var result = jobLegs.Join(DbContext.TblMasterCustomerLocation,
                        leg => leg.leg.MasterCustomerLocation_Guid,
                        loc => loc.Guid,
                        (leg, loc) => new { leg, loc })
                 .Join(DbContext.TblMasterCustomer, legLoc => legLoc.loc.MasterCustomer_Guid, cus => cus.Guid, (legLoc, cus) => new JobLegsView
                 {
                     JobGuid = legLoc.leg.leg.MasterActualJobHeader_Guid ?? Guid.Empty,
                     CustomerLocateionName = legLoc.loc.BranchName,
                     JobLegGuid = legLoc.leg.leg.Guid
                 });
            return result;
        }

        public IEnumerable<RunControlJobLegsDetailByJobGetResult> Func_GetJobLegsDetailByJob(Guid jobGuid, string formatDate, Guid? siteGuid)
        {
            return DbContext.Up_OceanOnlineMVC_RunControl_JobLegsDetailByJob_Get(jobGuid, siteGuid, formatDate);
        }

        public IEnumerable<LegView> GetLegsDetail(Guid jobGuid)
        {

            var result = from legs in DbContext.TblMasterActualJobServiceStopLegs
                         join loc in DbContext.TblMasterCustomerLocation on legs.MasterCustomerLocation_Guid equals loc.Guid
                         join cus in DbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cus.Guid
                         join action in DbContext.TblSystemJobAction on legs.CustomerLocationAction_Guid equals action.Guid
                         //Left Join
                         join route in DbContext.TblMasterRouteGroup_Detail on legs.MasterRouteGroupDetail_Guid equals route.Guid into routeEmp
                         from empRoute in routeEmp.DefaultIfEmpty()
                             //Left Join
                         join daily in DbContext.TblMasterDailyRunResource on legs.MasterRunResourceDaily_Guid equals daily.Guid into dailyEmp
                         from empDaily in dailyEmp.DefaultIfEmpty()
                             //Left Join
                         join run in DbContext.TblMasterRunResource on empDaily.MasterRunResource_Guid equals run.Guid into runEmp
                         from Emprun in runEmp.DefaultIfEmpty()

                         where legs.MasterActualJobHeader_Guid == jobGuid
                         select new LegView
                         {
                             ActionName = action.ActionName,
                             ActualTimeDT = legs.ActualTime,
                             ArrivalTimeDT = legs.ArrivalTime,
                             CustomerName = cus.CustomerFullName,
                             DepartTimeDT = legs.DepartTime,
                             LocationName = loc.BranchName,
                             ReceiptNo = legs.PrintedReceiptNumber,
                             Remarks = legs.RemarksLeg,
                             ServiceStopTransectionDate = legs.ServiceStopTransectionDate,
                             RouteGroupDetailName = empRoute.MasterRouteGroupDetailName,
                             RunResourceNumber = Emprun.VehicleNumber,
                             FlagNonBillable = legs.FlagNonBillable
                         };

            return result;
        }

        public IEnumerable<TblMasterActualJobServiceStopLegs> GetLegsForOptimizationByDailyRun(Guid jobGuid)
        {
            var result = from legs in DbContext.TblMasterActualJobServiceStopLegs
                         join loc in DbContext.TblMasterCustomerLocation on legs.MasterCustomerLocation_Guid equals loc.Guid
                         join cus in DbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cus.Guid
                         where legs.MasterRunResourceDaily_Guid.Value == jobGuid && cus.FlagChkCustomer == true
                         select legs;

            return result;
        }
        public IEnumerable<TblMasterActualJobServiceStopLegs> GetLegsForOptimizationByDailyRun(List<Guid> jobGuid)
        {
            var result = from legs in DbContext.TblMasterActualJobServiceStopLegs
                         join loc in DbContext.TblMasterCustomerLocation on legs.MasterCustomerLocation_Guid equals loc.Guid
                         join cus in DbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cus.Guid
                         where jobGuid.Contains(legs.MasterRunResourceDaily_Guid.Value) && cus.FlagChkCustomer == true
                         select legs;

            return result;
        }

        public IEnumerable<TblMasterActualJobServiceStopLegs> GetALLLegsByLegGuid(List<Guid> LegGuid)
        {
            var getJobT = (from legs in DbContext.TblMasterActualJobServiceStopLegs
                           join Head in DbContext.TblMasterActualJobHeader on legs.MasterActualJobHeader_Guid equals Head.Guid
                           join JType in DbContext.TblSystemServiceJobType on Head.SystemServiceJobType_Guid equals JType.Guid
                           where LegGuid.Contains(legs.Guid) && JType.ServiceJobTypeID == IntTypeJob.T
                           select Head.Guid).Distinct().ToList();

            var result = from legs in DbContext.TblMasterActualJobServiceStopLegs
                         join loc in DbContext.TblMasterCustomerLocation on legs.MasterCustomerLocation_Guid equals loc.Guid
                         join cus in DbContext.TblMasterCustomer on loc.MasterCustomer_Guid equals cus.Guid
                         where (LegGuid.Contains(legs.Guid)
                               || getJobT.Contains(legs.MasterActualJobHeader_Guid.Value))
                               && cus.FlagChkCustomer == true
                         select legs;

            return result;
        }

        public IEnumerable<TblMasterActualJobServiceStopLegs> FindDestinationByJobGuid(IEnumerable<Guid> jobGuid)
        {
            return DbContext.TblMasterActualJobServiceStopLegs.Where(x => x.FlagDestination)
                .Join(jobGuid,
                l => l.MasterActualJobHeader_Guid,
                j => j,
                (l, j) => l);
        }

        public IEnumerable<TblMasterActualJobServiceStopLegs> FindByJobGuidList(IEnumerable<Guid> jobGuid)
        {
            return DbContext.TblMasterActualJobServiceStopLegs
                .Join(jobGuid,
                l => l.MasterActualJobHeader_Guid,
                j => j,
                (l, j) => l);
        }

        public IEnumerable<TblMasterActualJobServiceStopLegs> FindByLegGuidList(IEnumerable<Guid> legsGuid)
        {
            var jobAction = DbContext.TblSystemJobAction.ToList();
            return DbContext.TblMasterActualJobServiceStopLegs
                .Join(legsGuid,
                ml => ml.Guid,
                l => l,
                (ml, l) => ml).AsEnumerable()
                .Select(o => {
                    o.ActionNameAbbr = jobAction.FirstOrDefault(e => e.Guid == o.CustomerLocationAction_Guid).ActionNameAbbrevaition;
                    return o;
                });
        }
    }
}
