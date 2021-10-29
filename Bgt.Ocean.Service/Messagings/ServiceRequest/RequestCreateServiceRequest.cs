using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{
    public class RequestCreateServiceRequest
    {
        /// <summary>
        /// For all service type
        /// </summary>
        public RequestCreateServiceRequest_MachineDetail machineDetail { get; set; }

        /// <summary>
        /// For all service type
        /// </summary>
        public RequestCreateServiceRequest_ServiceRequestDetail serviceRequestDetail { get; set; }

        /// <summary>
        /// For all service type
        /// </summary>
        public RequestCreateServiceRequest_DatetimeDetail datetimeDetail { get; set; }

        /// <summary>
        /// For eCash service type (Optional)
        /// </summary>
        public List<RequestCreateServiceRequest_EcashDetail> eCashDetail { get; set; }

        /// <summary>
        /// For Tech meet service type (Optional)
        /// </summary>
        public RequestCreateServiceRequest_TechMeetDetail techMeetDetail { get; set; }
    }

    public class RequestCreateServiceRequest_MachineDetail
    {
        /// <summary>
        /// Country abbreviation (Ex. US, CA, MX)
        /// </summary>
        [Required]
        public string countryAbb { get; set; }
        
        [Required]
        public string atmId { get; set; }
        
        [Required]
        public string machineServiceTypeName { get; set; }
    }

    public class RequestCreateServiceRequest_ServiceRequestDetail
    {
        /// <summary>
        /// ATM Machine = FLM, ECASH, TM;
        /// CompuSafe Machine = FLM, F/SLM
        /// </summary>
        [Required]
        public string serviceJobTypeAbb { get; set; }
        
        /// <summary>
        /// Preferred '13'
        /// </summary>
        [Required]
        public string openSourceId { get; set; }
        
        [Required]
        public string problemId { get; set; }
        
        public string contactName { get; set; }
        public string contactPhone { get; set; }
        public string customerReferenceNumber { get; set; }
        public string reportedIncidentDescription { get; set; }
        /// <summary>
        /// Always be 'OceanWebApiUser'
        /// </summary>
        public string createdUser { get; set; }
    }

    public class RequestCreateServiceRequest_DatetimeDetail
    {
        /// <summary>
        /// Required format yyyy-MM-dd HH:mm (Ex. 2018-12-31 23:30) in UTC +0
        /// </summary>
        [Required]
        public string notifiedDatetime { get; set; }

        /// <summary>
        /// Required format yyyy-MM-dd HH:mm (Ex. 2018-12-31 23:30) in UTC +0
        /// </summary>
        [Required]
        public string downedDateTime { get; set; }

        /// <summary>
        /// Required format yyyy-MM-dd HH:mm (Ex. 2018-12-31 23:30) in UTC +0
        /// </summary>
        [Required]
        public string servicedDatetime { get; set; }
    }

    public class RequestCreateServiceRequest_EcashDetail
    {
        [Required]
        public string currency { get; set; }

        [Required]
        public string denomination { get; set; }

        [Required]
        public string amount { get; set; }
    }

    public class RequestCreateServiceRequest_TechMeetDetail
    {
        public string techMeetName { get; set; }
        public string techMeetPhone { get; set; }
        public string techMeetCompanyName { get; set; }
        public string techMeetReason { get; set; }
        public bool techMeetSecurityRequired { get; set; } = false;
    }
}
