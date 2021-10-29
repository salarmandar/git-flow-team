using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models;
using Bgt.Ocean.Models.PricingRules;
using Bgt.Ocean.Repository.EntityFramework.Repositories.PricingRules;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Products;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using Bgt.Ocean.Service.Mapping.Mappers;
using Bgt.Ocean.Service.Messagings.PricingRuleService;
using Bgt.Ocean.Service.ModelViews.PricingRules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations.PricingRules
{
    public interface IPricingRuleService
    {
        IEnumerable<PricingRuleView> GetPricingRuleList(Guid product_Guid);
        PricingRuleView GetPricingRuleDetail(Guid pricingRule_Guid);
        IEnumerable<PricingRuleView> GetPricingRuleDetail(IEnumerable<Guid> pricingRule_Guids);
        IEnumerable<ChargeCategoryView> GetChargeCategoryList(Guid pricingRule_Guid);
        IEnumerable<RuleView> GetRulesInChargeCategory(Guid chargeCategory_Guid);
        IEnumerable<ActionView> GetActionsInChargeCategory(Guid chargeCategory_Guid);
        ActionView GetActionDetail(Guid action_Guid);
        DeleteResponse DeleteChargeCategory(DeleteRequest request);
        DeleteResponse DeletePricingRule(DeleteRequest request);
        DeleteResponse DeleteRule(DeleteRequest request);
        DeleteResponse DeleteAction(DeleteRequest request);
        SavePricingRuleResponse SavePricingRule(SavePricingRuleRequest request);
        SaveChargeCategoryResponse SaveChargeCategory(SaveChargeCategoryRequest request);
        SaveRuleResponse SaveRule(SaveRuleRequest request);
        SaveActionResponse SaveAction(SaveActionRequest request);
        void CopyPricingRule(CopyPricingRuleRequest request);
        void RemoveRulesAndActions(RemoveRulesAndActionsRequest request);
        bool IsExistNameInProduct(Guid product_Guid, Guid? pricingRule_Guid, string name);
        SyncPricingRuleView GetPricingRuleForSync(Guid pricingRule_Guid);
        IEnumerable<SyncPricingRuleView> GetPricingRulesForSync(IEnumerable<Guid> pricingRule_Guids);
    }

    public class PricingRuleService : IPricingRuleService
    {
        private readonly IPricingRuleRepository _pricingRuleRepository;
        private readonly IChargeCategoryRepository _chargeCategoryRepository;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly ILeedToCashProductRepository _productRepository;
        private readonly IUnitOfWork<OceanDbEntities> _uow;
        public PricingRuleService(
            IPricingRuleRepository pricingRuleRepository,
            IChargeCategoryRepository chargeCategoryRepository,
            ISystemMessageRepository systemMessageRepository,
            ILeedToCashProductRepository productRepository,
            IUnitOfWork<OceanDbEntities> uow)
        {
            _pricingRuleRepository = pricingRuleRepository;
            _chargeCategoryRepository = chargeCategoryRepository;
            _systemMessageRepository = systemMessageRepository;
            _productRepository = productRepository;
            _uow = uow;
        }

        public IEnumerable<PricingRuleView> GetPricingRuleList(Guid product_Guid)
        {
            return _pricingRuleRepository.FindAll(e => e.Product_Guid == product_Guid).ConvertToPricingRuleView();
        }

        public PricingRuleView GetPricingRuleDetail(Guid pricingRule_Guid)
        {
            return _pricingRuleRepository.FindById(pricingRule_Guid).ConvertToPricingRuleView();
        }

        public IEnumerable<PricingRuleView> GetPricingRuleDetail(IEnumerable<Guid> pricingRule_Guids)
        {
            var pricings = _pricingRuleRepository.FindByIds(pricingRule_Guids);
            return pricings.ConvertToPricingRuleView();
        }

        public IEnumerable<ChargeCategoryView> GetChargeCategoryList(Guid pricingRule_Guid)
        {
            return _chargeCategoryRepository.FindAll(e => e.PricingRule_Guid == pricingRule_Guid).ConvertToChargeCategoryView();
        }

        public IEnumerable<RuleView> GetRulesInChargeCategory(Guid chargeCategory_Guid)
        {
            var chargeCategory = _chargeCategoryRepository.FindById(chargeCategory_Guid);
            return chargeCategory.TblChargeCategory_Rule.ConvertToRuleView();
        }

        public IEnumerable<ActionView> GetActionsInChargeCategory(Guid chargeCategory_Guid)
        {
            var chargeCategory = _chargeCategoryRepository.FindById(chargeCategory_Guid);
            return chargeCategory.TblChargeCategory_Action.ConvertToActionView();
        }

        public bool IsExistNameInProduct(Guid product_Guid, Guid? pricingRule_Guid, string name)
        {
            // check duplicate
            var existNameInProduct = _pricingRuleRepository.FindAll(
                e => e.Product_Guid == product_Guid).Any(e => e.Name == name && e.Guid != pricingRule_Guid);
            return existNameInProduct;
        }

        /// <summary>
        /// Save pricing rule detail
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SavePricingRuleResponse SavePricingRule(SavePricingRuleRequest request)
        {
            SavePricingRuleResponse response = new SavePricingRuleResponse();

            // find last seq no
            var pricing_list = _pricingRuleRepository.FindAll(e => e.Product_Guid == request.Product_Guid).ToList();
            int lastSeqNo = pricing_list.Any() ? pricing_list.Max(e => e.SeqNo) : 0;

            TblPricingRule pricing;
            if (request.Guid.HasValue)
            {
                // edit
                pricing = _pricingRuleRepository.FindById(request.Guid);
                pricing.Name = request.Name;
                pricing.UserModified = request.UserName;
                pricing.DatetimeModified = request.ClientDateTime;
                pricing.MasterCurrency_Guid = request.MasterCurrency_Guid;
                pricing.WordShownInTariff = request.WordShownInTariff;
                // update time stamp
                pricing.TblLeedToCashProduct.UserModifed = request.UserName;
                pricing.TblLeedToCashProduct.DatetimeModified = request.LocalClientDateTime;
                pricing.TblLeedToCashProduct.UniversalDatetimeModified = request.UniversalDatetime;
                _pricingRuleRepository.Modify(pricing);
            }
            else
            {
                // create
                pricing = new TblPricingRule();
                pricing.Guid = Guid.NewGuid();
                pricing.Product_Guid = request.Product_Guid;
                pricing.Name = request.Name;
                pricing.UserCreated = request.UserName;
                pricing.DatetimeCreated = request.ClientDateTime;
                pricing.UserModified = request.UserName;
                pricing.DatetimeModified = request.ClientDateTime;
                pricing.MasterCurrency_Guid = request.MasterCurrency_Guid;
                pricing.WordShownInTariff = request.WordShownInTariff;
                pricing.MasterPricingRule_Guid = pricing.Guid;
                pricing.SeqNo = ++lastSeqNo;
                _pricingRuleRepository.Create(pricing);
            }
            _uow.Commit();

            response.PricingRule_Guid = pricing.Guid;
            response.IsSuccess = true;

            // setup message
            var msg = _systemMessageRepository.FindByMsgId(0, ApiSession.UserLanguage_Guid.Value).ConvertToMessageView();
            response.SetMessageView(msg);
            return response;
        }

        /// <summary>
        /// Delete pricing rule - click from grid on tab "Pricing" in product detail
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DeleteResponse DeletePricingRule(DeleteRequest request)
        {
            var pricingeRule = _pricingRuleRepository.FindById(request.Guid);

            // update time stamp
            var product = pricingeRule.TblLeedToCashProduct;
            product.UserModifed = request.UserName;
            product.DatetimeModified = request.LocalDateTime;
            product.UniversalDatetimeModified = request.UniversalDatetime;
            _productRepository.Modify(product);

            _pricingRuleRepository.Remove(pricingeRule);
            _uow.Commit();

            DeleteResponse response = new DeleteResponse();
            response.IsSuccess = true;

            // setup message
            var msg = _systemMessageRepository.FindByMsgId(0, ApiSession.UserLanguage_Guid.Value).ConvertToMessageView();
            response.SetMessageView(msg);
            return response;
        }

        public SaveChargeCategoryResponse SaveChargeCategory(SaveChargeCategoryRequest request)
        {
            SaveChargeCategoryResponse response = new SaveChargeCategoryResponse();

            // check duplicate
            var existNameInPricing = _chargeCategoryRepository.FindAll(
                e => e.PricingRule_Guid == request.PricingRule_Guid).Any(e => e.Name == request.Name && e.Guid != request.Guid);
            if (existNameInPricing)
            {
                response.IsSuccess = false;

                // setup message
                var text = new string[] { $"{request.Name}" };
                var duplicateMessage = _systemMessageRepository.FindByMsgId(-106, ApiSession.UserLanguage_Guid.Value).ConvertToMessageView();
                response.SetMessageView(duplicateMessage, text);
                return response;
            }

            // find last seq no
            var chargeCategory_list = _chargeCategoryRepository.FindAll(e => e.PricingRule_Guid == request.PricingRule_Guid).ToList();
            int lastSeqNo = chargeCategory_list.Any() ? chargeCategory_list.Max(e => e.SeqNo) : 0;

            TblChargeCategory chargeCategory;
            if (request.Guid.HasValue)
            {
                // edit
                chargeCategory = _chargeCategoryRepository.FindById(request.Guid);
                chargeCategory.IsVoid = request.IsVoid;
                chargeCategory.MasterRevenueCategory_Guid = request.MasterRevenueCategory_Guid;
                chargeCategory.Maximum = request.Maximum;
                chargeCategory.Minimum = request.Minimum;
                chargeCategory.Name = request.Name;
                chargeCategory.SystemPricingChargeType_Guid = request.SystemPricingChargeType_Guid;
                chargeCategory.SystemPricingCriteria_Guid = request.SystemPricingCriteria_Guid;
                chargeCategory.SystemPricingSatisfy_Guid = request.SystemPricingSatisfy_Guid;
                _chargeCategoryRepository.Modify(chargeCategory);
            }
            else
            {
                // create
                chargeCategory = new TblChargeCategory();
                chargeCategory.Guid = Guid.NewGuid();
                chargeCategory.PricingRule_Guid = request.PricingRule_Guid;
                chargeCategory.IsVoid = request.IsVoid;
                chargeCategory.MasterRevenueCategory_Guid = request.MasterRevenueCategory_Guid;
                chargeCategory.Maximum = request.Maximum;
                chargeCategory.Minimum = request.Minimum;
                chargeCategory.Name = request.Name;
                chargeCategory.SystemPricingChargeType_Guid = request.SystemPricingChargeType_Guid;
                chargeCategory.SystemPricingCriteria_Guid = request.SystemPricingCriteria_Guid;
                chargeCategory.SystemPricingSatisfy_Guid = request.SystemPricingSatisfy_Guid;
                chargeCategory.SeqNo = ++lastSeqNo;
                _chargeCategoryRepository.Create(chargeCategory);
            }

            // update time stamp
            var pricing = _pricingRuleRepository.FindById(request.PricingRule_Guid);
            pricing.UserModified = request.UserName;
            pricing.DatetimeModified = request.ClientDateTime;
            pricing.TblLeedToCashProduct.UserModifed = request.UserName;
            pricing.TblLeedToCashProduct.DatetimeModified = request.LocalClientDateTime;
            pricing.TblLeedToCashProduct.UniversalDatetimeModified = request.UniversalDatetime;
            if (pricing.TblLeedToCashQuotation_PricingRule_Mapping.Any())
                pricing.TblLeedToCashQuotation_PricingRule_Mapping.FirstOrDefault().FlagRateChanged = true;
            _pricingRuleRepository.Modify(pricing);
            _uow.Commit();

            response.ChargeCategory_Guid = chargeCategory.Guid;
            response.IsSuccess = true;

            // setup message
            var msg = _systemMessageRepository.FindByMsgId(0, ApiSession.UserLanguage_Guid.Value).ConvertToMessageView();
            response.SetMessageView(msg);
            return response;
        }

        public DeleteResponse DeleteChargeCategory(DeleteRequest request)
        {
            var chargeCategory = _chargeCategoryRepository.FindById(request.Guid);
            var product = chargeCategory.TblPricingRule.TblLeedToCashProduct;
            product.DatetimeModified = request.LocalDateTime;
            product.UserModifed = request.UserName;
            product.UniversalDatetimeModified = request.ClientDateTime;
            _productRepository.Modify(product);

            _chargeCategoryRepository.Remove(chargeCategory);
            _uow.Commit();

            DeleteResponse response = new DeleteResponse();
            response.IsSuccess = true;

            // setup message
            var msg = _systemMessageRepository.FindByMsgId(0, ApiSession.UserLanguage_Guid.Value).ConvertToMessageView();
            response.SetMessageView(msg);
            return response;
        }

        public SaveRuleResponse SaveRule(SaveRuleRequest request)
        {
            TblChargeCategory_Rule rule = new TblChargeCategory_Rule();
            rule.Guid = Guid.NewGuid();
            rule.ChargeCategory_Guid = request.ChargeCategory_Guid;
            rule.SeqNo = request.SeqNo;
            rule.SystemOperator_Guid = request.SystemOperator_Guid;
            rule.SystemPricingVariable_Guid = request.SystemPricingVariable_Guid;

            string key = "{key}";
            string attribute = "{attribute}";
            string query = $@"select distinct {attribute}
from 
{request.Entity_TableName}
where Job_Guid = '{key}'
and ";
            string setOfValueQuery = "(";
            foreach (var item in request.ValueList)
            {
                TblChargeCategory_Rule_Value value = new TblChargeCategory_Rule_Value();
                value.Guid = Guid.NewGuid();
                value.SeqNo = item.SeqNo;
                value.ChargeCategory_Rule_Guid = rule.Guid;
                value.Value = item.Value;
                _chargeCategoryRepository.CreateRuleValue(value);

                setOfValueQuery += $"'{item.Value}'";
                if (item != request.ValueList.Last())
                    setOfValueQuery += ",";
                else
                    setOfValueQuery += ")";
            }

            bool isExclude = request.Operator.Equals("not in");
            if (!isExclude)
                query += $"{attribute} {request.Operator} {setOfValueQuery}";
            else
                query += $"{attribute} in {setOfValueQuery}";

            rule.SQLStatement = query;
            _chargeCategoryRepository.CreateRule(rule);

            // update time stamp
            var chargeCategory = _chargeCategoryRepository.FindById(request.ChargeCategory_Guid);
            var pricing = _pricingRuleRepository.FindById(chargeCategory.PricingRule_Guid);
            pricing.UserModified = request.UserName;
            pricing.DatetimeModified = request.ClientDateTime;
            pricing.TblLeedToCashProduct.UserModifed = request.UserName;
            pricing.TblLeedToCashProduct.DatetimeModified = request.LocalClientDateTime;
            pricing.TblLeedToCashProduct.UniversalDatetimeModified = request.UniversalDatetime;
            if (pricing.TblLeedToCashQuotation_PricingRule_Mapping.Any())
                pricing.TblLeedToCashQuotation_PricingRule_Mapping.FirstOrDefault().FlagRateChanged = true;
            _pricingRuleRepository.Modify(pricing);
            _uow.Commit();

            SaveRuleResponse response = new SaveRuleResponse();
            response.Rule_Guid = rule.Guid;
            response.IsSuccess = true;

            // setup message
            var msg = _systemMessageRepository.FindByMsgId(0, ApiSession.UserLanguage_Guid.Value).ConvertToMessageView();
            response.SetMessageView(msg);
            return response;
        }

        public DeleteResponse DeleteRule(DeleteRequest request)
        {
            // get rule
            var rule = _chargeCategoryRepository.FindRuleByID(request.Guid);

            // update time stamp
            var pricing = _pricingRuleRepository.FindById(rule.TblChargeCategory.PricingRule_Guid);
            pricing.UserModified = request.UserName;
            pricing.DatetimeModified = request.ClientDateTime;
            pricing.TblLeedToCashProduct.UserModifed = request.UserName;
            pricing.TblLeedToCashProduct.DatetimeModified = request.LocalClientDateTime;
            pricing.TblLeedToCashProduct.UniversalDatetimeModified = request.UniversalDatetime;
            if (pricing.TblLeedToCashQuotation_PricingRule_Mapping.Any())
                pricing.TblLeedToCashQuotation_PricingRule_Mapping.FirstOrDefault().FlagRateChanged = true;
            _pricingRuleRepository.Modify(pricing);

            // remove rule
            _chargeCategoryRepository.RemoveRule(rule);
            _uow.Commit();

            DeleteResponse response = new DeleteResponse();
            response.IsSuccess = true;

            // setup message
            var msg = _systemMessageRepository.FindByMsgId(0, ApiSession.UserLanguage_Guid.Value).ConvertToMessageView();
            response.SetMessageView(msg);
            return response;
        }

        public SaveActionResponse SaveAction(SaveActionRequest request)
        {
            TblChargeCategory_Action action;
            if (request.Guid.HasValue)
            {
                // edit
                action = _chargeCategoryRepository.FindActionByID(request.Guid.Value);
                action.BaseRate = request.BaseRate;
                action.BasisPricingVariable_Guid = request.BasisPricingVariable_Guid;
                action.IsProgressive = request.IsProgressive;
                action.IsVoid = request.IsVoid;
                action.SystemPricingCriteria_Guid = request.SystemPricingCriteria_Guid;
                action.SystemPricingVariable_Guid = request.SystemPricingVariable_Guid;

                // remove old charge
                _chargeCategoryRepository.RemoveChargesInAction(action.TblChargeCategory_Action_Charge);
            }
            else
            {
                // create
                action = new TblChargeCategory_Action();
                action.Guid = Guid.NewGuid();
                action.ChargeCategory_Guid = request.ChargeCategory_Guid;
                action.BaseRate = request.BaseRate;
                action.BasisPricingVariable_Guid = request.BasisPricingVariable_Guid;
                action.IsProgressive = request.IsProgressive;
                action.IsVoid = request.IsVoid;
                action.SeqNo = request.SeqNo;
                action.SystemPricingCriteria_Guid = request.SystemPricingCriteria_Guid;
                action.SystemPricingVariable_Guid = request.SystemPricingVariable_Guid;
                _chargeCategoryRepository.CreateAction(action);
            }

            // loop create charge
            foreach (var req_charge in request.Action_ChargeList)
            {
                TblChargeCategory_Action_Charge charge = new TblChargeCategory_Action_Charge();
                charge.Guid = Guid.NewGuid();
                charge.ChargeCategory_Action_Guid = action.Guid;
                charge.BasisPricingVariable_Guid = req_charge.BasisPricingVariable_Guid;
                charge.Maximum = req_charge.Maximum;
                charge.Minimum = req_charge.Minimum;
                charge.FlagForEvery = req_charge.FlagForEvery;
                charge.QuantityOfBasis = req_charge.QuantityOfBasis;
                charge.SystemPricingVariable_Guid = req_charge.SystemPricingVariable_Guid;
                charge.SeqNo = req_charge.SeqNo;
                charge.SystemLogicalOperator_Guid = req_charge.SystemLogicalOperator_Guid;
                charge.SystemOperator_Guid = req_charge.SystemOperator_Guid;
                charge.Value = req_charge.Value;
                charge.SystemDivisionRounding_Guid = req_charge.SystemDivisionRounding_Guid;
                _chargeCategoryRepository.CreateActionCharge(charge);

                string key = "{key}";
                string query = $@"select *
from 
{req_charge.Entity_TableName}
where Job_Guid = '{key}'";


                if (req_charge.Action_Charge_ConditionList.Any())
                {
                    var last = req_charge.Action_Charge_ConditionList.Last();
                    string filter = " and (";
                    foreach (var req_charge_condition in req_charge.Action_Charge_ConditionList)
                    {
                        TblChargeCategory_Action_Charge_Condition condition = new TblChargeCategory_Action_Charge_Condition();
                        condition.Guid = Guid.NewGuid();
                        condition.ChargeCategory_Action_Charge_Guid = charge.Guid;
                        condition.SeqNo = req_charge_condition.SeqNo;
                        condition.SystemOperator_Guid = req_charge_condition.SystemOperator_Guid;
                        condition.SystemPricingVariable_Guid = req_charge_condition.SystemPricingVariable_Guid;
                        condition.Value = req_charge_condition.Value;
                        _chargeCategoryRepository.CreateActionChargeCondition(condition);

                        filter += $"{req_charge_condition.PricingVariable_AttributeName} {req_charge_condition.Operator} '{req_charge_condition.Value}'";
                        if (!req_charge_condition.Equals(last))
                        {
                            filter += $" {req_charge.Logical_Operator} ";
                        }
                    }
                    filter += ")";
                    query += filter;
                }
            }

            // update time stamp
            var chargeCategory = _chargeCategoryRepository.FindById(request.ChargeCategory_Guid);
            var pricing = _pricingRuleRepository.FindById(chargeCategory.PricingRule_Guid);
            pricing.UserModified = request.UserName;
            pricing.DatetimeModified = request.ClientDateTime;
            pricing.TblLeedToCashProduct.UserModifed = request.UserName;
            pricing.TblLeedToCashProduct.DatetimeModified = request.LocalClientDateTime;
            pricing.TblLeedToCashProduct.UniversalDatetimeModified = request.UniversalDatetime;
            if (pricing.TblLeedToCashQuotation_PricingRule_Mapping.Any())
                pricing.TblLeedToCashQuotation_PricingRule_Mapping.FirstOrDefault().FlagRateChanged = true;
            _pricingRuleRepository.Modify(pricing);
            _uow.Commit();

            SaveActionResponse response = new SaveActionResponse();
            response.Guid = action.Guid;
            response.IsSuccess = true;

            // setup message
            var msg = _systemMessageRepository.FindByMsgId(0, ApiSession.UserLanguage_Guid.Value).ConvertToMessageView();
            response.SetMessageView(msg);
            return response;
        }

        public DeleteResponse DeleteAction(DeleteRequest request)
        {
            // get action
            var action = _chargeCategoryRepository.FindActionByID(request.Guid);

            // update time stamp
            var pricing = _pricingRuleRepository.FindById(action.TblChargeCategory.PricingRule_Guid);
            pricing.UserModified = request.UserName;
            pricing.DatetimeModified = request.ClientDateTime;
            pricing.TblLeedToCashProduct.UserModifed = request.UserName;
            pricing.TblLeedToCashProduct.DatetimeModified = request.LocalClientDateTime;
            pricing.TblLeedToCashProduct.UniversalDatetimeModified = request.UniversalDatetime;
            if (pricing.TblLeedToCashQuotation_PricingRule_Mapping.Any())
                pricing.TblLeedToCashQuotation_PricingRule_Mapping.FirstOrDefault().FlagRateChanged = true;
            _pricingRuleRepository.Modify(pricing);

            // remove action
            _chargeCategoryRepository.RemoveAction(action);
            _uow.Commit();

            DeleteResponse response = new DeleteResponse();
            response.IsSuccess = true;

            // setup message
            var msg = _systemMessageRepository.FindByMsgId(0, ApiSession.UserLanguage_Guid.Value).ConvertToMessageView();
            response.SetMessageView(msg);
            return response;
        }

        public ActionView GetActionDetail(Guid action_Guid)
        {
            var action = _chargeCategoryRepository.FindActionByID(action_Guid);
            return action.ConvertToActionView();
        }

        public void CopyPricingRule(CopyPricingRuleRequest request)
        {
            // get product detail
            var product = _productRepository.FindById(request.Product_Guid);

            var source_product = _productRepository.FindById(request.SourceProduct_Guid);
            // loop cloning pricing rule to duplicated product
            foreach (var item in source_product.TblPricingRule)
            {
                var cloned = item.DeepCopy(product);
                _pricingRuleRepository.Create(cloned);
            }
            _uow.Commit();
        }
        
        public void RemoveRulesAndActions(RemoveRulesAndActionsRequest request)
        {
            var chargeCategory = _chargeCategoryRepository.FindById(request.ChargeCategory_Guid);
            chargeCategory.SystemPricingChargeType_Guid = request.SystemPricingChargeType_Guid;
            _chargeCategoryRepository.Modify(chargeCategory);

            // remove action
            if (chargeCategory.TblChargeCategory_Action.Any())
                _chargeCategoryRepository.RemoveActionList(chargeCategory.TblChargeCategory_Action);

            // remove rule
            if (chargeCategory.TblChargeCategory_Rule.Any())
                _chargeCategoryRepository.RemoveRuleList(chargeCategory.TblChargeCategory_Rule);

            _uow.Commit();
        }

        public SyncPricingRuleView GetPricingRuleForSync(Guid pricingRule_Guid)
        {
            var pricingRule = _pricingRuleRepository.FindById(pricingRule_Guid);
            return pricingRule.ConvertToSyncPricingRuleView();
        }

        public IEnumerable<SyncPricingRuleView> GetPricingRulesForSync(IEnumerable<Guid> pricingRule_Guids)
        {
            var pricingRules = _pricingRuleRepository.FindAll(e => pricingRule_Guids.Contains(e.Guid));
            return pricingRules.ConvertToSyncPricingRuleView();
        }
    }
}
