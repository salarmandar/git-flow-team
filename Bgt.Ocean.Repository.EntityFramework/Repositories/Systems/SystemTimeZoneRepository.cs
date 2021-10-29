using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    #region interface

    public interface ISystemTimeZoneRepository : IRepository<TblSystemTimezone>
    {

    }

    #endregion

    public class SystemTimeZoneRepository : Repository<OceanDbEntities, TblSystemTimezone>, ISystemTimeZoneRepository
    {
        public SystemTimeZoneRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
