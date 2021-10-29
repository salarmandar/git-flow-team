using System;

namespace Bgt.Ocean.Service.Messagings.MonitorService
{
    public class PodMonitorRequest : RequestBase
    {
        public Guid SiteGuid { get; set; }
        public string StrDateFrom { get; set; }
        public string StrDateTo { get; set; }
        public int Page { get; set; }
        public int Rows { get; set; }
        public string DateFormat { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        //Filter Column
        public string PodTypeName { get; set; }
        public string LocationName { get; set; }
        public string StrSuccessStatus { get; set; }
        public string Destination { get; set; }
        public string JobNo { get; set; }
        public string ActionType { get; set; }
        public string ReportFileName { get; set; }
        public string StrDatetimeCreated { get; set; }
        public DateTime? DatetimeCreated { get; set; }
        public bool SuccessStatus { get; set; }

        //Sorting
        public string SortBy { get; set; } = "UniversalDatetimeCreated";
        public string SortWith { get; set; } = "desc";

    }
}
