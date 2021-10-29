using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterSpecialCommandRepository : IRepository<TblMasterSpecialCommand>
    {
    }

    public class MasterSpecialCommandRepository : Repository<OceanDbEntities, TblMasterSpecialCommand>, IMasterSpecialCommandRepository
    {
        public MasterSpecialCommandRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
