using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    #region interface

    public interface ISystemDayOfWeekRepository : IRepository<TblSystemDayOfWeek>
    {

    }

    #endregion

    public class SystemDayOfWeekRepository : Repository<OceanDbEntities, TblSystemDayOfWeek>, ISystemDayOfWeekRepository
    {
        public SystemDayOfWeekRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
