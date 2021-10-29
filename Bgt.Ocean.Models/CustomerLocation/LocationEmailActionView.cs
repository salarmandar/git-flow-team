using System;

namespace Bgt.Ocean.Models.CustomerLocation
{
    public class LocationEmailActionView
    {
        public Guid? Guid { get; set; }
        public Guid? CustomerGuid { get; set; }
        public string CustomerName { get; set; }
        public Guid? LocationGuid { get; set; }
        public string LocationName { get; set; }
        public Guid EmailActionGuid { get; set; }
        public string EmailActionName { get; set; }
        public string Email { get; set; }
        public Guid? EmailTemplateGuid { get; set; }
        public string EmailTemplateName { get; set; }
    }
    public class SaveChangeCustomerRequest
    {
        public Guid LocationGuid { get; set; }
        public Guid CustomerGuid { get; set; }
    }
}
