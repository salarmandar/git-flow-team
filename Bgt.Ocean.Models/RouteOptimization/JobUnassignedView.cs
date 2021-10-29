using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.RouteOptimization
{
    public class JobUnassignedView
    {
        public Guid JobGuid { get; set; }
        public string JobNo { get; set; }
        public string LOBFullName { get; set; }
        public string ServiceJobTypeAbbr { get; set; }
        public string Action { get; set; }
        public string CustomerLocationDisplay { get; set; }
        public decimal? STC { get; set; }
        public string Currency { get; set; }
        public string StrSTCDisplay { get { return this.STC + " " + this.Currency; } }
        public string OptimizationStatus { get; set; }
        public int OptimizationStatusId { get; set; }
        public bool FlagShowAllStatus { get; set; }
        public Guid? JobHeaderGuid { get; set; }
    }

    public class JobUnassignedResponse
    {
        public IEnumerable<JobUnassignedView> JobUnassignedList { get; set; }

    }
}
