using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.History
{
    public interface IMasterHistory_DailyRunResourceRepository : IRepository<TblMasterHistory_DailyRunResource>
    {
    }

    public class MasterHistory_DailyRunResourceRepository : Repository<OceanDbEntities, TblMasterHistory_DailyRunResource>, IMasterHistory_DailyRunResourceRepository
    {
        public MasterHistory_DailyRunResourceRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

    }

}
