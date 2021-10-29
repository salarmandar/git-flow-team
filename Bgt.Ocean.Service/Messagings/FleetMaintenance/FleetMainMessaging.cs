using Bgt.Ocean.Models.FleetMaintenance;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.FleetMaintenance
{
    public class FleetMainRequest 
    {
        public Guid? SiteGuid { get; set; }
    }

    public class FleetMainResponse : BaseResponse
    {
        public IEnumerable<FleetRunResourceView> RunResourceList { get; set; } = new List<FleetRunResourceView>();
    }
}
