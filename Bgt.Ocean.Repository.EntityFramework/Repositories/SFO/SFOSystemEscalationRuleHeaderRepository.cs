using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOSystemEscalationRuleHeaderRepository : IRepository<SFOTblSystemEscalationRuleHeader>
    {
    }

    public class SFOSystemEscalationRuleHeaderRepository : Repository<OceanDbEntities, SFOTblSystemEscalationRuleHeader>, ISFOSystemEscalationRuleHeaderRepository
    {
        public SFOSystemEscalationRuleHeaderRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
