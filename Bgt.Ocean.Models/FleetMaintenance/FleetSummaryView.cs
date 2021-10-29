using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Models.BaseModel;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.FleetMaintenance
{
    public class FleetSummaryView
    {
        public IEnumerable<SummaryMaintenanceView> MaintenanceList { get; set; } = new List<SummaryMaintenanceView>();
        public IEnumerable<SummaryGasolineView> GasolineList { get; set; } = new List<SummaryGasolineView>();
        public IEnumerable<SummaryAccidentView> AccidentList { get; set; } = new List<SummaryAccidentView>();
        public int? Total { get; set; }
    }
    public class SummaryMaintenanceFilter : PagingBase
    {
        public Guid? SiteGuid { get; set; }
        public Guid? RunGuid { get; set; }
        public EnumFleetOption FleetOption { get; set; }
        public string Year { get; set; }

        /// <summary>
        /// Covert string Year to int userformat
        /// </summary>
        public int inYear { get { return ($"01/01/{Year}".ChangeFromStrDateToDateTime()).Year; } }
    }

    public class SummaryMaintenanceView
    {
        public string VendorName { get; set; }
        public string Currency { get; set; }
        public double TotalCost { get; set; }
    }
    public class SummaryGasolineView
    {
        public string Month { get; set; }
        public string Currency { get; set; }
        public double TotalCost { get; set; }
    }
    public class SummaryAccidentView
    {
        public string Month { get; set; }
        public string Accident { get; set; }
        public double TotalAccident { get; set; }
    }
}
