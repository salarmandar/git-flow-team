namespace Bgt.Ocean.Models.ServiceRequest
{
    public class QueryServiceRequestView
    {
        // Request
        public QueryServiceRequestView_Request request { get; set; } = new QueryServiceRequestView_Request();

        // Service request
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
        public string problem { get; set; }
        public string solution { get; set; }
        public string createdUser { get; set; }
        public string modifiedUser { get; set; }
        public string openSource { get; set; }
        public string closeSource { get; set; }
        public string onHold { get; set; }

        // Route
        public string brinksSiteCode { get; set; }
        public string brinksSiteName { get; set; }
        public string routeGroupName { get; set; }
        public string routeGroupDetailName { get; set; }
        public string runNumber { get; set; }
        public string technicianId { get; set; }
        public string technicianName { get; set; }
        public string technicianPhone { get; set; }

        // Datetime
        public string createdDatetime { get; set; }
        public string modifiedDatetime { get; set; }
        public string openedDatetime { get; set; }
        public string notifiedDatetime { get; set; }
        public string downedDatetime { get; set; }
        public string servicedDatetime { get; set; }
        public string dueDatetime { get; set; }
        public string dispatchedDatetime { get; set; }
        public string acknowlegedDatetime { get; set; }
        public string etaDatetime { get; set; }
        public string departedToOnsiteDatetime { get; set; }
        public string reportedOnsiteDatetime { get; set; }
        public string resolvedDatetime { get; set; }
        public string departedToOriginDatetime { get; set; }
        public string reportedToOriginDatetime { get; set; }
        public string closedDatetime { get; set; }
    }

    public class QueryServiceRequestView_Request
    {
        public string countryAbb { get; set; }
        public string atmId { get; set; }
        public string ticketNumber { get; set; }
        public string statusId { get; set; }
        public string customerReferenceNumber { get; set; }
        public string createdUser { get; set; }
        public string createdDatetimeFrom { get; set; }
        public string createdDatetimeTo { get; set; }
    }
}
