using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.RunControlService
{
    public class ValidateTruckLiabilityLimitRequest
    {
        public Guid OldDailyRunGuid { get; set; }
        public Guid NewDailyRunGuid { get; set; }
        public List<Guid> LegGuidList { get; set; }
    }
}
