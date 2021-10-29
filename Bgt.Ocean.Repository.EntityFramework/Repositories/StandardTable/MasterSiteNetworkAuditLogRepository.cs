using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable
{
    public interface IMasterSiteNetworkAuditLogRepository : IRepository<TblMasterSiteNetworkAuditLog>
    {

    }
    public class MasterSiteNetworkAuditLogRepository : Repository<OceanDbEntities, TblMasterSiteNetworkAuditLog>, IMasterSiteNetworkAuditLogRepository
    {
        public MasterSiteNetworkAuditLogRepository(IDbFactory<OceanDbEntities> dbFactory)
            : base(dbFactory)
        {

        }
    }
}
