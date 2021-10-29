using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOMasterJournalRepository : IRepository<SFOTblMasterJournal>
    {

    }

    #endregion

    public class SFOMasterJournalRepository : Repository<OceanDbEntities, SFOTblMasterJournal>, ISFOMasterJournalRepository
    {
        public SFOMasterJournalRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
