using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobCashAddRepository:IRepository<TblMasterActualJobCashAdd> 
    {
    }
    public class MasterActualJobCashAddRepository : Repository<OceanDbEntities, TblMasterActualJobCashAdd>, IMasterActualJobCashAddRepository
    {
        public MasterActualJobCashAddRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }


}
