using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public class MasterActualJobMachineReportRepository : Repository<OceanDbEntities, TblMasterActualJobMachineReport>,  IMasterActualJobMachineReportRepository
    {
        public MasterActualJobMachineReportRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }

    public interface IMasterActualJobMachineReportRepository :IRepository<TblMasterActualJobMachineReport>
    {
    }
}
