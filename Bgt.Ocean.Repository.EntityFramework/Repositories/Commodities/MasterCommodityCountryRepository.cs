using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface IMasterCommodityCountryRepository : IRepository<TblMasterCommodityCountry>
    {
    }

    public class MasterCommodityCountryRepository : Repository<OceanDbEntities, TblMasterCommodityCountry>, IMasterCommodityCountryRepository
    {
        public MasterCommodityCountryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
