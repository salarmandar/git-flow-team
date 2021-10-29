using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.NemoDynamicRoute
{
    /// <summary>
    /// Interface with definitions of actions related with table TblMasterDetailRouteOptimization.
    /// </summary>
    public interface IMasterDetailRouteOptimizationRepository : IRepository<TblMasterNemoDetailRouteOptimization>
    {
    
    }

    public class MasterDetailRouteOptimizationRepository : Repository<OceanDbEntities, TblMasterNemoDetailRouteOptimization>, IMasterDetailRouteOptimizationRepository
    {
        #region Contructor

        /// <summary>
        /// Inititalizes a new instance of the <see cref="MasterDetailRouteOptimizationRepository"/> class.
        /// </summary>
        /// <param name="dbFactory">factory with dataContext.</param>
        public MasterDetailRouteOptimizationRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
        #endregion

        #region Methods

        #region Public Methods

        #endregion

        #region Public Private
        #endregion

        #endregion
    }
}
