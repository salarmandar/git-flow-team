using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOMasterSourceRepository : IRepository<SFOTblMasterSource>
    {

    }

    #endregion

    public class SFOMasterSourceRepository : Repository<OceanDbEntities, SFOTblMasterSource>, ISFOMasterSourceRepository
    {
        public SFOMasterSourceRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
