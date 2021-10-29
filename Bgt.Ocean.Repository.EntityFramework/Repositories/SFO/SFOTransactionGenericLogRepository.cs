using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOTransactionGenericLogRepository : IRepository<SFOTblTransactionGenericLog>
    {

    }

    #endregion

    public class SFOTransactionGenericLogRepository : Repository<SFOLogDbEntities, SFOTblTransactionGenericLog>, ISFOTransactionGenericLogRepository
    {
        public SFOTransactionGenericLogRepository(IDbFactory<SFOLogDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
