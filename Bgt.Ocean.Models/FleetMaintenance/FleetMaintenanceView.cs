
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models.BaseModel;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.FleetMaintenance
{
    //get maintenance
    public class FleetMaintenanceFilter : PagingBase
    {
        public Guid? SiteGuid { get; set; }
        public Guid? RunGuid { get; set; }
        public bool FlagAllDate { get; set; }
        public EnumMaintenanceStatus MaintenanceStatusID { get; set; }
        public string strOpen_DateServiceFrom { get; set; }
        public string strOpen_DateServiceTo { get; set; }


        public DateTime Open_DateServiceFrom { get { return strOpen_DateServiceFrom.ChangeFromStrDateToDateTime(); } }
        public DateTime Open_DateServiceTo { get { return strOpen_DateServiceTo.ChangeFromStrDateToDateTime(); } }
    }
    //get grid
    public class FleetMaintenanceView
    {
        public Guid? RunGuid { get; set; }
        public Guid? MaintenanceGuid { get; set; }
        public int MaintenanceNo { get; set; }
        public EnumMaintenanceStatus MaintenanceStatusID { get; set; }
        public string MaintenanceStatusName { get; set; }
        public string DocRef { get; set; }        
        public string OdometerBefore { get; set; }
        public string OdometerAfter { get; set; }
        public decimal? CostEstimate { get; set; }
        public decimal? CostActual { get; set; }
        public string CurrencyCostEstimate { get; set; }
        public string CurrencyCostActual { get; set; }
        public string VendorName { get; set; }
        public string Remarks { get; set; }

        public string UserCreated { get; set; }
  
        public string UserModified { get; set; }  
        
        public DateTime? ReceivedDateTime { get; set; }
        public DateTime? DateTimeCreated { get; set; }
        public DateTime? DateTimeModified { get; set; }
        public DateTime? LastDateTimeModified { get; set; }
        public string ServiceDateRange { get; set; }

    }
    //get edit
    public class MaintenanceCategoryDetailView
    {
        public Guid? MaintenanceDetailGuid { get; set; }
        public Guid? MasterMaintenanceCategory_Guid { get; set; }
        public string MaintenanceCategoryName { get; set; }
        public string Description { get; set; }
        public int PartQty { get; set; }
        public decimal UnitPrice { get; set; }
        public EnumDiscountType DiscountType { get; set; }
        public double Discount { get; set; }
        public string CurrencyText { get; set; }
        public double Total { get; set; }

        public EnumState ItemState { get; set; }
        public EnumMaintenanceState State { get; set; }

        public bool Validate()
        {
            if (EnumDiscountType.Undefined == DiscountType)
            {
                return true;
            }
            else
            {
                double net = (PartQty * Convert.ToDouble(UnitPrice));
                var total = (EnumDiscountType.Currency == DiscountType ? (net - Discount) : net - (net * (Discount / 100)));
                return Total == total;
            }
        }
    }

    //insert/update
    public class FleetMaintenanceModel
    {
        public Guid? MaintenanceGuid { get; set; }
        public int MaintenanceID { get; set; }
        public Guid? MasterSite_Guid { get; set; }
        public Guid? MasterRunResource_Guid { get; set; }
        public Guid? MasterVendor_Guid { get; set; }
        public string VendorName { get; set; }

        public EnumMaintenanceStatus MaintenanceStatusID { get; set; }
        public string MaintenanceStatusName { get; set; }
        public string Open_OdoMeter { get; set; }

        public string strOpen_DateServiceFrom { get; set; }
        public string strOpen_TimeServiceFrom { get; set; }
        public string strOpen_DateServiceTo { get; set; }
        public string strOpen_TimeServiceTo { get; set; }
        public string strClose_DateService { get; set; }
        public string strClose_TimeService { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Cost_Actual { get; set; }
        public decimal? Cost_Estimate { get; set; }

        public Guid? CurrencyGuid { get; set; }
        public string CurrencyAbb { get; set; }

        public string DocumentRef_No { get; set; }
        public string Remarks { get; set; }
        public string Close_OdoMeter { get; set; }

        public bool FlagDisable { get; set; }
        public double? DiscountValue { get; set; }
        public EnumDiscountType Discount_Type { get; set; }
        public string VehicleNumber { get; set; } //required

        //internal use
        public EnumMaintenanceState State { get { return this.MaintenanceStatusID == EnumMaintenanceStatus.Closed ? EnumMaintenanceState.Actual : EnumMaintenanceState.Estimate; } }
        public EnumRunResourceStatus RunResourceStatus { get { return MaintenanceStatusID == EnumMaintenanceStatus.InProgress ? EnumRunResourceStatus.InService : EnumRunResourceStatus.Active; } }
    }
    public class FleetMaintenanceCategoryModel
    {
        public IEnumerable<MaintenanceCategoryDetailView> MaintenanceCategoryDetailList { get; set; }
    }
}
