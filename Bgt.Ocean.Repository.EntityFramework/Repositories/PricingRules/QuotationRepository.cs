using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.PricingRules
{
    public interface IQuotationRepository : IRepository<TblLeedToCashQuotation>
    {
        void MappingPricing(TblLeedToCashQuotation_PricingRule_Mapping q_pricing);
        void MappingClause(TblLeedToCashQuotation_Clause q_clause);
        void MappingAttribute(TblLeedToCashQuotation_ProductAttribute q_attribute);

        void RemovePricingMappings(IEnumerable<TblLeedToCashQuotation_PricingRule_Mapping> q_pricing_list);          
        void RemoveClauseMappings(IEnumerable<TblLeedToCashQuotation_Clause> q_clause_list);
        void RemoveAttributeMappings(IEnumerable<TblLeedToCashQuotation_ProductAttribute> q_attribute_list);
        void RemoveAccountMappings(IEnumerable<TblLeedToCashQuotation_Customer_Mapping> q_account_list);
        void RemoveLocationMappings(IEnumerable<TblLeedToCashQuotation_Location_Mapping> q_location_list);
    }

    public class QuotationRepository : Repository<OceanDbEntities, TblLeedToCashQuotation>, IQuotationRepository
    {
        public QuotationRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public void MappingAttribute(TblLeedToCashQuotation_ProductAttribute q_attribute)
        {
            DbContext.TblLeedToCashQuotation_ProductAttribute.Add(q_attribute);
        }

        public void MappingClause(TblLeedToCashQuotation_Clause q_clause)
        {
            DbContext.TblLeedToCashQuotation_Clause.Add(q_clause);
        }

        public void MappingPricing(TblLeedToCashQuotation_PricingRule_Mapping q_pricing)
        {
            DbContext.TblLeedToCashQuotation_PricingRule_Mapping.Add(q_pricing);
        }

        public void RemoveAccountMappings(IEnumerable<TblLeedToCashQuotation_Customer_Mapping> q_account_list)
        {
            DbContext.TblLeedToCashQuotation_Customer_Mapping.RemoveRange(q_account_list);
        }

        public void RemoveAttributeMappings(IEnumerable<TblLeedToCashQuotation_ProductAttribute> q_attribute_list)
        {
            DbContext.TblLeedToCashQuotation_ProductAttribute.RemoveRange(q_attribute_list);
        }

        public void RemoveClauseMappings(IEnumerable<TblLeedToCashQuotation_Clause> q_clause_list)
        {
            DbContext.TblLeedToCashQuotation_Clause.RemoveRange(q_clause_list);
        }

        public void RemoveLocationMappings(IEnumerable<TblLeedToCashQuotation_Location_Mapping> q_location_list)
        {
            DbContext.TblLeedToCashQuotation_Location_Mapping.RemoveRange(q_location_list);
        }

        public void RemovePricingMappings(IEnumerable<TblLeedToCashQuotation_PricingRule_Mapping> q_pricing_list)
        {
            var current = q_pricing_list.Select(e => e.TblPricingRule).ToList();
            var source = q_pricing_list.Select(e => e.TblPricingRuleSource).ToList();
            DbContext.TblPricingRule.RemoveRange(current);
            DbContext.TblPricingRule.RemoveRange(source);
            DbContext.TblLeedToCashQuotation_PricingRule_Mapping.RemoveRange(q_pricing_list);
        }
    }
}
