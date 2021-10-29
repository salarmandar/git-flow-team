using System;


namespace Bgt.Ocean.WebAPI.Models.FleetMaintenance
{
    public class RunResourceAccidentViewModel
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public Guid runResourceGuid { get; set; }
        public Guid siteGuid { get; set; }
        public Guid? runResourceBrandGuid { get; set; }
        public Guid? runResourceTypeGuid { get; set; }
        public bool flagBrinksIsFault { get; set; }
        public bool flagShowAll { get; set; }
        public string counterParty { get; set; }
    }
}
