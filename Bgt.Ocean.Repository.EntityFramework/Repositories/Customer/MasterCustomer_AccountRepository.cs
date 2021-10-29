using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Customer
{
    public interface IMasterCustomer_AccountRepository : IRepository<TblMasterCustomer_Account>
    {
    }

    public class MasterCustomer_AccountRepository : Repository<OceanDbEntities, TblMasterCustomer_Account>, IMasterCustomer_AccountRepository
    {
        public MasterCustomer_AccountRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }


 
}
