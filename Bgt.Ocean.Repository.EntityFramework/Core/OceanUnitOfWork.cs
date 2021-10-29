using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Core
{
    public class OceanUnitOfWork : UnitOfWork<OceanDbEntities>
    {
        public OceanUnitOfWork(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
