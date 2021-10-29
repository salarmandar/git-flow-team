using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.PricingRules;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Products;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Helpers;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.QuotationService;
using Bgt.Ocean.Service.ModelViews.PricingRules;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bgt.Ocean.Infrastructure.Helpers.L2CHelper;

namespace Bgt.Ocean.Service.Implementations.PricingRules
{
    public interface IQuotationService
    {
        IEnumerable<ProductTreeView> GetPricingRuleTreeView(Guid brinksCompany_Guid, Dictionary<Guid, bool> productSelectedGuids, List<Guid> productGuidsForDisable, Guid? contract_Guid);
        IEnumerable<QuotationMappingPricingView> GetPricingRuleInQuotation(Guid quotation_Guid);
        void MappingPricingRule(MappingPricingRuleRequest request);
        SavePricingRuleToBidResponse SavePricingRuleToBid(SavePricingRuleToBidRequest request);
        bool CheckRateExceedInProduct(Guid quotation_Guid, Guid product_Guid, Guid masterUser_Guid);
        bool CheckRateExceed(Guid quotation_Guid, Guid masterUser_Guid);
        void SaveLegalToQuotation(SaveLegalToQuotationRequest requst);
        void AdjustPricingRule(AdjustRateRequest request);
        bool IsExistNameInProduct(Guid product_Guid, Guid quotation_Guid, Guid? pricingRule_Guid, string name);
        SaveCopyProductResponse SaveCopyProduct(SaveCopyProductRequest request);
    }

    public class QuotationService : IQuotationService
    {
        private readonly ILeedToCashProductRepository _leedToCashProductRepository;
        private readonly IQuotation_PricingRule_MappingRepository _quotation_PricingRule_MappingRepository;
        private readonly IQuotationRepository _quotationRepository;
        private readonly IPricingRuleRepository _pricingRuleRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IMasterUserRepository _masterUserRepository;
        private readonly IMasterClauseRepository _masterClauseRepository;
        private readonly IMasterCustomerContractRepository _masterContractRepository;
        private readonly IQuotation_ProductRepository _quotation_ProductRepository;
        private readonly IMasterCountryRepository _masterCountryRepository;
        private readonly IQuotation_HistoryRepository _quotation_HistoryRepository;
        public QuotationService(
            ILeedToCashProductRepository leedToCashProductRepository,
            IQuotation_ProductRepository quotation_ProductRepository,
            IQuotation_PricingRule_MappingRepository quotation_PricingRule_MappingRepository,
            IQuotationRepository quotationRepository,
            IPricingRuleRepository pricingRuleRepository,
            IUnitOfWork<OceanDbEntities> uow,
            ISystemMessageRepository systemMessageRepository,
            IMasterUserRepository masterUserRepository,
            IMasterClauseRepository masterClauseRepository,
            IMasterCustomerContractRepository masterContractRepository,
            IMasterCountryRepository masterCountryRepository,
            IQuotation_HistoryRepository quotation_HistoryRepository)
        {
            _leedToCashProductRepository = leedToCashProductRepository;
            _quotation_PricingRule_MappingRepository = quotation_PricingRule_MappingRepository;
            _quotationRepository = quotationRepository;
            _pricingRuleRepository = pricingRuleRepository;
            _uow = uow;
            _systemMessageRepository = systemMessageRepository;
            _masterUserRepository = masterUserRepository;
            _masterClauseRepository = masterClauseRepository;
            _masterContractRepository = masterContractRepository;
            _quotation_ProductRepository = quotation_ProductRepository;
            _masterCountryRepository = masterCountryRepository;
            _quotation_HistoryRepository = quotation_HistoryRepository;
        }

        public IEnumerable<ProductTreeView> GetPricingRuleTreeView(Guid brinksCompany_Guid, Dictionary<Guid, bool> productSelectedGuids, List<Guid> productGuidsForDisable, Guid? contract_Guid)
        {
            List<ProductTreeView> tmpProducts = _leedToCashProductRepository.FindAll(e => e.MasterCustomer_Guid == brinksCompany_Guid && !e.IsCopyData && !e.FlagRemove
                && !e.IsBidProduct && !string.IsNullOrEmpty(e.ProductName)).ConvertToProductView().ToList();

            List<ProductTreeView> p = new List<ProductTreeView>();
            tmpProducts.RemoveAll(e => e.CashStatusID == L2CHelper.Status.Draft && e.FlagProductLeaf || e.FlagDisable || e.FlagRemove);

            foreach (var item in tmpProducts.Where(e => e.ProductLevel >= 0))
            {
                if (!item.FlagProductLeaf && !tmpProducts.Any(e => e.ParentProduct_Guid == item.Guid))
                    p.Add(item);
            }
            tmpProducts.RemoveAll(e => p.Select(a => a.Guid).Contains(e.Guid));

            var temp = tmpProducts.ManageRootProductPricingRuleDictionary(productSelectedGuids, productGuidsForDisable).ToList();
            List<ProductTreeView> products = Finding(temp);
            return products;
        }

        private List<ProductTreeView> Finding(List<ProductTreeView> productTreeView)
        {
            int productTreeCount = productTreeView.Count - 1;
            for (int i = productTreeCount; i >= 0; i--)
            {
                productTreeView[i].CountChildLeaf = CountLeafChild(productTreeView[i]);
                if (productTreeView[i].CountChildLeaf > 0)
                {
                    var child = productTreeView[i].ChildProductList.ToList();
                    if (child.Any())
                    {
                        productTreeView[i].ChildProductList = Finding(child);
                    }
                }
            }

            productTreeView = productTreeView.OrderByDescending(e => e.CountChildLeaf).ToList();
            for (int i = productTreeCount; i >= 0; i--)
            {
                if (productTreeView[i].CountChildLeaf == 0 && !productTreeView[i].FlagProductLeaf) productTreeView.RemoveAt(i);
            }

            productTreeView = productTreeView.OrderBy(e => e.ProductNameDisplay).ToList();
            return productTreeView;
        }

        private int CountLeafChild(ProductTreeView productView)
        {
            if (productView.ChildProductList != null && productView.ChildProductList.Any(e => e.FlagProductLeaf))
                return 1;
            else if (productView.ChildProductList == null)
                return 0;
            else
            {
                int subCount = 0;
                foreach (var item in productView.ChildProductList)
                {
                    subCount += CountLeafChild(item);
                }
                return subCount;
            }
        }

        public IEnumerable<QuotationMappingPricingView> GetPricingRuleInQuotation(Guid quotation_Guid)
        {
            List<QuotationMappingPricingView> quotation_pricingList = new List<QuotationMappingPricingView>();
            string[] productIds = new string[] { };
            var quotation = _quotationRepository.FindById(quotation_Guid);
            if (quotation.AddendumToContract_Guid.HasValue)
                productIds = _masterContractRepository.FindById(quotation.AddendumToContract_Guid.Value).TblLeedToCashContract_PricingRules.Select(e => e.TblPricingRule.TblLeedToCashProduct.ProductID).ToArray();

            var quotation_pricingRules = quotation.TblLeedToCashQuotation_PricingRule_Mapping;
            foreach (var item in quotation_pricingRules)
            {
                var q_pricing = item.ConvertToQuotationPricingView();
                q_pricing.FlagFromContract = productIds.Contains(item.ProductId);
                quotation_pricingList.Add(q_pricing);
            }
            return quotation_pricingList;
        }

