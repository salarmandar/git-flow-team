using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.History
{
    public interface IMasterHistory_SealRepository : IRepository<TblMasterHistory_Seal>
    {
    }
    public class MasterHistory_SealRepository : Repository<OceanDbEntities, TblMasterHistory_Seal>, IMasterHistory_SealRepository
    {
        public MasterHistory_SealRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

    }
}
