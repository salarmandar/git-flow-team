using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Core
{
    public class SFOLogDbUnitOfWork : UnitOfWork<SFOLogDbEntities>
    {
        public SFOLogDbUnitOfWork(IDbFactory<SFOLogDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
