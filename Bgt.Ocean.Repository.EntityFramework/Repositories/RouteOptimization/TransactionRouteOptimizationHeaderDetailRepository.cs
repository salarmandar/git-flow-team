using System;
using System.Linq;
using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
namespace Bgt.Ocean.Repository.EntityFramework.Repositories.RouteOptimization
{
    public interface ITransactionRouteOptimizationHeaderDetailRepository : IRepository<TblTransactionRouteOptimizationHeader_Detail> { }
    public class TransactionRouteOptimizationHeaderDetailRepository : Repository<OceanDbEntities, TblTransactionRouteOptimizationHeader_Detail>, ITransactionRouteOptimizationHeaderDetailRepository
    {
        public TransactionRouteOptimizationHeaderDetailRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
