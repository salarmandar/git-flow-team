using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOMasterProblemRepository : IRepository<SFOTblMasterProblem>
    {

    }

    #endregion

    public class SFOMasterProblemRepository : Repository<OceanDbEntities, SFOTblMasterProblem>, ISFOMasterProblemRepository
    {
        public SFOMasterProblemRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
