using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.RunControl
{
    public class TabLegView
    {
        public IEnumerable<LegView> LegList { get; set; }
    }
    public class LegView
    {
        public string ActionName { get; set; }
        public string CustomerName { get; set; }
        public string LocationName { get; set; }
        public string WorkDate { get; set; }

        public string ArrivalTime { get; set; }
        public string ActualTime { get; set; }
        public string DepartTime { get; set; }

        public string RouteGroupDetailName { get; set; }
        public string RunResourceNumber { get; set; }
        public string ReceiptNo { get; set; }
        public string Remarks { get; set; }

        public bool FlagNonBillable { get; set; }
        public Nullable<System.DateTime> ServiceStopTransectionDate { get; set; }
        public Nullable<System.DateTime> ArrivalTimeDT { get; set; }
        public Nullable<System.DateTime> DepartTimeDT { get; set; }
        public Nullable<System.DateTime> ActualTimeDT { get; set; }

    }

    public class LegOptimizationView
    {
        public string jobNo { get; set; }
        public int? jobOrder { get; set; }
        public Guid MasterActualJobServiceStopLegs_Guid { get; set; }
        public Guid MasterCustomerLocation_Guid { get; set; }
    }

}