        public void MappingPricingRule(MappingPricingRuleRequest request)
        {
            // list all product in quotation
            List<Guid> refresh_product_using = new List<Guid>();

            // get quotation detail
            var quotation = _quotationRepository.FindById(request.Quotation_Guid);
            var productInQuotation = quotation.TblLeedToCashQuotation_PricingRule_Mapping.Select(e => e.TblPricingRule.TblLeedToCashProduct).Distinct((x, y) => x.Guid == y.Guid).ToDictionary(e => e.ProductID, e => e.Guid);
            var pricingRuleInQuotation = quotation.TblLeedToCashQuotation_PricingRule_Mapping.Select(e => e.TblPricingRule);

            if (!request.PricingRuleInQuotationList.Any())
            {
                // This case must not be occurs. because every quotation must have least 1 pricing rule.
                return;
            }
            else
            {
                #region remove pricing not in list and clear junk product
                // get pricing except in list for remove
                var pricing_not_in_quotation = pricingRuleInQuotation.Select(e => e.Guid)
                    .Except(request.PricingRuleInQuotationList.Where(e => e.Guid.HasValue).Select(e => e.PricingRuleView.Guid)).ToList();

                // remove pricing rule not in list of request
                var removed_pricing_in_quotation = quotation.TblLeedToCashQuotation_PricingRule_Mapping.Where(e => pricing_not_in_quotation.Contains(e.PricingRule_Guid.Value)).ToList();
                var removed_product_list = removed_pricing_in_quotation.Where(e => e.TblPricingRule.TblLeedToCashProduct.TblPricingRule.Any()).Select(e => e.TblPricingRule.TblLeedToCashProduct).ToList();
                _quotationRepository.RemovePricingMappings(removed_pricing_in_quotation);

                // refresh list
                foreach (var item in removed_product_list)
                {
                    productInQuotation.Remove(item.ProductID);
                }

                // remove junk product
                var using_in_quotation = quotation.TblLeedToCashQuotation_PricingRule_Mapping.Where(e => !pricing_not_in_quotation.Contains(e.PricingRule_Guid.Value)).ToList();
                var using_product_list = using_in_quotation.Select(e => e.TblPricingRule.TblLeedToCashProduct).ToList();
                var junk_product_list = removed_product_list.Except(using_product_list).ToList();
                _quotationRepository.RemoveAttributeMappings(quotation.TblLeedToCashQuotation_ProductAttribute.Where(e => junk_product_list.Select(a => a.Guid).Contains(e.TblLeedToCashProduct_ProductAttribute.TblLeedToCashProduct.Guid)));
                _leedToCashProductRepository.RemoveAttribute(junk_product_list.SelectMany(e => e.TblLeedToCashProduct_ProductAttribute).ToList());
                _leedToCashProductRepository.RemoveRange(junk_product_list);

                refresh_product_using.AddRange(using_product_list.Select(e => e.Guid));

                // remove account releted to pricing not use in quotation
                var removed_accounts = quotation.TblLeedToCashQuotation_Customer_Mapping.Where(
                        e => e.PricingRule_Guid.HasValue && pricing_not_in_quotation.Contains(e.PricingRule_Guid.Value));
                _quotationRepository.RemoveAccountMappings(removed_accounts);

                // remove location releted to pricing not use in quotation
                var removed_locations = quotation.TblLeedToCashQuotation_Location_Mapping.Where(
                    e => e.PricingRule_Guid.HasValue && pricing_not_in_quotation.Contains(e.PricingRule_Guid.Value));
                _quotationRepository.RemoveLocationMappings(removed_locations);
                #endregion

                // loop copy pricing rule & product from [only new pricing rule added]
                foreach (var item in request.PricingRuleInQuotationList.Where(e => !e.Guid.HasValue))
                {
                    #region loop copy pricing rule & product
                    // get source pricing from add new for mapping
                    var source_pricing = _pricingRuleRepository.FindById(item.PricingRuleView.Guid);

                    // define product guid for mapping quotation
                    Guid product_Guid = source_pricing.Product_Guid;
                    TblLeedToCashProduct cloned_product = new TblLeedToCashProduct();
                    if (productInQuotation.Keys.Contains(source_pricing.TblLeedToCashProduct.ProductID))
                    {
                        // use old copied key to save pricing
                        cloned_product = _leedToCashProductRepository.FindById(productInQuotation[source_pricing.TblLeedToCashProduct.ProductID]);
                        product_Guid = productInQuotation[source_pricing.TblLeedToCashProduct.ProductID];
                    }
                    else
                    {
                        // cloning product & attribute
                        cloned_product = source_pricing.TblLeedToCashProduct.DeepCopy();
                        cloned_product.TblLeedToCashProduct_ProductAttribute = source_pricing.TblLeedToCashProduct.TblLeedToCashProduct_ProductAttribute.DeepCopy(cloned_product);
                        _leedToCashProductRepository.Create(cloned_product);

                        product_Guid = cloned_product.Guid;
                        productInQuotation[source_pricing.TblLeedToCashProduct.ProductID] = cloned_product.Guid;
                    }

                    // regis product using
                    refresh_product_using.Add(source_pricing.Product_Guid);

                    // in case bid quotation must mapping in table TblLeedToCashQuotation_Product
                    if (quotation.TblSystemLeedToCashQuotationType.TypeID == QuotationType.Bid)
                    {
                        // create product in quotation
                        TblLeedToCashQuotation_Product newQuotationProduct = new TblLeedToCashQuotation_Product();
                        newQuotationProduct.Guid = Guid.NewGuid();
                        newQuotationProduct.LeedToCashProduct_Guid = product_Guid;
                        newQuotationProduct.LeedToCashQuotation_Guid = quotation.Guid;
                        quotation.TblLeedToCashQuotation_Product.Add(newQuotationProduct);
                    }

                    // revise
                    TblPricingRule clonedPricingRuleSource;
                    if (quotation.RefQuotation_Guid != quotation.Guid)
                    {
                        var quotation_mapping_pricingRule = _quotation_PricingRule_MappingRepository.FindAll(e => e.PricingRule_Guid == item.PricingRuleView.Guid).FirstOrDefault();
                        clonedPricingRuleSource = quotation_mapping_pricingRule == null ? source_pricing : quotation_mapping_pricingRule.TblPricingRuleSource;
                    }
                    else
                    {
                        // clone pricing master for source keeping
                        clonedPricingRuleSource = source_pricing.DeepCopy(cloned_product);
                        clonedPricingRuleSource.TblLeedToCashQuotation_Customer_Mapping = source_pricing.TblLeedToCashQuotation_Customer_Mapping.DeepCopy(quotation, clonedPricingRuleSource);
                        clonedPricingRuleSource.TblLeedToCashQuotation_Location_Mapping = source_pricing.TblLeedToCashQuotation_Location_Mapping.DeepCopy(quotation, clonedPricingRuleSource);
                        _pricingRuleRepository.Create(clonedPricingRuleSource);
                    }

                    // clone pricing & customer location(if)
                    var clonedPricingRule = source_pricing.DeepCopy(cloned_product);
                    clonedPricingRule.TblLeedToCashQuotation_Customer_Mapping = source_pricing.TblLeedToCashQuotation_Customer_Mapping.DeepCopy(quotation, clonedPricingRule);
                    clonedPricingRule.TblLeedToCashQuotation_Location_Mapping = source_pricing.TblLeedToCashQuotation_Location_Mapping.DeepCopy(quotation, clonedPricingRule);
                    _pricingRuleRepository.Create(clonedPricingRule);

                    TblLeedToCashQuotation_PricingRule_Mapping q_pricing = new TblLeedToCashQuotation_PricingRule_Mapping();
                    q_pricing.Guid = Guid.NewGuid();
                    q_pricing.LeedToCashQuotation_Guid = request.Quotation_Guid;
                    q_pricing.PricingRuleSource_Guid = clonedPricingRuleSource.Guid;
                    q_pricing.PricingRule_Guid = clonedPricingRule.Guid;
                    q_pricing.ProductId = clonedPricingRule.TblLeedToCashProduct.ProductID;
                    q_pricing.ProductName = clonedPricingRule.TblLeedToCashProduct.ProductName;
                    q_pricing.ProductDescription = clonedPricingRule.TblLeedToCashProduct.Description;
                    q_pricing.LOBName = clonedPricingRule.TblLeedToCashProduct.LOBFullName;

                    if (source_pricing.TblLeedToCashQuotation_PricingRule_Mapping.Any())
                    {
                        q_pricing.FlagExcess = source_pricing.TblLeedToCashQuotation_PricingRule_Mapping.FirstOrDefault().FlagExcess;
                        q_pricing.FlagRateChanged = source_pricing.TblLeedToCashQuotation_PricingRule_Mapping.FirstOrDefault().FlagRateChanged;
                        q_pricing.FlagApproved = source_pricing.TblLeedToCashQuotation_PricingRule_Mapping.FirstOrDefault().FlagApproved;
                    }
                    _quotationRepository.MappingPricing(q_pricing);
                    #endregion
                }
            }

            // alway refresh clause in quotation every time when save 
            #region refresh clause in quotation
            // remove old clause in quotation
            _quotationRepository.RemoveClauseMappings(quotation.TblLeedToCashQuotation_Clause);

            // prepare clause to cloning
            List<ProductClauseView> clause_prepare_list = new List<ProductClauseView>();
            foreach (var product_guid in refresh_product_using.Distinct())
            {
                var product = _leedToCashProductRepository.FindById(product_guid);
                List<TblLeedToCashProduct_Clause> findInherit = new List<TblLeedToCashProduct_Clause>();

                // find clause inherit
                if (product.Product_Guid.HasValue)
                {
                    var all_parent = _leedToCashProductRepository.FindParent(product.Product_Guid.Value);
                    var all_parent_guids = all_parent.Select(e => e.Guid).ToList();

                    // find clause inherit
                    findInherit = _leedToCashProductRepository.FindClauseInherit(all_parent_guids).OrderBy(e => e.SeqNo).ToList();
                    var clause_inherit = findInherit.Select(e => new ProductClauseView()
                    {
                        ProductID = product.ProductID,
                        FlagInherit = true,
                        Product_Clause = e
                    });
                    clause_prepare_list.AddRange(clause_inherit);
                }

                // find all publish clause in product
                var findClauseInProduct = _leedToCashProductRepository.FindClauseInProduct(product_guid);
                foreach (var clause in findClauseInProduct)
                {
                    if (!findInherit.Any(e => e.TblLeedToCashMasterClause.ClauseID == clause.TblLeedToCashMasterClause.ClauseID && e.TblLeedToCashProduct.ProductID == product.ProductID))
                    {
                        ProductClauseView clause_in_product = new ProductClauseView();
                        clause_in_product.ProductID = product.ProductID;
                        clause_in_product.Product_Clause = clause;
                        clause_prepare_list.Add(clause_in_product);
                    }
                }
            }

            // add new clause in quotation by fresh product
            int i = 1;
            foreach (var item in clause_prepare_list.OrderByDescending(e => e.Product_Clause.FlagMandatory).ThenByDescending(e => e.FlagInherit))
            {
                TblLeedToCashQuotation_Clause mapping_clause = new TblLeedToCashQuotation_Clause();
                mapping_clause.Guid = Guid.NewGuid();
                mapping_clause.ProductId = item.ProductID;
                mapping_clause.LeedToCashQuotation_Guid = quotation.Guid;
                mapping_clause.ClauseID = item.Product_Clause.TblLeedToCashMasterClause.ClauseID;
                mapping_clause.ClauseName = item.Product_Clause.TblLeedToCashMasterClause.ClauseName;
                mapping_clause.ClauseCategoryName = item.Product_Clause.TblLeedToCashMasterClause.TblLeedToCashMasterCategoryClause.ClauseCategory;
                mapping_clause.ClauseDescription = item.Product_Clause.TblLeedToCashMasterClause.Description;
                mapping_clause.FlagMandatory = item.Product_Clause.FlagMandatory;
                mapping_clause.FlagInherit = item.FlagInherit;
                mapping_clause.FlagEditableInContract = item.Product_Clause.TblLeedToCashMasterClause.TblLeedToCashMasterClauseScope.FirstOrDefault()?.FlagEdiable;
                mapping_clause.FlagVisibleInContract = item.Product_Clause.FlagVisibleInContract;
                mapping_clause.FlagVisibleInQuotation = item.Product_Clause.FlagVisibleInQuotation;
                mapping_clause.ClauseSeqNo = i;
                mapping_clause.MasterClauseCat_Guid = item.Product_Clause.TblLeedToCashMasterClause.MasterClauseCat_Guid;
                mapping_clause.MasterClause_Guid = item.Product_Clause.TblLeedToCashMasterClause.Guid;
                mapping_clause.ClauseDetail = item.Product_Clause.TblLeedToCashMasterClause.ClauseDetail;
                mapping_clause.ClauseDetailFieldID = item.Product_Clause.TblLeedToCashMasterClause.ClauseDetailFieldID;
                _quotationRepository.MappingClause(mapping_clause);
                i++;
            }
            #endregion

            #region refresh attribute in quotation
            // loop cloning attribute
            foreach (var product_guid in refresh_product_using.Distinct())
            {
                var product = _leedToCashProductRepository.FindById(product_guid);
                var latest_attr = product.TblLeedToCashProduct_ProductAttribute.OrderByDescending(e => e.VersionProductAttibute).FirstOrDefault();

                if (latest_attr != null)
                {
                    TblLeedToCashQuotation_ProductAttribute attr = new TblLeedToCashQuotation_ProductAttribute();
                    attr.Guid = Guid.NewGuid();
                    attr.LeedToCashProduct_ProductAttribute_Guid = latest_attr.Guid;
                    attr.TblLeedToCashProduct_ProductAttribute = latest_attr;
                    attr.LeedToCashQuotation_Guid = quotation.Guid;
                    attr.ProductId = product.ProductID;
                    _quotationRepository.MappingAttribute(attr);
                }
            }
            #endregion

            _uow.Commit();
        }

