using Bgt.Ocean.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Models.PreVault
{
    public class VaultBalanceView
    {
        public IEnumerable<VaultStateModel> VaultStateList { get; set; }
        public int Affected { get; set; }
    }


    public class VaultBalanceItemModel
    {
        public VaultStateModel VaultStateModel { get; set; }
        public IEnumerable<TblMasterActualJobItemsSeal> SealList { get; set; }
        public IEnumerable<TblMasterActualJobItemsCommodity> CommList { get; set; }
        public IEnumerable<TblMasterConAndDeconsolidate_Header> ConList { get; set; }

        public int Affected
        {
            get
            {
                return (SealList == null ? 0 : SealList.Count()) + (CommList == null ? 0 : CommList.Count()) + (ConList == null ? 0 : ConList.Count());
            }
        }
    }


    public class VaultStateModel
    {
        public Guid? VaultBalanceHeader_Guid { get; set; }
        public Guid? InternalDepartment_Guid { get; set; }
        public EnumVaultState VaultState { get; set; }
        public string InternalFullName { get; set; }
    }


    public class AutoUpdateVaultBalanceModel
    {
        public Guid? InternalGuid { get; set; }
        public IEnumerable<Guid?> SealGuidList { get; set; }
        public IEnumerable<Guid?> CommodityGuidList { get; set; }
        public IEnumerable<Guid?> ConsolidateGuidList { get; set; }
    }

    public class VaultBalanceSealModel : VaultBalanceJobDetailModel
    {
        public Guid Guid { get; set; }
        public string SealNo { get; set; }
        public decimal STC { get; set; }
        public string Commodity { get; set; }
        public string Currency { get; set; }
        public Guid? LiabilityGuid { get; set; }
        public EnumItemState ScanItemState { get; set; }

    }

    public class VaultBalanceNonbarcodeModel
    {
        public Guid CommodityGuid { get; set; }
        public string CommodityName { get; set; }
        public decimal STC { get; set; }
        public decimal CommodityValue { get; set; }
        public decimal CommodityAmount { get; set; }
        public int PreAdviceQty { get; set; }
        public int ActualQty { get; set; }
        public EnumState ItemState { get; set; } //Use 0,2
        public bool FlagTemp { get; set; }
    }

    public class VaultBalanceJobDetailModel
    {
        public Guid JobGuid { get; set; }
        public string JobNo { get; set; }
        public string PickUpLocation { get; set; }
        public string DeliveryLocation { get; set; }
        public string ServiceType { get; set; }
        public DateTime WorkDate { get; set; }
        public bool FlagJobInterBr { get; set; }
        public bool FlagJobMultiBr { get; set; }

        public Guid? CustomerPickupGuid { get; set; }
        public Guid? LocationPickupGuid { get; set; }
        public Guid? CustomerDeliveryGuid { get; set; }
        public Guid? LocationDeliveryGuid { get; set; }
    }

    public class ConsolidateDetailModel
    {
        public Guid MasterGuid { get; set; }
        public string MasterID { get; set; }
        public string ConsolidateType { get; set; }
    }

}
