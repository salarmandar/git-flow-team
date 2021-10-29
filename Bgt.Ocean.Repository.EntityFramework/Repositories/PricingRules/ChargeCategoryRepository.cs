using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.PricingRules
{
    public interface IChargeCategoryRepository : IRepository<TblChargeCategory>
    {
        void CreateRule(TblChargeCategory_Rule rule);
        void CreateRuleValue(TblChargeCategory_Rule_Value ruleValue);
        void CreateAction(TblChargeCategory_Action action);
        void CreateActionCharge(TblChargeCategory_Action_Charge charge);
        void CreateActionChargeCondition(TblChargeCategory_Action_Charge_Condition condition);

        TblChargeCategory_Rule FindRuleByID(Guid rule_Guid);
        TblChargeCategory_Action FindActionByID(Guid action_Guid);

        void RemoveAction(TblChargeCategory_Action action);
        void RemoveActionList(IEnumerable<TblChargeCategory_Action> actions);
        void RemoveRule(TblChargeCategory_Rule rule);
        void RemoveRuleList(IEnumerable<TblChargeCategory_Rule> rules);
        void RemoveChargesInAction(IEnumerable<TblChargeCategory_Action_Charge> charges);
    }

    public class ChargeCategoryRepository : Repository<OceanDbEntities, TblChargeCategory>, IChargeCategoryRepository
    {
        public ChargeCategoryRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public void CreateAction(TblChargeCategory_Action action)
        {
            DbContext.TblChargeCategory_Action.Add(action);
        }

        public void CreateActionCharge(TblChargeCategory_Action_Charge charge)
        {
            DbContext.TblChargeCategory_Action_Charge.Add(charge);
        }

        public void CreateActionChargeCondition(TblChargeCategory_Action_Charge_Condition condition)
        {
            DbContext.TblChargeCategory_Action_Charge_Condition.Add(condition);
        }

        public void CreateRule(TblChargeCategory_Rule rule)
        {
            DbContext.TblChargeCategory_Rule.Add(rule);
        }

        public void CreateRuleValue(TblChargeCategory_Rule_Value ruleValue)
        {
            DbContext.TblChargeCategory_Rule_Value.Add(ruleValue);
        }

        public TblChargeCategory_Action FindActionByID(Guid action_Guid)
        {
            return DbContext.TblChargeCategory_Action.FirstOrDefault(e => e.Guid == action_Guid);
        }

        public TblChargeCategory_Rule FindRuleByID(Guid rule_Guid)
        {
            return DbContext.TblChargeCategory_Rule.FirstOrDefault(e => e.Guid == rule_Guid);
        }

        public void RemoveAction(TblChargeCategory_Action action)
        {
            DbContext.TblChargeCategory_Action.Remove(action);
        }

        public void RemoveActionList(IEnumerable<TblChargeCategory_Action> actions)
        {
            DbContext.TblChargeCategory_Action.RemoveRange(actions);
        }

        public void RemoveChargesInAction(IEnumerable<TblChargeCategory_Action_Charge> charges)
        {
            DbContext.TblChargeCategory_Action_Charge.RemoveRange(charges);
        }

        public void RemoveRule(TblChargeCategory_Rule rule)
        {
            DbContext.TblChargeCategory_Rule.Remove(rule);
        }

        public void RemoveRuleList(IEnumerable<TblChargeCategory_Rule> rules)
        {
            DbContext.TblChargeCategory_Rule.RemoveRange(rules);
        }
    }
}
