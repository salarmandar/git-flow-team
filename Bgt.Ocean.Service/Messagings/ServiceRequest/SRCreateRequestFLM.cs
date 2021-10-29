using System;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{

    public class SRCreateRequestFLM : SRCreateOptionalRequest
    {
        // This model contains mandatory property for create SR.
        [Required]
        public Guid MasterCountryGuid { get; set; }
        [Required]
        public Guid MachineGuid { get; set; }
        [Required]
        public Guid OpenSourceGuid { get; set; }
        [Required]
        public Guid MachineServiceTypeGuid { get; set; }
        /// <summary>
        /// Call type
        /// </summary>
        [Required]
        public Guid ProblemGuid { get; set; }
        [Required]
        public DateTime DateTimeNotified { get; set; }
        [Required]
        public DateTime DateTimeDown { get; set; }
        [Required]
        public DateTime DateTimeServiceDate { get; set; }
    }

    public class SRCreateRequestFLMWithSFI : SRCreateRequestFLM
    {
        public SRCreateRequestSFI SFIModel { get; set; }
    }

    #region Optional Field Model

    public abstract class SRCreateOptionalRequest
    {
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string CustomerReferenceNumber { get; set; }
        public string ReportedIncidentDescription { get; set; }
    }

    #endregion
}