        public SavePricingRuleToBidResponse SavePricingRuleToBid(SavePricingRuleToBidRequest request)
        {
            SavePricingRuleToBidResponse response = new SavePricingRuleToBidResponse();

            // find last seq no
            var pricing_list = _pricingRuleRepository.FindAll(e => e.Product_Guid == request.BidProduct_Guid).ToList();
            int lastSeqNo = pricing_list.Any() ? pricing_list.Max(e => e.SeqNo) : 0;

            #region create pricing rule
            TblPricingRule pricing;
            if (request.Guid.HasValue)
            {
                // edit
                pricing = _pricingRuleRepository.FindById(request.Guid);
                pricing.Name = request.PricingRuleName;
                pricing.UserModified = request.UserName;
                pricing.DatetimeModified = request.ClientDateTime;
                pricing.MasterCurrency_Guid = request.MasterCurrency_Guid;
                pricing.WordShownInTariff = request.WordShownInTariff;
                _pricingRuleRepository.Modify(pricing);
            }
            else
            {
                // create
                pricing = new TblPricingRule();
                pricing.Guid = Guid.NewGuid();
                pricing.Product_Guid = request.BidProduct_Guid;
                pricing.Name = request.PricingRuleName;
                pricing.UserCreated = request.UserName;
                pricing.DatetimeCreated = request.ClientDateTime;
                pricing.UserModified = request.UserName;
                pricing.DatetimeModified = request.ClientDateTime;
                pricing.MasterCurrency_Guid = request.MasterCurrency_Guid;
                pricing.WordShownInTariff = request.WordShownInTariff;
                pricing.MasterPricingRule_Guid = pricing.Guid;
                pricing.SeqNo = ++lastSeqNo;
                _pricingRuleRepository.Create(pricing);

                #region mapping pricing rule to bid quotation           
                var bidProduct = _leedToCashProductRepository.FindById(request.BidProduct_Guid);
                var q_pricing = new TblLeedToCashQuotation_PricingRule_Mapping()
                {
                    Guid = Guid.NewGuid(),
                    LeedToCashQuotation_Guid = request.BidQuotation_Guid,
                    PricingRule_Guid = pricing.Guid,
                    ProductId = bidProduct.ProductID,
                    ProductName = bidProduct.ProductName,
                    LOBName = bidProduct.LOBFullName,
                    TblPricingRule = pricing,
                    ProductDescription = bidProduct.Description,
                    FlagApproved = false
                };
                _quotationRepository.MappingPricing(q_pricing);
                #endregion
            }
            #endregion

            // commit to db
            _uow.Commit();

            response.PricingRule_Guid = pricing.Guid;
            response.IsSuccess = true;

            // setup message
            var msg = _systemMessageRepository.FindByMsgId(0, ApiSession.UserLanguage_Guid.Value).ConvertToMessageView();
            response.SetMessageView(msg);
            return response;
        }

