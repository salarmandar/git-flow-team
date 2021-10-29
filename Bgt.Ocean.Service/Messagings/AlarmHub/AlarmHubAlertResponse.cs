using System;

namespace Bgt.Ocean.Service.Messagings.AlarmHub
{
    public class AlarmHubTriggeredResponse
    {
        public Guid Guid { get; set; }
        public Guid? MasterDailyRunResource_Guid { get; set; }
        public Guid? MasterSite_Guid { get; set; }
        public string EmployeeName { get; set; }
        public string Phone { get; set; }
        public string SiteName { get; set; }
        public string RouteName { get; set; }
        public string RunNo { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string LocationName { get; set; }
        public bool FlagAllowAcknowledged { get; set; }        
        public string UserCreated { get; set; }
        public DateTime DatetimeCreated { get; set; }
        public DateTimeOffset UniversalDatetimeCreated { get; set; }
    }
}
