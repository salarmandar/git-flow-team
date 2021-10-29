using Bgt.Ocean.Models.SiteNetwork;
using Bgt.Ocean.Service.Messagings;
using Bgt.Ocean.Service.ModelViews.AuditLog;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.ModelViews.SiteNetWork
{
    public class SiteNetworkResponse
    {
        public PopupSiteDestinationView PopupSiteNetworkData { get; set; }
    }

    public class SiteNetWorkRequest
    {
        /// <summary>
        /// Origin of site guid.
        /// </summary>
        [Required]
        public Guid SiteGuid { get; set; }

        [Required]
        public int ServiceJobTypeID { get; set; }

        public Guid? OriginCustomerLocation_Guid { get; set; }

    }

    public class SiteNetworkViewResponse : SiteNetworkMemberView
    {
        public SystemMessageView Message { get; set; }
    }

    public class SiteNetworkViewRequest : SiteNetworkMemberView
    {
        public RequestBase UserData { get; set; } = new RequestBase();
    }

    public class SiteNetworkAuditLogView : AuditLogBase
    {
        public Guid? MasterSiteNetworkGuid { get; set; }
    }
}
