using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.RunControlService
{
    public class ValidateCrewOnPortalResponse
    {
        /// <summary>
        /// 1 = Success, 0 = Invalid request, -1 = Error
        /// </summary>
        public string responseCode { get; set; } = "1";
        public string responseMessage { get; set; } = "Success";
        /// <summary>
        /// Number of row
        /// </summary>
        public int rows { get; set; } = 0;
        public List<ValidateCrewOnPortalResponse_Main> result { get; set; } = new List<ValidateCrewOnPortalResponse_Main>();
    }

    public class ValidateCrewOnPortalResponse_Main
    {
        public string crewId { get; set; }
        public string name { get; set; }
        public string role { get; set; }
        public string currentlyActive { get; set; }
        public string licensePlateNo { get; set; }
        public string roofNo { get; set; }
        public string employeeImage { get; set; }
        public string signatureImage { get; set; }
        public string truckImage { get; set; }
    }
}
