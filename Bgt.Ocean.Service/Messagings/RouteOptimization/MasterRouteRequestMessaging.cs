using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.RouteOptimization
{
    public class MasterRouteRequestResponse : BaseResponse {
        public bool IsValid { get; set; }

        public IEnumerable<Guid> InValid_DailyRoute_Guid { get; set; }
        public IEnumerable<Guid> InValid_UnassignLeg_Guid { get; set; }
    }
}
