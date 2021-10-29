using Bgt.Ocean.Models;
using Bgt.Ocean.Models.PricingRules;
using Bgt.Ocean.Service.ModelViews.PricingRules;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class PricingRuleMapper
    {
        public static PricingRuleView ConvertToPricingRuleView(this TblPricingRule src)
            => ServiceMapperBootstrapper.MapperService.Map<PricingRuleView>(src);

        public static QuotationMappingPricingView ConvertToQuotationPricingView(this TblLeedToCashQuotation_PricingRule_Mapping src)
            => ServiceMapperBootstrapper.MapperService.Map<QuotationMappingPricingView>(src);

        public static IEnumerable<PricingRuleView> ConvertToPricingRuleView(this IEnumerable<TblPricingRule> src)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<PricingRuleView>>(src);

        public static ChargeCategoryView ConvertToChargeCategoryView(this TblChargeCategory src)
            => ServiceMapperBootstrapper.MapperService.Map<ChargeCategoryView>(src);

        public static ActionView ConvertToActionView(this TblChargeCategory_Action src)
            => ServiceMapperBootstrapper.MapperService.Map<ActionView>(src);

        public static IEnumerable<ChargeCategoryView> ConvertToChargeCategoryView(this IEnumerable<TblChargeCategory> src)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<ChargeCategoryView>>(src);

        public static IEnumerable<RuleView> ConvertToRuleView(this IEnumerable<TblChargeCategory_Rule> src)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<RuleView>>(src);

        public static IEnumerable<ActionView> ConvertToActionView(this IEnumerable<TblChargeCategory_Action> src)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<ActionView>>(src);

        public static IEnumerable<Action_ChargeView> ConvertToAction_ChargeView(this IEnumerable<TblChargeCategory_Action_Charge> src)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<Action_ChargeView>>(src);

        public static IEnumerable<Action_Charge_ConditionView> ConvertToAction_Charge_ConditionView(this IEnumerable<TblChargeCategory_Action_Charge_Condition> src)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<Action_Charge_ConditionView>>(src);

        public static IEnumerable<ProductTreeView> ConvertToProductView(this IEnumerable<TblLeedToCashProduct> src)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<ProductTreeView>>(src);

        public static TblLeedToCashProduct DeepCopy(this TblLeedToCashProduct src)
            => ServiceMapperBootstrapper.MapperService.Map<TblLeedToCashProduct>(src);

        public static List<TblLeedToCashQuotation_Customer_Mapping> DeepCopy(this IEnumerable<TblLeedToCashQuotation_Customer_Mapping> src, TblLeedToCashQuotation quotation, TblPricingRule pricingRule)
        {
            List<TblLeedToCashQuotation_Customer_Mapping> cloned =  ServiceMapperBootstrapper.MapperService.Map<List<TblLeedToCashQuotation_Customer_Mapping>>(src);
            foreach (var item in cloned)
            {
                item.TblLeedToCashQuotation = quotation;
                item.LeedToCashQuotation_Guid = quotation.Guid;
                item.TblPricingRule = pricingRule;
                item.PricingRule_Guid = pricingRule.Guid;
            }
            return cloned;
        }

        public static List<TblLeedToCashQuotation_Location_Mapping> DeepCopy(this IEnumerable<TblLeedToCashQuotation_Location_Mapping> src, TblLeedToCashQuotation quotation, TblPricingRule pricingRule)
        {
            List<TblLeedToCashQuotation_Location_Mapping> cloned = ServiceMapperBootstrapper.MapperService.Map<List<TblLeedToCashQuotation_Location_Mapping>>(src);
            foreach (var item in cloned)
            {
                item.TblLeedToCashQuotation = quotation;
                item.LeedToCashQuotation_Guid = quotation.Guid;
                item.TblPricingRule = pricingRule;
                item.PricingRule_Guid = pricingRule.Guid;
            }
            return cloned;
        }

        public static TblPricingRule DeepCopy(this TblPricingRule src, TblLeedToCashProduct product)
        {
            TblPricingRule cloned = ServiceMapperBootstrapper.MapperService.Map<TblPricingRule>(src);
            cloned.TblLeedToCashProduct = product;
            cloned.Product_Guid = product.Guid;
            return cloned;
        }

        public static List<TblLeedToCashProduct_ProductAttribute> DeepCopy(this IEnumerable<TblLeedToCashProduct_ProductAttribute> src, TblLeedToCashProduct product)
        {
            List<TblLeedToCashProduct_ProductAttribute> cloned = ServiceMapperBootstrapper.MapperService.Map<List<TblLeedToCashProduct_ProductAttribute>>(src);
            foreach (var item in cloned)
            {
                item.TblLeedToCashProduct = product;
                item.LeedToCashProduct_Guid = product.Guid;
            }
            return cloned;
        }

        public static IEnumerable<SyncChargeCategoryView> ConvertToSyncChargeCategoryView(this IEnumerable<TblChargeCategory> src)
           => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<SyncChargeCategoryView>>(src);

        public static IEnumerable<SyncChargeCategory_ActionView> ConvertToSyncChargeCategory_ActionView(this IEnumerable<TblChargeCategory_Action> src)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<SyncChargeCategory_ActionView>>(src);

        public static IEnumerable<SyncChargeCategory_RuleView> ConvertToSyncChargeCategory_RuleView(this IEnumerable<TblChargeCategory_Rule> src)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<SyncChargeCategory_RuleView>>(src);

        public static IEnumerable<SyncChargeCategory_Rule_ValueView> ConvertToSyncChargeCategory_Rule_ValueView(this IEnumerable<TblChargeCategory_Rule_Value> src)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<SyncChargeCategory_Rule_ValueView>>(src);

        public static IEnumerable<SyncChargeCategory_Action_ChargeView> ConvertToSyncChargeCategory_Action_ChargeView(this IEnumerable<TblChargeCategory_Action_Charge> src)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<SyncChargeCategory_Action_ChargeView>>(src);

        public static IEnumerable<SyncChargeCategory_Action_Charge_ConditionView> ConvertToSyncChargeCategory_Action_Charge_ConditionView(this IEnumerable<TblChargeCategory_Action_Charge_Condition> src)
            => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<SyncChargeCategory_Action_Charge_ConditionView>>(src);

        public static SyncPricingRuleView ConvertToSyncPricingRuleView(this TblPricingRule src)
            => ServiceMapperBootstrapper.MapperService.Map<SyncPricingRuleView>(src);

        public static IEnumerable<SyncPricingRuleView> ConvertToSyncPricingRuleView(this IEnumerable<TblPricingRule> src)
           => ServiceMapperBootstrapper.MapperService.Map<IEnumerable<SyncPricingRuleView>>(src);
    }
}
