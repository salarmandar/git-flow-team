using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Monitoring
{
    public interface IMasterHistoryReportPushToSmartRepository : IRepository<TblMasterHistory_ReportPushToSmart>
    {
        IEnumerable<TblMasterHistory_ReportPushToSmart> GetSmartBillingAutoGenBySite(Guid siteGuid);
    }
    public class MasterHistoryReportPushToSmartRepository : Repository<OceanDbEntities, TblMasterHistory_ReportPushToSmart>, IMasterHistoryReportPushToSmartRepository
    {
        public MasterHistoryReportPushToSmartRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }


        public IEnumerable<TblMasterHistory_ReportPushToSmart> GetSmartBillingAutoGenBySite(Guid siteGuid)
        {
            return DbContext.TblMasterHistory_ReportPushToSmart
                   .Where(e => e.MasterSite_Guid == siteGuid && e.FlagAutoGen).ToList();
        }
    }
}
