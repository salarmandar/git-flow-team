using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.NemoDynamicRoute
{
    /// <summary>
    /// Interface with definitions of actions related with table TblMasterQueueRouteOptimization.
    /// </summary>
    public interface IMasterQueueRouteOptimizationRepository : IRepository<TblMasterNemoQueueRouteOptimization>
    {
        TblMasterNemoQueueRouteOptimization GetNemoQueueRouteOptimizationByTaskGuid(Guid routeOptimizeTaskGuid);
        IEnumerable<TblMasterNemoQueueRouteOptimization> GetListNemoQueueRouteOptimizationByTaskGuid(Guid routeOptimizeTaskGuid);
        IEnumerable<RoutesDataResult> GetMultipleRoutePlaned(string dailyRunResourceGuids, bool getDataFromMasterRoute);
    }
    public class MasterQueueRouteOptimizationRepository : Repository<OceanDbEntities, TblMasterNemoQueueRouteOptimization>, IMasterQueueRouteOptimizationRepository
    {
        #region Contructor

        /// <summary>
        /// Inititalizes a new instance of the <see cref="MasterQueueRouteOptimizationRepository"/> class.
        /// </summary>
        /// <param name="dbFactory">factory with dataContext.</param>
        public MasterQueueRouteOptimizationRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
        #endregion

        #region Methods

        #region Public Methods
        public TblMasterNemoQueueRouteOptimization GetNemoQueueRouteOptimizationByTaskGuid(Guid routeOptimizeTaskGuid)
        {
            var nemoQueueRouteOptimization = DbContext.TblMasterNemoQueueRouteOptimization.FirstOrDefault(e => e.RouteOptimizeTaskGuid == routeOptimizeTaskGuid);

            return nemoQueueRouteOptimization;
        }

        public IEnumerable<TblMasterNemoQueueRouteOptimization> GetListNemoQueueRouteOptimizationByTaskGuid(Guid routeOptimizeTaskGuid)
        {
            var nemoQueueRouteOptimization = DbContext.TblMasterNemoQueueRouteOptimization.Where(e => e.RouteOptimizeTaskGuid == routeOptimizeTaskGuid).OrderByDescending(z => z.OptimizationOrder);
            return nemoQueueRouteOptimization;
        }

        public IEnumerable<RoutesDataResult> GetMultipleRoutePlaned(string dailyRunResourceGuids, bool getDataFromMasterRoute)
        {
            //parameter is null because ef receive two parameter
            var dataSP = this.DbContext.Up_OceanOnlineMVC_RouteOptimization_GetRoutesData(dailyRunResourceGuids, getDataFromMasterRoute);
            var result = dataSP?.ToList();
            return result != null ? result : new List<RoutesDataResult>();
        }
        #endregion

        #region Public Private
        #endregion

        #endregion
    }
}
