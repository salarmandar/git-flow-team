using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{
    public class ResponseQueryServiceRequest
    {
        public string responseCode { get; set; } = "1";
        public string responseMessage { get; set; } = "Success";
        public int rows { get; set; } = 0;
        public List<ResponseQueryServiceRequest_Main> result { get; set; } = new List<ResponseQueryServiceRequest_Main>();
    }
    public class ResponseQueryServiceRequest_Main
    {
        public ResponseQueryServiceRequest_ServiceRequestDetail serviceRequestDetail { get; set; } = new ResponseQueryServiceRequest_ServiceRequestDetail();
        public ResponseQueryServiceRequest_RouteDetail routeDetail { get; set; } = new ResponseQueryServiceRequest_RouteDetail();
        public ResponseQueryServiceRequest_DatetimeDetail datetimeDetail { get; set; } = new ResponseQueryServiceRequest_DatetimeDetail();
    }

    public class ResponseQueryServiceRequest_ServiceRequestDetail
    {
        public string guid { get; set; }
        public string countryAbb { get; set; }
        public string ticketNumber { get; set; }
        public string statusId { get; set; }
        public string statusName { get; set; }
        public string responderStatusName { get; set; }
        public string machineServiceType { get; set; }
        public string lob { get; set; }
        public string serviceJobType { get; set; }
        public string reportedIncidentDescription { get; set; }
        public string problemId { get; set; }
        public string problemName { get; set; }
        public string priority { get; set; }
        public string solutionId { get; set; }
        public string solutionName { get; set; }
        public string createdUser { get; set; }
        public string modifiedUser { get; set; }
        public string openSource { get; set; }
        public string closeSource { get; set; }
        public string onHold { get; set; }
    }

    public class ResponseQueryServiceRequest_RouteDetail
    {
        public string brinksSiteCode { get; set; }
        public string brinksSiteName { get; set; }
        public string routeGroupName { get; set; }
        public string routeGroupDetailName { get; set; }
        public string runNumber { get; set; }
        public string technicianId { get; set; }
        public string technicianName { get; set; }
        public string technicianPhone { get; set; }
    }

    public class ResponseQueryServiceRequest_DatetimeDetail
    {
        /// <summary>
        /// UTC +0
        /// </summary>
        public string createdDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string modifiedDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string openedDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string notifiedDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string downedDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string servicedDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string dueDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string dispatchedDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string acknowlegedDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string etaDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string departedToOnsiteDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string reportedOnsiteDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string resolvedDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string departedToOriginDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string reportedToOriginDatetime { get; set; }

        /// <summary>
        /// UTC +0
        /// </summary>
        public string closedDatetime { get; set; }
    }
}
