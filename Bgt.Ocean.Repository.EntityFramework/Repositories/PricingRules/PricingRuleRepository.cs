using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.PricingRules
{
    public interface IPricingRuleRepository : IRepository<TblPricingRule>
    {
        IEnumerable<TblPricingRule> FindByIds(IEnumerable<Guid> guids);
        void SaveCustomer(IEnumerable<TblLeedToCashQuotation_Customer_Mapping> customerList);
    }

    public class PricingRuleRepository : Repository<OceanDbEntities, TblPricingRule>, IPricingRuleRepository
    {
        public PricingRuleRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblPricingRule> FindByIds(IEnumerable<Guid> guids)
        {
            return DbContext.TblPricingRule.Where(e => guids.Contains(e.Guid));
        }

        public void SaveCustomer(IEnumerable<TblLeedToCashQuotation_Customer_Mapping> customerList)
        {
            DbContext.TblLeedToCashQuotation_Customer_Mapping.AddRange(customerList);
        }
    }
}
