using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation
{
    #region Interface

    public interface IMasterCustomerLocationBrinksSiteRepository : IRepository<TblMasterCustomerLocation_BrinksSite>
    {
       
    }

    #endregion
    public class MasterCustomerLocationBrinksSiteRepository : Repository<OceanDbEntities, TblMasterCustomerLocation_BrinksSite>, IMasterCustomerLocationBrinksSiteRepository
    {
        public MasterCustomerLocationBrinksSiteRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

    }
}
