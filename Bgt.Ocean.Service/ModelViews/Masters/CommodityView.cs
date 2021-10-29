using System;

namespace Bgt.Ocean.Service.ModelViews.Masters
{
    public class CommodityView
    {
        public Guid Guid { get; set; }
        public string CommodityName { get; set; }
        public Double? CommodityAmount { get; set; }
        public Double? CommodityValue { get; set; }
        public string CommodityGroupName { get; set; }
        public string ColumnInReport { get; set; }
        public bool FlagDefault { get; set; }
        public Guid? SystemGlobalUnit_Guid { get; set; }
        public string UnitDisplayText { get; set; }
    }
}
