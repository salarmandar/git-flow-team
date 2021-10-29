using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Customer
{
    #region Interface

    public interface IMasterCustomerContractServiceLocationRepository : IRepository<TblMasterCustomerContract_ServiceLocation>
    {

    }

    #endregion

    public class MasterCustomerContractServiceLocationRepository : Repository<OceanDbEntities, TblMasterCustomerContract_ServiceLocation>, IMasterCustomerContractServiceLocationRepository
    {
        public MasterCustomerContractServiceLocationRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
