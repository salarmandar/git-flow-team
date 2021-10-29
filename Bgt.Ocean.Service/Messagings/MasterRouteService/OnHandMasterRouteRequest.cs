using System;

namespace Bgt.Ocean.Service.Messagings.MasterRouteService
{
    public class OnHandMasterRouteRequest
    {
        public Guid? DailyRunGuid { get; set; }
        public DateTime WorkDate { get; set; }
        public Guid SiteGuid { get; set; }
    }
}
