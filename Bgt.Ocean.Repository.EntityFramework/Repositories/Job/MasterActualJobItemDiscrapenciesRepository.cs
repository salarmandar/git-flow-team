using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobItemDiscrapenciesRepository : IRepository<TblMasterActualJobItemDiscrapencies>
    {
    }

    public class MasterActualJobItemDiscrapenciesRepository : Repository<OceanDbEntities, TblMasterActualJobItemDiscrapencies>, IMasterActualJobItemDiscrapenciesRepository
    {
        public MasterActualJobItemDiscrapenciesRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
