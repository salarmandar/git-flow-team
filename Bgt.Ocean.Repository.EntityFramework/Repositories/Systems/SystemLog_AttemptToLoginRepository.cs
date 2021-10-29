using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemLog_AttemptToLoginRepository : IRepository<TblSystemLog_AttemptToLogin>
    {
    }

    public class SystemLog_AttemptToLoginRepository : Repository<OceanDbEntities, TblSystemLog_AttemptToLogin>, ISystemLog_AttemptToLoginRepository
    {
        public SystemLog_AttemptToLoginRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
