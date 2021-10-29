using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Nemo
{
    #region Interface
    public interface IMasterNemoQueueRouteOptimizationDetailRepository : IRepository<TblMasterNemoQueueRouteOptimization_Detail>
    {

    }
    #endregion

    public class MasterNemoQueueRouteOptimizationDetailRepository : Repository<OceanDbEntities, TblMasterNemoQueueRouteOptimization_Detail>, IMasterNemoQueueRouteOptimizationDetailRepository
    {
        public MasterNemoQueueRouteOptimizationDetailRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
