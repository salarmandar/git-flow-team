using Bgt.Ocean.Service.ModelViews.Systems;
using System;

namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{

    public class CreateServiceRequestResponse
    {
        public SystemMessageView SystemMessageView { get; set; } = new SystemMessageView();
        public Guid? ServiceRequestGuid { get; set; }
        public Guid? JobHeaderGuid { get; set; }
        public string TicketNumber { get; set; }

    }
}
