using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.ModelViews.PreVault.VaultBalance
{
    public class VaultBalanceModelView
    {
        public Guid BrinkSiteGuid { get; set; }
        public Guid PrevaultGuid { get; set; }
        public TblVaultBalanceHeader VaultBalanceHeader { get; set; }
        public IEnumerable<TblMasterActualJobItemsSeal> ActualItemSealList { get; set; }
        public IEnumerable<TblMasterActualJobItemsCommodity> ActualItemCommodityList { get; set; }
    }
    public class VaultBalanceMappingModelView
    {
        [Description("Guid of actual item seal or master")]
        public Guid Guid { get; set; }
        public Guid? JobGuid { get; set; }
        public string JobNo { get; set; }
        public string ConsolidateType { get; set; }
        public string PickUpLocation { get; set; }
        public string DeliveryLocation { get; set; }
        public string SealNo { get; set; }
        public bool FlagScan { get; set; }
    }
    public class VaultBalanceSummaryModelView
    {
        [Description("ActualSealGuid Or MasterConsolidaeGuid Or ActualCommodityGuid")]
        public Guid? ItemGuid { get; set; }
        public string ItemName { get; set; }
        public Guid? JobGuid { get; set; }
        public int Shortage { get; set; }
        public int Overage { get; set; }
        public Guid? ReasonTypeGuid { get; set; }
        public string Remark { get; set; }

        //For Back - end
        public Guid? CommodityGuid { get; set; }
        public string ColumnInReport { get; set; }
        public string CommodityCode { get; set; }
        public EnumItemType ItemType { get; set; }

    }

    public class ShortageSealModelView
    {
        public Guid? Guid { get; set; }
        public string SealNo { get; set; }
    }
    public class VaultBalanceHeaderModelView
    {
        public Guid? VaultBalanceGuid { get; set; }
    }

    public class LocationModelView
    {
        //For insert to discrepancy
        public Guid? CustomerPickupGuid { get; set; }
        public Guid? LocationPickupGuid { get; set; }
        public Guid? CustomerDeliveryGuid { get; set; }
        public Guid? LocationDeliveryGuid { get; set; }
    }

}
