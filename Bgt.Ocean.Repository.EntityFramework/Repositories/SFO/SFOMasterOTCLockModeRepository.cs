using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOMasterOTCLockModeRepository : IRepository<SFOTblMasterOTCLockMode> { }
    public class SFOMasterOTCLockModeRepository : Repository<OceanDbEntities, SFOTblMasterOTCLockMode>, ISFOMasterOTCLockModeRepository
    {
        public SFOMasterOTCLockModeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
