using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models.BaseModel;
using Bgt.Ocean.Service.ModelViews.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Bgt.Ocean.Infrastructure.Util.EnumMonitoring;

namespace Bgt.Ocean.Service.Messagings.MonitorService
{
    public class SmartBillingMonitorRequest : PagingBase
    {
        public Guid SiteGuid { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public DateTime _fromDate { get { return FromDate.ChangeFromStrDateToDateTime(); } }
        public DateTime _toDate { get { return ToDate.ChangeFromStrDateToDateTime(); } }
    }

    public class SmartBillingConfigRequest : SmartBillingConfigView
    {
        public string ScheduleTime { get; set; }
        public DateTime _schduleTime { get { return ScheduleTime.ChangeFromStrTimeToDateTime(); } }

        public RequestBase RequestBase { get; set; }
    }

    public class SmartBillingErrorMsgRequest
    {
        public Guid AutoGenGuid { get; set; }
        public EnumErrorMsgType ErrorMsgTypeID { get; set; }
    }
}
