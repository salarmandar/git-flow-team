using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{

    public interface IMasterActualJobMCSRecyclingActualCountEntryRepository : IRepository<TblMasterActualJobMCSRecyclingActualCountEntry> { }
    public class MasterActualJobMCSRecyclingActualCountEntryRepository : Repository<OceanDbEntities, TblMasterActualJobMCSRecyclingActualCountEntry>, IMasterActualJobMCSRecyclingActualCountEntryRepository
    {
        public MasterActualJobMCSRecyclingActualCountEntryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
