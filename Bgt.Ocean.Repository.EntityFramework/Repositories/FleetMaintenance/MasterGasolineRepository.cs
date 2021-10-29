using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance
{

    public interface IMasterGasolineRepository : IRepository<TblMasterGasloine>
    {
    }
    public class MasterGasolineRepository : Repository<OceanDbEntities, TblMasterGasloine>, IMasterGasolineRepository
    {
        public MasterGasolineRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

    }
}
