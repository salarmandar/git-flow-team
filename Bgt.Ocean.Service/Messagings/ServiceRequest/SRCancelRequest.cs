using System;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{
    public class SRCancelRequest
    {
        [Required]
        public Guid TicketGuid { get; set; }
        [Required]
        public Guid MasterReasonTypeGuid { get; set; }
        public string CancelDescription { get; set; }
    }
}
