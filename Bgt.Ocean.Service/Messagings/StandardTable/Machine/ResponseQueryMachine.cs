using Bgt.Ocean.Service.Messagings.StandardTable;
using System.Collections.Generic;

namespace Bgt.Ocean.Services.Messagings.StandardTable.Machine
{
    public class ResponseQueryMachine : BaseResponse
    {
        public List<ResponseQueryMachine_Main> result { get; set; } = new List<ResponseQueryMachine_Main>();
    }

    public class ResponseQueryMachine_Main : BaseResponseQuery
    {
        public string atmId { get; set; }
        public string atmName { get; set; }
        public string atmType { get; set; }
        public string customerName { get; set; }
        public string branchName { get; set; }
        public string state { get; set; }
        public string address { get; set; }
        public string customerId { get; set; }

        public ResponseQueryMachine_Timezone timeZoneDetail { get; set; } = new ResponseQueryMachine_Timezone();
        public List<ResponseQueryMachine_ServiceHour> serviceHourDetail { get; set; } = new List<ResponseQueryMachine_ServiceHour>();
        public ResponseQueryMachine_SLA slaDetail { get; set; } = new ResponseQueryMachine_SLA();

        /// <summary>
        /// True = Inactive, False = Active
        /// </summary>
        public bool flagDisable { get; set; } = false;

        /// <summary>
        /// True = Suspension, False = Available
        /// </summary>
        public bool flagSuspension { get; set; } = false;
    }

    public class ResponseQueryMachine_ServiceHour
    {
        public string lobAbb { get; set; }
        public string lobName { get; set; }
        public string serviceTypeAbb { get; set; }
        public string serviceTypeName { get; set; }
        public string dayOfWeek { get; set; }
        public string startedTime { get; set; }
        public string closedTime { get; set; }
    }

    public class ResponseQueryMachine_Timezone
    {
        public string countryTimeZoneId { get; set; }
        public string timeZoneId { get; set; }
        public string timeZoneIdentifier { get; set; }
        public string timeZoneStandardName { get; set; }
        public string timeZoneDisplayName { get; set; }
    }

    public class ResponseQueryMachine_SLA
    {
        /// <summary>
        /// Time in minute
        /// </summary>
        public string flmSlaTime { get; set; }

        /// <summary>
        /// Time in minute
        /// </summary>
        public string eCashSlaTime { get; set; }

        /// <summary>
        /// Time in minute
        /// </summary>
        public string compuSafeSlaTime { get; set; }
    }
}