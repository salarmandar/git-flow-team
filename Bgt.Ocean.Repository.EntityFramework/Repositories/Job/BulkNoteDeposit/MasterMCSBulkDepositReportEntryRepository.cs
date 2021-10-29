using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterMCSBulkDepositReportEntryRepository : IRepository<TblMasterActualJobMCSBulkDepositReportEntry> { }
    public class MasterMCSBulkDepositReportEntryRepository : Repository<OceanDbEntities, TblMasterActualJobMCSBulkDepositReportEntry>, IMasterMCSBulkDepositReportEntryRepository
    {
        public MasterMCSBulkDepositReportEntryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
