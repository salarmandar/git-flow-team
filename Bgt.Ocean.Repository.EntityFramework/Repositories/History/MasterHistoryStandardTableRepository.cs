
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.History
{

    public interface IMasterHistoryStandardTableRepository : IRepository<TblMasterHistory_StandardTable>
    {
    }
    public class MasterHistoryStandardTableRepository : Repository<OceanDbEntities, TblMasterHistory_StandardTable>, IMasterHistoryStandardTableRepository
    {
        public MasterHistoryStandardTableRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

    }
}
