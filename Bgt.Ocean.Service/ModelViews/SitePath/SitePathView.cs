using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.ModelViews.AuditLog;
using Bgt.Ocean.Service.ModelViews.Systems;
using Bgt.Ocean.Models.StandardTable;
using System;

namespace Bgt.Ocean.Service.ModelViews.SitePath
{
    public class SitePathViewResponse : SitePathView
    {
        public SystemMessageView Message { get; set; }
    }

    public class SitePathViewRequest : SitePathView
    {
        public RequestBase UserData { get; set; } = new RequestBase();
    }

    public class SitePathAuditLogView : AuditLogBase
    {
        public Guid? MasterSitePathGuid { get; set; }
    }

    public class SitePathDropdownViewRequest
    {
        public Guid? pickupLocationGuid { get; set; }

        public Guid? deliveryLocationGuid { get; set; }

        public Guid? pickupSiteGuid { get; set; }

        public Guid? deliverySiteGuid { get; set; }

        public int? jobType { get; set; }
    }

    public class SitePathDropdownViewResponse
    {
        public string SitePath_Name { get; set; }

        public Guid? SitePath_Guid { get; set; }
    }
}
