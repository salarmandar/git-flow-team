using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models.Nemo.RouteOptimization
{
    public class NemoOptimizationTransactionResponse
    {
        public Guid NemoQueueOptimization_Guid { get; set; }
        public int TransactionID { get; set; }
        public Guid? MasterRoute_Guid { get; set; }
        public Guid DailyRunResource_Guid { get; set; }
        public string DailyRunResource_FullName { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; }

        public DateTime? RequestedDate { get; set; }
        public string RequestedBy { get; set; }

        public DateTime? OptimizedDate { get; set; }
        public int? OptimizedDistance { get; set; }
        public int? OptimizedTime { get; set; }

        public DateTime? ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }


        public List<NemoOptimizationTransactionDetail> Stops { get; set; } = new List<NemoOptimizationTransactionDetail>();
    }
}
