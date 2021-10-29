using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Collections.Generic;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterReportRepository : IRepository<TblMasterReport>
    {
        IEnumerable<ReportProductivityMexResult> Func_Report_Productivity_Mex_Get(string dateFrom, string dateTo, string brinksSiteCode, string userName);
    }

    public class MasterReportRepository : Repository<OceanDbEntities, TblMasterReport>, IMasterReportRepository
    {
        public MasterReportRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<ReportProductivityMexResult> Func_Report_Productivity_Mex_Get(string dateFrom, string dateTo, string brinksSiteCode, string userName)
        {
            return DbContext.Up_OceanOnlineMVC_Report_Productivity_Mex_Get(dateFrom, dateTo, brinksSiteCode, userName);
        }
    }
}