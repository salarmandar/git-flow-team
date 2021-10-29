using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterPlaceRepository : IRepository<TblMasterPlace>
    {

    }
    public class MasterPlaceRepository : Repository<OceanDbEntities, TblMasterPlace>, IMasterPlaceRepository
    {
        public MasterPlaceRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

    }
}
