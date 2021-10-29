using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.Messagings.RunControlService
{
    public class TruckToTruckRequest
    {
        public Guid OldDailyRunGuid { get; set; }
        public Guid NewDailyRunGuid { get; set; }
        public List<Guid> LegGuidList { get; set; }
        public Guid SenderGuid { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverSignature { get; set; }
        public string UserCreated { get; set; }

        /* Required! Format "yyyy/MM/dd HH:mm:ss" */
        public DateTime DatetimeCreated { get; set; } 

        //For Truck Limit
        public string TruckLimitReasonName { get; set; }
        public string TruckLimitComment { get; set; }
    }
}
