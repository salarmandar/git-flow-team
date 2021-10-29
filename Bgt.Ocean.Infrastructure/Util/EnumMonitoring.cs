using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Infrastructure.Util
{
    public class EnumMonitoring
    {
        public static class SmartBillingGenerateStatus
        {
            public const string Success = "Success";
            public const string Failed = "Failed";
            public const string NA = "N/A";
        }

        public enum EnumErrorMsgType
        {
            [EnumDescription("Auto Generate")]
            AutoGen = 1,
            [EnumDescription("Email Sending")]
            EmailSending = 2,
            [EnumDescription("File Dropping")]
            FileDropping = 3,
        }
    }
}
