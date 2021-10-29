using Bgt.Ocean.Models.PricingRules;
using Bgt.Ocean.Service.Implementations;
using Bgt.Ocean.Service.Implementations.PricingRules;
using Bgt.Ocean.Service.Messagings.PricingRuleService;
using Bgt.Ocean.Service.ModelViews.PricingRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bgt.Ocean.WebAPI.Controllers.Internals.v2
{
    public class v2_PricingRuleController : ApiControllerBase
    {
        private readonly IPricingRuleService _pricingRuleService;
        private readonly ISystemService _systemService;
        public v2_PricingRuleController(
            IPricingRuleService pricingRuleService,
            ISystemService systemService)
        {
            _pricingRuleService = pricingRuleService;
            _systemService = systemService;
        }

        #region Pricing Rule Detail
        /// <summary>
        /// Get pricing rule detail
        /// </summary>
        /// <param name="pricingRule_Guid"></param>
        /// <returns></returns>
        [HttpGet]
        public PricingRuleView GetPricingRuleDetail(Guid pricingRule_Guid)
        {
            return _pricingRuleService.GetPricingRuleDetail(pricingRule_Guid);
        }

        /// <summary>
        /// Get pricing rule detail as list.
        /// Request with multiple selected pricing rule to get list of pricing rule detail
        /// </summary>
        /// <param name="pricingRule_Guids"></param>
        /// <returns></returns>
        [HttpPost]
        public IEnumerable<PricingRuleView> GetMultiplePricingRuleDetail([FromBody]IEnumerable<Guid> pricingRule_Guids)
        {
            return _pricingRuleService.GetPricingRuleDetail(pricingRule_Guids);
        }

        /// <summary>
        /// Get charge category list sorting by SeqNo
        /// </summary>
        /// <param name="pricingRule_Guid"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<ChargeCategoryView> GetChargeCategoryList(Guid pricingRule_Guid)
        {
            return _pricingRuleService.GetChargeCategoryList(pricingRule_Guid).OrderBy(e => e.SeqNo);
        }

        [HttpGet]
        public IEnumerable<RuleView> GetRulesInChargeCategory(Guid chargeCategory_Guid)
        {
            return _pricingRuleService.GetRulesInChargeCategory(chargeCategory_Guid).OrderBy(e => e.SeqNo);
        }

        [HttpGet]
        public IEnumerable<ActionView> GetActionsInChargeCategory(Guid chargeCategory_Guid)
        {
            return _pricingRuleService.GetActionsInChargeCategory(chargeCategory_Guid).OrderBy(e => e.SeqNo);
        }

        [HttpPost]
        public SaveChargeCategoryResponse SaveChargeCategory(SaveChargeCategoryRequest request)
        {
            return _pricingRuleService.SaveChargeCategory(request);
        }

        [HttpPost]
        public DeleteResponse DeleteChargeCategory(DeleteRequest request)
        {
            return _pricingRuleService.DeleteChargeCategory(request);
        }

        /// <summary>
        /// Copy pricing rule from source to duplicated product
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage CopyPricingRule(CopyPricingRuleRequest request)
        {
            _pricingRuleService.CopyPricingRule(request);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Remove all rule and action if switch charge type
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage RemoveRulesAndActions(RemoveRulesAndActionsRequest request)
        {
            _pricingRuleService.RemoveRulesAndActions(request);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        #endregion

        #region Tab Pricing
        [HttpGet]
        public IEnumerable<PricingRuleView> GetPricingRuleList(Guid product_Guid)
        {
            return _pricingRuleService.GetPricingRuleList(product_Guid).OrderBy(e => e.SeqNo);
        }

        /// <summary>
        /// Save pricing rule
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public SavePricingRuleResponse SavePricingRule(SavePricingRuleRequest request)
        {
            var existNameInProduct = _pricingRuleService.IsExistNameInProduct(request.Product_Guid, request.Guid, request.Name);
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

        /// <summary>
        /// Delete pricing rule
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public DeleteResponse DeletePricingRule(DeleteRequest request)
        {
            return _pricingRuleService.DeletePricingRule(request);
        }
        #endregion

        #region Rule
        /// <summary>
        /// Delete rule
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public DeleteResponse DeleteRule(DeleteRequest request)
        {
            return _pricingRuleService.DeleteRule(request);
        }

        /// <summary>
        /// Save rule detail when submit on popup
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public SaveRuleResponse SaveRule(SaveRuleRequest request)
        {
            return _pricingRuleService.SaveRule(request);
        }
        #endregion

        #region Action
        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public DeleteResponse DeleteAction(DeleteRequest request)
        {
            return _pricingRuleService.DeleteAction(request);
        }

        /// <summary>
        /// Save action detail with list of condition
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public SaveActionResponse SaveAction(SaveActionRequest request)
        {
            return _pricingRuleService.SaveAction(request);
        }

        /// <summary>
        /// Get detail of "Action" popup
        /// </summary>
        /// <param name="action_Guid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionView GetActionDetail(Guid action_Guid)
        {
            return _pricingRuleService.GetActionDetail(action_Guid);
        }
        #endregion

        #region For Syncing
        [HttpGet]
        public SyncPricingRuleView GetPricingRuleForSync(Guid pricingRule_Guid)
        {
            var pricingRule = _pricingRuleService.GetPricingRuleForSync(pricingRule_Guid);
            return pricingRule;
        }

        [HttpPost]
        public IEnumerable<SyncPricingRuleView> GetPricingRulesForSync([FromBody]IEnumerable<Guid> pricingRule_Guids)
        {
            var pricingRules = _pricingRuleService.GetPricingRulesForSync(pricingRule_Guids);
            return pricingRules;
        }
        #endregion
    }
}