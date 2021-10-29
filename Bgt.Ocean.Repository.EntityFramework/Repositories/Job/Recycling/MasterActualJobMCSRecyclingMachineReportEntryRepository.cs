using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobMCSRecyclingMachineReportEntryRepository : IRepository<TblMasterActualJobMCSRecyclingMachineReportEntry> { }
    public class MasterActualJobMCSRecyclingMachineReportEntryRepository : Repository<OceanDbEntities, TblMasterActualJobMCSRecyclingMachineReportEntry>, IMasterActualJobMCSRecyclingMachineReportEntryRepository
    {
        public MasterActualJobMCSRecyclingMachineReportEntryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
