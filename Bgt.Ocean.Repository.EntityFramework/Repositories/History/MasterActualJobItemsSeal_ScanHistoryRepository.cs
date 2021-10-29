using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.History
{
    public interface IMasterActualJobItemsSeal_ScanHistoryRepository : IRepository<TblMasterActualJobItemsSeal_ScanHistory>
    {
    }

    public class MasterActualJobItemsSeal_ScanHistoryRepository : Repository<OceanDbEntities, TblMasterActualJobItemsSeal_ScanHistory>, IMasterActualJobItemsSeal_ScanHistoryRepository
    {
        public MasterActualJobItemsSeal_ScanHistoryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

    }
}