        public bool CheckRateExceedInProduct(Guid quotation_Guid, Guid product_Guid, Guid masterUser_Guid)
        {
            // get sale detail for check exceed
            var sale_user = _masterUserRepository.FindById(masterUser_Guid);
            var percent_can_discount = decimal.Parse(sale_user.TblMasterSalesLevel.PercentDiscount.ToString());
            List<ExceedView> exceedList = new List<ExceedView>();

            var quotation = _quotationRepository.FindById(quotation_Guid);
            var pricingRuleInQuotation = quotation.TblLeedToCashQuotation_PricingRule_Mapping.ToList();

            // loop for check exceed per pricing
            foreach (var item in pricingRuleInQuotation.Where(e => e.TblPricingRule.Product_Guid == product_Guid || e.TblPricingRuleSource.Product_Guid == product_Guid))
            {
                var exceedInPricing = GetRateExceed(item.TblPricingRuleSource, item.TblPricingRule);
                var maxRateDiscount = exceedInPricing.Max(e => e.PercentExceed);
                if (maxRateDiscount > percent_can_discount)
                    item.FlagExcess = true;
                else
                    item.FlagExcess = false;
                exceedList.AddRange(exceedInPricing);
                item.FlagRateChanged = exceedList.Any(e => e.IsRateChanged);
            }

            // find most percent value
            var most_exceed_rate = exceedList.Max(e => e.PercentExceed);
            if (most_exceed_rate > percent_can_discount)
            {
                // adj flag required approve
                quotation.FlagRequiredApproved = true;
                quotation.PercentDiscount = most_exceed_rate > quotation.PercentDiscount.GetValueOrDefault() ? most_exceed_rate : quotation.PercentDiscount;
                _quotationRepository.Modify(quotation);
                _uow.Commit();
                return true;
            }
            else
            {
                // adj flag required approve
                quotation.FlagRequiredApproved = pricingRuleInQuotation.Any(e => e.FlagExcess);
                if (!quotation.FlagRequiredApproved)
                    quotation.PercentDiscount = null;
                else
                    quotation.PercentDiscount = most_exceed_rate > quotation.PercentDiscount.GetValueOrDefault() ? most_exceed_rate : quotation.PercentDiscount;
                _uow.Commit();
                return false;
            }
        }

        public bool CheckRateExceed(Guid quotation_Guid, Guid masterUser_Guid)
        {
            // get sale detail for check exceed
            var sale_user = _masterUserRepository.FindById(masterUser_Guid);
            var percent_can_discount = decimal.Parse(sale_user.TblMasterSalesLevel.PercentDiscount.ToString());

            // get quotation detail
            var quotation = _quotationRepository.FindById(quotation_Guid);
            List<ExceedView> exceedList = new List<ExceedView>();
            foreach (var item in quotation.TblLeedToCashQuotation_PricingRule_Mapping)
            {
                // prepare pricing detail both source and current
                var pricing = item.TblPricingRule;
                var source_pricing = item.TblPricingRuleSource;
                var exceedPerPricingList = GetRateExceed(source_pricing, pricing);
                exceedList.AddRange(exceedPerPricingList);

                // find most percent value
                var most_exceed_pricing_rate = exceedPerPricingList.Max(e => e.PercentExceed);
                if (most_exceed_pricing_rate > percent_can_discount)
                {
                    // update flag
                    item.FlagExcess = true;
                    item.FlagRateChanged = exceedPerPricingList.Any(e => e.IsRateChanged);
                }
                else
                {
                    // update flag
                    item.FlagExcess = false;
                    item.FlagRateChanged = exceedPerPricingList.Any(e => e.IsRateChanged);
                }
            }

            var most_exceed_rate = exceedList.Max(e => e.PercentExceed);
            if (most_exceed_rate > percent_can_discount)
            {
                // adj flag required approve
                quotation.FlagRequiredApproved = true;
                quotation.PercentDiscount = most_exceed_rate;
            }
            else
            {
                // update flag
                quotation.FlagRequiredApproved = false;
                quotation.PercentDiscount = null;
            }
            _quotationRepository.Modify(quotation);
            _uow.Commit();

            return quotation.FlagRequiredApproved;
        }

