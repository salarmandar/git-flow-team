using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOSystemEscalationRuleHeader_DetailRepository : IRepository<SFOTblSystemEscalationRuleHeader_EscalationRuleDetail>
    {
    }

    public class SFOSystemEscalationRuleHeader_DetailRepository : Repository<OceanDbEntities, SFOTblSystemEscalationRuleHeader_EscalationRuleDetail>, ISFOSystemEscalationRuleHeader_DetailRepository
    {
        public SFOSystemEscalationRuleHeader_DetailRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
