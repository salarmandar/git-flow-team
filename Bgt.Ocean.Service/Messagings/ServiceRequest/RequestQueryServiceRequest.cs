using Bgt.Ocean.Service.Messagings.StandardTable;

namespace Bgt.Ocean.Service.Messagings.ServiceRequest
{
    public class RequestQueryServiceRequest : BaseRequestQuery
    {
        public string atmId { get; set; }
        public string ticketNumber { get; set; }
        /// <summary>
        /// 1 = Open, 9 = Closed, 103 = Pending, 104 = Dispatched, 105 = Cancelled, 107 = Planned, 108 = Reopen, 109 = On Hold
        /// </summary>
        public string statusId { get; set; }
        public string customerReferenceNumber { get; set; }
        /// <summary>
        /// Preferred 'OceanWebApiUser'
        /// </summary>
        public string createdUser { get; set; }
    }
}
