using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOTransactionServiceRequestSolutionRepository : IRepository<SFOTblTransactionServiceRequest_Solution>
    {

    }
    #endregion

    public class SFOTransactionServiceRequestSolutionRepository : Repository<OceanDbEntities, SFOTblTransactionServiceRequest_Solution>, ISFOTransactionServiceRequestSolutionRepository
    {
        public SFOTransactionServiceRequestSolutionRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
