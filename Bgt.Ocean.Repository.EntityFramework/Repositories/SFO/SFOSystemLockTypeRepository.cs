using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOSystemLockTypeRepository : IRepository<SFOTblSystemLockType>
    {

    }

    #endregion

    public class SFOSystemLockTypeRepository : Repository<OceanDbEntities, SFOTblSystemLockType>, ISFOSystemLockTypeRepository
    {
        public SFOSystemLockTypeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
