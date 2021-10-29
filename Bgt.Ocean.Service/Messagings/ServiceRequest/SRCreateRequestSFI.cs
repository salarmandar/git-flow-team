using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{
    public class SRCreateRequestSFI
    {
        [MaxLength(50)]
        public string FileReference { get; set; }
        [MaxLength(50)]
        public string NetworkID { get; set; }
        [MaxLength(200)]
        public string CustomerCommentIN { get; set; }
    }
}
