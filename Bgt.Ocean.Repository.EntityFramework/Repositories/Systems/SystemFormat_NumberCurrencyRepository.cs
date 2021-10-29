using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemFormat_NumberCurrencyRepository : IRepository<TblSystemFormat_NumberCurrency>
    {
    }

    public class SystemFormat_NumberCurrencyRepository : Repository<OceanDbEntities, TblSystemFormat_NumberCurrency>, ISystemFormat_NumberCurrencyRepository
    {
        public SystemFormat_NumberCurrencyRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
