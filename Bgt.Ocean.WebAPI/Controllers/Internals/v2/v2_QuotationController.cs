using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.PricingRules;
using Bgt.Ocean.Service.Messagings.PricingRuleService;
using Bgt.Ocean.Service.Messagings.QuotationService;
using Bgt.Ocean.Service.ModelViews.PricingRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static Bgt.Ocean.Service.Helpers.TreeViewHelper;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v2
{
    public class v2_QuotationController : ApiControllerBase
    {
        private readonly IProductService _productService;
        private readonly IPricingRuleService _pricingRuleService;
        private readonly ISystemService _systemService;
        private readonly IQuotationService _quotationService;
        private readonly IContractService _contractService;
        public v2_QuotationController(
            IProductService productService,
            IPricingRuleService pricingRuleService,
            ISystemService systemService,
            IQuotationService quotationService,
            IContractService contractService)
        {
            _productService = productService;
            _pricingRuleService = pricingRuleService;
            _systemService = systemService;
            _quotationService = quotationService;
            _contractService = contractService;
        }

        /// <summary>
        /// Get product - pricing rule list when press button "Manage product" on Quotation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IEnumerable<ProductTreeView> GetPricingRuleTreeView(GetPricingRuleTreeViewReqeust request)
        {
            List<Guid> listFilterByContract = new List<Guid>();
            if (request.Contract_Guid.HasValue)
            {
                var pricingRules = _contractService.GetPricingRuleInContract(request.Contract_Guid.Value);
                foreach (var item in pricingRules)
                {
                    Guid product_Guid = item.Product_Guid;
                    if (item.MasterPricingRule_Guid.HasValue)
                        product_Guid = _pricingRuleService.GetPricingRuleDetail(item.MasterPricingRule_Guid.Value).Product_Guid;

                    // get product parent
                    var allProductParentGuids = _productService.GetProductParentList(product_Guid).ToList();
                    allProductParentGuids.Add(item.Product_Guid);
                    request.ProductGuidsForDisable.AddRange(allProductParentGuids);

                    // check it is master onlay can select
                    if (item.MasterPricingRule_Guid.HasValue)
                    {
                        request.ProductSelectedGuids.Add(item.MasterPricingRule_Guid.Value, false);
                        listFilterByContract.Add(item.MasterPricingRule_Guid.Value);
                    }
                }
            }

            var products = _quotationService.GetPricingRuleTreeView(request.BrinksCompany_Guid, request.ProductSelectedGuids, request.ProductGuidsForDisable, request.Contract_Guid);
            var filterProducts = products.FilterByContractPricingRule(listFilterByContract);
            return filterProducts.FilterProductTreeView(false);
        }

        /// <summary>
        /// Get pricing rule mapping in quotation
        /// </summary>
        /// <param name="quotation_Guid"></param>
        /// <param name="product_Guid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<QuotationMappingPricingView> GetPricingRuleInQuotation(Guid quotation_Guid, Guid? product_Guid = null)
        {
            var pricingRuleInQuotation = _quotationService.GetPricingRuleInQuotation(quotation_Guid);
            if (product_Guid.HasValue)
            {
                var productID = _productService.GetProductID(product_Guid.Value);
                return pricingRuleInQuotation.Where(e => e.PricingRuleView != null && e.PricingRuleView.ProductID == productID);
            }
            return pricingRuleInQuotation;
        }

        /// <summary>
        /// [Proposal] quotation need to mapping pricing rule when save after save quotation detail.
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        public HttpResponseMessage MappingPricingRule(MappingPricingRuleRequest request)
        {
            _quotationService.MappingPricingRule(request);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Save pricing rule in Bid Quotation
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        public SavePricingRuleToBidResponse SavePricingRuleToBid(SavePricingRuleToBidRequest request)
        {
            var existNameInProduct = _quotationService.IsExistNameInProduct(request.BidProduct_Guid, request.BidQuotation_Guid, request.Guid, request.PricingRuleName);
            if (existNameInProduct)
            {
                SavePricingRuleToBidResponse response = new SavePricingRuleToBidResponse();
                response.IsSuccess = false;

                // setup message
                var text = new string[] { $"{request.PricingRuleName}" };
                var duplicateMessage = _systemService.GetMessageByMsgId(-106);
                response.SetMessageView(duplicateMessage, text);
                return response;
            }

            return _quotationService.SavePricingRuleToBid(request);
        }

        /// <summary>
        /// Check exceed after save pricing rule in quotation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public bool CheckRateExceed(CheckRateExceedRequest request)
        {
            return _quotationService.CheckRateExceed(request.Quotation_Guid, request.MasterUser_Guid);
        }

        [HttpPost]
        public HttpResponseMessage SaveLegalToQuotation(SaveLegalToQuotationRequest requst)
        {
            _quotationService.SaveLegalToQuotation(requst);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Adjust rate pricing rule
        /// Support case adjust all in product and case single pricing
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AdjustPricingRule(AdjustRateRequest request)
        {
            // adjust rate
            _quotationService.AdjustPricingRule(request);

            // check exceed
            _quotationService.CheckRateExceed(request.Quotation_Guid, request.MasterUser_Guid);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Save pricing rule in quotation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public SavePricingRuleResponse SavePricingRule(SavePricingRuleRequest request)
        {
            var existNameInProduct = _quotationService.IsExistNameInProduct(request.Product_Guid, request.Quotation_Guid, request.Guid, request.Name);
            if (existNameInProduct)
            {
                SavePricingRuleResponse response = new SavePricingRuleResponse();
                response.IsSuccess = false;

                // setup message
                var text = new string[] { $"{request.Name}" };
                var duplicateMessage = _systemService.GetMessageByMsgId(-106);
                response.SetMessageView(duplicateMessage, text);
                return response;
            }

            return _pricingRuleService.SavePricingRule(request);
        }

        [HttpPost]
        public HttpResponseMessage SaveCopyProduct(SaveCopyProductRequest request)
        {
            var response = _quotationService.SaveCopyProduct(request);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        #region Get pricing dule detail with adjust rate for support mass
        [HttpGet]
        public IEnumerable<ChargeCategoryView> GetChargeCategoryListByIncrease(Guid pricingRule_Guid, decimal percentage)
        {
            var chargesInPricing = _pricingRuleService.GetChargeCategoryList(pricingRule_Guid).OrderBy(e => e.SeqNo);
            foreach (var item in chargesInPricing)
            {
                item.Maximum = item.Maximum.AdjustRateValue(percentage, 0, 0);
                item.Minimum = item.Minimum.AdjustRateValue(percentage, 0, 0);
            }
            return chargesInPricing;
        }

        [HttpGet]
        public IEnumerable<RuleView> GetRulesInChargeCategoryByIncrease(Guid chargeCategory_Guid, decimal percentage)
        {
            var rulesInCharge = _pricingRuleService.GetRulesInChargeCategory(chargeCategory_Guid).OrderBy(e => e.SeqNo);
            foreach (var rule in rulesInCharge)
            {
                List<RuleValueView> adjValList = new List<RuleValueView>();
                foreach (var item in rule.ValueList.OrderBy(e => e.SeqNo))
                {
                    item.Value = item.Value.AdjustRateValue(percentage, 0, 0);
                    adjValList.Add(item);
                }
                rule.ValueList = adjValList;
            }
            return rulesInCharge;
        }

        [HttpGet]
        public IEnumerable<ActionView> GetActionsInChargeCategoryByIncrease(Guid chargeCategory_Guid, decimal percentage)
        {
            var actionsInCharge = _pricingRuleService.GetActionsInChargeCategory(chargeCategory_Guid).OrderBy(e => e.SeqNo);
            foreach (var action in actionsInCharge)
            {
                action.BaseRate = action.BaseRate.AdjustRateValue(percentage, 0, 0);
            }
            return actionsInCharge;
        }

        [HttpGet]
        public ActionView GetActionDetailByIncrease(Guid action_Guid, decimal percentage)
        {
            var actionView = _pricingRuleService.GetActionDetail(action_Guid);
            actionView.BaseRate = actionView.BaseRate.AdjustRateValue(percentage, 0, 0);
            foreach (var item in actionView.Action_ChargeList)
            {
                item.Value = item.Value.AdjustRateValue(percentage, 0, 0);
                item.Maximum = item.Maximum.AdjustRateValue(percentage, 0, 0);
                item.Minimum = item.Minimum.AdjustRateValue(percentage, 0, 0);
            }
            return actionView;
        }
        #endregion
    }
}