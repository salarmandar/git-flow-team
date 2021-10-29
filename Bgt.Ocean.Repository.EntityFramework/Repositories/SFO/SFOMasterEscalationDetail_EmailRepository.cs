using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    public interface ISFOMasterEscalationDetail_EmailRepository : IRepository<SFOTblMasterEscalationDetail_AdditionalEmail>
    {
    }

    public class SFOMasterEscalationDetail_EmailRepository : Repository<OceanDbEntities, SFOTblMasterEscalationDetail_AdditionalEmail>, ISFOMasterEscalationDetail_EmailRepository
    {
        public SFOMasterEscalationDetail_EmailRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
