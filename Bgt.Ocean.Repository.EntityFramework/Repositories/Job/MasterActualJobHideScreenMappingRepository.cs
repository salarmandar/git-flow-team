using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobHideScreenMappingRepository : IRepository<TblMasterActualJobHideScreenMapping>
    {
    }

    public class MasterActualJobHideScreenMappingRepository : Repository<OceanDbEntities, TblMasterActualJobHideScreenMapping>, IMasterActualJobHideScreenMappingRepository
    {
        public MasterActualJobHideScreenMappingRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

    }
}
