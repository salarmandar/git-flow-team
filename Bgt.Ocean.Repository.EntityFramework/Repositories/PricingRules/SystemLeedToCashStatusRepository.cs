using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.PricingRules
{
    public interface ISystemLeedToCashStatusRepository : IRepository<TblSystemLeedToCashStatus>
    {
    }

    public class SystemLeedToCashStatusRepository : Repository<OceanDbEntities, TblSystemLeedToCashStatus>, ISystemLeedToCashStatusRepository
    {
        public SystemLeedToCashStatusRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
