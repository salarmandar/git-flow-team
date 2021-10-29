using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.PricingRules
{
    public interface IQuotation_HistoryRepository : IRepository<TblLeedToCashQuotation_History>
    {
    }

    public class Quotation_HistoryRepository : Repository<OceanDbEntities, TblLeedToCashQuotation_History>, IQuotation_HistoryRepository
    {
        public Quotation_HistoryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
