using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region interface

    public interface ISFOTransactionServiceRequestProblemRepository : IRepository<SFOTblTransactionServiceRequest_Problem>
    {

    }

    #endregion

    public class SFOTransactionServiceRequestProblemRepository : Repository<OceanDbEntities, SFOTblTransactionServiceRequest_Problem>, ISFOTransactionServiceRequestProblemRepository
    {
        public SFOTransactionServiceRequestProblemRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
