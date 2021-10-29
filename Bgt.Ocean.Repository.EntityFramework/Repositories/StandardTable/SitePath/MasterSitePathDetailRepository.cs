using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable.SitePath
{
    public interface IMasterSitePathDetailRepository : IRepository<TblMasterSitePathDetail>
    {
    }
    public class MasterSitePathDetailRepository : Repository<OceanDbEntities, TblMasterSitePathDetail>, IMasterSitePathDetailRepository
    {
        public MasterSitePathDetailRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
