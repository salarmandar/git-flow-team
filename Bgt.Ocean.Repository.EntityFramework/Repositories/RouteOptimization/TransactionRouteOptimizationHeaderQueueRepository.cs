using System;
using System.Linq;
using Bgt.Ocean.Models;
using Bgt.Ocean.Infrastructure.Configuration;
using System.Collections.Generic;
namespace Bgt.Ocean.Repository.EntityFramework.Repositories.RouteOptimization
{
    public interface ITransactionRouteOptimizationHeaderQueueRepository : IRepository<TblTransactionRouteOptimizationHeader_Queue> {
        IEnumerable<TblTransactionRouteOptimizationHeader_Queue> GetTransactionRouteOptimizationByRequestID(IEnumerable<Guid> guids);
    }
    public class TransactionRouteOptimizationHeaderQueueRepository : Repository<OceanDbEntities, TblTransactionRouteOptimizationHeader_Queue>, ITransactionRouteOptimizationHeaderQueueRepository
    {
        public TransactionRouteOptimizationHeaderQueueRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblTransactionRouteOptimizationHeader_Queue> GetTransactionRouteOptimizationByRequestID(IEnumerable<Guid> guids)
        {
            return DbContext.TblTransactionRouteOptimizationHeader_Queue.Where(w => guids.Any(a => a == w.TransactionRouteOptimizationHeader_Guid));

        }
    }
}
