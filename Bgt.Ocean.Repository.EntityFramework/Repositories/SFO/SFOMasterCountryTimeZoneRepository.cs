using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOMasterCountryTimeZoneRepository : IRepository<SFOTblMasterCountryTimeZone>
    {

    }

    #endregion

    public class SFOMasterCountryTimeZoneRepository : Repository<OceanDbEntities, SFOTblMasterCountryTimeZone>, ISFOMasterCountryTimeZoneRepository
    {
        public SFOMasterCountryTimeZoneRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
