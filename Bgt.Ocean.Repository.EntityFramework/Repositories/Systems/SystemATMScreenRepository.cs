using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemATMScreenRepository : IRepository<TblSystemATMScreen> {
    }
    public class SystemATMScreenRepository : Repository<OceanDbEntities, TblSystemATMScreen>, ISystemATMScreenRepository
    {
        public SystemATMScreenRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
