using System;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{
    public class SRRescheduleRequest
    {
        [Required]
        public Guid TicketGuid { get; set; }
        public string ReasonName { get; set; }
        public string ReportedIncidentDescription { get; set; }
        [Required]
        public DateTime RescheduleDateTime { get; set; }

    }
}
