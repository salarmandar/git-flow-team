namespace Bgt.Ocean.Models.RunControl
{
    public class ValidateCrewOnPortalView
    {
        public string CrewID { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string CurrentlyActive { get; set; }
        public string LicensePlateNumber { get; set; }
        public string RoofNo { get; set; }
        public string EmployeeImage { get; set; }
        public string SignatureImage { get; set; }
        public string TruckImage { get; set; }
    }

    public class ValidateCrewOnPortalView_Request
    {
        public string CrewID { get; set; }
        public string RunDate { get; set; }
        public string SiteCode { get; set; }
    }
}
