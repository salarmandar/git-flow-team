using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOMasterMonitoringNetworkRepository : IRepository<SFOTblMasterMonitoringNetwork>
    {
    }

    public class SFOMasterMonitoringNetworkRepository : Repository<OceanDbEntities, SFOTblMasterMonitoringNetwork>, ISFOMasterMonitoringNetworkRepository
    {
        public SFOMasterMonitoringNetworkRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

    }
  
}
