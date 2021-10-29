using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.OnHandRoute;
using Bgt.Ocean.Models.RouteOptimization;
using Bgt.Ocean.Repository.EntityFramework.Extensions;
using Bgt.Ocean.Repository.EntityFramework.StringQuery.OnHandRoute;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.MasterRoute
{
    public interface IMasterRouteRepository : IRepository<TblMasterRoute>
    {
        IEnumerable<TblMasterRoute> GetMasterRoute(MasterRouteNameRequestModel request);
        IEnumerable<RunControlRunResourceDailyBySiteAndDateGetResult> GetDailyRunBySiteAndWorkDate(Guid siteGuid, DateTime workDate, Guid? userGuid);
        List<JobDetailOnRunView> GetJobDetailOnRun(List<Guid> dailyRunGuidList, Guid languageGuid, bool flagIncludeJobCancel = false);
    }
    public class MasterRouteRepository : Repository<OceanDbEntities, TblMasterRoute>, IMasterRouteRepository
    {
        #region Objects & Variables
        private readonly JobDetailOnRunQuery jobDetailOnRunQuery = new JobDetailOnRunQuery();
        #endregion
        public MasterRouteRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblMasterRoute> GetMasterRoute(MasterRouteNameRequestModel request)
        {
            return DbContext.TblMasterRoute.Where(w => !w.FlagDisable
                                                    && w.MasterSite_Guid == request.SiteGuid
                                                    && w.MasterDayOfweek_Guid == request.DayOfWeekGuid
                                                    && w.SystemMaterRouteTypeOfWeek_Guid == request.WeekGuid
                                                    && w.FlagHoliday == request.FlagHoliday);
        }

        public IEnumerable<RunControlRunResourceDailyBySiteAndDateGetResult> GetDailyRunBySiteAndWorkDate(Guid siteGuid, DateTime workDate, Guid? userGuid)
        {
            return DbContext.Up_OceanOnlineMVC_RunControl_RunResourceDailyBySiteAndDate_Get(siteGuid, userGuid, workDate, 0);
        }

        public List<JobDetailOnRunView> GetJobDetailOnRun(List<Guid> dailyRunGuidList,Guid languageGuid, bool flagIncludeJobCancel = false)
        {
            using (OceanDbEntities context = new OceanDbEntities())
            {
                var command = new SqlCommand(jobDetailOnRunQuery.GetJobDetailOnRun);
                var parameters = command.AddArrayParameters("DailyRunGuid", dailyRunGuidList);
                parameters.Add(new SqlParameter("@FlagIncludeJobCancel", flagIncludeJobCancel));
                parameters.Add(new SqlParameter("@LanguageGuid", languageGuid));


                return context.Database.SqlQuery<JobDetailOnRunView>(command.CommandText, parameters.ToArray()).ToList();
            }
        }
    }
}
