using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Messagings.ProductService;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v2
{
    public class v2_ContractController : ApiControllerBase
    {
        private readonly IProductService _productService;
        public v2_ContractController(
            IProductService productService)
        { 
            _productService = productService;
        }

        /// <summary>
        /// [Coral] Use for Standard Contract 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SaveProductResponse SaveProduct(SaveProductRequest request)
        {
            return _productService.SaveProductToContract(request);
        }
    }
}