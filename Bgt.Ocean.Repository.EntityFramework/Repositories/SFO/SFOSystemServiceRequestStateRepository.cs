using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOSystemServiceRequestStateRepository : IRepository<SFOTblSystemServiceRequestState>
    {
    }

    public class SFOSystemServiceRequestStateRepository : Repository<OceanDbEntities, SFOTblSystemServiceRequestState>, ISFOSystemServiceRequestStateRepository
    {
        public SFOSystemServiceRequestStateRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
