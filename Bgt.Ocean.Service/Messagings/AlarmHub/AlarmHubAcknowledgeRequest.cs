using System;

namespace Bgt.Ocean.Service.Messagings.AlarmHub
{
    public class AlarmHubAcknowledgeRequest
    {
        public Guid MasterDailyRunAlarmGuid { get; set; }
        public string UserAcknowledged { get; set; }
        public DateTime DateTimeAcknowledged { get; set; }
    }
}
