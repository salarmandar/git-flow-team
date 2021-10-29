using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable.SitePath
{
    public interface IMasterSitePathAuditLogRepository : IRepository<TblMasterSitePathAuditLog> { }
    public class MasterSitePathAuditLogRepository : Repository<OceanDbEntities, TblMasterSitePathAuditLog>, IMasterSitePathAuditLogRepository
    {
        public MasterSitePathAuditLogRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
