using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.MasterRoute;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkExtras.EF6;
using Bgt.Ocean.Models.Nemo.RouteOptimization;
using System.Data.SqlClient;
using Bgt.Ocean.Repository.EntityFramework.StringQuery.RunResource;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.MasterRoute
{
    #region Interface

    public interface IMasterRouteJobHeaderRepository : IRepository<TblMasterRouteJobHeader>
    {
        MasterRouteCopyJobsProcedure MasterRouteCopyJobs(MasterRouteCopyJobsProcedure proc);
        IEnumerable<MasterRouteOptimizationView> GetMasterRoute_Optimizations(Guid siteGuid);

        IEnumerable<MasterRouteCopyDeliveryView> GetMasterRouteDeliveryLegs(IEnumerable<Guid> masterJobGuids);
        IEnumerable<TblMasterRouteJobHeader> GetMasterRouteByHeaders(List<Guid> header);
    }

    #endregion

    public class MasterRouteJobHeaderRepository : Repository<OceanDbEntities, TblMasterRouteJobHeader>, IMasterRouteJobHeaderRepository
    {
        #region Objects & Variables
        private readonly DailyRunResourceQuery dailyRunResourceQuery = new DailyRunResourceQuery();
        #endregion

        public MasterRouteJobHeaderRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        /// <summary>
        /// => TFS#62737: GET|Copy/Move TV-D List(API) => GetMasterRouteDeliveryLegs
        /// </summary>
        /// <param name="masterJobGuids"></param>
        /// <returns></returns>
        public IEnumerable<MasterRouteCopyDeliveryView> GetMasterRouteDeliveryLegs(IEnumerable<Guid> masterJobGuids)
        {
            var jobGuids = masterJobGuids.Distinct();
            var TVJobs = new int[] { IntTypeJob.TV, IntTypeJob.TV_MultiBr };
            string InterBr = " (Inter Br.)";
            string MultiBr = " (Multi Br.)";
            var ja = DbContext.TblSystemJobAction.FirstOrDefault(o => o.ActionNameAbbrevaition == JobActionAbb.StrDelivery);
            var _mrl = DbContext.TblMasterRouteJobServiceStopLegs.Where(o => jobGuids.Contains((Guid)o.MasterRouteJobHeader_Guid) && o.CustomerLocationAction_Guid == ja.Guid && o.SequenceStop == 2).ToList();
            var _mrj = DbContext.TblMasterRouteJobHeader.Where(o => jobGuids.Contains(o.Guid)).ToList();

            var mrGuids = _mrj.Select(o => o.MasterRoute_Guid);
            var dmrGuids = _mrl.Where(o => o.MasterRouteDeliveryLeg_Guid.HasValue).Select(o => o.MasterRouteDeliveryLeg_Guid).Distinct();
            var locGuids = _mrl.Select(o => o.MasterCustomerLocation_Guid).Distinct();
            var siteGuids = _mrl.Select(o => o.MasterSite_Guid).Distinct();
            var rgdGuids = _mrl.Where(o => o.MasterRouteGroupDetail_Guid.HasValue).Select(o => o.MasterRouteGroupDetail_Guid).Distinct();
            var _mr = DbContext.TblMasterRoute.Where(o => mrGuids.Contains(o.Guid)).ToList();
            var _jt = DbContext.TblSystemServiceJobType.Where(o => TVJobs.Contains((int)o.ServiceJobTypeID)).ToList();

            var _loc = DbContext.TblMasterCustomerLocation.Where(o => locGuids.Contains(o.Guid)).ToList();
            var _site = DbContext.TblMasterSite.Where(o => siteGuids.Contains(o.Guid)).ToList();
            var mrd = DbContext.TblMasterRoute.Where(o => dmrGuids.Contains(o.Guid)).ToList();

            var rgd = DbContext.TblMasterRouteGroup_Detail.Where(o => rgdGuids.Contains(o.Guid)).ToList();
            var rgGuids = rgd.Select(o => o.MasterRouteGroup_Guid).Distinct();
            var rg = DbContext.TblMasterRouteGroup.Where(o => rgGuids.Contains(o.Guid)).ToList();
            var allRgd = from _rg in rg
                         join _rgd in rgd on _rg.Guid equals _rgd.MasterRouteGroup_Guid
                         select new { rgGuid = _rg.Guid, rgName = _rg.MasterRouteGroupName, rgdGuid = _rgd.Guid, rgdName = _rgd.MasterRouteGroupDetailName };

            var defaultRGD = new { rgGuid = Guid.Empty, rgName = string.Empty, rgdGuid = Guid.Empty, rgdName = string.Empty };
            var defaultMRD = new TblMasterRoute { Guid = Guid.Empty, MasterRouteName = string.Empty };
            var jobs = (from mrj in _mrj
                        join mr in _mr on mrj.MasterRoute_Guid equals mr.Guid
                        join mrl in _mrl on mrj.Guid equals mrl.MasterRouteJobHeader_Guid
                        join jt in _jt on mrj.SystemServiceJobType_Guid equals jt.Guid
                        join loc in _loc on mrl.MasterCustomerLocation_Guid equals loc.Guid
                        join site in _site on mrl.MasterSite_Guid equals site.Guid
                        select new { mrj, mrl, mr, jt, loc, site })
                       .Select(o =>
                        {
                            var _mrd = mrd.FirstOrDefault(m => m.Guid == o.mrl.MasterRouteDeliveryLeg_Guid) ?? defaultMRD;
                            var _rgd = allRgd.FirstOrDefault(e => e.rgdGuid == o.mrl.MasterRouteGroupDetail_Guid) ?? defaultRGD;
                            var otherType = o.mrj.FlagJobMultiBranch ? (o.jt.ServiceJobTypeNameAbb + MultiBr) : o.jt.ServiceJobTypeNameAbb;

                            return new MasterRouteCopyDeliveryView
                            {
                                ActionNameAbbrevaition = ja.ActionNameAbbrevaition,
                                DayInVault = o.mrj.DayInVault ?? 0,
                                LocationName = o.loc.BranchName,
                                MasterRouteJobServiceStopLegGuid = o.mrl.Guid,
                                ServiceJobTypeNameAbb = o.mrj.FlagJobInterBranch ? (o.jt.ServiceJobTypeNameAbb + InterBr) : otherType,
                                BrinksiteGuid = o.mrl.MasterSite_Guid,
                                BrinksiteName = o.site.SiteCode + " - " + o.site.SiteName,
                                DayOfWeek_Sequence = o.mrl.DayOfWeek_Sequence ?? 0,
                                FlagHoliday = o.mr.FlagHoliday ?? false,
                                MasterDayOfweek_Guid = o.mr.MasterDayOfweek_Guid,
                                MasterRoute_Guid = _mrd.Guid,
                                RouteGroupDetailGuid = o.mrl.MasterRouteGroupDetail_Guid,

                                Original_MasterRouteName = _mrd.MasterRouteName,
                                Original_RouteGroupDetail = _rgd.rgName + " - " + _rgd.rgdName,
                                Original_MasterRouteGuid = _mrd.Guid,
                                Original_RouteGroupDetailGuid = _rgd.rgdGuid,

                                SequenceStop = o.mrl.SequenceStop,
                                JobOrder = o.mrl.JobOrder
                            };
                        })
                        .OrderByDescending(o => o.DayInVault)
                        .ThenBy(o => o.LocationName);

            return jobs;
        }


        public MasterRouteCopyJobsProcedure MasterRouteCopyJobs(MasterRouteCopyJobsProcedure proc)
        {
            proc.CantCopyJobs = DbContext.Database.ExecuteStoredProcedure<MasterRouteCopyJobsResult>(proc);
            return proc;
        }

        public IEnumerable<MasterRouteOptimizationView> GetMasterRoute_Optimizations(Guid siteGuid)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                var command = new SqlCommand(dailyRunResourceQuery.GetMasterRoute);
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@SiteGuid", siteGuid));

                return context.Database.SqlQuery<MasterRouteOptimizationView>(command.CommandText, parameters.ToArray()).ToList();
            }
        }

        public IEnumerable<TblMasterRouteJobHeader>GetMasterRouteByHeaders(List<Guid> header)
        {
            var data = DbContext.TblMasterRouteJobHeader.Where(w=>header.Any(a=>a==w.Guid));
            return data;
        }
    }
}
