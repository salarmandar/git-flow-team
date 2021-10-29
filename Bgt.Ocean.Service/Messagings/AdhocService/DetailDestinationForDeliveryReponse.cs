using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.AdhocService
{
    public class DetailDestinationForDeliveryReponse
    {
        public Guid SiteGuid { get; set; }
        public string SiteName { get; set; }
        public bool FlagInterBr { get; set; }
        public bool FlagDefaultOnward { get; set; }
        public bool FlagDisableInterBr { get; set; }
        public bool FlagNoDestination { get; set; }
        //public IEnumerable<CustomerLocation_InternalDepartmentView> DestinationList { get; set; } = new List<CustomerLocation_InternalDepartmentView>();
        public List<CustomerLocation_InternalDepartmentView> ComboDestination { get; set; } = new List<CustomerLocation_InternalDepartmentView>();
    }

    //public class CustomerLocation_InternalDepartmentView
    //{
    //    public System.Guid Guid { get; set; }
    //    public string InterDepartmentName { get; set; }
    //    public bool FlagDefaultOnward { get; set; }
    //}

    public class CustomerLocation_InternalDepartmentView
    {
        public System.Guid id { get; set; }
        public string text { get; set; }
        public int onwardTypeId { get; set; }
        public bool flagDefaultOnward { get; set; }
    }
}
