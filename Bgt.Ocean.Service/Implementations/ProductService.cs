using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.PricingRules;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Products;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.ProductService;
using Bgt.Ocean.Service.ModelViews.PricingRules;
using Bgt.Ocean.Service.ModelViews.Products;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations
{
    public interface IProductService
    {
        SaveProductResponse SaveProductToContract(SaveProductRequest request);
        string GetProductID(Guid product_Guid);
        IEnumerable<Guid> GetProductParentList(Guid childProduct_Guid);
        IEnumerable<ProductView> GetProductList(Guid brinksCompanyGuid);
        IEnumerable<ProductView> GetProductInContract(Guid contractGuid);
        IEnumerable<PricingRuleView> GetPricingRuleInContractProduct(Guid contractGuid, Guid productGuid);
        IEnumerable<Guid> GetExistsLOBInProduct(Guid company_Guid);
        IEnumerable<ProductTreeView> GetProductLeafByLOB(Guid company_Guid, Guid systemLineOfBusiness_Guid);
    }

    public class ProductService : IProductService
    {
        private readonly ILeedToCashProductRepository _leedToCashProductRepository;
        private readonly IMasterCustomerContractRepository _masterCustomerContractRepository;
        private readonly ISystemLeedToCashStatusRepository _systemLeedToCashStatusRepository;
        private readonly IMasterCountryRepository _masterCountryRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        public ProductService(
            ILeedToCashProductRepository leedToCashProductRepository,
            IMasterCustomerContractRepository masterCustomerContractRepository,
            ISystemLeedToCashStatusRepository systemLeedToCashStatusRepository,
            IMasterCountryRepository masterCountryRepository,
            IUnitOfWork<OceanDbEntities> uow)
        {
            _leedToCashProductRepository = leedToCashProductRepository;
            _masterCustomerContractRepository = masterCustomerContractRepository;
            _systemLeedToCashStatusRepository = systemLeedToCashStatusRepository;
            _masterCountryRepository = masterCountryRepository;
            _uow = uow;
        }

        public IEnumerable<Guid> GetExistsLOBInProduct(Guid company_Guid)
        {
            return _leedToCashProductRepository.FindAll(
                e => e.MasterCustomer_Guid == company_Guid
                && e.TblSystemLeedToCashStatus.StatusID == L2CHelper.Status.Published
                && !e.IsCopyData
                && !e.IsBidProduct
                && !e.FlagDisable
                && !e.FlagRemove
                && e.FlagProductLeaf
                && e.SystemLineOfBusiness_Guid.HasValue).Select(e => e.SystemLineOfBusiness_Guid.Value).Distinct();
        }

        public IEnumerable<ProductTreeView> GetProductLeafByLOB(Guid company_Guid, Guid systemLineOfBusiness_Guid)
        {
            var products = _leedToCashProductRepository.FindAll(
                e => e.MasterCustomer_Guid == company_Guid
                && e.TblSystemLeedToCashStatus.StatusID == L2CHelper.Status.Published
                && !e.IsCopyData
                && !e.IsBidProduct
                && !e.FlagDisable
                && !e.FlagRemove
                && e.FlagProductLeaf
                && e.SystemLineOfBusiness_Guid == systemLineOfBusiness_Guid);
            return products.ConvertToProductView();
        }

        /// <summary>
        /// Get product in company
        /// </summary>
        /// <param name="brinksCompanyGuid"></param>
        /// <returns></returns>
        public IEnumerable<ProductView> GetProductList(Guid brinksCompanyGuid)
        {
            var products = _leedToCashProductRepository.FindAll(
                 e => e.MasterCustomer_Guid == brinksCompanyGuid
                 && e.TblSystemLeedToCashStatus.StatusID == L2CHelper.Status.Published
                 && !string.IsNullOrEmpty(e.ProductName)
                 && !e.FlagRemove
                 && !e.FlagDisable
                 && !e.IsCopyData
                 && e.FlagProductLeaf);

            List<ProductView> productList = new List<ProductView>();
            foreach (var item in products)
            {
                ProductView productApiView = new ProductView();
                productApiView.Guid = item.Guid;
                productApiView.ProductID = item.ProductID;
                productApiView.ProductDisplayName = $"{item.ProductID} - {item.ProductName}";
                productApiView.LOBDisplayName = item.LOBFullName;
                productApiView.SystemLineOfBusiness_Guid = item.SystemLineOfBusiness_Guid;
                productList.Add(productApiView);
            }
            return productList;
        }

        /// <summary>
        /// Get product in contract
        /// </summary>
        /// <param name="contractGuid"></param>
        /// <returns></returns>
        public IEnumerable<ProductView> GetProductInContract(Guid contractGuid)
        {
            var contractDetail = _masterCustomerContractRepository.FindById(contractGuid);
            var productInContract = contractDetail.TblLeedToCashContract_PricingRules.Distinct((x, y) => x.ProductId == y.ProductId).Select(e => new ProductView()
            {
                ProductID = e.ProductId,
                Guid = e.TblPricingRule.Product_Guid,
                LOBDisplayName = e.LOBName,
                SystemLineOfBusiness_Guid = e.TblPricingRule.TblLeedToCashProduct.SystemLineOfBusiness_Guid,
                ProductDisplayName = $"{e.ProductId} - {e.ProductName}"
            });
            return productInContract;
        }

        /// <summary>
        /// Get pricing rule in product of contract
        /// </summary>
        /// <param name="contractGuid"></param>
        /// <param name="productGuid"></param>
        /// <returns></returns>
        public IEnumerable<PricingRuleView> GetPricingRuleInContractProduct(Guid contractGuid, Guid productGuid)
        {
            var allPricingRuleInProduct = _masterCustomerContractRepository.FindPricingRuleInProduct(contractGuid, productGuid);
            List<PricingRuleView> pricingRuleList = new List<PricingRuleView>();
            foreach (var pricingRule in allPricingRuleInProduct)
            {
                PricingRuleView pricingView = new PricingRuleView();
                pricingView.Guid = pricingRule.Guid;
                pricingView.ProductID = pricingRule.ProductId;
                pricingView.Name = pricingRule.TblPricingRule.Name;
                pricingView.MasterCurrencyAbbreviation = pricingRule.TblPricingRule.TblMasterCurrency.MasterCurrencyAbbreviation;
                pricingView.MasterCurrency_Guid = pricingRule.TblPricingRule.MasterCurrency_Guid;
                pricingView.Product_Guid = pricingRule.TblPricingRule.Product_Guid;
                pricingView.ProductName = pricingRule.ProductName;
                pricingView.SystemLineOfBusiness_Guid = pricingRule.TblPricingRule.TblLeedToCashProduct.SystemLineOfBusiness_Guid;
                pricingRuleList.Add(pricingView);
            }
            return pricingRuleList;
        }

        public IEnumerable<Guid> GetProductParentList(Guid childProduct_Guid)
        {
            var products = new List<Guid>();
            var parent_Guid = _leedToCashProductRepository.FindById(childProduct_Guid)?.Product_Guid;
            var rootParent = _leedToCashProductRepository.FindById(parent_Guid);
            if (rootParent != null && rootParent.Product_Guid != rootParent.Guid)
            {
                var otherRootParent = this.GetProductParentList(rootParent.Guid).ToList();
                products.AddRange(otherRootParent);
                products.Add(rootParent.Guid);
            }
            return products;
        }

        public string GetProductID(Guid product_Guid)
        {
            return _leedToCashProductRepository.FindById(product_Guid)?.ProductID;
        }

        public SaveProductResponse SaveProductToContract(SaveProductRequest request)
        {
            SaveProductResponse response = new SaveProductResponse();
            TblLeedToCashProduct product;
            if (request.Guid.HasValue)
            {
                product = _leedToCashProductRepository.FindById(request.Guid);
                product.SystemLineOfBusiness_Guid = request.SystemLineOfBusiness_Guid;
                product.ProductName = request.ProductName;
                product.MasterCountry_Guid = request.Country_Guid;
                product.Description = request.Description;
                product.ProductLevel = 1;
                product.IsBidProduct = false;
                product.IsCopyData = true;
                product.FlagProductLeaf = true;
                product.MasterCustomer_Guid = request.Company_Guid;
                product.DatetimeModified = request.LocalClientDateTime;
                product.UniversalDatetimeModified = request.ClientDateTime;
                product.UserModifed = request.UserName;
                _leedToCashProductRepository.Modify(product);
            }
            else
            {
                var publishedStatus = _systemLeedToCashStatusRepository.FindAll(e => e.StatusID == L2CHelper.Status.Published).FirstOrDefault();
                product = new TblLeedToCashProduct();
                product.Guid = Guid.NewGuid();
                product.SystemLineOfBusiness_Guid = request.SystemLineOfBusiness_Guid;
                product.ProductID = string.IsNullOrWhiteSpace(request.ProductID) ? GenerateProductID(request.Country_Guid) : request.ProductID;
                product.ProductName = request.ProductName;
                product.MasterCountry_Guid = request.Country_Guid;
                product.Description = request.Description;
                product.ProductLevel = 1;
                product.IsBidProduct = false;
                product.IsCopyData = true;
                product.FlagProductLeaf = true;
                product.TblSystemLeedToCashStatus = publishedStatus;
                product.MasterCustomer_Guid = request.Company_Guid;
                product.DatetimeCreated = request.LocalClientDateTime;
                product.UniversalDatetimeCreated = request.ClientDateTime;
                product.UserCreated = request.UserName;
                product.DatetimeModified = request.LocalClientDateTime;
                product.UniversalDatetimeModified = request.ClientDateTime;
                product.UserModifed = request.UserName;
                _leedToCashProductRepository.Create(product);
            }
            _uow.Commit();

            ProductView productApiView = new ProductView();
            productApiView.Guid = product.Guid;
            productApiView.ProductID = product.ProductID;
            productApiView.ProductDisplayName = $"{product.ProductID} - {product.ProductName}";
            productApiView.SystemLineOfBusiness_Guid = product.SystemLineOfBusiness_Guid;
            productApiView.Description = product.Description;
            productApiView.ServiceType_Guids = request.ServiceType_Guids;
            response.Product = productApiView;
            return response;
        }

        private string GenerateProductID(Guid countryGuid)
        {
            int runningNumber = 0;
            var countryCode = _masterCountryRepository.FindById(countryGuid)?.MasterCountryAbbreviation;

            // get from product
            var product = _leedToCashProductRepository.FindAll(e => e.MasterCountry_Guid == countryGuid && e.ProductID != null).Where(e => e.ProductID.StartsWith($"PI{countryCode}")).ToList();
            var productIds = product.Select(e => e.ProductID);

            var maxRunning = !productIds.IsEmpty() ? productIds.Max(e => e.GetNumber()) : 0;
            runningNumber = maxRunning + 1;

            string productID = $"PI{countryCode}{runningNumber.ToString("d3")}";
            return productID;
        }
    }
}
