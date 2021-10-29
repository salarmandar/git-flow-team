using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;


namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{

    public interface IMasterVendorRepository : IRepository<TblMasterVendor>
    {
    }

    public class MasterVendorRepository : Repository<OceanDbEntities, TblMasterVendor>, IMasterVendorRepository
    {
        public MasterVendorRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

    }
}
