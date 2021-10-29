using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.SFO
{
    #region Interface

    public interface ISFOMasterMachineServiceTypeRepository : IRepository<SFOTblMasterMachineServiceType>
    {

    }

    #endregion

    public class SFOMasterMachineServiceTypeRepository : Repository<OceanDbEntities, SFOTblMasterMachineServiceType>, ISFOMasterMachineServiceTypeRepository
    {
        public SFOMasterMachineServiceTypeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
