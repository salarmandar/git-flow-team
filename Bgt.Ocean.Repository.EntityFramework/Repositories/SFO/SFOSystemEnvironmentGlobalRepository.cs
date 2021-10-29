using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOSystemEnvironmentGlobalRepository : IRepository<SFOTblSystemEnvironment_Global>
    {
        SFOTblSystemEnvironment_Global FindByAppKey(string inAppKey);
    }

    public class SFOSystemEnvironmentGlobalRepository : Repository<OceanDbEntities, SFOTblSystemEnvironment_Global>, ISFOSystemEnvironmentGlobalRepository
    {
        public SFOSystemEnvironmentGlobalRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public SFOTblSystemEnvironment_Global FindByAppKey(string inAppKey) => FindAll(e => e.AppKey == inAppKey).FirstOrDefault();
    }
}
