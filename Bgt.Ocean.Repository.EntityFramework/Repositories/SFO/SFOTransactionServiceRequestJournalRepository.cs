using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOTransactionServiceRequestJournalRepository : IRepository<SFOTblTransactionServiceRequest_Journal>
    {

    }

    #endregion

    public class SFOTransactionServiceRequestJournalRepository : Repository<OceanDbEntities, SFOTblTransactionServiceRequest_Journal>, ISFOTransactionServiceRequestJournalRepository
    {
        public SFOTransactionServiceRequestJournalRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
