using System;

namespace Bgt.Ocean.Models.ActualJob
{
    public class TemplateCommodityView
    {
        public Guid? MasterActualJobItemsCommodity_Guid { get; set; }
        public Guid? MasterCommodity_Guid { get; set; }
        public Guid? MasterCommodityGroup_Guid { get; set; }
        public Guid? MasterActualJobHeader_Guid { get; set; }
        public Guid? CCGuid { get; set; } //CCGuid can be MasterCommodity_Guid or MasterCommodityGroup_Guid
        public int Quantity { get; set; } = 0;
        public int QuantityExpected { get; set; } = 0;
        public string CommodityName { get; set; }
        public string CommodityCode { get; set; }
        public string CommodityGroupName { get; set; }
        public string DenoText { get; set; }
        public bool FlagCommodityGlobal { get; set; }
        public string ColumnInReport { get; set; }
        public double CommodityAmount { get; set; }
        public double CommodityValue { get; set; }
        public int ItemState { get; set; } = 0;
        public bool? FlagRequireSeal { get; set; }
        public bool FlagReqDetail { get; set; }
        public double calSTC { get; set; }
        public int calQTY { get; set; }

    }
}
