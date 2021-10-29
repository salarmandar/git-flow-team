
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models.FleetMaintenance;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.FleetMaintenance
{
    /// <summary>
    /// Main grid filter
    /// </summary>
    public class FleetMaintenanceFilterRequest
    {
        public FleetMaintenanceFilter Filters { get; set; }
    }
    public class FleetMaintenanceFilterResponse : BaseResponse
    {
        public IEnumerable<FleetMaintenanceView> MaintenanceList { get; set; } = new List<FleetMaintenanceView>();
    }

    /// <summary>
    /// Maintenace Detail By Id
    /// </summary>
    public class FleetMaintenanceDetailRequest
    {
        public Guid? MaintenanceGuid { get; set; }
    }
    public class FleetMaintenanceDetailBaseResponse : BaseResponse
    {
        public FleetMaintenanceModel MaintenanceModel { get; set; }
    }

    /// <summary>
    /// Maintenace Category
    /// </summary>
    public class FleetMaintenanceCategoryDetailRequest
    {
        public Guid? MaintenanceGuid { get; set; }
        public EnumMaintenanceState State { get; set; }
    }
    public class FleetMaintenanceCategoryDetailResponse : BaseResponse
    {
        public FleetMaintenanceCategoryModel MaintenanceCategoryModel { get; set; }
    }

    /// <summary>
    /// Maintenace Category Items
    /// </summary>
    public class FleetMaintenanceCategoryItemsRequest
    {
        public Guid? BrinksCompanyGuid { get; set; }
        public Guid? MaintenanceCategoryGuid { get; set; }
        public string CurrencyText { get; set; }
    }
    public class FleetMaintenanceCategoryItemsResponse : BaseResponse
    {
      
    }

    /// <summary>
    /// Create/Update Maintenance
    /// </summary>
    public class FleetMaintenanceRequest
    {
        public FleetMaintenanceModel MaintenanceModel { get; set; } = new FleetMaintenanceModel();
        public FleetMaintenanceCategoryModel MaintenanceCategoryModel { get; set; } = new FleetMaintenanceCategoryModel();
    }
    public class FleetMaintenanceResponse : BaseResponse
    {
    }

    public class FleetMaintenanceCancelRequest {
    }
    public class FleetMaintenanceCancelResponse : BaseResponse
    {
        public Guid? MaintenanceGuid { get; set; }
    }



}
