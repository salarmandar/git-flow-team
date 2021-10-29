using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.BaseModel;
using Bgt.Ocean.Models.RouteOptimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.RouteOptimization
{
    public interface IMasterRouteOptimizationStatusRepository : IRepository<TblMasterRoute_OptimizationStatus>
    {
        IEnumerable<MasterRouteOptimizationViewModel> GetRouteOptimize(MasterRouteOptimizationRequestListRequestModel request);
        IEnumerable<MasterJobRequestOptimizeViewModel> GetMasterRouteJobLegRequestOptimize(Guid masterRouteGuid, IEnumerable<Guid> routeGroupDetailGuid, Guid masterSiteGuid);
        IEnumerable<string> ValidateStatusBeforeRequestSave(IEnumerable<Guid> routeGroupDetailGuid, Guid masterSiteGuid);
        ValidateSameRouteGroupDetailNameInprogress ValidateSameRouteGroupDetailNameInprogress(IEnumerable<Guid> routeGroupDetailGuid, Guid masterSiteGuid);
        IEnumerable<MasterRouteSummaryView> GetRouteOptimizeSummary(MasterRouteSummaryRequest request);
    }
    public class MasterRouteOptimizationStatusRepository : Repository<OceanDbEntities, TblMasterRoute_OptimizationStatus>, IMasterRouteOptimizationStatusRepository
    {
        public MasterRouteOptimizationStatusRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<MasterRouteOptimizationViewModel> GetRouteOptimize(MasterRouteOptimizationRequestListRequestModel request)
        {
            IEnumerable<MasterRouteOptimizationViewModel> result = null;
            if (request.UserLangquage.IsNullOrEmpty())
            {
                request.UserLangquage = DbContext.TblSystemLanguage.First(f => f.Abbreviation == "EN-US" && f.FlagDisable == false).Guid;
            }
            var mainRoute = (from H in DbContext.TblMasterRouteJobHeader.Where(w => w.MasterRoute_Guid == request.MasterRouteGuid && !(bool)w.FlagDisable)
                             join L in DbContext.TblMasterRouteJobServiceStopLegs.Where(w => !w.FlagDisableRoute && !(bool)w.FlagDeliveryLegForTV) on H.Guid equals L.MasterRouteJobHeader_Guid
                             join Loc in DbContext.TblMasterCustomerLocation on L.MasterCustomerLocation_Guid equals Loc.Guid
                             join Cust in DbContext.TblMasterCustomer.Where(w => (bool)w.FlagChkCustomer) on Loc.MasterCustomer_Guid equals Cust.Guid
                             select new
                             {
                                 JobHeadGuid = L.MasterRouteJobHeader_Guid,
                                 LegGuid = L.Guid,
                                 Location = L.MasterCustomerLocation_Guid,
                                 FlagLocationDisable = Loc.FlagDisable,
                                 FlagCustomerDisable = Cust.FlagDisable ?? false,
                                 FlagDefaultBrinks = Loc.TblMasterCustomerLocation_BrinksSite.Any(w => w.MasterSite_Guid == request.SiteGuid && w.FlagDefaultBrinksSite),
                                 RouteGroupDetailGuid = L.MasterRouteGroupDetail_Guid ?? Guid.Empty,
                                 JobOrder = L.JobOrder ?? 0
                             }
                             );

            var anotherRoute = (from L in DbContext.TblMasterRouteJobServiceStopLegs.Where(w => !w.FlagDisableRoute && w.MasterRouteDeliveryLeg_Guid == request.MasterRouteGuid && (bool)w.FlagDeliveryLegForTV)
                                join H in DbContext.TblMasterRouteJobHeader.Where(w => !(bool)w.FlagDisable) on L.MasterRouteJobHeader_Guid equals H.Guid
                                join Loc in DbContext.TblMasterCustomerLocation on L.MasterCustomerLocation_Guid equals Loc.Guid
                                join Cust in DbContext.TblMasterCustomer.Where(w => (bool)w.FlagChkCustomer) on Loc.MasterCustomer_Guid equals Cust.Guid
                                select new
                                {
                                    JobHeadGuid = L.MasterRouteJobHeader_Guid,
                                    LegGuid = L.Guid,
                                    Location = L.MasterCustomerLocation_Guid,
                                    FlagLocationDisable = Loc.FlagDisable,
                                    FlagCustomerDisable = Cust.FlagDisable ?? false,
                                    FlagDefaultBrinks = Loc.TblMasterCustomerLocation_BrinksSite.Any(w => w.MasterSite_Guid == request.SiteGuid && w.FlagDefaultBrinksSite),
                                    RouteGroupDetailGuid = L.MasterRouteGroupDetail_Guid ?? Guid.Empty,
                                    JobOrder = L.JobOrder ?? 0
                                }
                             );
            var allData = mainRoute.Union(anotherRoute)?.ToList();
            if (allData.Any())
            {
                var ingnore = allData.Where(w => w.FlagCustomerDisable || w.FlagLocationDisable || !w.FlagDefaultBrinks).Select(s => s.JobHeadGuid).Distinct();
                var activeJob = allData.Where(w => !ingnore.Any(a => a == w.JobHeadGuid)).ToList();
                var routeGroupFromTemplate = allData.GroupBy(g => g.RouteGroupDetailGuid).Select(s => s.Key).ToList();
                var optimizStatus = DbContext.TblMasterRoute_OptimizationStatus.Where(w => w.MasterRoute_Guid == request.MasterRouteGuid && routeGroupFromTemplate.Any(a => a == w.MasterRouteGroupDetail_Guid))?.ToList();
                var sysOptimize = DbContext.TblSystemRouteOptimizationStatus
                                .Join(
                                    DbContext.TblSystemDisplayTextControlsLanguage.Where(w => w.SystemLanguageGuid == request.UserLangquage)
                                    , op => op.SystemDisplayTextControlsLanguage_Guid
                                    , dis => dis.Guid
                                    , (op, dis) => new
                                    {
                                        SystemOptimizeGuid = op.Guid,
                                        DisplayText = dis.DisplayText,
                                        OptimizeId = op.RouteOptimizationStatusID
                                    });
                var none = sysOptimize?.FirstOrDefault(f => f.OptimizeId == OptimizationStatusID.NONE);
                result = DbContext.TblMasterRouteGroup_Detail.Where(w => routeGroupFromTemplate.Any(a => a == w.Guid))
                                .Join(DbContext.TblMasterRouteGroup, rgd => rgd.MasterRouteGroup_Guid, rg => rg.Guid
                                , (rgd, rg) => new
                                {
                                    RouteGroupDetailGuid = rgd.Guid,
                                    RouteGroupDetailName = rgd.MasterRouteGroupDetailName,
                                    RoutGroupGuid = rgd.MasterRouteGroup_Guid,
                                    RouteGroupName = rg.MasterRouteGroupName,
                                    FlagRgdDisable = rgd.FlagDisable,
                                    FlagRgDisable = rg.FlagDisable

                                }).AsEnumerable()
                                .Take(request.MaxRow)
                                .Select(s =>
                                {
                                    var jobInRgd = activeJob.Where(w => w.RouteGroupDetailGuid == s.RouteGroupDetailGuid);
                                    var totalStop = jobInRgd.GroupBy(g => g.JobOrder).Count();
                                    var totalLocation = jobInRgd.GroupBy(g => g.Location).Count();
                                    var totalJob = jobInRgd.GroupBy(g => g.JobHeadGuid).Count();
                                    var opStatus = optimizStatus?.FirstOrDefault(w => w.MasterRouteGroupDetail_Guid == s.RouteGroupDetailGuid);
                                    int status = OptimizationStatusID.NONE;
                                    string statusName = "";
                                    if (opStatus != null)
                                    {
                                        var displayStatus = sysOptimize.FirstOrDefault(f => f.SystemOptimizeGuid == opStatus.SystemRouteOptimizationStatus_Guid);
                                        status = displayStatus.OptimizeId;
                                        statusName = displayStatus.DisplayText;
                                    }
                                    else
                                    {
                                        statusName = none.DisplayText;
                                    }
                                    return new MasterRouteOptimizationViewModel
                                    {
                                        MasterRouteGroupDetailGuid = s.RouteGroupDetailGuid,
                                        RouteGroupDetail = s.RouteGroupDetailName,
                                        RouteGroup = s.RouteGroupName,
                                        Stops = totalStop,
                                        StopsStr = totalStop.ToString(),
                                        Locations = totalLocation,
                                        LocationsStr = totalLocation.ToString(),
                                        Jobs = totalJob,
                                        JobsStr = totalJob.ToString(),
                                        OptimizationStatus = statusName,
                                        OptimizationStatusID = status
                                    };
                                });
            }
            return result;

        }


        public IEnumerable<MasterRouteSummaryView> GetRouteOptimizeSummary(MasterRouteSummaryRequest request)
        {
            IEnumerable<MasterRouteSummaryView> result = null;
            if (request.UserLangquage.IsNullOrEmpty())
            {
                request.UserLangquage = DbContext.TblSystemLanguage.First(f => f.Abbreviation == "EN-US" && f.FlagDisable == false).Guid;
            }
            var weekGuid = DbContext.TblSystemMaterRouteTypeOfWeek.FirstOrDefault(f => f.WeekTypeInt == request.WeekTypeInt)?.Guid;
            var mainRoute = (from R in DbContext.TblMasterRoute.Where(w => w.MasterSite_Guid == request.SiteGuid && w.MasterDayOfweek_Guid == request.DayOfWeekGuid && w.SystemMaterRouteTypeOfWeek_Guid == weekGuid)
                             join H in DbContext.TblMasterRouteJobHeader.Where(w => !(bool)w.FlagDisable) on R.Guid equals H.MasterRoute_Guid
                             join L in DbContext.TblMasterRouteJobServiceStopLegs.Where(w => !w.FlagDisableRoute && !(bool)w.FlagDeliveryLegForTV) on H.Guid equals L.MasterRouteJobHeader_Guid
                             join Loc in DbContext.TblMasterCustomerLocation on L.MasterCustomerLocation_Guid equals Loc.Guid
                             join Cust in DbContext.TblMasterCustomer.Where(w => (bool)w.FlagChkCustomer) on Loc.MasterCustomer_Guid equals Cust.Guid
                             select new
                             {
                                 MasterRouteGuid = R.Guid,
                                 MasterRouteName = R.MasterRouteName,
                                 JobHeadGuid = L.MasterRouteJobHeader_Guid,
                                 LegGuid = L.Guid,
                                 Location = L.MasterCustomerLocation_Guid,
                                 FlagLocationDisable = Loc.FlagDisable,
                                 FlagCustomerDisable = Cust.FlagDisable ?? false,
                                 FlagDefaultBrinks = Loc.TblMasterCustomerLocation_BrinksSite.Any(w => w.MasterSite_Guid == request.SiteGuid && w.FlagDefaultBrinksSite),
                                 RouteGroupDetailGuid = L.MasterRouteGroupDetail_Guid ?? Guid.Empty,
                                 JobOrder = L.JobOrder ?? 0,
                                 FlagHoliday = R.FlagHoliday
                             }
                             );

            var anotherRoute = (from R in DbContext.TblMasterRoute.Where(w => w.MasterSite_Guid == request.SiteGuid && w.MasterDayOfweek_Guid == request.DayOfWeekGuid && w.SystemMaterRouteTypeOfWeek_Guid == weekGuid)
                                join L in DbContext.TblMasterRouteJobServiceStopLegs.Where(w => !w.FlagDisableRoute && (bool)w.FlagDeliveryLegForTV) on R.Guid equals L.MasterRouteDeliveryLeg_Guid
                                join H in DbContext.TblMasterRouteJobHeader.Where(w => !(bool)w.FlagDisable) on L.MasterRouteJobHeader_Guid equals H.Guid
                                join Loc in DbContext.TblMasterCustomerLocation on L.MasterCustomerLocation_Guid equals Loc.Guid
                                join Cust in DbContext.TblMasterCustomer.Where(w => (bool)w.FlagChkCustomer) on Loc.MasterCustomer_Guid equals Cust.Guid
                                select new
                                {
                                    MasterRouteGuid = R.Guid,
                                    MasterRouteName = R.MasterRouteName,
                                    JobHeadGuid = L.MasterRouteJobHeader_Guid,
                                    LegGuid = L.Guid,
                                    Location = L.MasterCustomerLocation_Guid,
                                    FlagLocationDisable = Loc.FlagDisable,
                                    FlagCustomerDisable = Cust.FlagDisable ?? false,
                                    FlagDefaultBrinks = Loc.TblMasterCustomerLocation_BrinksSite.Any(w => w.MasterSite_Guid == request.SiteGuid && w.FlagDefaultBrinksSite),
                                    RouteGroupDetailGuid = L.MasterRouteGroupDetail_Guid ?? Guid.Empty,
                                    JobOrder = L.JobOrder ?? 0,
                                    FlagHoliday = R.FlagHoliday
                                }
                             );
            var allData = mainRoute.Union(anotherRoute);
            if (allData.Any())
            {
                //var ingnore = allData.Where(w => w.FlagCustomerDisable || w.FlagLocationDisable || !w.FlagDefaultBrinks).Select(s => s.JobHeadGuid).Distinct()
                var activeJob = allData.ToList();
                var routeGroupFromTemplate = allData.GroupBy(g => new { g.MasterRouteGuid, g.MasterRouteName, g.RouteGroupDetailGuid })
                    .Select(s => new
                    {
                        s.Key.MasterRouteGuid,
                        s.Key.MasterRouteName,
                        s.Key.RouteGroupDetailGuid
                    }).ToList();
                var masterRoute = routeGroupFromTemplate.GroupBy(g => g.MasterRouteGuid).Select(s => s.Key);
                var optimizStatus = DbContext.TblMasterRoute_OptimizationStatus.Where(w => masterRoute.Any(a => a == w.MasterRoute_Guid
                //&& a.RouteGroupDetailGuid == w.MasterRouteGroupDetail_Guid
                )
                )?.ToList();
                var sysOptimize = DbContext.TblSystemRouteOptimizationStatus
                                .Join(
                                    DbContext.TblSystemDisplayTextControlsLanguage.Where(w => w.SystemLanguageGuid == request.UserLangquage)
                                    , op => op.SystemDisplayTextControlsLanguage_Guid
                                    , dis => dis.Guid
                                    , (op, dis) => new
                                    {
                                        SystemOptimizeGuid = op.Guid,
                                        DisplayText = dis.DisplayText,
                                        OptimizeId = op.RouteOptimizationStatusID
                                    });
                var rgdList = routeGroupFromTemplate.GroupBy(g => g.RouteGroupDetailGuid).Select(s => s.Key);
                var none = sysOptimize?.FirstOrDefault(f => f.OptimizeId == OptimizationStatusID.NONE);
                var rgdData = DbContext.TblMasterRouteGroup_Detail.Where(w => rgdList.Any(a => a == w.Guid))
                                .Join(DbContext.TblMasterRouteGroup, rgd => rgd.MasterRouteGroup_Guid, rg => rg.Guid
                                 , (rgd, rg) => new
                                 {
                                     RouteGroupDetailGuid = rgd.Guid,
                                     RouteGroupDetailName = rgd.MasterRouteGroupDetailName,
                                     RoutGroupGuid = rgd.MasterRouteGroup_Guid,
                                     RouteGroupName = rg.MasterRouteGroupName,
                                     FlagRgdDisable = rgd.FlagDisable,
                                     FlagRgDisable = rg.FlagDisable
                                 });
                result = routeGroupFromTemplate
                           .Join(rgdData,
                           r => r.RouteGroupDetailGuid
                           , rgd => rgd.RouteGroupDetailGuid,
                           (r, rgd) =>
                           {

                               var jobInRgd = activeJob.Where(w => w.RouteGroupDetailGuid == r.RouteGroupDetailGuid && w.MasterRouteGuid == r.MasterRouteGuid);
                               var totalStop = jobInRgd.GroupBy(g => g.JobOrder).Count();
                               var totalLocation = jobInRgd.GroupBy(g => g.Location).Count();
                               var totalJob = jobInRgd.GroupBy(g => g.JobHeadGuid).Count();
                               var opStatus = optimizStatus?.FirstOrDefault(w => r.MasterRouteGuid == w.MasterRoute_Guid && w.MasterRouteGroupDetail_Guid == r.RouteGroupDetailGuid);
                               int status = OptimizationStatusID.NONE;
                               string statusName = "";
                               if (opStatus != null)
                               {
                                   var displayStatus = sysOptimize.FirstOrDefault(f => f.SystemOptimizeGuid == opStatus.SystemRouteOptimizationStatus_Guid);
                                   status = displayStatus.OptimizeId;
                                   statusName = displayStatus.DisplayText;
                               }
                               else
                               {
                                   statusName = none.DisplayText;
                               }
                               return new MasterRouteSummaryView
                               {
                                   //MasterDayOfWeek_Sequence = 
                                   MasterRouteName = r.MasterRouteName,
                                   Holiday = jobInRgd.Any(a => (bool)a.FlagHoliday) ? "Yes" : "No",
                                   RouteGroupDetailGuid = rgd.RouteGroupDetailGuid,
                                   MasterRouteGroupDetailName = rgd.RouteGroupDetailName,
                                   MasterRouteGroupName = rgd.RouteGroupName,
                                   Stops = totalStop,
                                   StrStops = totalStop.ToString(),
                                   Locations = totalLocation,
                                   StrLocations = totalLocation.ToString(),
                                   Jobs = totalJob,
                                   StrJobs = totalJob.ToString(),
                                   OptimizationStatus = statusName,
                                   OptimizationStatusId = status
                               };
                           }).OrderBy(o => o.MasterRouteName).ThenBy(o => o.MasterRouteGroupName).ThenBy(o => o.MasterRouteGroupDetailName);
            }
            return result;

        }

        public IEnumerable<MasterJobRequestOptimizeViewModel> GetMasterRouteJobLegRequestOptimize(Guid masterRouteGuid, IEnumerable<Guid> routeGroupDetailGuid, Guid masterSiteGuid)
        {
            IEnumerable<MasterJobRequestOptimizeViewModel> result = null;
            var mainRoute = (from H in DbContext.TblMasterRouteJobHeader.Where(w => w.MasterRoute_Guid == masterRouteGuid && !(bool)w.FlagDisable)
                             join L in DbContext.TblMasterRouteJobServiceStopLegs.Where(w => !w.FlagDisableRoute && !(bool)w.FlagDeliveryLegForTV && routeGroupDetailGuid.Any(a => a == w.MasterRouteGroupDetail_Guid)) on H.Guid equals L.MasterRouteJobHeader_Guid
                             join Loc in DbContext.TblMasterCustomerLocation on L.MasterCustomerLocation_Guid equals Loc.Guid
                             join Cust in DbContext.TblMasterCustomer.Where(w => (bool)w.FlagChkCustomer) on Loc.MasterCustomer_Guid equals Cust.Guid
                             select new
                             {
                                 JobHeadGuid = L.MasterRouteJobHeader_Guid,
                                 LegGuid = L.Guid,
                                 Location = L.MasterCustomerLocation_Guid,
                                 FlagLocationDisable = Loc.FlagDisable,
                                 FlagCustomerDisable = Cust.FlagDisable ?? false,
                                 FlagDefaultBrinks = Loc.TblMasterCustomerLocation_BrinksSite.Any(w => w.MasterSite_Guid == masterSiteGuid && w.FlagDefaultBrinksSite),
                                 RouteGroupDetailGuid = L.MasterRouteGroupDetail_Guid ?? Guid.Empty,
                                 JobOrder = L.JobOrder ?? 0,
                                 SystemJobTypeGuid = (Guid)H.SystemServiceJobType_Guid
                             }
                            );

            var anotherRoute = (from L in DbContext.TblMasterRouteJobServiceStopLegs.Where(w => !w.FlagDisableRoute && w.MasterRouteDeliveryLeg_Guid == masterRouteGuid && (bool)w.FlagDeliveryLegForTV && routeGroupDetailGuid.Any(a => a == w.MasterRouteGroupDetail_Guid))
                                join H in DbContext.TblMasterRouteJobHeader.Where(w => !(bool)w.FlagDisable) on L.MasterRouteJobHeader_Guid equals H.Guid
                                join Loc in DbContext.TblMasterCustomerLocation on L.MasterCustomerLocation_Guid equals Loc.Guid
                                join Cust in DbContext.TblMasterCustomer.Where(w => (bool)w.FlagChkCustomer) on Loc.MasterCustomer_Guid equals Cust.Guid
                                select new
                                {
                                    JobHeadGuid = L.MasterRouteJobHeader_Guid,
                                    LegGuid = L.Guid,
                                    Location = L.MasterCustomerLocation_Guid,
                                    FlagLocationDisable = Loc.FlagDisable,
                                    FlagCustomerDisable = Cust.FlagDisable ?? false,
                                    FlagDefaultBrinks = Loc.TblMasterCustomerLocation_BrinksSite.Any(w => w.MasterSite_Guid == masterSiteGuid && w.FlagDefaultBrinksSite),
                                    RouteGroupDetailGuid = L.MasterRouteGroupDetail_Guid ?? Guid.Empty,
                                    JobOrder = L.JobOrder ?? 0,
                                    SystemJobTypeGuid = (Guid)H.SystemServiceJobType_Guid

                                }
                             );
            var allData = mainRoute.Union(anotherRoute)?.ToList();
            if (allData.Any())
            {
                var ingnore = allData.Where(w => w.FlagCustomerDisable || w.FlagLocationDisable || !w.FlagDefaultBrinks)
                    .Select(s => s.JobHeadGuid).Distinct();
                result = allData.Where(w => !ingnore.Any(a => a == w.JobHeadGuid))
                    .Select(s => new MasterJobRequestOptimizeViewModel
                    {
                        RouteGroupDetailGuid = s.RouteGroupDetailGuid,
                        JobHeaderGuid = s.JobHeadGuid ?? Guid.Empty,
                        JobLegGuid = s.LegGuid,
                        LocationGuid = (Guid)s.Location,
                        JobOrder = s.JobOrder,
                        SystemServiceJobTypeGuid = s.SystemJobTypeGuid
                    });
            }
            return result;
        }

        public ValidateSameRouteGroupDetailNameInprogress ValidateSameRouteGroupDetailNameInprogress(IEnumerable<Guid> routeGroupDetailGuid, Guid masterSiteGuid)
        {
            ValidateSameRouteGroupDetailNameInprogress result = null;
            var countryGuid = DbContext.TblMasterSite.FirstOrDefault(w => w.Guid == masterSiteGuid).MasterCountry_Guid;
            var tmpRgd = DbContext.TblMasterRouteGroup_Detail.Where(w => routeGroupDetailGuid.Any(a => a == w.Guid))
                            .Select(s => s.MasterRouteGroupDetailName);

            var rgddata = (from opDetail in DbContext.TblTransactionRouteOptimizationHeader_Detail
                                                        .Where(w =>w.TblTransactionRouteOptimizationHeader.TblSystemRouteOptimizationRouteType.RouteOptimizationRouteTypeCode == OptimizationRouteTypeID.RM && tmpRgd.Any(a => a == w.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName)
                                                                        && (w.TblTransactionRouteOptimizationHeader.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID == OptimizationStatusID.REQUESTING 
                                                                        || w.TblTransactionRouteOptimizationHeader.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID == OptimizationStatusID.INPROGRESS
                                                                        || w.TblTransactionRouteOptimizationHeader.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID == OptimizationStatusID.CANCELING
                                                                        ))
                           join site in DbContext.TblMasterSite.Where(w => w.MasterCountry_Guid == countryGuid && !w.FlagDisable)
                                on opDetail.TblMasterRouteGroup_Detail.MasterSite_Guid equals site.Guid
                           select new
                           {
                               RequestID = opDetail.TblTransactionRouteOptimizationHeader.RequestID,
                               RgdName = opDetail.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName
                           });      

            var unionData = rgddata;
            if (unionData.Any())
            {
                string name = string.Join(",", unionData.GroupBy(g => g.RgdName).Select(s => s.Key));
                string requestId = string.Join(",", unionData.GroupBy(g => g.RequestID).Select(s => s.Key));
                result = new ValidateSameRouteGroupDetailNameInprogress()
                {
                    RouteGroupDetailName = name,
                    RequestId = requestId

                };

            }
            return result;
        }

        public IEnumerable<string> ValidateStatusBeforeRequestSave(IEnumerable<Guid> routeGroupDetailGuid, Guid masterSiteGuid)
        {

            int[] allowCreate = new int[] {
                    OptimizationStatusID.NONE,
                    OptimizationStatusID.COMPLETED,
                    OptimizationStatusID.BROKEN,
                    OptimizationStatusID.CANCELED,
                    OptimizationStatusID.FAILED
                };

            var hasRoute = DbContext.TblMasterRoute_OptimizationStatus
                            .Where(
                                    w => routeGroupDetailGuid.Any(a => a == w.MasterRouteGroupDetail_Guid)
                                    && !allowCreate.Any(a => a == w.TblSystemRouteOptimizationStatus.RouteOptimizationStatusID))
                           .Select(s => s.TblMasterRouteGroup_Detail.MasterRouteGroupDetailName);


            return hasRoute;
        }

        public IEnumerable<TblMasterRoute_OptimizationStatus> GetRouteOptimizeStatusByRequest (IEnumerable<Guid> routeRequestIds)
        {
            //var d = DbContext.TblMasterRoute_OptimizationStatus.Where(w=>w.)
            return null;
        }
    }
}
