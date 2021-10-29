using Bgt.Ocean.Models.MasterRoute;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.MasterRouteService
{
    public class MasterRouteCopyJobsRequest
    {
        //SOURCE: PARAMS
        public Guid Src_RouteGuid { get; set; }

        //SOURCE: UDT 
        public List<MasterRouteOverNightJobs> OverNightJobs { get; set; }
        public List<MasterRouteSeletedJobs> SeletedJobs { get; set; }


        //DESTINATION: PARAMS
        public Guid Dst_DayOfWeekGuid { get; set; }
        public Guid? Dst_RouteGroupDetailGuid { get; set; }
        public Guid Dst_RouteGuid { get; set; }
        public bool FlagAllTemplate { get; set; }


        //USER CONFIG 
        public Guid LanguageGuid { get; set; }
        public Guid UserGuid { get; set; }
        public DateTime ClientDate { get; set; }


    }



    public class MasterRouteCopyJobsResponse 
    {
        public IEnumerable<MasterRouteCopyJobsResult> CantCopyJobs { get; set; }
        public SystemMessageView Message { get; set; }
    }
}
