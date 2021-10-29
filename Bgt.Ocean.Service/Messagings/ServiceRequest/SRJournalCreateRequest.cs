using System;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{
    public class SRJournalCreateRequest
    {
        [Required]
        public Guid TicketGuid { get; set; }
        [Required]
        [MaxLength(200)]
        public string JournalDescription { get; set; }
    }
}
