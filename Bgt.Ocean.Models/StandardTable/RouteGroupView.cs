using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Models.StandardTable
{
    #region ## Request model
    public class RouteGroupDetailRequest
    {
        [Required]
        public Guid? GroupDetailGuid { get; set; }

        [Required]
        public string strWorkDate { get; set; }
        [Required]
        public Guid? SiteGuid { get; set; }
        public Guid? RunGuid { get; set; }
        [Required]
        public Guid? JobTypeGuid { get; set; }

        public string LegName { get; set; }

        public string DateTimeFormat { get; set; }
    }
    public class RouteGroupView_Request
    {
        public string countryAbb { get; set; }
        public string createdDatetimeFrom { get; set; }
        public string createdDatetimeTo { get; set; }
    }
    #endregion

   #region ## View Model
    public class RouteGroupView
    {
        public string guid { get; set; }
        public string countryAbb { get; set; }

        public string brinksSite { get; set; }
        public string routeGroupDetailName { get; set; }
        public string routeGroupName { get; set; }

        public string createdDatetime { get; set; }
        public string createdUser { get; set; }
        public string modifiedDatetime { get; set; }
        public string modifiedUser { get; set; }
    }


    public class RouteGroupAndGroupDetailView
    {
        public IEnumerable<RouteGroupDetailView> RouteGroup { get; set; } = new List<RouteGroupDetailView>();
        //public Guid RouteGuid { get; set; }
        //public string RouteName { get; set; }
        public IEnumerable<RouteGroupDetailView> RouteGrupDetail { get; set; } = new List<RouteGroupDetailView>();
        //public RouteGroupDetailView RouteGrupDetail { get; set; }

    }

    public class RouteGroupDetailView
    {
        public Guid RouteGuid { get; set; }
        public string RouteName { get; set; }

        public Guid DetailGuid { get; set; }
        public string DetailName { get; set; }
        public Guid MasterSiteGuid { get; set; }
    }
    #endregion

}
