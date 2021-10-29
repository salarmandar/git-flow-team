using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{


    public interface IMasterActualJobMCSCoinCashAddEntryRepository : IRepository<TblMasterActualJobMCSCoinCashAddEntry> { }
    public class MasterActualJobMCSCoinCashAddEntryRepository : Repository<OceanDbEntities, TblMasterActualJobMCSCoinCashAddEntry>, IMasterActualJobMCSCoinCashAddEntryRepository
    {
        public MasterActualJobMCSCoinCashAddEntryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
