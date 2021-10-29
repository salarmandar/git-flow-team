using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Monitoring
{
    public interface IMasterHistoryErrorPodByServiceRepository : IRepository<TblMasterHistoryErrorPodByService>
    {
    }

    public class MasterHistoryErrorPodByServiceRepository : Repository<OceanDbEntities, TblMasterHistoryErrorPodByService>, IMasterHistoryErrorPodByServiceRepository
    {
        public MasterHistoryErrorPodByServiceRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
