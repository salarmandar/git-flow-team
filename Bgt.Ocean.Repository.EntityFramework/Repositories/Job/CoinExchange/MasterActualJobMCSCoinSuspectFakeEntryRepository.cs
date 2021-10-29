using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobMCSCoinSuspectFakeEntryRepository : IRepository<TblMasterActualJobMCSCoinSuspectFakeEntry> { }
    public class MasterActualJobMCSCoinSuspectFakeEntryRepository : Repository<OceanDbEntities, TblMasterActualJobMCSCoinSuspectFakeEntry>, IMasterActualJobMCSCoinSuspectFakeEntryRepository
    {
        public MasterActualJobMCSCoinSuspectFakeEntryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
