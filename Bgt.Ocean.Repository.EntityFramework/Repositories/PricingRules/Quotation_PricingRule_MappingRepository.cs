using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.PricingRules
{
    public interface IQuotation_PricingRule_MappingRepository : IRepository<TblLeedToCashQuotation_PricingRule_Mapping>
    {
    }

    public class Quotation_PricingRule_MappingRepository : Repository<OceanDbEntities, TblLeedToCashQuotation_PricingRule_Mapping>, IQuotation_PricingRule_MappingRepository
    {
        public Quotation_PricingRule_MappingRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }
    }
}
