using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Service.ModelViews.MasterRoute;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.MasterRouteService
{
    public class ManualSortSeqIndexRequest : RequestBase
    {      
        public Guid MasterRouteGudGuid { get; set; }
        public Guid? Langquage
        {
            get
            {
                return ApiSession.UserLanguage_Guid;
            }

        }
        public Guid? RouteGroupDetailGuid { get; set; }
        public int TargetJobOrder { get; set; }
        public int TargetSeqindex { get; set; }
        public List<MasterRouteServiceStopLegView> SourceJobLegs { get; set; }

    }
    public class ManualSortSeqIndexResponse : BaseResponse
    {

    }


}
