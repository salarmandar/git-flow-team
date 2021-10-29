using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.ModelViews.PricingRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v2
{
    public class v2_ProductController : ApiControllerBase
    {
        private readonly IProductService _productService;
        public v2_ProductController(
            IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IEnumerable<Guid> GetExistsLOBInProduct(Guid company_Guid)
        {
            return _productService.GetExistsLOBInProduct(company_Guid);
        }

        [HttpGet]
        public IEnumerable<ProductTreeView> GetProductLeafByLOB(Guid company_Guid, Guid systemLineOfBusiness_Guid)
        {
            return _productService.GetProductLeafByLOB(company_Guid, systemLineOfBusiness_Guid).OrderBy(e => e.ProductNameDisplay);
        }
    }
}