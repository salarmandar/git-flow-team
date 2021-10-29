using System;

namespace Bgt.Ocean.Service.Messagings.MonitorService
{
    public class PodMonitorResponse
    {
        public Guid LogPod_Guid { get; set; }
        public string PodTypeName { get; set; }
        public string LocationName { get; set; }
        public string StrSuccessStatus { get; set; }
        public string Destination { get; set; }
        public string JobNo { get; set; }
        public string ActionType { get; set; }
        public string ReportFileName { get; set; }
        public string StrDatetimeCreated { get; set; }
        public bool SuccessStatus { get; set; }
        public DateTime DatetimeCreated { get; set; }
    }

    public class PodMonitorErrorResponse
    {
        public Guid ErrorPod_Guid { get; set; }
        public Guid LogPod_Guid { get; set; }
        public string ErrorLogDetail { get; set; }
    }
}
