using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobActualCountRepository : IRepository<TblMasterActualJobActualCount> { }
    public class MasterActualJobActualCountRepository : Repository<OceanDbEntities, TblMasterActualJobActualCount>, IMasterActualJobActualCountRepository
    {
        public MasterActualJobActualCountRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
