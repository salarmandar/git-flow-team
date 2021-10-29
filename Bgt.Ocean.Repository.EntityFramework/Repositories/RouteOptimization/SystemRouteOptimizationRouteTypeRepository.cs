using System;
using System.Linq;
using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.RouteOptimization
{
    public interface ISystemRouteOptimizationRouteTypeRepository : IRepository<TblSystemRouteOptimizationRouteType> {
        TblSystemRouteOptimizationRouteType GetRouteOptimizationRouteTypeByCode(string code);
    }
    public class SystemRouteOptimizationRouteTypeRepository : Repository<OceanDbEntities, TblSystemRouteOptimizationRouteType>, ISystemRouteOptimizationRouteTypeRepository
    {
        public SystemRouteOptimizationRouteTypeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblSystemRouteOptimizationRouteType GetRouteOptimizationRouteTypeByCode(string code) {
            return DbContext.TblSystemRouteOptimizationRouteType.First(w => w.RouteOptimizationRouteTypeCode == code);
        }
    }
}
