using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.AdhocService
{
    public class DetailDestinationForMultibranchReponseJobP
    {
        public string SiteGuid { get; set; }
        public string SiteName { get; set; }

        public List<ModelDestinationResponse> ComboDestination { get; set; }
    }


    public class ModelDestination
    {
        public Guid SiteGuid { get; set; }
        public string SiteName { get; set; }
        public System.Guid InternalDepartmentGuid { get; set; }
        public string InterDepartmentName { get; set; }

    }

    public class ModelDestinationResponse
    {
        public Guid id { get; set; }

        public string text { get; set; }

        public int onwardTypeId { get; set; }

        public bool flagDefaultOnward { get; set; }
    }


    //public class CustomerLocation_InternalDepartmentView
    //{
    //    public System.Guid Guid { get; set; }
    //    public string InterDepartmentName { get; set; }
    //    public bool FlagDefaultOnward { get; set; }
    //}
}
