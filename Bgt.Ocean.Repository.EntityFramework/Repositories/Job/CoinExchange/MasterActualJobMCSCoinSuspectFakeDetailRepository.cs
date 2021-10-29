using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobMCSCoinSuspectFakeDetailRepository : IRepository<TblMasterActualJobMCSCoinSuspectFakeDetail> { }
    public class MasterActualJobMCSCoinSuspectFakeDetailRepository : Repository<OceanDbEntities, TblMasterActualJobMCSCoinSuspectFakeDetail>, IMasterActualJobMCSCoinSuspectFakeDetailRepository
    {
        public MasterActualJobMCSCoinSuspectFakeDetailRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
