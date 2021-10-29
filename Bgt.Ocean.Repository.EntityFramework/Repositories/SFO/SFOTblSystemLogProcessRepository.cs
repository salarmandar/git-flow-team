using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOTblSystemLogProcessRepository : IRepository<SFOTblSystemLogProcess>
    {

    }

    #endregion

    public class SFOTblSystemLogProcessRepository : Repository<OceanDbEntities, SFOTblSystemLogProcess>, ISFOTblSystemLogProcessRepository
    {
        public SFOTblSystemLogProcessRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }   
}
