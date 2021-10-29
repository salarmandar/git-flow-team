using System;

namespace Bgt.Ocean.Service.Messagings.PreVault.DiscrepancyManagement
{
    public class DiscrepancyItemsResponse
    {
        public Nullable<System.Guid> JobGuid { get; set; }
        public string JobNo { get; set; }
        public string PU_Location { get; set; }
        public string DL_Location { get; set; }
        public int QtyShortage { get; set; }
        public int QtyOverage { get; set; }
        public string SealNo { get; set; }
        public string ReasonTypeName { get; set; }
        public string Remarks { get; set; }
        public string UsernameSupervisorVerify { get; set; }
        public string DatetimeVerify { get; set; }
        public System.Guid Guid { get; set; }
        public Nullable<System.Guid> MasterRunResourceDaily_Guid { get; set; }
        public Nullable<System.Guid> MasterCustomerLocation_InternalDepartment_Guid { get; set; }
        public string ClientHostNameScan { get; set; }
        public Nullable<System.Guid> MasterCommodity_Guid { get; set; }
        public Nullable<System.DateTime> DatetimeSupervisorVerify { get; set; }
        public Nullable<bool> FlagNonDelivery { get; set; }
        public Nullable<System.Guid> CusLocPGuid { get; set; }
        public Nullable<System.Guid> SitePickUpGuid { get; set; }
        public Nullable<System.Guid> CusLocDGuid { get; set; }
        public Nullable<System.Guid> SiteDeliveryGuid { get; set; }
        public string ItemName { get; set; }
        public Nullable<bool> FlagItemPartial { get; set; }
        public Nullable<bool> FlagNotAllowReturn { get; set; }
    }
}
