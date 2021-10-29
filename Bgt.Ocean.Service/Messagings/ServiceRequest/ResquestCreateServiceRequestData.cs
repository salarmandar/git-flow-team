
namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{
    public class ResquestCreateServiceRequestData
    {
        public string masterCountryGuid { get; set; }
        public string masterCountryAbb { get; set; }
        public string atmGuid { get; set; }
        public string atmId { get; set; }
        public string machineServiceTypeGuid { get; set; }
        public string machineServiceTypeName { get; set; }
        public string serviceJobTypeGuid { get; set; }
        public string serviceJobTypeAbb { get; set; }
        public string openSourceGuid { get; set; }
        public string openSourceId { get; set; }
        public string problemGuid { get; set; }
        public string problemId { get; set; }
        public string contactName { get; set; }
        public string contactPhone { get; set; }
        public string customerReferenceNumber { get; set; }
        public string reportedIncidentDescription { get; set; }
        public string createdUser { get; set; }
        public string notifiedDatetime { get; set; }
        public string downedDateTime { get; set; }
        public string servicedDateTime { get; set; }
    }
}
