using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.RunControlService
{
    public class DolphinCreateJobsRequest : BaseDolphinAuthen
    {
        public string action { get; set; }        
        public string registerId { get; set; }
        public IEnumerable<Guid> jobGuid { get; set; }
        public string clientDateTime { get; set; }
        public string routeName { get; set; }
        public string userAction { get; set; }
        public Guid runDailyGuid { get; set; }
    }

    public class BaseDolphinAuthen
    {
        public DolphinAuthen authen { get; set; }
    }
}
