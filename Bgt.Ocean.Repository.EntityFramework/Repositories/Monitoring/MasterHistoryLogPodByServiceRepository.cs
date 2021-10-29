using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Monitoring
{
    public interface IMasterHistoryLogPodByServiceRepository : IRepository<TblMasterHistoryLogPodByService>
    {
    }

    public class MasterHistoryLogPodByServiceRepository : Repository<OceanDbEntities, TblMasterHistoryLogPodByService>, IMasterHistoryLogPodByServiceRepository
    {
        public MasterHistoryLogPodByServiceRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

    }
}
