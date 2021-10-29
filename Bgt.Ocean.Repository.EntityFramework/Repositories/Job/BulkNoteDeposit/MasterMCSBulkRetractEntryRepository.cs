using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterMCSBulkRetractEntryRepository : IRepository<TblMasterActualJobMCSBulkRetractEntry> { }
    public class MasterMCSBulkRetractEntryRepository : Repository<OceanDbEntities, TblMasterActualJobMCSBulkRetractEntry>, IMasterMCSBulkRetractEntryRepository
    {
        public MasterMCSBulkRetractEntryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
