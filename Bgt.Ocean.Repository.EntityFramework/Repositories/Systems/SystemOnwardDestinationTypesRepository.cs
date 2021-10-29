using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    #region Interface

    public interface ISystemOnwardDestinationTypesRepository : IRepository<TblSystemOnwardDestinationType>
    {        
    }

    #endregion

    public class SystemOnwardDestinationTypesRepository : Repository<OceanDbEntities, TblSystemOnwardDestinationType>, ISystemOnwardDestinationTypesRepository
    {
        public SystemOnwardDestinationTypesRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
