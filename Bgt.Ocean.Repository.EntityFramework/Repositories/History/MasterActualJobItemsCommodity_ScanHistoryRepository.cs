using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.History
{
    public interface IMasterActualJobItemsCommodity_ScanHistoryRepository : IRepository<TblMasterActualJobItemsCommodity_ScanHistory>
    {
    }
    public class MasterActualJobItemsCommodity_ScanHistoryRepository : Repository<OceanDbEntities, TblMasterActualJobItemsCommodity_ScanHistory>, IMasterActualJobItemsCommodity_ScanHistoryRepository
    {
        public MasterActualJobItemsCommodity_ScanHistoryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
