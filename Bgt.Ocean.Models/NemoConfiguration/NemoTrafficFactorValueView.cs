using System;

namespace Bgt.Ocean.Models.NemoConfiguration
{
    public class NemoTrafficFactorValueView
    {
        public Guid Guid { get; set; }
        public Guid? MasterCountry_Guid { get; set; }
        public Guid? MasterSite_Guid { get; set; }
        public Guid? DayofWeek_Guid { get; set; }
        public string DayOfWeekName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string TrafficMultiplier { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DatetimeCreated { get; set; }
        public DateTimeOffset? UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public DateTime? DatetimeModified { get; set; }
        public DateTimeOffset? UniversalDatetimeModified { get; set; }

    }
}
