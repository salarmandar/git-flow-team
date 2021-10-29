using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Configuration;
namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemCustomerLocationTypeRepository : IRepository<TblSystemCustomerLocationType> { }
    public class SystemCustomerLocationTypeRepository : Repository<OceanDbEntities, TblSystemCustomerLocationType>, ISystemCustomerLocationTypeRepository
    {
        public SystemCustomerLocationTypeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
