using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.History
{
    public interface IMasterHistory_ActualJobRepository : IRepository<TblMasterHistory_ActualJob>
    {
    }
    public class MasterHistory_ActualJobRepository : Repository<OceanDbEntities, TblMasterHistory_ActualJob>, IMasterHistory_ActualJobRepository
    {
        public MasterHistory_ActualJobRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
      
    }
}
