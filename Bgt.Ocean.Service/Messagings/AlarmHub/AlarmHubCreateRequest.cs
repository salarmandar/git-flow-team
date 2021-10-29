using System;

namespace Bgt.Ocean.Service.Messagings.AlarmHub
{
    public class AlarmHubCreateRequest
    {
        public Guid MasterDailyRunResouceGuid { get; set; }
        public Guid MasterSiteGuid { get; set; }
        public string EmployeeName { get; set; }
        public string Phone { get; set; }
        public string SiteName { get; set; }
        public string RouteName { get; set; }
        public string RunNo { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string UserCreated { get; set; }
        public DateTime DateTimeAlert { get; set; }
        public Guid? EmployeeGuid { get; set; }
        public Guid? LocationGuid { get; set; }
    }
}
