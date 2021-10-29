using System;

namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{
    public class SRCreateResponse : BaseResponse
    {
        public string TicketNumber { get; set; }
        public Guid? TicketGuid { get; set; }
        public Guid? JobHeaderGuid { get; set; }
    }
}
