using System;
using System.Collections.Generic;
using static Bgt.Ocean.Models.Consolidation.ConsolidationView;

namespace Bgt.Ocean.Service.Messagings.Consolidation
{
    public class ConMultiBranchCreateUpdateRequest : RequestBase
    {
        public List<ConSealView> ItemSeals { get; set; } = new List<ConSealView>();
        public List<ConCommodityView> ItemCommodity { get; set; } = new List<ConCommodityView>();

        public Guid MasterID_Guid { get; set; }
        public Guid? MasterSite_Guid { get; set; }
        public Guid? Destination_MasterSite_Guid { get; set; }
        public Guid? MasterCustomerLocation_Guid { get; set; }
        public Guid? MasterRouteGroup_Detail_Guid { get; set; }
        public Guid? MasterDailyRunResource_Guid { get; set; }
        public Guid? OnwardDestination_Guid { get; set; }
        public Guid? MasterSitePathHeader_Guid { get; set; }
        public DateTime WorkDate { get; set; }
        public string StrWorkDate { get; set; }
        public string FormatDate { get; set; }
        public string MasterID { get; set; }
        public string reasonName { get; set; } //use for update con
        public string remark { get; set; } //use for update con
        /// <summary>
        /// true: Click Seal (Sealed), false: Click Save (In-Process)
        /// </summary>
        public bool FlagSealed { get; set; } = false;
        public bool FlagUnsealed { get; set; }
    }

    public class ConMultiBranchEditModelRequest
    {
        public Guid MasterID_Guid { get; set; }
        public Guid MasterSite_Guid { get; set; }
    }

    public class ItemAvailableConRequest : RequestBase
    {
        public string FormatDate { get; set; }
        public string StrWorkDate { get; set; }
        public DateTime WorkDate { get; set; } //use for new
        public Guid SiteGuid { get; set; } //use for new
        public Guid? DestinationSiteGuid { get; set; } //use for new
        public Guid? SitePathGuid { get; set; }
        public Guid MasterID_Guid { get; set; } //use for edit
    }

    public class SitePathDDLRequest : RequestBase
    {
        public string FormatDate { get; set; }
        public string StrWorkDate { get; set; }
        public DateTime WorkDate { get; set; } //use for new
        public Guid SiteGuid { get; set; } //use for new
        public Guid DestinationSiteGuid { get; set; } //use for new
    }
}
