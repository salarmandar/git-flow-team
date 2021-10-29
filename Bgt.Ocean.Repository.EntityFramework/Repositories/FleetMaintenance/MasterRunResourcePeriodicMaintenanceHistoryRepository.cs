

using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance
{
    public interface IMasterRunResourcePeriodicMaintenanceHistoryRepository : IRepository<TblMasterRunResource_PeriodicMaintenance_History>
    {
    }

    public class MasterRunResourcePeriodicMaintenanceHistoryRepository : Repository<OceanDbEntities, TblMasterRunResource_PeriodicMaintenance_History>, IMasterRunResourcePeriodicMaintenanceHistoryRepository
    {
        public MasterRunResourcePeriodicMaintenanceHistoryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }

}
