using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterMCSBulkJammedEntryRepository : IRepository<TblMasterActualJobMCSBulkJammedEntry> { }
    public class MasterMCSBulkJammedEntryRepository : Repository<OceanDbEntities, TblMasterActualJobMCSBulkJammedEntry>, IMasterMCSBulkJammedEntryRepository
    {
        public MasterMCSBulkJammedEntryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