        public void SaveLegalToQuotation(SaveLegalToQuotationRequest requst)
        {
            _uow.ConfigAutoDetectChanges(false);
            var quotation = _quotationRepository.FindById(requst.Quotation_Guid);
            var product = _leedToCashProductRepository.FindById(requst.Product_Guid);

            // remove old clause in quotation
            _quotationRepository.RemoveClauseMappings(quotation.TblLeedToCashQuotation_Clause.Where(e => e.ProductId == product.ProductID));

            List<ProductClauseView> clause_prepare_list = new List<ProductClauseView>();
            foreach (var item in requst.LegalUpdatedList)
            {
                var clause = _masterClauseRepository.FindById(item.MasterClause_Guid);
                var mapping = product.TblLeedToCashProduct_Clause.FirstOrDefault(e => e.TblLeedToCashMasterClause.ClauseID == clause.ClauseID);
                if (mapping == null)
                {
                    // mapping to product
                    mapping = new TblLeedToCashProduct_Clause();
                    mapping.Guid = Guid.NewGuid();
                    mapping.FlagMandatory = clause.TblLeedToCashMasterClauseScope.FirstOrDefault().FlagMandatory;
                    mapping.FlagMandatory = item.FlagMandatory;
                    mapping.FlagInherit = item.FlagInherit;
                    mapping.FlagVisibleInContract = item.FlagVisibleInContract;
                    mapping.FlagVisibleInQuotation = item.FlagVisibleInQuotation;
                    mapping.LeedToCashMasterClause_Guid = item.MasterClause_Guid;
                    mapping.LeedToCashProduct_Guid = requst.Product_Guid;
                    mapping.TblLeedToCashMasterClause = clause;
                    mapping.TblLeedToCashProduct = product;
                    mapping.SeqNo = item.ClauseSeqNo;
                }

                ProductClauseView clause_in_product = new ProductClauseView();
                clause_in_product.ProductID = product.ProductID;
                clause_in_product.Product_Clause = mapping;
                clause_in_product.ClauseCategoryName = item.ClauseCategoryName;
                clause_in_product.ClauseDescription = item.ClauseDescription;
                clause_in_product.ClauseDetail = item.ClauseDetail;
                clause_in_product.ClauseDetailFieldID = item.ClauseDetailFieldID;
                clause_in_product.ClauseID = item.ClauseID;
                clause_in_product.ClauseName = item.ClauseName;
                clause_in_product.FlagEditableInContract = item.FlagEditableInContract;
                clause_in_product.FlagInherit = item.FlagInherit;
                clause_in_product.FlagMandatory = item.FlagMandatory;
                clause_in_product.FlagVisibleInContract = item.FlagVisibleInContract;
                clause_in_product.FlagVisibleInQuotation = item.FlagVisibleInQuotation;
                clause_in_product.MasterClause_Guid = item.MasterClause_Guid;
                clause_in_product.MasterClauseCat_Guid = item.MasterClauseCat_Guid;
                clause_prepare_list.Add(clause_in_product);
            }

            int i = 1;
            foreach (var item in clause_prepare_list.OrderBy(e => e.Product_Clause.SeqNo))
            {
                TblLeedToCashQuotation_Clause mapping_clause = new TblLeedToCashQuotation_Clause();
                mapping_clause.Guid = Guid.NewGuid();
                mapping_clause.ProductId = item.ProductID;
                mapping_clause.LeedToCashQuotation_Guid = quotation.Guid;
                mapping_clause.ClauseID = item.ClauseID;
                mapping_clause.ClauseName = item.ClauseName;
                mapping_clause.ClauseCategoryName = item.ClauseCategoryName;
                mapping_clause.ClauseDescription = item.ClauseDescription;
                mapping_clause.FlagMandatory = item.FlagMandatory;
                mapping_clause.FlagInherit = item.FlagInherit;
                mapping_clause.FlagEditableInContract = item.FlagEditableInContract;
                mapping_clause.FlagVisibleInContract = item.FlagVisibleInContract;
                mapping_clause.FlagVisibleInQuotation = item.FlagVisibleInQuotation;
                mapping_clause.ClauseSeqNo = i;
                mapping_clause.MasterClauseCat_Guid = item.MasterClauseCat_Guid;
                mapping_clause.MasterClause_Guid = item.MasterClause_Guid;
                mapping_clause.ClauseDetail = item.ClauseDetail;
                mapping_clause.ClauseDetailFieldID = item.ClauseDetailFieldID;
                _quotationRepository.MappingClause(mapping_clause);
                i++;
            }
            _uow.Commit();
        }

        #region Check Exceed - Private Method
        private List<ExceedView> GetRateExceed(TblPricingRule source_pricing, TblPricingRule pricing)
        {
            List<ExceedView> exceedList = new List<ExceedView>();
            // 1. prepare list of charge
            #region list of charge
            List<Dictionary<string, decimal>> charge = new List<Dictionary<string, decimal>>();
            foreach (var item in pricing.TblChargeCategory.OrderBy(e => e.SeqNo))
            {
                var prop1 = item.GetDecimalToCompare();
                charge.Add(prop1);
            }

            List<Dictionary<string, decimal>> source_charge = new List<Dictionary<string, decimal>>();
            foreach (var item in source_pricing.TblChargeCategory.OrderBy(e => e.SeqNo))
            {
                var prop2 = item.GetDecimalToCompare();
                source_charge.Add(prop2);
            }
            #endregion
            exceedList.Add(GetPercentExceed(source_charge, charge));

            // 2. prepare list of rule value
            #region list of rule
            List<Dictionary<string, decimal>> rule = new List<Dictionary<string, decimal>>();
            foreach (var item in pricing.TblChargeCategory.OrderBy(e => e.SeqNo).SelectMany(e => e.TblChargeCategory_Rule).OrderBy(e => e.SeqNo).SelectMany(e => e.TblChargeCategory_Rule_Value).OrderBy(e => e.SeqNo))
            {
                var prop1 = item.GetDecimalToCompare();
                rule.Add(prop1);
            }

            List<Dictionary<string, decimal>> source_rule = new List<Dictionary<string, decimal>>();
            foreach (var item in source_pricing.TblChargeCategory.OrderBy(e => e.SeqNo).SelectMany(e => e.TblChargeCategory_Rule).OrderBy(e => e.SeqNo).SelectMany(e => e.TblChargeCategory_Rule_Value).OrderBy(e => e.SeqNo))
            {
                var prop2 = item.GetDecimalToCompare();
                source_rule.Add(prop2);
            }
            #endregion
            exceedList.Add(GetPercentExceed(source_rule, rule));

            // 3.prepare list of action
            #region list of action
            List<Dictionary<string, decimal>> action = new List<Dictionary<string, decimal>>();
            foreach (var item in pricing.TblChargeCategory.OrderBy(e => e.SeqNo).SelectMany(e => e.TblChargeCategory_Action).OrderBy(e => e.SeqNo))
            {
                var prop1 = item.GetDecimalToCompare();
                action.Add(prop1);
            }

            List<Dictionary<string, decimal>> source_action = new List<Dictionary<string, decimal>>();
            foreach (var item in source_pricing.TblChargeCategory.OrderBy(e => e.SeqNo).SelectMany(e => e.TblChargeCategory_Action).OrderBy(e => e.SeqNo))
            {
                var prop2 = item.GetDecimalToCompare();
                source_action.Add(prop2);
            }
            #endregion
            exceedList.Add(GetPercentExceed(source_action, action));

            // 4.prepare list of action charge
            #region list of action charge
            List<Dictionary<string, decimal>> action_value = new List<Dictionary<string, decimal>>();
            foreach (var item in pricing.TblChargeCategory.OrderBy(e => e.SeqNo).SelectMany(e => e.TblChargeCategory_Action).OrderBy(e => e.SeqNo)
                .SelectMany(e => e.TblChargeCategory_Action_Charge).OrderBy(e => e.SeqNo))
            {
                var prop1 = item.GetDecimalToCompare();
                action_value.Add(prop1);
            }

            List<Dictionary<string, decimal>> source_action_value = new List<Dictionary<string, decimal>>();
            foreach (var item in source_pricing.TblChargeCategory.OrderBy(e => e.SeqNo).SelectMany(e => e.TblChargeCategory_Action).OrderBy(e => e.SeqNo)
                .SelectMany(e => e.TblChargeCategory_Action_Charge).OrderBy(e => e.SeqNo))
            {
                var prop2 = item.GetDecimalToCompare();
                source_action_value.Add(prop2);
            }
            #endregion
            exceedList.Add(GetPercentExceed(source_action_value, action_value));

            // 5.prepare list of action condition
            #region list of action condition
            List<Dictionary<string, decimal>> action_condition = new List<Dictionary<string, decimal>>();
            foreach (var item in pricing.TblChargeCategory.OrderBy(e => e.SeqNo).SelectMany(e => e.TblChargeCategory_Action).OrderBy(e => e.SeqNo)
                .SelectMany(e => e.TblChargeCategory_Action_Charge).OrderBy(e => e.SeqNo)
                .SelectMany(e => e.TblChargeCategory_Action_Charge_Condition).OrderBy(e => e.SeqNo))
            {
                var prop1 = item.GetDecimalToCompare();
                action_condition.Add(prop1);
            }

            List<Dictionary<string, decimal>> source_action_condition = new List<Dictionary<string, decimal>>();
            foreach (var item in source_pricing.TblChargeCategory.OrderBy(e => e.SeqNo).SelectMany(e => e.TblChargeCategory_Action).OrderBy(e => e.SeqNo)
                .SelectMany(e => e.TblChargeCategory_Action_Charge).OrderBy(e => e.SeqNo)
                .SelectMany(e => e.TblChargeCategory_Action_Charge_Condition).OrderBy(e => e.SeqNo))
            {
                var prop2 = item.GetDecimalToCompare();
                source_action_condition.Add(prop2);
            }
            #endregion
            exceedList.Add(GetPercentExceed(source_action_condition, action_condition));

            return exceedList;
        }

