using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.FleetMaintenance
{
    public interface IMasterAccidentListDetailDamagedRespository : IRepository<TblMasterRunResource_Accident_ListDetailDamaged>
    {
    }
    public class MasterAccidentListDetailDamagedRespository : Repository<OceanDbEntities, TblMasterRunResource_Accident_ListDetailDamaged>, IMasterAccidentListDetailDamagedRespository
    {
        public MasterAccidentListDetailDamagedRespository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
