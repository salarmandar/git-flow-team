
namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{
    public class SRCreateRequestTechMeet : SRCreateRequestFLM
    {
        public string TechMeetName { get; set; }
        public string TechMeetPhone { get; set; }
        public string TechMeetCompanyName { get; set; }
        public string TechMeetReason { get; set; }
        public bool TechMeetSecurityRequired { get; set; }
    }

    public class SRCreateRequestTechMeetWithSFI : SRCreateRequestTechMeet
    {
        public SRCreateRequestSFI SFIModel { get; set; }
    }
}
