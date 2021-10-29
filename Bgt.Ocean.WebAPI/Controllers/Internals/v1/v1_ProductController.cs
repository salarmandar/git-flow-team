using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.ModelViews.PricingRules;
using Bgt.Ocean.Service.ModelViews.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v1
{
    public class v1_ProductController : ApiControllerBase
    {
        private readonly IProductService _productService;
        public v1_ProductController(
            IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IEnumerable<ProductView> GetProductInCompany(Guid brinksCompanyGuid)
        {
            return _productService.GetProductList(brinksCompanyGuid).OrderBy(e => e.ProductDisplayName);
        }

        [HttpGet]
        public IEnumerable<ProductView> GetProductInContract(Guid contractGuid)
        {
            return _productService.GetProductInContract(contractGuid).OrderBy(e => e.ProductDisplayName);
        }

        [HttpGet]
        public IEnumerable<PricingRuleView> GetPricingRuleInProduct(Guid contractGuid, Guid productGuid)
        {
            var pricingList = _productService.GetPricingRuleInContractProduct(contractGuid, productGuid).OrderBy(e => e.Name);
            return pricingList;
        }
    }
}
