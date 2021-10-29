using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Job
{
    public interface IMasterActualJobCashReturnRepository:IRepository<TblMasterActualJobCashReturn>
    {
    }
    public class MasterActualJobCashReturnRepository : Repository<OceanDbEntities, TblMasterActualJobCashReturn>, IMasterActualJobCashReturnRepository
    {
        public MasterActualJobCashReturnRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }


}
