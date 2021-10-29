
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance
{
    public interface IMasterRunResourceMaintenanceDetailRepository : IRepository<TblMasterRunResource_Maintenance_Detail>
    {
    }

    public class MasterRunResourceMaintenanceDetailRepository : Repository<OceanDbEntities, TblMasterRunResource_Maintenance_Detail>, IMasterRunResourceMaintenanceDetailRepository
    {
        public MasterRunResourceMaintenanceDetailRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
