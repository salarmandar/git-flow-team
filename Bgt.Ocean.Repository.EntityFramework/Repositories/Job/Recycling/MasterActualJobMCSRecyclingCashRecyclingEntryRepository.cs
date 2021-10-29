using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{

    public interface IMasterActualJobMCSRecyclingCashRecyclingEntryRepository : IRepository<TblMasterActualJobMCSRecyclingCashRecyclingEntry> { }
    public class MasterActualJobMCSRecyclingCashRecyclingEntryRepository : Repository<OceanDbEntities, TblMasterActualJobMCSRecyclingCashRecyclingEntry>, IMasterActualJobMCSRecyclingCashRecyclingEntryRepository
    {
        public MasterActualJobMCSRecyclingCashRecyclingEntryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
