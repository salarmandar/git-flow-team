using Bgt.Ocean.Service.ModelViews.PricingRules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Helpers
{
    public static class TreeViewHelper
    {
        public static IEnumerable<ProductTreeView> FilterByContractPricingRule(this IEnumerable<ProductTreeView> product, List<Guid> listPricingRuleGuid)
        {
            List<ProductTreeView> listProduct = product.ToList();
            List<ProductTreeView> listProductResult = product.ToList();
            foreach (var itemProduct in listProduct)
            {
                if (itemProduct.ChildProductList != null)
                {
                    itemProduct.ChildProductList = FilterByContractPricingRule(itemProduct.ChildProductList, listPricingRuleGuid);
                    if (!itemProduct.ChildProductList.Any())
                    {
                        var removeProduct = listProductResult.Find(p => p.Guid == itemProduct.Guid);
                        listProductResult.Remove(removeProduct);
                    }
                }
                else
                {
                    if (listPricingRuleGuid.Contains(itemProduct.Guid))
                    {
                        var removePricingRule = listProductResult.Find(p => p.Guid == itemProduct.Guid);
                        listProductResult.Remove(removePricingRule);
                    }
                }
            }

            listProductResult.ForEach(p => p.enable = true);
            return listProductResult;
        }

        public static IEnumerable<ProductTreeView> FilterProductTreeView(this IEnumerable<ProductTreeView> product, bool flagDisable)
        {
            var result = product.Where(e => e.FlagDisable == flagDisable);
            foreach (var item in result)
            {
                if (item.ChildProductList != null)
                    item.ChildProductList = FilterProductTreeView(item.ChildProductList, flagDisable);
            }

            return result;
        }

        public static List<ProductTreeView> ManageRootProductPricingRuleDictionary(this List<ProductTreeView> productMapped, Dictionary<Guid, bool> productSelectedGuids, List<Guid> productGuidsForDisable = null)
        {
            List<ProductTreeView> subProductList = new List<ProductTreeView>();
            var root = productMapped.Where(e => e.ProductLevel == 0).OrderBy(e => e.ProductName).ToList();
            foreach (var item in root)
            {
                var product = new ProductTreeView();
                product.Guid = item.Guid;
                product.IsProductLOB = true;
                product.SystemLineOfBusiness_Guid = item.SystemLineOfBusiness_Guid;
                product.ProductName = item.ProductName;
                product.PricingRuleList = item.PricingRuleList;
                product.@checked = productSelectedGuids.Any(e => e.Key == item.Guid);
                product.enable = CheckForEnable(null, item.Guid, productSelectedGuids, productGuidsForDisable);
                var subResult = productMapped.ManageChildProductPricingRuleDictionary(null, item.SystemLineOfBusiness_Guid, productSelectedGuids, productGuidsForDisable);
                if (subResult.Any())
                    product.ChildProductList = subResult.OrderBy(e => e.FlagProductLeaf).ThenBy(e => e.ProductName);
                subProductList.Add(product);
            }
            return subProductList.OrderBy(e => e.ProductName).ToList();
        }

        private static List<ProductTreeView> ManageChildProductPricingRuleDictionary(this List<ProductTreeView> productMapped, Guid? parentGuid, Guid LOB_Guid, Dictionary<Guid, bool> productSelectedGuids, List<Guid> productGuidsForDisable = null)
        {
            List<ProductTreeView> subProductList = new List<ProductTreeView>();
            List<ProductTreeView> childProducts = new List<ProductTreeView>();

            if (parentGuid.HasValue)
                childProducts = productMapped.Where(e => e.ParentProduct_Guid == parentGuid && e.Guid != parentGuid && e.SystemLineOfBusiness_Guid == LOB_Guid).ToList();
            else
                childProducts = productMapped.Where(e => e.ProductLevel == 1 && e.SystemLineOfBusiness_Guid == LOB_Guid).ToList();
            if (childProducts.Any())
            {
                foreach (var item in childProducts.OrderBy(e => e.ProductIndex))
                {
                    var subResult = productMapped.ManageChildProductPricingRuleDictionary(item.Guid, LOB_Guid, productSelectedGuids, productGuidsForDisable);
                    if (subResult.Any())
                        item.ChildProductList = subResult.OrderBy(e => e.FlagProductLeaf).ThenBy(e => e.ProductName);

                    if (item.PricingRuleList != null && item.PricingRuleList.Any())
                    {
                        item.ChildProductList = item.PricingRuleList.Select(e => new ProductTreeView()
                        {
                            Guid = e.Guid,
                            ProductName = e.DisplayName,
                            FlagPricingRule = true,
                            @checked = productSelectedGuids.Any(a => a.Key == e.Guid),
                            enable = CheckForEnable(e.Guid, null, productSelectedGuids, productGuidsForDisable)
                        });
                    }

                    item.enable = CheckForEnable(null, item.Guid, productSelectedGuids, productGuidsForDisable);
                    item.@checked = productGuidsForDisable.Any(e => e == item.Guid);
                    subProductList.Add(item);
                }
            }

            return subProductList;
        }

        private static bool CheckForEnable(Guid? pricingGuid, Guid? productGuid, Dictionary<Guid, bool> productSelectedGuids, List<Guid> productGuidsForDisable)
        {
            bool a = productSelectedGuids.Any(e => e.Key == pricingGuid) ? productSelectedGuids.FirstOrDefault(e => e.Key == pricingGuid).Value : true;
            bool b = productGuidsForDisable.Any(e => e == productGuid) ? false : true;
            if (!productGuid.HasValue)
                return a;

            return a && b;
        }
    }
}
