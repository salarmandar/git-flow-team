
namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{
    public class SRCreateRequestSLM : SRCreateRequestFLM
    {        
    }

    public class SRCreateRequestSLMWithSFI : SRCreateRequestSLM
    {
        public SRCreateRequestSFI SFIModel { get; set; }
    }
}