        private ExceedView GetPercentExceed(List<Dictionary<string, decimal>> source_charge, List<Dictionary<string, decimal>> charge)
        {
            ExceedView exceed = new ExceedView();
            int point = 0;
            foreach (var item in source_charge)
            {
                for (int i = 0; i < item.Values.Count; i++)
                {
                    var source = item.ElementAt(i).Value;
                    var chargeIndex = charge.ElementAt(point);
                    if (chargeIndex.Count == 0 || source == 0) continue;

                    var current = chargeIndex.ElementAt(i).Value;
                    var discount_exceed = (((source - current) / Math.Abs(source)) * 100);
                    if (exceed.PercentExceed < discount_exceed)
                    {
                        exceed.SourceName = item.ElementAt(i).Key;
                        exceed.SourceValue = source;
                        exceed.CurrentName = charge.ElementAt(point).Keys.ElementAt(i);
                        exceed.CurrentValue = current;
                        exceed.PercentExceed = discount_exceed;
                        exceed.IsRateChanged = true;
                    }
                    else if (discount_exceed != 0 || source != current)
                    {
                        exceed.IsRateChanged = true;
                    }
                }
                point++;
            }
            return exceed;
        }

        private class ExceedView
        {
            public string SourceName { get; set; }
            public decimal SourceValue { get; set; }
            public string CurrentName { get; set; }
            public decimal CurrentValue { get; set; }
            public decimal PercentExceed { get; set; }
            public bool IsRateChanged { get; set; }
        }
        #endregion

        public void AdjustPricingRule(AdjustRateRequest request)
        {
            var quotation = _quotationRepository.FindById(request.Quotation_Guid);
            if (request.Product_Guid.HasValue)
            {
                var adjustedPricing = quotation.TblLeedToCashQuotation_PricingRule_Mapping.Where(
                    e => e.TblPricingRule.Product_Guid == request.Product_Guid);
                foreach (var item in adjustedPricing)
                {
                    if (request.Percentage == decimal.Zero)
                    {
                        // amount
                        item.AdjustedAmount = request.Amount;
                    }
                    else
                    {
                        // percent
                        item.AdjustedAmount = request.Percentage;
                        item.AdjustedTypeText = "%";
                    }
                }

                // adjust all rate in pricing of product
                var pricingRuleInQuotaton = quotation.TblLeedToCashQuotation_PricingRule_Mapping.Where(
                    e => e.TblPricingRule.Product_Guid == request.Product_Guid).Select(e => e.TblPricingRule);
                // get pricing rule from source
                var pricingRuleSourceInQuotaton = quotation.TblLeedToCashQuotation_PricingRule_Mapping.Where(
                   e => e.TblPricingRule.Product_Guid == request.Product_Guid).Select(e => e.TblPricingRuleSource);

                // adjust rate single pricing as selected
                AdjustRateInChargeCategory(pricingRuleSourceInQuotaton, pricingRuleInQuotaton.SelectMany(e => e.TblChargeCategory), request);
            }
            else
            {
                var adjustedPricing = quotation.TblLeedToCashQuotation_PricingRule_Mapping.Where(
                    e => e.PricingRule_Guid == request.PricingRule_Guid);
                foreach (var item in adjustedPricing)
                {
                    if (request.Percentage == decimal.Zero)
                    {
                        // amount
                        item.AdjustedAmount = request.Amount;
                    }
                    else
                    {
                        // percent
                        item.AdjustedAmount = request.Percentage;
                        item.AdjustedTypeText = "%";
                    }
                }

                // adjust rate of selected pricing rule 
                var pricing = _pricingRuleRepository.FindById(request.PricingRule_Guid);
                // get pricing rule from source
                var pricingRuleSourceInQuotaton = adjustedPricing.Select(e => e.TblPricingRuleSource);

                // adjust rate single pricing as selected
                AdjustRateInChargeCategory(pricingRuleSourceInQuotaton, pricing.TblChargeCategory, request);
                _pricingRuleRepository.Modify(pricing);
            }

            // stamp time to quotation updated
            quotation.DatetimeModified = request.ClientDateTime.DateTime;
            quotation.UserModifed = request.UserName;
            quotation.UniversalDatetimeModified = request.ClientDateTime;
            _quotationRepository.Modify(quotation);
            _uow.Commit();
        }

        private void AdjustRateInChargeCategory(IEnumerable<TblPricingRule> pricingRuleSourceInQuotaton, IEnumerable<TblChargeCategory> chargeCategoryList, AdjustRateRequest request)
        {
            #region adjust rate single pricing as selected
            // 1. adjust charge rate
            foreach (var item in chargeCategoryList)
            {
                var source = pricingRuleSourceInQuotaton.Where(e => e.Name == item.TblPricingRule.Name)
                    .SelectMany(e => e.TblChargeCategory).FirstOrDefault(e => e.SeqNo == item.SeqNo);
                item.Maximum = source.Maximum.AdjustRateValue(request.Percentage, request.Amount, request.AdjustRateTypeID);
                item.Minimum = source.Minimum.AdjustRateValue(request.Percentage, request.Amount, request.AdjustRateTypeID);
            }

            // 2. adjust action rate
            foreach (var item in chargeCategoryList
                .SelectMany(e => e.TblChargeCategory_Action))
            {
                var source = pricingRuleSourceInQuotaton.Where(e => e.Name == item.TblChargeCategory.TblPricingRule.Name)
                    .SelectMany(e => e.TblChargeCategory).FirstOrDefault(e => e.SeqNo == item.TblChargeCategory.SeqNo)
                    .TblChargeCategory_Action.FirstOrDefault(e => e.SeqNo == item.SeqNo);
                item.BaseRate = source.BaseRate.AdjustRateValue(request.Percentage, request.Amount, request.AdjustRateTypeID);
            }

            // 3. adjust action charge rate
            foreach (var item in chargeCategoryList
                .SelectMany(e => e.TblChargeCategory_Action)
                .SelectMany(e => e.TblChargeCategory_Action_Charge))
            {
                var source = pricingRuleSourceInQuotaton.Where(e => e.Name == item.TblChargeCategory_Action.TblChargeCategory.TblPricingRule.Name)
                    .SelectMany(e => e.TblChargeCategory).FirstOrDefault(e => e.SeqNo == item.TblChargeCategory_Action.TblChargeCategory.SeqNo)
                    .TblChargeCategory_Action.FirstOrDefault(e => e.SeqNo == item.TblChargeCategory_Action.SeqNo)
                    .TblChargeCategory_Action_Charge.FirstOrDefault(e => e.SeqNo == item.SeqNo);
                item.Value = source.Value.AdjustRateValue(request.Percentage, request.Amount, request.AdjustRateTypeID);
                item.Maximum = source.Maximum.AdjustRateValue(request.Percentage, request.Amount, request.AdjustRateTypeID);
                item.Minimum = source.Minimum.AdjustRateValue(request.Percentage, request.Amount, request.AdjustRateTypeID);
            }
            #endregion
        }

