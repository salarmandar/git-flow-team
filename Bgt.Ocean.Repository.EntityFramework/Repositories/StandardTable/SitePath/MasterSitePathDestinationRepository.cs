using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.StandardTable.SitePath
{
    public interface IMasterSitePathDestinationRepository : IRepository<TblMasterSitePathDestination>
    {

    }
    public class MasterSitePathDestinationRepository : Repository<OceanDbEntities, TblMasterSitePathDestination>, IMasterSitePathDestinationRepository
    {
        
        public MasterSitePathDestinationRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {

        }
    }
}
