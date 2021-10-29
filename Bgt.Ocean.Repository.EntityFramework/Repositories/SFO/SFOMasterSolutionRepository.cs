using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOMasterSolutionRepository : IRepository<SFOTblMasterSolution>
    {

    }

    #endregion

    public class SFOMasterSolutionRepository : Repository<OceanDbEntities, SFOTblMasterSolution>, ISFOMasterSolutionRepository
    {
        public SFOMasterSolutionRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
