using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.PricingRules
{
    public interface IQuotation_ProductRepository : IRepository<TblLeedToCashQuotation_Product>
    {
    }

    public class Quotation_ProductRepository : Repository<OceanDbEntities, TblLeedToCashQuotation_Product>, IQuotation_ProductRepository
    {
        public Quotation_ProductRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
