using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface ISystemEnvironmentMasterSiteValueRepository : IRepository<TblSystemEnvironmentMasterSiteValue>
    {
    }
    public class SystemEnvironmentMasterSiteValueRepository : Repository<OceanDbEntities, TblSystemEnvironmentMasterSiteValue>, ISystemEnvironmentMasterSiteValueRepository
    {
        public SystemEnvironmentMasterSiteValueRepository(IDbFactory<OceanDbEntities> dbFactory)
            : base(dbFactory)
        {
        }
    }
}
