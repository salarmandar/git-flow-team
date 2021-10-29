namespace Bgt.Ocean.Models.StandardTable
{
    public class MachineView
    {
        public string guid { get; set; }
        public string countryAbb { get; set; }
        public string atmId { get; set; }
        public string atmName { get; set; }
        public string atmType { get; set; }
        public string customerName { get; set; }
        public string branchName { get; set; }
        public string state { get; set; }
        public string address { get; set; }
        public string customerId { get; set; }
        public string createdDatetime { get; set; }
        public string createdUser { get; set; }
        public string modifiedDatetime { get; set; }
        public string modifiedUser { get; set; }
        public bool flagDisable { get; set; } = false;
        public bool flagSuspension { get; set; } = false;
        public string countryTimeZoneId { get; set; }
        public string timeZoneId { get; set; }
        public string timeZoneIdentifier { get; set; }
        public string timeZoneStandardName { get; set; }
        public string timeZoneDisplayName { get; set; }
        public string flmSlaTime { get; set; }
        public string eCashSlaTime { get; set; }
        public string compuSafeSlaTime { get; set; }
    }

    public class MachineView_Request
    {
        public string countryAbb { get; set; }
        public string createdDatetimeFrom { get; set; }
        public string createdDatetimeTo { get; set; }
    }

    public class MachineView_ServiceHour
    {
        public string lobAbb { get; set; }
        public string lobName { get; set; }
        public string serviceTypeAbb { get; set; }
        public string serviceTypeName { get; set; }
        public string dayOfWeek { get; set; }
        public string startedTime { get; set; }
        public string closedTime { get; set; }
    }
}
