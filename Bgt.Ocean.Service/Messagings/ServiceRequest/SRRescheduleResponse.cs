using System;

namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{
    public class SRRescheduleResponse : BaseResponse
    {
        public Guid NewTicketStatusGuid { get; set; }
        public Guid OldDailyRunGuid { get; set; }
        public DateTime OldWorkDate { get; set; }
    }
}
