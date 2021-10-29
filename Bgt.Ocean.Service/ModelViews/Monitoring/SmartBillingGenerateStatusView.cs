using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.ModelViews.Monitoring
{
    public class SmartBillingGenerateStatusView
    {
        public Guid SmartBillingStatusGuid { get; set; }
        public bool FlagGenerateStatus { get; set; }
        public string FileGenerating_Status { get; set; }
        public bool FlagEmailSendingStatus { get; set; }
        public string EmailSending_Status { get; set; }
        public bool FlagDroppingStatus { get; set; }
        public string FileDropping_Status { get; set; }
        public string ReportName { get; set; }
        public string Email { get; set; }
        public string DropFilePath { get; set; }
        public DateTime Date { get; set; }
       
    }

    public class SmartBillingScheduleView
    {
        public string TimeConfig { get; set; }
        public string DaysConfig { get; set; }
    }

    public class SmartBillingScheduleErrorMsgView
    {
        public string AutoGenErrorMsg { get; set; }
        public string SendEmailErrorMsg { get; set; }
        public string DroppingErrorMsg { get; set; }
    }
}
