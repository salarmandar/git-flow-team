

using Bgt.Ocean.Models.MasterRoute;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.MasterRouteService
{
    public class GetMasterRouteDeliveryLegRequest
    {
        public IEnumerable<Guid> MasterJobGuids  { get; set; }
    }

    public class GetMasterRouteDeliveryLegResponse : BaseResponse
    {
        public IEnumerable<MasterRouteCopyDeliveryView> MasterRouteDeliveryLegList { get; set; }
    }
}
