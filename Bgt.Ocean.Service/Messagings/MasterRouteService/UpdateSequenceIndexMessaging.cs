using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.MasterRouteService
{
    public class UpdateSequenceIndexRequest:RequestBase
    {
        public Guid MasterRouteGudGuid { get; set; }
        public Guid? Langquage { get; set; }
        public List<Guid?> RouteGroupDetailGuid { get; set; } = new List<Guid?>();
    }

    public class UpdateSequenceIndexResponse : BaseResponse
    {
    

    }
}
