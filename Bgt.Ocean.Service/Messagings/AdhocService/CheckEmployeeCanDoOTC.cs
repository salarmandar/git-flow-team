using System;

namespace Bgt.Ocean.Service.Messagings.AdhocService
{
    public class CheckEmployeeCanDoOTC
    {
        public Guid? MasterSite_Guid { get; set; }

        public Guid? DailyRunResource_Guid { get; set; }

        public Guid?[] CustomerLocation_Guids { get; set; }
    }

    
}