        public bool IsExistNameInProduct(Guid product_Guid, Guid quotation_Guid, Guid? pricingRule_Guid, string name)
        {
            var quotation = _quotationRepository.FindById(quotation_Guid);
            var source_pricing_Guid = quotation.TblLeedToCashQuotation_PricingRule_Mapping.FirstOrDefault(e => e.PricingRule_Guid == pricingRule_Guid)?.PricingRuleSource_Guid;

            // check duplicate
            var existNameInProduct = _pricingRuleRepository.FindAll(
                e => e.Product_Guid == product_Guid).Any(e => e.Name == name && e.Guid != pricingRule_Guid && e.Guid != source_pricing_Guid);
            return existNameInProduct;
        }

        private string GenerateProductBidQuotationId(Guid countryGuid)
        {
            int runningNumber = 0;
            var countryCode = _masterCountryRepository.FindById(countryGuid)?.MasterCountryAbbreviation;

            // get from product
            var product = _leedToCashProductRepository.FindAll(e => e.MasterCountry_Guid == countryGuid && e.ProductID != null).Where(e => e.ProductID.StartsWith($"PQ{countryCode}")).ToList();
            var productIds = product.Select(e => e.ProductID);

            var maxRunning = !productIds.IsEmpty() ? productIds.Max(e => e.GetNumber()) : 0;
            runningNumber = maxRunning + 1;

            string bidProductID = $"PQ{countryCode}{runningNumber.ToString("d3")}";
            return bidProductID;
        }

