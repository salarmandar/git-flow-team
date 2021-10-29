using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOMasterEscalationDetail_PositionRepository : IRepository<SFOTblMasterEscalationDetail_Position>
    {
    }

    public class SFOMasterEscalationDetail_PositionRepository : Repository<OceanDbEntities, SFOTblMasterEscalationDetail_Position>, ISFOMasterEscalationDetail_PositionRepository
    {
        public SFOMasterEscalationDetail_PositionRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
