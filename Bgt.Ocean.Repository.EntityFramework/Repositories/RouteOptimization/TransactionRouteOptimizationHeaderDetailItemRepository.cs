using System;
using System.Linq;
using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.RouteOptimization
{
    public interface ITransactionRouteOptimizationHeaderDetailItemRepository : IRepository<TblTransactionRouteOptimizationHeader_Detail_Item> { }
    public class TransactionRouteOptimizationHeaderDetailItemRepository : Repository<OceanDbEntities, TblTransactionRouteOptimizationHeader_Detail_Item>, ITransactionRouteOptimizationHeaderDetailItemRepository
    {
        public TransactionRouteOptimizationHeaderDetailItemRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
