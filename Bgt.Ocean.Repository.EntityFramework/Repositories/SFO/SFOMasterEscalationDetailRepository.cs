using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOMasterEscalationDetailRepository : IRepository<SFOTblMasterEscalationDetail>
    {
    }

    public class SFOMasterEscalationDetailRepository : Repository<OceanDbEntities, SFOTblMasterEscalationDetail>, ISFOMasterEscalationDetailRepository
    {
        public SFOMasterEscalationDetailRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
