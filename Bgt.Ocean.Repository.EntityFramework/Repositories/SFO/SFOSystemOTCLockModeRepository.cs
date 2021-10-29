using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOSystemOTCLockModeRepository :IRepository<SFOTblSystemOTCLockMode> { }
    public class SFOSystemOTCLockModeRepository : Repository<OceanDbEntities, SFOTblSystemOTCLockMode>, ISFOSystemOTCLockModeRepository
    {
        public SFOSystemOTCLockModeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
