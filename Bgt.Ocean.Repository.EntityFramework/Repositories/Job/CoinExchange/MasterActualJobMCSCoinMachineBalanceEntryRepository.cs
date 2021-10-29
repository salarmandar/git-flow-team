using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{

    public interface IMasterActualJobMCSCoinMachineBalanceEntryRepository : IRepository<TblMasterActualJobMCSCoinMachineBalanceEntry> { }
    public class MasterActualJobMCSCoinMachineBalanceEntryRepository : Repository<OceanDbEntities, TblMasterActualJobMCSCoinMachineBalanceEntry>, IMasterActualJobMCSCoinMachineBalanceEntryRepository
    {
        public MasterActualJobMCSCoinMachineBalanceEntryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
