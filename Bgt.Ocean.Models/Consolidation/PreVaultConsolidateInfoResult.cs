using System;
using System.Collections.Generic;
using static Bgt.Ocean.Models.Consolidation.ConsolidationView;

namespace Bgt.Ocean.Models.Consolidation
{
    public class PreVaultConsolidateInfoResult
    {
        public string MasterID { get; set; }
        public Nullable<System.Guid> SystemCoAndDeSolidateStatus_Guid { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string StatusName { get; set; }
        public Nullable<System.Guid> MasterCustomerLocation_Guid { get; set; }
        public Nullable<System.Guid> MasterRouteGroup_Detail_Guid { get; set; }
        public DateTime Workdate { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public Nullable<int> SystemConsolidateSourceID { get; set; }
        public Nullable<System.Guid> MasterSite_Guid { get; set; }
        public Nullable<System.Guid> ConsolidationRoute_Guid { get; set; }
        public Nullable<int> RunResourceDailyStatusID { get; set; }
        public string ConOrDeconsolidateType { get; set; }
        public Nullable<System.Guid> MasterID_Guid { get; set; }
        public string LocationName { get; set; }
        public string RouteName { get; set; }
        public Nullable<System.Guid> MasterDailyRunResource_Guid { get; set; }
        public decimal Liability { get; set; }
        public Nullable<System.Guid> MasterCustomerLocation_InternalDepartment_Guid { get; set; }
        public Nullable<int> ConOrDeconsolidateTypeID { get; set; }
        public Nullable<System.Guid> Destination_MasterSite_Guid { get; set; }
        public string Destination_MasterSite_Name { get; set; }
        public Nullable<System.Guid> OnwardGuid { get; set; }
        public string InternalDepartmentName { get; set; }
        public string SitePathName { get; set; }
        public Nullable<System.Guid> SitePathGuid { get; set; }
    }

    public class ConAvailableItemView //PreVaultConsolidationAvailableSealResult
    {
        public Nullable<System.Guid> JobGuid { get; set; }
        public string JobNo { get; set; }
        public string ServiceJobTypeNameAbb { get; set; }
        public string PickUp_Location { get; set; }
        public string Delivery_Location { get; set; }
        public string WorkDate { get; set; }
        public Nullable<System.Guid> CustomerGuid { get; set; }
        public Nullable<System.Guid> CustomerLocationGuid { get; set; }
        public string SealNo { get; set; }
        public string Commodity { get; set; }
        public string CurrencyNameAbb { get; set; }
        public Nullable<int> Qty { get; set; }
        public string RouteName { get; set; }
        public string GroupScanName { get; set; }
        public Nullable<System.Guid> DailyRunGuid { get; set; }
        public Nullable<System.Guid> SealGuid { get; set; }
        public Nullable<System.Guid> LiabilityGuid { get; set; }
        public Nullable<System.Guid> CommodityGuid { get; set; }
        public Nullable<double> Liability { get; set; }
        public string GroupSeal { get; set; }
        public Nullable<System.Guid> MasterRouteGroupDetail_Guid { get; set; }
        public Nullable<System.Guid> OnwardDestinationGuid { get; set; }
        public string InterDepartmentName { get; set; }
        public string MasterID { get; set; }
        public Nullable<System.Guid> MasterID_Guid { get; set; }
        public Nullable<bool> FlagChkCustomer { get; set; }

        #region ## Use for calculate total summary.
        public string ItemsSummary { get; set; }
        public double Total { get; set; }
        public bool FlagCheckEdit { get; set; } = false;
        public bool FlagDisable { get; set; } = true;
        public string DataRecordType { get; set; }
        public List<LiabilityView> LiabilityList { get; set; } = new List<LiabilityView>();
        #endregion

    }

    public class PreVaultConsolidationLiabilityValueDetailResult
    {
        public System.Guid MasterID_Guid { get; set; }
        public string MasterID { get; set; }
        public System.Guid SystemCoAndDeSolidateStatus_Guid { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public Nullable<System.Guid> MasterCustomerLocation_Guid { get; set; }
        public string LocationName { get; set; }
        public Nullable<System.Guid> MasterRouteGroup_Detail_Guid { get; set; }
        public System.DateTime Workdate { get; set; }
        public string UserCreated { get; set; }
        public Nullable<System.DateTime> DatetimeCreated { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public int SystemConsolidateSourceID { get; set; }
        public Nullable<System.Guid> MasterSite_Guid { get; set; }
        public Nullable<System.Guid> ConsolidationRoute_Guid { get; set; }
        public string RunResourceDailyStatusID { get; set; }
        public string ConOrDeconsolidateType { get; set; }
        public int ConOrDeconsolidateTypeID { get; set; }
        public string RouteName { get; set; }
        public Nullable<System.Guid> MasterDailyRunResource_Guid { get; set; }
        public Nullable<double> Liability { get; set; }
        public string CurrencyNameAbb { get; set; }
        public Nullable<System.Guid> Liability_Guid { get; set; }
        public Nullable<System.Guid> MasterCustomerLocation_InternalDepartment_Guid { get; set; }
        public Nullable<System.Guid> OnwardDestination_Guid { get; set; }
    }

    public class LocationConsolidateInfoView
    {
        public Nullable<System.Guid> MasterID_Guid { get; set; }
        public string MasterID { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public string LocationName { get; set; }
        public string RouteName { get; set; }
    }

    public class RouteConsolidateInfoView
    {
        public Nullable<System.Guid> MasterID_Guid { get; set; }
        public string MasterID { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public string RouteName { get; set; }
    }

    public class InterBranchConsolidateInfoView
    {
        public Nullable<System.Guid> MasterID_Guid { get; set; }
        public string MasterID { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public string LocationName { get; set; }
        public string Destination_MasterSite_Name { get; set; }
        public string RouteName { get; set; }
        public string InternalDepartmentName { get; set; }
        public string SitePathName { get; set; }
    }

    public class MultiBranchConsolidateInfoView
    {
        public Nullable<System.Guid> MasterID_Guid { get; set; }
        public string MasterID { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public string LocationName { get; set; }
        public string Destination_MasterSite_Name { get; set; }
        public string RouteName { get; set; }
        public string InternalDepartmentName { get; set; }
        public string SitePathName { get; set; }
    }
}

