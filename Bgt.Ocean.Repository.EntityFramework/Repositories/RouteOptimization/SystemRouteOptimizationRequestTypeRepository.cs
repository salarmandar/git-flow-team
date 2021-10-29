using System;
using System.Linq;
using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.RouteOptimization
{
    public interface ISystemRouteOptimizationRequestTypeRepository : IRepository<TblSystemRouteOptimizationRequestType> { }
    public class SystemRouteOptimizationRequestTypeRepository : Repository<OceanDbEntities, TblSystemRouteOptimizationRequestType>, ISystemRouteOptimizationRequestTypeRepository
    {
        public SystemRouteOptimizationRequestTypeRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
