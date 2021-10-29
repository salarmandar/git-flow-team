using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.ActualJob
{
    public class JobWithStcView
    {
        public Guid JobGuid { get; set; }
        public double STC { get; set; }
        public string strCommodity { get; set; }
        public int qtyCommodity { get; set; }
        public List<CommodityValueView> CommodityList { get; set; }
        public List<CurrencyValueView> CurrencyList { get; set; }
        public bool FlagExConvertToRun { get; set; }
        public List<string> CurrencyNotConvert { get; set; }
    }

    public class CommodityValueView
    {
        public string CommodityName { get; set; }
        public int CommodityQty { get; set; }
        public int GoldenRuleNo { get; set; }
    }

    public class CurrencyValueView
    {
        public string CurrencyName { get; set; }
        public double LiabilityValue { get; set; }
        public double UserLiabilityValue { get; set; }
        public bool FlagExConvert { get; set; }
    }
}
