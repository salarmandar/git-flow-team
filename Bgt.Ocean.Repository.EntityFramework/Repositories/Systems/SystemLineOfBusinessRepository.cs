using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemLineOfBusinessRepository : IRepository<TblSystemLineOfBusiness> { }
    public class SystemLineOfBusinessRepository : Repository<OceanDbEntities, TblSystemLineOfBusiness>, ISystemLineOfBusinessRepository
    {
        public SystemLineOfBusinessRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
