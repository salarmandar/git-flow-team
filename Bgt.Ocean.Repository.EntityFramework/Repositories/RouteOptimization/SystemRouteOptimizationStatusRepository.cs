using System;
using System.Linq;
using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.RouteOptimization
{
    public interface ISystemRouteOptimizationStatusRepository : IRepository<TblSystemRouteOptimizationStatus> {
        TblSystemRouteOptimizationStatus GetRouteOptimizationStatusByID(int optimizeId);
    }
    public class SystemRouteOptimizationStatusRepository : Repository<OceanDbEntities, TblSystemRouteOptimizationStatus>, ISystemRouteOptimizationStatusRepository
    {
        public SystemRouteOptimizationStatusRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
        public TblSystemRouteOptimizationStatus GetRouteOptimizationStatusByID(int optimizeId) {
            return DbContext.TblSystemRouteOptimizationStatus.First(f=>f.RouteOptimizationStatusID == optimizeId);
        }
    }
}
