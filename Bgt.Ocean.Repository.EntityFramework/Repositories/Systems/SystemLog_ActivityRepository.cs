using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemLog_ActivityRepository : IRepository<TblSystemLog_Activity>
    {
    }

    public class SystemLog_ActivityRepository : Repository<OceanDbEntities, TblSystemLog_Activity>, ISystemLog_ActivityRepository
    {
        public SystemLog_ActivityRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }        
    }
}
