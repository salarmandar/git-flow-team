using Bgt.Ocean.Models.BaseModel;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.RouteOptimization
{
    public class DailyRouteRequest : PagingBase
    {
        // For case view
        public Guid? RequestGuid { get; set; }
        public Guid SiteGuid { get; set; }
        public DateTime WorkDate { get; set; }

        public bool FlagShowAllOptStatus { get; set; }

        public bool? FlagShowAllRunStatus { get; set; }
    }

    public class DailyRouteDetailRequest
    {
        public Guid SiteGuid { get; set; }       
        public DateTime WorkDate { get; set; }        
        public Guid RequestTypeGuid { get; set; }
        public IEnumerable<Guid> DailyRouteGuidList { get; set; }
        /// <summary>
        /// keep data JoblegGuid
        /// </summary>
        public IEnumerable<Guid> UnassignedJobGuidList { get; set; }
    }
}
