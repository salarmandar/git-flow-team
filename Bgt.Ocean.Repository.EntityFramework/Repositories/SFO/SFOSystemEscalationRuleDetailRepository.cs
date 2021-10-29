using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOSystemEscalationRuleDetailRepository : IRepository<SFOTblSystemEscalationRuleDetail>
    {
    }

    public class SFOSystemEscalationRuleDetailRepository : Repository<OceanDbEntities, SFOTblSystemEscalationRuleDetail>, ISFOSystemEscalationRuleDetailRepository
    {
        public SFOSystemEscalationRuleDetailRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
