using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterSubServiceTypeRepository : IRepository<TblMasterSubServiceType>
    {
        IQueryable<TblMasterSubServiceType> FindAllAsQueryable();
    }

    public class MasterSubServiceTypeRepository : Repository<OceanDbEntities, TblMasterSubServiceType>, IMasterSubServiceTypeRepository
    {
        public MasterSubServiceTypeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }

        public IQueryable<TblMasterSubServiceType> FindAllAsQueryable()
        {
            return DbContext.TblMasterSubServiceType;
        }
    }
}
