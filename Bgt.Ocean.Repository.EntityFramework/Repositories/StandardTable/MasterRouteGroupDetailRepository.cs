using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.StandardTable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable
{
    public interface IMasterRouteGroupDetailRepository : IRepository<TblMasterRouteGroup_Detail>
    {
        IEnumerable<TblMasterRouteGroup_Detail> FindBySite(Guid siteGuid);
        RouteGroupAndGroupDetailView FindRouteGroupAndGroupDetailBySite(Guid siteGuid);
        string GetRouteName(Guid routeGroup_Detail_Guid, Guid dailyRunGuid);
        string GetFullname(Guid routeGroupDetailGuid);
        string GetFullRouteNameByDailyRun(Guid dailyRunGuid);
    }
    public class MasterRouteGroupDetailRepository : Repository<OceanDbEntities, TblMasterRouteGroup_Detail>, IMasterRouteGroupDetailRepository
    {
        public MasterRouteGroupDetailRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterRouteGroup_Detail> FindBySite(Guid siteGuid)
        {
            var allGroupEnable = DbContext.TblMasterRouteGroup.Where(a => a.FlagDisable == false).Select(a => a.Guid).ToList();
            return DbContext.TblMasterRouteGroup_Detail.Where(e => e.MasterSite_Guid == siteGuid && e.FlagDisable == false && allGroupEnable.Contains(e.MasterRouteGroup_Guid));
        }


        //public IEnumerable<RouteGroupAndGroupDetailView> FindRouteGroupAndGroupDetailBySite(Guid siteGuid)
        //{
        //    IEnumerable<RouteGroupAndGroupDetailView> result = new List<RouteGroupAndGroupDetailView>();


        //    var routeAll = DbContext.TblMasterRouteGroups.Where(a => a.FlagDisable == false)
        //        .Join(DbContext.TblMasterRouteGroup_Detail.Where(w => w.MasterSite_Guid == siteGuid && w.FlagDisable == false), g => g.Guid, d => d.MasterRouteGroup_Guid, (g, d) => new
        //        {
        //            g,
        //            d
        //        });
        //    var routeGroup = routeAll.Select(s => new { s.g.Guid, s.g.MasterRouteGroupName }).Distinct();
        //    result = routeGroup.Select(s => new RouteGroupAndGroupDetailView
        //    {
        //        RouteGuid = s.Guid,
        //        RouteName = s.MasterRouteGroupName
        //        ,
        //        RouteGrupDetail = routeAll.Where(w => w.d.MasterRouteGroup_Guid == s.Guid).Select(ss => new RouteGroupDetailView
        //        {
        //            DetailGuid = ss.d.Guid,
        //            DetailName = ss.d.MasterRouteGroupDetailName
        //        })
        //    });

        //    return result;
        //}


        public RouteGroupAndGroupDetailView FindRouteGroupAndGroupDetailBySite(Guid siteGuid)
        {
            RouteGroupAndGroupDetailView result = new RouteGroupAndGroupDetailView();
            var routeAll = DbContext.TblMasterRouteGroup.Where(a => a.FlagDisable == false)
                .Join(DbContext.TblMasterRouteGroup_Detail.Where(w => w.MasterSite_Guid == siteGuid && w.FlagDisable == false),
                g => g.Guid, d => d.MasterRouteGroup_Guid, (g, d) => new
                {
                    g,
                    d
                });
            result.RouteGroup = routeAll.Select(s => new RouteGroupDetailView
            {
                RouteGuid = s.g.Guid,
                RouteName = s.g.MasterRouteGroupName
            }).Distinct().OrderBy(o => o.RouteName).ToList();
            result.RouteGrupDetail = routeAll.Select(s => new RouteGroupDetailView
            {
                RouteGuid = s.g.Guid,
                DetailGuid = s.d.Guid,
                DetailName = s.d.MasterRouteGroupDetailName
            }).OrderBy(o => o.DetailName).ToList();

            return result;
        }

        public string GetRouteName(Guid routeGroup_Detail_Guid, Guid dailyRunGuid)
        {
            var groupDetail = DbContext.TblMasterRouteGroup_Detail.FirstOrDefault(w => w.Guid == routeGroup_Detail_Guid);
            var dailyRun = DbContext.TblMasterDailyRunResource.FirstOrDefault(w => w.Guid == dailyRunGuid);
            var runResourceNo = DbContext.TblMasterRunResource.FirstOrDefault(w => w.Guid == dailyRun.MasterRunResource_Guid).VehicleNumber;
            var dailyRunShift = dailyRun.MasterRunResourceShift;
            string routeName = dailyRunShift == 1 ? groupDetail?.MasterRouteGroupDetailName + " - " + runResourceNo : groupDetail?.MasterRouteGroupDetailName + " - " + runResourceNo + " (" + dailyRunShift + ")";
            return routeName;
        }

        public string GetFullname(Guid routeGroupDetailGuid)
        {
            return DbContext.TblMasterRouteGroup_Detail.Where(o => o.Guid == routeGroupDetailGuid)
                        .Join(DbContext.TblMasterRouteGroup, rDetail => rDetail.MasterRouteGroup_Guid, rGroup => rGroup.Guid, (rDetail, rGroup) => rGroup.MasterRouteGroupName + " - " + rDetail.MasterRouteGroupDetailName)
                        .FirstOrDefault();
        }

        public string GetFullRouteNameByDailyRun(Guid dailyRunGuid)
        {
            var dailyRun = DbContext.TblMasterDailyRunResource.FirstOrDefault(w => w.Guid == dailyRunGuid);
            var groupDetail = DbContext.TblMasterRouteGroup_Detail.FirstOrDefault(w => w.Guid == dailyRun.MasterRouteGroup_Detail_Guid);
            var runResourceNo = DbContext.TblMasterRunResource.FirstOrDefault(w => w.Guid == dailyRun.MasterRunResource_Guid).VehicleNumber;
            var dailyRunShift = dailyRun.MasterRunResourceShift;
            var routeGrp = DbContext.TblMasterRouteGroup.FirstOrDefault(x => x.Guid == groupDetail.MasterRouteGroup_Guid)?.MasterRouteGroupName;
            string[] str = new string[] { routeGrp, groupDetail?.MasterRouteGroupDetailName, runResourceNo + (dailyRunShift == 1 ? string.Empty : " (" + dailyRunShift + ")") };
            return string.Join(" - ", str);
        }
    }


}
