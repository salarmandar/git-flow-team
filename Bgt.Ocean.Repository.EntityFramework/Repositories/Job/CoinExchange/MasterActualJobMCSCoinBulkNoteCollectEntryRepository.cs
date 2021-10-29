using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobMCSCoinBulkNoteCollectEntryRepository : IRepository<TblMasterActualJobMCSCoinBulkNoteCollectEntry> { }
    public class MasterActualJobMCSCoinBulkNoteCollectEntryRepository : Repository<OceanDbEntities, TblMasterActualJobMCSCoinBulkNoteCollectEntry>, IMasterActualJobMCSCoinBulkNoteCollectEntryRepository
    {
        public MasterActualJobMCSCoinBulkNoteCollectEntryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
