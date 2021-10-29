using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobMCSItemSealRepository : IRepository<TblMasterActualJobMCSItemSeal> { }
    public class MasterActualJobMCSItemSealRepository : Repository<OceanDbEntities, TblMasterActualJobMCSItemSeal>, IMasterActualJobMCSItemSealRepository
    {
        public MasterActualJobMCSItemSealRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }


}
