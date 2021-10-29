using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.RunControlService
{
    public class ValidateCrewOnPortalRequest
    {
        [Required]
        public string crewId { get; set; }

        /// <summary>
        /// Required date format (MM/dd/yyyy Ex. 12/31/2018)
        /// </summary>
        [Required]
        public string runDate { get; set; }

        [Required]
        public string siteCode { get; set; }
    }
}
