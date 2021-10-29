using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Models.MasterRoute;
using System;
using System.Collections.Generic;
using static Bgt.Ocean.Infrastructure.Util.EnumMasterRoute;

namespace Bgt.Ocean.Service.Messagings.MasterRouteService
{
    public class UpdateJobOrderRequest : RequestBase
    {
        /// <summary>
        /// true : IF u wanna update job order all route detail in template
        /// </summary>
        public bool FlagInRoue { get; set; }
        /// <summary>
        /// not required,can get from session
        /// </summary>
        public Guid? LanguageGuid { get; set; } = ApiSession.UserLanguage_Guid;
        /// <summary>
        /// IF u wanna update job order all route detail in template
        /// </summary>
        public List<Guid> RouteGuids { get; set; }
        /// <summary>
        /// update some route detail 
        /// </summary>
        public List<JobDetailOrder> JobDetails { get; set; } = new List<JobDetailOrder>();
        public bool IsUpdate { get; set; }

        public DateTime? DateTimeModify { get; set; }
    }

    public class JobDetailOrder
    {
        public Guid routeGuid { get; set; }
        public Guid? routeGroupDetailGuid { get; set; }
    }
    public class MassUpdateDataJobOrderViewRequest : RequestBase
    {
        public EnumMasterRouteLogCategory SystemCategoryID { get; set; }
        public IEnumerable<MasterrouteView> Masterroute { get; set; }
    }
}
