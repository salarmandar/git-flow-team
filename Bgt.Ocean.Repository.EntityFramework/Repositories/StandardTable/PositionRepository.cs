using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable
{
    public interface IPositionRepository : IRepository<TblMasterPosition>
    {

    }

    public class PositionRepository : Repository<OceanDbEntities, TblMasterPosition>, IPositionRepository
    {
        public PositionRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
