using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.ModelViews.PricingRules
{
    public class ActionView
    {
        public Guid? Guid { get; set; }
        public Guid ChargeCategory_Guid { get; set; }
        public decimal BaseRate { get; set; }
        public Guid BasisPricingVariable_Guid { get; set; }
        public int SeqNo { get; set; }
        public bool IsVoid { get; set; }
        public bool IsProgressive { get; set; }
        public Guid? SystemPricingCriteria_Guid { get; set; }
        public Guid? SystemPricingVariable_Guid { get; set; }
        public IEnumerable<Action_ChargeView> Action_ChargeList { get; set; }
    }

    public class Action_ChargeView
    {
        public Guid? Guid { get; set; }
        public Guid? Action_Guid { get; set; }
        public int SeqNo { get; set; }
        public Guid SystemOperator_Guid { get; set; }
        public bool FlagForEvery { get; set; }

        /// <summary>
        /// Basis as same as detail of charge
        /// </summary>
        public Guid BasisPricingVariable_Guid { get; set; }
        public Guid? SystemLogicalOperator_Guid { get; set; }
        public decimal Value { get; set; }

        /// <summary>
        /// For every {QuantityOfBasis} of basis 
        /// </summary>
        public decimal QuantityOfBasis { get; set; }

        /// <summary>
        /// Selected value from drop-down beside 'For every'
        /// </summary>
        public Guid SystemPricingVariable_Guid { get; set; }

        public decimal? Minimum { get; set; }
        public decimal? Maximum { get; set; }
        public Guid SystemDivisionRounding_Guid { get; set; }
        public IEnumerable<Action_Charge_ConditionView> Action_Charge_ConditionList { get; set; } = new List<Action_Charge_ConditionView>();

        /// <summary>
        /// TableName from TblSystemPricingEntity
        /// </summary>
        public string Entity_TableName { get; set; }

        /// <summary>
        /// SQL operator from TblSystemLogicalOperator
        /// </summary>
        public string Logical_Operator { get; set; }
    }

    public class Action_Charge_ConditionView
    {
        public Guid? Guid { get; set; }
        public Guid? Action_Charge_Guid { get; set; }
        public int SeqNo { get; set; }
        public Guid? SystemPricingVariable_Guid { get; set; }
        public Guid? SystemOperator_Guid { get; set; }
        public string Value { get; set; }

        /// <summary>
        /// SQL operator from TblSystemOperator, It must depend on SystemOperator_Guid
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// AttributeName from TblSystemPricingVariable
        /// </summary>
        public string PricingVariable_AttributeName { get; set; }
    }
}
