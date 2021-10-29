using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Products
{
    public interface ILeedToCashProductRepository : IRepository<TblLeedToCashProduct>
    {
        void RemoveAllServiceType(IEnumerable<TblLeedToCashProduct_ServiceType> serviceTypes);
        IEnumerable<TblLeedToCashProduct_Clause> FindClauseInProduct(Guid product_guid);
        IEnumerable<TblLeedToCashProduct_Clause> FindClauseInherit(IEnumerable<Guid> product_guids);
        IEnumerable<TblLeedToCashProduct> FindParent(Guid parent_guid);
        void MappingClause(TblLeedToCashProduct_Clause mapping);
        void SaveAttribute(IEnumerable<TblLeedToCashProduct_ProductAttribute> attr_list);
        void RemoveAttribute(IEnumerable<TblLeedToCashProduct_ProductAttribute> attr_list);
    }

    public class LeedToCashProductRepository : Repository<OceanDbEntities, TblLeedToCashProduct>, ILeedToCashProductRepository
    {
        public LeedToCashProductRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<TblLeedToCashProduct_Clause> FindClauseInherit(IEnumerable<Guid> product_guids)
        {
            List<TblLeedToCashProduct_Clause> allClause = new List<TblLeedToCashProduct_Clause>();
            foreach (var guid in product_guids)
            {
                var product = DbContext.TblLeedToCashProduct.FirstOrDefault(e => e.Guid == guid);
                if (product.TblSystemLeedToCashStatus.StatusID == L2CHelper.Status.Published)
                {
                    var product_clauses = DbContext.TblLeedToCashProduct_Clause.Where(
                        e => e.LeedToCashProduct_Guid == guid
                        && e.TblLeedToCashMasterClause.FlagDisable == false
                        && e.TblLeedToCashMasterClause.TblSystemLeedToCashStatus.StatusID == L2CHelper.Status.Published);

                    allClause.AddRange(product_clauses);
                }
            }

            return allClause.Distinct((x, y) => x.LeedToCashMasterClause_Guid == y.LeedToCashMasterClause_Guid);
        }

        public IEnumerable<TblLeedToCashProduct_Clause> FindClauseInProduct(Guid product_guid)
        {
            var product_clauses = DbContext.TblLeedToCashProduct_Clause.Where(
               e => e.LeedToCashProduct_Guid == product_guid
               && !e.TblLeedToCashMasterClause.FlagDisable
               && e.TblLeedToCashMasterClause.TblSystemLeedToCashStatus.StatusID == L2CHelper.Status.Published);
            return product_clauses;
        }

        public IEnumerable<TblLeedToCashProduct> FindParent(Guid parent_guid)
        {
            var products = new List<TblLeedToCashProduct>();
            var root_parent = DbContext.TblLeedToCashProduct.FirstOrDefault(e => e.Guid == parent_guid);

            if (root_parent != null && root_parent.Product_Guid.HasValue && root_parent.Product_Guid != root_parent.Guid)
            {
                var other_root_parent = this.FindParent(root_parent.Product_Guid.Value).ToList();
                products.AddRange(other_root_parent);
            }

            products.Add(root_parent);
            return products;
        }

        public void MappingClause(TblLeedToCashProduct_Clause mapping)
        {
            DbContext.TblLeedToCashProduct_Clause.Add(mapping);
        }

        public void RemoveAllServiceType(IEnumerable<TblLeedToCashProduct_ServiceType> serviceTypes)
        {
            //foreach (var item in serviceTypes)
            //{
            //    DbContext.Entry(item).State = EntityState.Deleted;
            //}        
            //DbContext.TblLeedToCashProduct_ServiceType.RemoveRange(serviceTypes);
        }

        public void RemoveAttribute(IEnumerable<TblLeedToCashProduct_ProductAttribute> attr_list)
        {
            var product_clause = attr_list.SelectMany(e => e.TblLeedToCashProduct.TblLeedToCashProduct_Clause).Distinct();
            DbContext.TblLeedToCashProduct_Clause.RemoveRange(product_clause);

            // remove all relate in attribute
            foreach (var item in attr_list)
            {
                DbContext.TblLeedToCashProduct_ProductAttribute_Certification.RemoveRange(item.TblLeedToCashProduct_ProductAttribute_Certification);
                DbContext.TblLeedToCashProduct_ProductAttribute_Discrepancy.RemoveRange(item.TblLeedToCashProduct_ProductAttribute_Discrepancy);
                DbContext.TblLeedToCashProduct_ProductAttribute_GeoLocation_City.RemoveRange(item.TblLeedToCashProduct_ProductAttribute_GeoLocation_City);
                DbContext.TblLeedToCashProduct_ProductAttribute_Info.RemoveRange(item.TblLeedToCashProduct_ProductAttribute_Info);
                DbContext.TblLeedToCashProduct_ProductAttribute_Limit_Items.RemoveRange(item.TblLeedToCashProduct_ProductAttribute_Limit_Items);
                DbContext.TblLeedToCashProduct_ProductAttribute_Limit_Values.RemoveRange(item.TblLeedToCashProduct_ProductAttribute_Limit_Values);
                DbContext.TblLeedToCashProduct_ProductAttribute_Limit_Weight.RemoveRange(item.TblLeedToCashProduct_ProductAttribute_Limit_Weight);
                DbContext.TblLeedToCashProduct_ProductAttribute_ResponseTime.RemoveRange(item.TblLeedToCashProduct_ProductAttribute_ResponseTime);
                DbContext.TblLeedToCashProduct_ProductAttribute_ServiceWindows.RemoveRange(item.TblLeedToCashProduct_ProductAttribute_ServiceWindows);
                DbContext.TblLeedToCashProduct_ProductAttribute_TimeDeadLine.RemoveRange(item.TblLeedToCashProduct_ProductAttribute_TimeDeadLine);
                DbContext.TblLeedToCashProduct_ProductAttribute_TimeSendReport_Detail.RemoveRange(item.TblLeedToCashProduct_ProductAttribute_TimeSendReport.SelectMany(e=>e.TblLeedToCashProduct_ProductAttribute_TimeSendReport_Detail));
                DbContext.TblLeedToCashProduct_ProductAttribute_TimeSendReport.RemoveRange(item.TblLeedToCashProduct_ProductAttribute_TimeSendReport);
            }
            DbContext.TblLeedToCashProduct_ProductAttribute.RemoveRange(attr_list);
        }

        public void SaveAttribute(IEnumerable<TblLeedToCashProduct_ProductAttribute> attr_list)
        {
            DbContext.TblLeedToCashProduct_ProductAttribute.AddRange(attr_list);
        }
    }
}
