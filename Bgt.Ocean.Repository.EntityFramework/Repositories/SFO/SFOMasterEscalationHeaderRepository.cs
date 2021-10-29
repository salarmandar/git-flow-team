using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOMasterEscalationHeaderRepository : IRepository<SFOTblMasterEscalationHeader>
    {
    }

    public class SFOMasterEscalationHeaderRepository : Repository<OceanDbEntities, SFOTblMasterEscalationHeader>, ISFOMasterEscalationHeaderRepository
    {
        public SFOMasterEscalationHeaderRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
