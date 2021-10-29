using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{

    public interface IMasterActualJobMCSCoinCashReturnEntryRepository : IRepository<TblMasterActualJobMCSCoinCashReturnEntry> { }
    public class MasterActualJobMCSCoinCashReturnEntryRepository : Repository<OceanDbEntities, TblMasterActualJobMCSCoinCashReturnEntry>, IMasterActualJobMCSCoinCashReturnEntryRepository
    {
        public MasterActualJobMCSCoinCashReturnEntryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
