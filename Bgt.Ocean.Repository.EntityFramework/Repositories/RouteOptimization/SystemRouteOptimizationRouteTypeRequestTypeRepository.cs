using System;
using System.Linq;
using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.RouteOptimization
{
    public interface ISystemRouteOptimizationRouteTypeRequestTypeRepository : IRepository<TblSystemRouteOptimizationRouteType_RequestType> { }
    public class SystemRouteOptimizationRouteTypeRequestTypeRepository : Repository<OceanDbEntities, TblSystemRouteOptimizationRouteType_RequestType>, ISystemRouteOptimizationRouteTypeRequestTypeRepository
    {
        public SystemRouteOptimizationRouteTypeRequestTypeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
