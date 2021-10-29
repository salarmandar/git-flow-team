using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterMCSBulkSuspectFakeEntryRepository : IRepository<TblMasterActualJobMCSBulkSuspectFakeEntry> { }
    public class MasterMCSBulkSuspectFakeEntryRepository : Repository<OceanDbEntities, TblMasterActualJobMCSBulkSuspectFakeEntry>, IMasterMCSBulkSuspectFakeEntryRepository
    {
        public MasterMCSBulkSuspectFakeEntryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
