using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemApplicationRepository : IRepository<TblSystemApplication>
    {
        TblSystemApplication FindByApplicationID(int appID);
    }

    public class SystemApplicationRepository : Repository<OceanDbEntities, TblSystemApplication>, ISystemApplicationRepository
    {
        public SystemApplicationRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblSystemApplication FindByApplicationID(int appID)
        {
            return DbContext.TblSystemApplication.FirstOrDefault(e => e.ApplicationID == appID);
        }
    }
}
