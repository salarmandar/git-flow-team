using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    public interface ISystemNotificationConfigPeriodsUsersRepository : IRepository<TblSystemNotificationConfigPeriodsUsers>
    {
    }

    public class SystemNotificationConfigPeriodsUsersRepository : Repository<OceanDbEntities, TblSystemNotificationConfigPeriodsUsers>, ISystemNotificationConfigPeriodsUsersRepository
    {
        public SystemNotificationConfigPeriodsUsersRepository(IDbFactory<OceanDbEntities> dbFactory)
            : base(dbFactory)
        {

        }
    }
}