        public SaveCopyProductResponse SaveCopyProduct(SaveCopyProductRequest request)
        {
            SaveCopyProductResponse response = new SaveCopyProductResponse();
            var findProduct = _leedToCashProductRepository.FindAll(e => e.ProductID == request.ProductID && e.MasterCountry_Guid == request.MasterCountry_Guid);
            response.IsDuplicate = findProduct.Any();
            if (response.IsDuplicate)
            {
                var text = new string[] { $"Product Id {request.ProductID}" };
                response.Message = _systemMessageRepository.FindByMsgId(-106, ApiSession.UserLanguage_Guid.Value).ConvertToMessageView();
                response.Message.MessageTextContent = string.Format(response.Message.MessageTextContent, text);
                return response;
            }

            // quotation detail
            var quotation = _quotationRepository.FindById(request.Quotation_Guid);

            // source product detail
            var sourceProduct = _leedToCashProductRepository.FindById(request.SourceProduct_Guid);

            // copy product
            var cloned_product = sourceProduct.DeepCopy();
            cloned_product.SystemLineOfBusiness_Guid = sourceProduct.SystemLineOfBusiness_Guid;
            cloned_product.ProductID = string.IsNullOrWhiteSpace(request.ProductID) ? GenerateProductBidQuotationId(quotation.MasterCountry_Guid) : request.ProductID;
            cloned_product.ProductName = request.Name;
            cloned_product.MasterCountry_Guid = request.MasterCountry_Guid;
            cloned_product.Description = request.Description;
            cloned_product.LOBFullName = sourceProduct.LOBFullName;
            cloned_product.ProductLevel = 1;
            cloned_product.IsBidProduct = true;
            cloned_product.FlagProductLeaf = true;
            cloned_product.Product_Guid = sourceProduct.Product_Guid;
            cloned_product.TblSystemLeedToCashStatus = sourceProduct.TblSystemLeedToCashStatus;
            cloned_product.MasterCustomer_Guid = sourceProduct.MasterCustomer_Guid;
            cloned_product.DatetimeModified = request.LocalClientDateTime;
            cloned_product.UniversalDatetimeModified = request.ClientDateTime;
            cloned_product.UserModifed = request.UserName;

            // save with new service type list
            cloned_product.TblLeedToCashProduct_ServiceType.Clear();
            foreach (var type in request.SystemServiceType_Guids)
            {
                TblLeedToCashProduct_ServiceType newServiceType = new TblLeedToCashProduct_ServiceType();
                newServiceType.Guid = Guid.NewGuid();
                newServiceType.LeedToCashProduct_Guid = cloned_product.Guid;
                newServiceType.SystemServiceJobType_Guid = type;
                cloned_product.TblLeedToCashProduct_ServiceType.Add(newServiceType);
            }

            // copy attribute
            cloned_product.TblLeedToCashProduct_ProductAttribute = sourceProduct.TblLeedToCashProduct_ProductAttribute.DeepCopy(cloned_product);
            _leedToCashProductRepository.Create(cloned_product);

            // create product in quotation
            TblLeedToCashQuotation_Product newQuotationProduct = new TblLeedToCashQuotation_Product();
            newQuotationProduct.Guid = Guid.NewGuid();
            newQuotationProduct.LeedToCashProduct_Guid = cloned_product.Guid;
            newQuotationProduct.LeedToCashQuotation_Guid = quotation.Guid;
            newQuotationProduct.TblLeedToCashProduct = cloned_product;
            newQuotationProduct.TblLeedToCashQuotation = quotation;
            _quotation_ProductRepository.Create(newQuotationProduct);

            foreach (var source_pricing in sourceProduct.TblPricingRule)
            {
                TblPricingRule clonedPricingRuleSource;
                // clone pricing master for source keeping
                clonedPricingRuleSource = source_pricing.DeepCopy(cloned_product);
                clonedPricingRuleSource.TblLeedToCashQuotation_Customer_Mapping = source_pricing.TblLeedToCashQuotation_Customer_Mapping.DeepCopy(quotation, clonedPricingRuleSource);
                clonedPricingRuleSource.TblLeedToCashQuotation_Location_Mapping = source_pricing.TblLeedToCashQuotation_Location_Mapping.DeepCopy(quotation, clonedPricingRuleSource);
                _pricingRuleRepository.Create(clonedPricingRuleSource);

                // clone pricing & customer location(if)
                var clonedPricingRule = source_pricing.DeepCopy(cloned_product);
                clonedPricingRule.TblLeedToCashQuotation_Customer_Mapping = source_pricing.TblLeedToCashQuotation_Customer_Mapping.DeepCopy(quotation, clonedPricingRule);
                clonedPricingRule.TblLeedToCashQuotation_Location_Mapping = source_pricing.TblLeedToCashQuotation_Location_Mapping.DeepCopy(quotation, clonedPricingRule);
                _pricingRuleRepository.Create(clonedPricingRule);

                TblLeedToCashQuotation_PricingRule_Mapping q_pricing = new TblLeedToCashQuotation_PricingRule_Mapping();
                q_pricing.Guid = Guid.NewGuid();
                q_pricing.LeedToCashQuotation_Guid = request.Quotation_Guid;
                q_pricing.PricingRuleSource_Guid = clonedPricingRuleSource.Guid;
                q_pricing.PricingRule_Guid = clonedPricingRule.Guid;
                q_pricing.ProductId = clonedPricingRule.TblLeedToCashProduct.ProductID;
                q_pricing.ProductName = clonedPricingRule.TblLeedToCashProduct.ProductName;
                q_pricing.ProductDescription = clonedPricingRule.TblLeedToCashProduct.Description;
                q_pricing.LOBName = clonedPricingRule.TblLeedToCashProduct.LOBFullName;

                if (source_pricing.TblLeedToCashQuotation_PricingRule_Mapping.Any())
                {
                    q_pricing.FlagExcess = source_pricing.TblLeedToCashQuotation_PricingRule_Mapping.FirstOrDefault().FlagExcess;
                    q_pricing.FlagRateChanged = source_pricing.TblLeedToCashQuotation_PricingRule_Mapping.FirstOrDefault().FlagRateChanged;
                    q_pricing.FlagApproved = source_pricing.TblLeedToCashQuotation_PricingRule_Mapping.FirstOrDefault().FlagApproved;
                }
                _quotationRepository.MappingPricing(q_pricing);
            }

            #region prepare clause to cloning
            List<ProductClauseView> clause_prepare_list = new List<ProductClauseView>();
            List<TblLeedToCashProduct_Clause> findInherit = new List<TblLeedToCashProduct_Clause>();

            // find clause inherit
            if (sourceProduct.Product_Guid.HasValue)
            {
                var all_parent = _leedToCashProductRepository.FindParent(sourceProduct.Product_Guid.Value);
                var all_parent_guids = all_parent.Select(e => e.Guid).ToList();

                // find clause inherit
                findInherit = _leedToCashProductRepository.FindClauseInherit(all_parent_guids).OrderBy(e => e.SeqNo).ToList();
                var clause_inherit = findInherit.Select(e => new ProductClauseView()
                {
                    ProductID = sourceProduct.ProductID,
                    FlagInherit = true,
                    Product_Clause = e
                });
                clause_prepare_list.AddRange(clause_inherit);
            }

            // find all publish clause in product
            var findClauseInProduct = _leedToCashProductRepository.FindClauseInProduct(sourceProduct.Guid);
            foreach (var clause in findClauseInProduct)
            {
                if (!findInherit.Any(e => e.TblLeedToCashMasterClause.ClauseID == clause.TblLeedToCashMasterClause.ClauseID && e.TblLeedToCashProduct.ProductID == sourceProduct.ProductID))
                {
                    ProductClauseView clause_in_product = new ProductClauseView();
                    clause_in_product.ProductID = sourceProduct.ProductID;
                    clause_in_product.Product_Clause = clause;
                    clause_prepare_list.Add(clause_in_product);
                }
            }

            // add new clause in quotation by fresh product
            int i = 1;
            foreach (var item in clause_prepare_list.OrderByDescending(e => e.Product_Clause.FlagMandatory).ThenByDescending(e => e.FlagInherit))
            {
                TblLeedToCashQuotation_Clause mapping_clause = new TblLeedToCashQuotation_Clause();
                mapping_clause.Guid = Guid.NewGuid();
                mapping_clause.ProductId = cloned_product.ProductID;
                mapping_clause.LeedToCashQuotation_Guid = quotation.Guid;
                mapping_clause.ClauseID = item.Product_Clause.TblLeedToCashMasterClause.ClauseID;
                mapping_clause.ClauseName = item.Product_Clause.TblLeedToCashMasterClause.ClauseName;
                mapping_clause.ClauseCategoryName = item.Product_Clause.TblLeedToCashMasterClause.TblLeedToCashMasterCategoryClause.ClauseCategory;
                mapping_clause.ClauseDescription = item.Product_Clause.TblLeedToCashMasterClause.Description;
                mapping_clause.FlagMandatory = false;
                mapping_clause.FlagInherit = false;
                mapping_clause.FlagEditableInContract = item.Product_Clause.TblLeedToCashMasterClause.TblLeedToCashMasterClauseScope.FirstOrDefault()?.FlagEdiable;
                mapping_clause.FlagVisibleInContract = item.Product_Clause.FlagVisibleInContract;
                mapping_clause.FlagVisibleInQuotation = item.Product_Clause.FlagVisibleInQuotation;
                mapping_clause.ClauseSeqNo = i;
                mapping_clause.MasterClauseCat_Guid = item.Product_Clause.TblLeedToCashMasterClause.MasterClauseCat_Guid;
                mapping_clause.MasterClause_Guid = item.Product_Clause.TblLeedToCashMasterClause.Guid;
                mapping_clause.ClauseDetail = item.Product_Clause.TblLeedToCashMasterClause.ClauseDetail;
                mapping_clause.ClauseDetailFieldID = item.Product_Clause.TblLeedToCashMasterClause.ClauseDetailFieldID;
                mapping_clause.TblLeedToCashQuotation_Product = newQuotationProduct;
                _quotationRepository.MappingClause(mapping_clause);
                i++;
            }
            #endregion

            #region refresh attribute in quotation
            var latest_attr = cloned_product.TblLeedToCashProduct_ProductAttribute.OrderByDescending(e => e.VersionProductAttibute).FirstOrDefault();
            if (latest_attr != null)
            {
                TblLeedToCashQuotation_ProductAttribute attr = new TblLeedToCashQuotation_ProductAttribute();
                attr.Guid = Guid.NewGuid();
                attr.LeedToCashProduct_ProductAttribute_Guid = latest_attr.Guid;
                attr.TblLeedToCashProduct_ProductAttribute = latest_attr;
                attr.LeedToCashQuotation_Guid = quotation.Guid;
                attr.ProductId = cloned_product.ProductID;
                _quotationRepository.MappingAttribute(attr);
            }
            #endregion

            // stamp time           
            quotation.DatetimeModified = request.LocalClientDateTime;
            quotation.UniversalDatetimeModified = request.ClientDateTime;
            quotation.UserModifed = request.UserName;
            var msg426 = _systemMessageRepository.FindByMsgId(426, ApiSession.UserLanguage_Guid.Value).ConvertToMessageView();
            var historyParameter = string.Format(msg426.MessageTextContent, $"ProductID: {cloned_product.ProductID}", "added");

            TblLeedToCashQuotation_History history = new TblLeedToCashQuotation_History();
            history.Guid = Guid.NewGuid();
            history.UniversalDatetimeCreated = request.ClientDateTime;
            history.UserCreated = request.UserName;
            history.DatetimeCreated = request.LocalClientDateTime;
            history.MsgID = 347;
            history.MsgParameter = historyParameter;
            history.LeedToCashQuotation_Guid = quotation.Guid;
            history.RefQuotation_Guid = quotation.RefQuotation_Guid;
            _quotation_HistoryRepository.Create(history);
            _uow.Commit();
            return response;
        }
    }
}
