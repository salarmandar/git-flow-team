using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Configuration;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISFOSystemEnvironment_GlobalRepository : IRepository<SFOTblSystemEnvironment_Global>
    {        
    }

    public class SFOSystemEnvironment_GlobalRepository : Repository<OceanDbEntities, SFOTblSystemEnvironment_Global>, ISFOSystemEnvironment_GlobalRepository
    {
        public SFOSystemEnvironment_GlobalRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
       
    }

 }

