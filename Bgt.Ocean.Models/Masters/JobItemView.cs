using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Masters
{
    public class CitSealView
    {
        public string SealNo { get; set; }
        public string MasterID { get; set; }
        public string MasterID_Route { get; set; }
        public int? SealTypeID { get; set; }
    }

    public class SmallBagAndBulkCashSealView
    {
        public string SealNo { get; set; }
        public double Liability { get; set; }
        public string CurrencyAbbr { get; set; }
        public string CommodityName { get; set; }
        public Guid? MasterConAndDeconsolidateHeaderMasterIDRoute_Guid { get; set; }
        public Guid? MasterConAndDeconsolidateHeaderMasterID_Guid { get; set; }
        public Guid Liability_Guid { get; set; }
    }

    public class LiabilityView
    {
        public string DocumentRef { get; set; }
        public double Liability { get; set; }
        public string CurrencyAbbr { get; set; }
        public string CommodityName { get; set; }
        public string CustomerOrderNumber { get; set; }
        public IEnumerable<CitSealView> SealList { get; set; }
    }

    public class CitDeliveryView
    {
        public string Seal { get; set; }
        public string Commodity { get; set; }
        public string CustomerOrderNumber { get; set; }
        public decimal? Liability { get; set; }
        public string Currency { get; set; }
        public DateTime? DeliveryDatetime { get; set; }
        public string Status { get; set; }
        public string ScanStatus { get; set; }
        public string CancelReason { get; set; }
        public string Comment { get; set; }
    }

    public class CommodityView
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

    public class RoadNetCommodityView : CommodityView {
        public Guid? SiteGuid { get; set; }
        public Guid? CountryGuid { get; set; }
    }


}
