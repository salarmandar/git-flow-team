using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOTransactionServiceRequestEcashRepository : IRepository<SFOTblTransactionServiceRequest_Ecash>
    {

    }

    #endregion

    public class SFOTransactionServiceRequestEcashRepository : Repository<OceanDbEntities, SFOTblTransactionServiceRequest_Ecash>, ISFOTransactionServiceRequestEcashRepository
    {
        public SFOTransactionServiceRequestEcashRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
